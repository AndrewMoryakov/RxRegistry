# RxRegistry

Lightweight in-memory registry for .NET that combines a **typed key-value store** with a **reactive event bus**. Zero dependencies, static API, generic type safety.

## Installation

```
dotnet add package RxRegistry
```

**Supported frameworks:** .NET Framework 4.5+, .NET Standard 2.0, .NET 8, .NET 10

## Quick Start

```csharp
using DreamPlace.Lib.Rx;

// Store a value
Registry.Public("connection-string-here");

// Retrieve it anywhere
string cs = Registry.GetValue<string>();

// Subscribe to events
Registry.Subscribe<int>(e =>
{
    Console.WriteLine($"Received: {e.Value}");
}, id: "counter");

// Send an event to subscribers
Registry.OnNext(42, id: "counter");
```

## Core Concepts

RxRegistry provides two independent mechanisms on a shared storage:

| Mechanism | Methods | Purpose |
|---|---|---|
| **Store** | `Public` / `GetValue` / `GetValues` | Typed key-value storage by type + optional id |
| **Event Bus** | `Subscribe` / `OnNext` / `UnSubscribe` | Reactive notifications by type + optional id |

### Two API Levels

- **`Registry`** (static facade) -- simplified API, no generic class parameters needed
- **`Registry<TTargetType, TValue>`** -- full generic API with sender-type isolation

## Store: Publish and Retrieve Values

### Basic usage (by type)

```csharp
Registry.Public(new AppConfig { Theme = "dark" });

AppConfig config = Registry.GetValue<AppConfig>();
```

### Multiple values of the same type (by id)

```csharp
Registry.Public("primary-db", id: "db1");
Registry.Public("replica-db",  id: "db2");

string primary = Registry.GetValue<string>("db1"); // "primary-db"
string replica  = Registry.GetValue<string>("db2"); // "replica-db"
```

### Replace semantics

Calling `Public` with the same type and id updates the existing value:

```csharp
Registry.Public("v1");
Registry.Public("v2"); // replaces "v1"
Registry.GetValue<string>(); // "v2"
```

## Event Bus: Subscribe and Notify

```csharp
// Subscribe
Registry.Subscribe<OrderEvent>(e =>
{
    Console.WriteLine($"Order {e.Value.Id} — mode: {e.Mode}");
}, id: "orders");

// Fire event
Registry.OnNext(new OrderEvent { Id = 123 }, id: "orders");
```

`RegistryEventArgs<T>` carries:
- `Value` -- the payload
- `Mode` -- `ActionMode.Update` or `ActionMode.Delete`
- `Source` -- optional sender reference

## Sender-Type Isolation

The full generic API lets you partition data by sender type:

```csharp
// Two senders, same target/value types, isolated storage
Registry<IViewModel, string>.Public<ViewA>("hello");
Registry<IViewModel, string>.Public<ViewB>("world");

Registry<IViewModel, string>.Get<ViewA>(); // "hello"
Registry<IViewModel, string>.Get<ViewB>(); // "world"
```

This gives compile-time isolation without relying on string ids.

## Lifecycle Management

### Remove (with confirmation contract)

Removal is two-phase: you get a contract describing what will be removed, then confirm it.

```csharp
var contract = Registry.Remove<string>("db1");

// Inspect before confirming
Console.WriteLine($"Removing {contract.Count} entries, {contract.SubscriberCount} subscribers");

contract.Confirm(); // actually removes
```

If you don't call `Confirm()`, nothing is removed. Double `Confirm()` throws `InvalidOperationException`.

### Clear (all entries of a type)

```csharp
var contract = Registry.Clear<string>();
contract.Confirm(); // removes all string entries
```

### Scoped Lifetime

`RegistryScope` auto-removes everything registered through it on `Dispose`:

```csharp
using (var scope = Registry.CreateScope())
{
    scope.Public<string>("temp-value", id: "key");
    scope.Public<int>(42, id: "answer");

    Registry.GetValue<string>("key"); // "temp-value"
}
// Both entries are removed here
```

## WeakReference Mode

Store values without preventing garbage collection. Useful for ViewModels, caches, or any object whose lifetime is managed elsewhere.

```csharp
Registry.PublicWeak(myViewModel, id: "main-vm");

// While a strong reference exists elsewhere -- value is available
Registry.GetValue<MyViewModel>("main-vm"); // myViewModel

// After all strong references are released and GC runs:
Registry.GetValue<MyViewModel>("main-vm"); // null
```

Clean up collected entries explicitly:

```csharp
Registry.CleanupDeadReferences<MyViewModel>();
```

Only reference types are supported (`where TValue : class` constraint at compile time).

## API Reference

### `Registry` (facade)

| Method | Description |
|---|---|
| `Public<T>(value)` | Store a value by type |
| `Public<T>(value, id)` | Store a value by type + id |
| `PublicWeak<T>(value, id)` | Store as weak reference |
| `GetValue<T>()` | Get single value by type |
| `GetValue<T>(id)` | Get value by type + id |
| `GetValues<T>()` | Get all values of a type |
| `Subscribe<T>(action, id)` | Subscribe to events |
| `OnNext<T>(value, id)` | Send event to subscribers |
| `Remove<T>(id)` | Get removal contract for entry |
| `Clear<T>()` | Get removal contract for all entries of type |
| `CleanupDeadReferences<T>()` | Remove dead weak reference entries |
| `CreateScope()` | Create a scoped lifetime container |

### `Registry<TTarget, TValue>` (full generic)

Same operations as facade, plus:
- `Public<TSender>(value, id)` / `Get<TSender>(id)` -- sender-type partitioning
- `Subscribe<TSender>(action, id)` / `OnNext<TSender>(value, id)` -- sender-scoped events
- `UnSubscribe(id)` / `UnSubscribe<TSender>(id)` -- remove all subscribers

## License

MIT

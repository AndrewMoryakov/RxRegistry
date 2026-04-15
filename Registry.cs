using System;
using System.Collections.Generic;
using System.Linq;

namespace DreamPlace.Lib.Rx
{
	internal class OwnType
	{
	}

	public static class Registry
	{
		public static IEnumerable<TValue> GetValues<TValue>()
		{
			List<RegistryElement<TValue>> allValues = Registry<OwnType, TValue>.Values;

			if (allValues == null)
				throw new ArgumentException($"Нет данных для {typeof(TValue)}");

			return allValues.Select(el => el.Value);
		}

		public static TValue GetValue<TValue>()
		{
			var reslt = Registry<OwnType, TValue>.Values.FirstOrDefault();

			if (reslt == null)
			{
				throw new ArgumentException($"{typeof(TValue)} isn't published");
			}

			return reslt.Value;
		}

		public static TValue GetValue<TValue>(object id)
		{
			var reslt = Registry<OwnType, TValue>.Find<OwnType>(id).FirstOrDefault();

			if (reslt == null)
			{
				return default(TValue);
			}

			return reslt.Value;
		}

		public static void Public<TValue>(TValue value)
			=> Registry<OwnType, TValue>.Public<OwnType>(value);

		public static void Public<TValue>(TValue value, object id)
			=> Registry<OwnType, TValue>.Public<OwnType>(value, id);

		public static void PublicWeak<TValue>(TValue value, object id = null) where TValue : class
			=> Registry<OwnType, TValue>.PublicWeak<OwnType>(value, id);

		public static void OnNext<TValue>(RegistryEventArgs<TValue> e, object id)
		{
			var targetElement = Registry<OwnType, TValue>.Find<OwnType>(id).FirstOrDefault();
			targetElement?.EventActions.ForEach(l => l?.Invoke(e));
		}

		public static void OnNext<TValue>(TValue e, object id) => OnNext(new RegistryEventArgs<TValue>(e), id);

		public static void Subscribe<TValue>(Action<RegistryEventArgs<TValue>> subscriber, object id = null)
		{
			var targetElement = Registry<OwnType, TValue>.Find<OwnType>(id).FirstOrDefault();
			if (targetElement == null)
			{
				Registry<OwnType, TValue>.Public(default(TValue), id);
				targetElement = Registry<OwnType, TValue>.Find<OwnType>(id).FirstOrDefault();
			}

			targetElement?.EventActions.Add(subscriber);
		}

		public static RemovalContract<TValue> Remove<TValue>(object id = null)
		{
			return Registry<OwnType, TValue>.Remove<OwnType>(id);
		}

		public static RemovalContract<TValue> Clear<TValue>()
		{
			return Registry<OwnType, TValue>.Clear();
		}

		public static void CleanupDeadReferences<TValue>()
		{
			Registry<OwnType, TValue>.CleanupDeadReferences();
		}

		public static RegistryScope CreateScope()
		{
			return new RegistryScope();
		}
	}

	public static class Registry<TTargetType, TValue>
	{
		internal static List<RegistryElement<TValue>> Values;

		static Registry()
		{
			if (Values == null)
				Values = new List<RegistryElement<TValue>>();
		}

		public static void Subscribe(Action<RegistryEventArgs<TValue>> subscriber, object id = null)
		{
			var targetElement = Find<OwnType>(id).FirstOrDefault();
			if (targetElement == null)
			{
				Public<OwnType>(default(TValue), id);
				targetElement = Find<OwnType>(id).FirstOrDefault();
			}

			targetElement?.EventActions.Add(subscriber);
		}

		public static void Subscribe<TSenderType>(Action<RegistryEventArgs<TValue>> subscriber, object id = null)
		{
			var targetElement = Find<TSenderType>(id).FirstOrDefault();
			if (targetElement == null)
			{
				Public<TSenderType>(default(TValue), id);
				targetElement = Find<TSenderType>(id).FirstOrDefault();
			}

			targetElement?.EventActions.Add(subscriber);
		}

		public static void UnSubscribe(object id)
		{
			var targetElement = Find<OwnType>(id).FirstOrDefault();
			targetElement?.EventActions.Clear();
		}

		public static void UnSubscribe<TSenderType>(object id)
		{
			var targetElement = Find<TSenderType>(id).FirstOrDefault();
			targetElement?.EventActions.Clear();
		}

		public static RemovalContract<TValue> Remove(object id = null)
		{
			return Remove<OwnType>(id);
		}

		public static RemovalContract<TValue> Remove<TSenderType>(object id = null)
		{
			var elements = Find<TSenderType>(id).ToList();
			if (elements.Count == 0) return null;

			return new RemovalContract<TValue>(elements, () =>
			{
				foreach (var el in elements)
				{
					el.EventActions.Clear();
					Values.Remove(el);
				}
			});
		}

		public static RemovalContract<TValue> Clear()
		{
			var elements = Values.ToList();
			if (elements.Count == 0) return null;

			return new RemovalContract<TValue>(elements, () =>
			{
				foreach (var el in elements)
					el.EventActions.Clear();
				Values.Clear();
			});
		}

		public static void Public(TValue value, object id = null)
		{
			var targetElement = Find<OwnType>(id).FirstOrDefault();
			if (targetElement == null)
			{
				Values.Add(new RegistryElement<TValue>(typeof(OwnType), typeof(TTargetType), value, id));
			}
			else
			{
				targetElement.Value = value;
			}
		}

		public static void Public<TSenderType>(TValue value, object id = null)
		{
			var targetElement = Find<TSenderType>(id).FirstOrDefault();
			if (targetElement == null)
			{
				Values.Add(new RegistryElement<TValue>(typeof(TSenderType), typeof(TTargetType), value, id));
			}
			else
			{
				targetElement.Value = value;
			}
		}

		public static void PublicWeak(TValue value, object id = null)
		{
			PublicWeak<OwnType>(value, id);
		}

		public static void PublicWeak<TSenderType>(TValue value, object id = null)
		{
			if (!typeof(TValue).IsClass && !typeof(TValue).IsInterface)
				throw new InvalidOperationException(
					$"WeakReference mode is only supported for reference types. {typeof(TValue)} is a value type.");

			var element = Find<TSenderType>(id).FirstOrDefault();
			if (element == null)
			{
				Values.Add(new RegistryElement<TValue>(typeof(TSenderType), typeof(TTargetType), value, id, isWeak: true));
			}
			else
			{
				element.IsWeak = true;
				element.Value = value;
			}
		}

		public static void CleanupDeadReferences()
		{
			Values.RemoveAll(el => el.IsWeak && !el.IsAlive);
		}

		public static TValue Get(object id = null)
		{
			var reslt = Find<OwnType>(id).FirstOrDefault();

			if (reslt == null)
			{
				return default(TValue);
			}
			else
			{
				return reslt.Value;
			}
		}

		public static TValue Get<TSenderType>(object id = null)
		{
			var reslt = Find<TSenderType>(id).FirstOrDefault();

			if (reslt == null)
			{
				return default(TValue);
			}
			else
			{
				return reslt.Value;
			}
		}

		public static IEnumerable<TValue> GetValues<TSenderType>(object id = null)
		{
			return Find<TSenderType>(id).Select(el => el.Value);
		}

		public static IEnumerable<TValue> GetValues(object id = null)
		{
			return Find<OwnType>(id).Select(el => el.Value);
		}

		/// <exception cref="NullReferenceException">Нет получателя</exception>
		/// <exception cref="Exception">A delegate callback throws an exception.</exception>
		public static void OnNext(RegistryEventArgs<TValue> e, object id)
		{
			var targetElement = Find<OwnType>(id).FirstOrDefault();
			targetElement?.EventActions.ForEach(l => l?.Invoke(e));
		}

		public static void OnNext<TSender>(RegistryEventArgs<TValue> e, object id)
		{
			var targetElement = Find<TSender>(id).FirstOrDefault();
			targetElement?.EventActions.ForEach(l => l?.Invoke(e));
		}

		public static void OnNext<TSender>(TValue e, object id = null)
		{
			OnNext<TSender>(new RegistryEventArgs<TValue>(e), id);
		}

		public static void OnNext(TValue e, object id = null)
		{
			OnNext(new RegistryEventArgs<TValue>(e), id);
		}

		internal static IEnumerable<RegistryElement<TValue>> Find<TSender>(object id)
		{
			return Values.Where(el =>
			{
				return el?.SourceType == typeof(TSender)
				       && el.TargetType == typeof(TTargetType)
				       && el.ValueType == typeof(TValue)
				       && Equals(el.Id, id);
			});
		}

		internal static IEnumerable<RegistryElement<TValue>> Find()
		{
			return Values;
		}
	}
}

using System;
using System.Collections.Generic;

namespace DreamPlace.Lib.Rx
{
	public class RegistryElement<TValue>
	{
		private TValue _value;
		private WeakReference _weakRef;

		public RegistryElement()
		{
			EventActions = new List<Action<RegistryEventArgs<TValue>>>();
		}

		public RegistryElement(Type sourceType, Type targetType, TValue value, object id, bool isWeak = false) :this()
		{
			SourceType = sourceType;
			TargetType = targetType;
			ValueType = typeof(TValue);
			IsWeak = isWeak;
			Id = id;

			Value = value;
		}

		public object Id { get; set; }
		public Type SourceType { get; set; }
		public Type ValueType { get; set; }
		public Type TargetType { get; private set; }
		public bool IsWeak { get; internal set; }

		public bool IsAlive
		{
			get
			{
				if (!IsWeak) return true;
				return _weakRef != null && _weakRef.IsAlive;
			}
		}

		public TValue Value
		{
			get
			{
				if (!IsWeak) return _value;
				if (_weakRef != null && _weakRef.IsAlive)
					return (TValue)_weakRef.Target;
				return default(TValue);
			}
			set
			{
				if (IsWeak)
				{
					_weakRef = value != null ? new WeakReference(value) : null;
					_value = default(TValue);
				}
				else
				{
					_value = value;
					_weakRef = null;
				}
			}
		}

		public List<Action<RegistryEventArgs<TValue>>> EventActions { get; set; }
	}
}

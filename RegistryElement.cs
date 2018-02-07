using System;

namespace DreamPlace.Lib.Rx
{
	public class RegistryElement<TValue>
	{
		public RegistryElement(Type sourceType, Type targetType, Type valueType, TValue value, object id)
		{
			SourceType = sourceType;
			TargetType = targetType;
			ValueType = valueType;
			Value = value;
			Id = id;
		}

		public RegistryElement(Type targetType, Type valueType, TValue value, object id)
		{
			TargetType = targetType;
			ValueType = valueType;
			Value = value;
			Id = id;
		}

		public object Id { get; set; }
		public Type SourceType { get; set; }
		public Type ValueType { get; set; }
		public Type TargetType { get; private set; }
		public TValue Value { get; set; }
		public Action<RegistryEventArgs<TValue>> EventAction { get; set; }

	}
}

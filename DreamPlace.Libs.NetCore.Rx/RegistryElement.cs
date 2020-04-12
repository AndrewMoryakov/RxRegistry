using System;

namespace DreamPlace.Libs.NetCore.Rx
{
	public class RegistryElement<TValue>
	{
		public RegistryElement(Type sourceType, Type targetType, TValue value, object id)
		{
			TargetType = targetType;
			SourceType = sourceType;
			ValueType = typeof(TValue);
			
			Value = value;
			Id = id;
		}

		public object Id { get; set; }
		public Type SourceType { get; set; }
		public Type ValueType { get; set; }
		public Type TargetType { get; private set; }
		public TValue Value { get; set; }
		public Action<object, RegistryEventArgs<TValue>> EventAction { get; set; }
	}
}

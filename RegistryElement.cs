using System;
using System.Collections.Generic;

namespace DreamPlace.Lib.Rx
{
	public class RegistryElement<TValue>
	{
		public RegistryElement()
		{
			EventActions = new List<Action<RegistryEventArgs<TValue>>>();
		}

		public RegistryElement(Type sourceType, Type targetType, TValue value, object id) :this()
		{
			SourceType = sourceType;
			TargetType = targetType;
			ValueType = typeof(TValue);
			
			Value = value;
			Id = id;
		}

		public object Id { get; set; }
		public Type SourceType { get; set; }
		public Type ValueType { get; set; }
		public Type TargetType { get; private set; }
		public TValue Value { get; set; }
		public List<Action<RegistryEventArgs<TValue>>> EventActions { get; set; }
	}
}

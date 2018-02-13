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

		public RegistryElement(Type sourceType, Type targetType, Type valueType, TValue value, object id) :this()
		{
			SourceType = sourceType;
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
		public List<Action<RegistryEventArgs<TValue>>> EventActions { get; set; }

	}

	public class RegistryElement
	{
		public RegistryElement(Type sourceType, Type targetType, Type valueType, object id)
		{
			SourceType = sourceType;
			TargetType = targetType;
			ValueType = valueType;
			Id = id;
		}

		public RegistryElement(Type targetType, Type valueType, object id)
		{
			TargetType = targetType;
			ValueType = valueType;
			Id = id;
		}

		public object Id { get; set; }
		public Type SourceType { get; set; }
		public Type ValueType { get; set; }
		public Type TargetType { get; private set; }
		public List<Action<RegistryEventArgs>> EventActions { get; set; }

	}
}

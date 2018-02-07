using System;
using System.Collections.Generic;
using System.Linq;

namespace DreamPlace.Lib.Rx
{
	public enum PublicMode
	{
		AddNew,
		Replace
	}

	public static class Registry<TTargetType, TValue>
	{
		private static event Action<object, RegistryEventArgs<TValue>> _registryEvent;
		private static List<RegistryElement<TValue>> _values;

		static Registry()
		{
			if (_values == null)
				_values = new List<RegistryElement<TValue>>();
		}

		public static void Subscribe(Action<RegistryEventArgs<TValue>> subscriber, object id = null)
		{
			var taergetElement = Find(id);
			if (taergetElement == null)
			{
				PublicValue(default(TValue), id);
				taergetElement = Find(id);
			}

			if (taergetElement != null)
				taergetElement.EventAction = subscriber;
		}

		public static void Subscribe<TSenderType>(Action<RegistryEventArgs<TValue>> subscriber, object id = null)
		{
			var taergetElement = Find<TSenderType>(id);
			if (taergetElement == null)
			{
				PublicValue<TSenderType>(default(TValue), id);
				taergetElement = Find<TSenderType>(id);
			}

			if (taergetElement != null)
				taergetElement.EventAction = subscriber;
		}

		public static void UnSubscribe(object id)
		{
			var taergetElement = Find(id);

			if(taergetElement!=null)
			taergetElement.EventAction = null;
		}

		public static void UnSubscribe<TSenderType>(object id)
		{
			var taergetElement = Find<TSenderType>(id);

			if (taergetElement != null)
				taergetElement.EventAction = null;
		}

		public static void PublicValue(TValue value, object id = null)
		{
			var taergetElement = Find(id);
			if (taergetElement == null)
			{
				_values.Add(new RegistryElement<TValue>(typeof(TTargetType), typeof(TValue), value, id));
			}
			else
			{
				taergetElement.Value = value;
			}
		}

		public static void PublicValue<TSenderType>(TValue value, object id = null)
		{
			var taergetElement = Find<TSenderType>(id);
			if (taergetElement == null)
			{
				_values.Add(new RegistryElement<TValue>(typeof(TSenderType), typeof(TTargetType), typeof(TValue), value, id));
			}
			else
			{
				taergetElement.Value = value;
			}
		}

		public static TValue GetValue(object id = null)
		{
			var reslt = Find(id);

			if (reslt == null)
			{
				return default(TValue);
			}
			else
			{
				return reslt.Value;
			}
		}

		public static TValue GetValue<TSendrType>(object id = null)
		{
			var reslt = Find<TSendrType>(id);

			if (reslt == null)
			{
				return default(TValue);
			}
			else
			{
				return reslt.Value;
			}
		}

		public static IEnumerable<TValue> GetValues<TSendrType>()
		{
			var reslt = Find<TSendrType>();

			if (reslt == null)
			{
				return default(IEnumerable<TValue>);
			}
			else
			{
				return reslt.Select(el=>el.Value);
			}
		}

		public static IEnumerable<TValue> GetValues()
		{
			var reslt = Find();

			if (reslt == null)
			{
				return default(IEnumerable<TValue>);
			}
			else
			{
				return reslt.Select(el => el.Value); ;
			}
		}

		//public static void RemoveValue(object id = null)
		//{
		//	var t = Find(id);
		//	t.Value = default(TValue);
		//}

		//public static void RemoveValue<TSenderType>(object id = null)
		//{
		//	var t = Find<TSenderType>(id);
		//	t.Value = default(TValue);
		//}

		/// <exception cref="NullReferenceException">Нет получателя</exception>
		/// <exception cref="Exception">A delegate callback throws an exception.</exception>
		public static void OnNext(RegistryEventArgs<TValue> e, object id)
		{
			var targetElement = Find(id);

			targetElement?.EventAction?.Invoke(e);
		}

		public static void OnNext<TSender>(RegistryEventArgs<TValue> e, object id = null)
		{
			var targetElement = Find<TSender>(id);

			targetElement?.EventAction?.Invoke(e);
		}

		private static RegistryElement<TValue> Find(object id)
		{
			return _values.FirstOrDefault(el => el!=null && el.Id.Equals(id));
		}
		private static RegistryElement<TValue> Find<TSender>(object id)
		{
			return _values.FirstOrDefault(el => 
			el?.SourceType == typeof(TSender)
			&&
			el.Id.Equals(id)
			);
		}

		private static IEnumerable<RegistryElement<TValue>> Find<TSender>()
		{
			return _values.Where(el => el?.SourceType == typeof(TSender));
		}
		private static IEnumerable<RegistryElement<TValue>> Find()
		{
			return _values;
		}
	}
}
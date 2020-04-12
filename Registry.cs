using System;
using System.Collections.Generic;
using System.Linq;

namespace DreamPlace.Lib.Rx
{
	internal class OwnType
	{	
	}

	public enum PublicMode
	{
		AddNew,
		Replace
	}
	
	public static class Registry
	{
		public static IEnumerable<TValue> GetValues<TValue>()
		{
			List<RegistryElement<TValue>> allValues = Registry<OwnType, TValue>.Values;

			if (allValues == null)
				throw new ArgumentException($"Нет данных для {typeof(TValue)}");

			return allValues.Select(el=>el.Value);
		}
		
		public static TValue GetValue<TValue>()
		{
			var reslt = Registry<OwnType, TValue>.Values;

			if (reslt == null)
			{
				throw new ArgumentException($"{typeof(TValue)} isn't published");
			}

			return reslt.FirstOrDefault().Value;
		}
		//
		// public static TValue GetValues<TValue>(object id)
		// {
		// 	RegistryElement<TValue> allValues = Registry<OwnType, TValue>.Values.Single(id);
		//
		// 	if (allValues == null)
		// 		throw new ArgumentException($"Нет данных для {typeof(TValue)}");
		//
		// 	return allValues
		// }
		
		public static TValue GetValue<TValue>(object id)
		{
			var reslt = Registry<OwnType, TValue>.Find<OwnType>(id);

			if (reslt == null)
			{
				throw new ArgumentException($"Нет реализации {typeof(TValue)}");
			}

			return reslt.FirstOrDefault().Value;
		}

		// public static void Add<TValue>(TValue value)
		// {
		// 	throw new NotImplementedException();
		// }
		
		public static void Public<TValue>(TValue value) 
			=> Registry<OwnType, TValue>.Public<OwnType>(value);

		// public static void Add<TValue>(TValue value, object id)
		// {
		// 	throw new NotImplementedException();
		// }
		
		public static void Public<TValue>(TValue value, object id) 
			=> Registry<OwnType, TValue>.Public<OwnType>(value, id);

		public static void OnNext<TValue>(RegistryEventArgs<TValue> e, object id)
		{
			var targetElement = Registry<OwnType, TValue>.Find<OwnType>(id).Single();
			targetElement?.EventActions.ForEach(l => l?.Invoke(e));
		}

		public static void OnNext<TValue>(TValue e, object id) => OnNext(new RegistryEventArgs<TValue>(e), id);

		public static void Subscribe<TValue>(Action<RegistryEventArgs<TValue>> subscriber, object id = null)
		{
			var taergetElement = Registry<OwnType, TValue>.Find<OwnType>(id).FirstOrDefault();
			if (taergetElement == null)
			{
				Registry<OwnType, TValue>.Public(default(TValue), id);
				taergetElement = Registry<OwnType, TValue>.Find<OwnType>(id).FirstOrDefault();
			}

			taergetElement?.EventActions.Add(subscriber);
		}
	}
	
//	public static class Registry<TTargetType>
//	{
//		public static TTargetType GetValue()
//		{
//			var reslt = Registry<TTargetType, TTargetType>.Find();
//
//			if (reslt == null)
//			{
//				throw new ArgumentException($"{typeof(TTargetType)} isn't published");
//			}
//
//			return reslt.FirstOrDefault().Value;
//		}
//
//		public static TTargetType GetValue(object id)
//		{
//			var reslt = Registry<TTargetType, TTargetType>.Find<OwnSender>(id);
//
//			if (reslt == null)
//			{
//				throw new ArgumentException($"Нет реализации {typeof(TTargetType)}");
//			}
//
//			return reslt.FirstOrDefault().Value;
//		}
//
//		public static void Public<TValue>(TValue value) where TValue:TTargetType
//		{
//			Registry<TTargetType, TTargetType>.Public<OwnSender>(value);
//		}
//
//		public static void Public<TValue>(TValue value, object id) where TValue : TTargetType
//		{
//			Registry<TTargetType, TTargetType>.Public<OwnSender>(value, id);
//		}
//
//
//		public static void OnNext(RegistryEventArgs<TTargetType> e, object id)
//		{
//			var targetElement = Registry<TTargetType, TTargetType>.Find<OwnSender>(id).FirstOrDefault();
//			targetElement?.EventActions.ForEach(l => l?.Invoke(e));
//		}
//
//		public static void OnNext(TTargetType e, object id)
//		{
//			OnNext(new RegistryEventArgs<TTargetType>(e), id);
//		}
//
//		public static void Subscribe(Action<RegistryEventArgs<TTargetType>> subscriber, object id = null)
//		{
//			var taergetElement = Registry<TTargetType, TTargetType>.Find<OwnSender>(id).FirstOrDefault();
//			if (taergetElement == null)
//			{
//				Registry<TTargetType, TTargetType>.Public(default(TTargetType), id);
//				taergetElement = Registry<TTargetType, TTargetType>.Find<OwnSender>(id).FirstOrDefault();
//			}
//
//			taergetElement?.EventActions.Add(subscriber);
//		}
//	}

	public static class Registry<TTargetType, TValue>
	{
		private static event Action<object, RegistryEventArgs<TValue>> _registryEvent;
		internal static List<RegistryElement<TValue>> Values;

		static Registry()
		{
			if (Values == null)
				Values = new List<RegistryElement<TValue>>();
		}

		public static void Subscribe(Action<RegistryEventArgs<TValue>> subscriber, object id = null)
		{
			var taergetElement = Find<OwnType>(id).FirstOrDefault();
			if (taergetElement == null)
			{
				Public<OwnType>(default(TValue), id);
				taergetElement = Find<OwnType>(id).FirstOrDefault();
			}
			
			taergetElement?.EventActions.Add(subscriber);
		}

		public static void Subscribe<TSenderType>(Action<RegistryEventArgs<TValue>> subscriber, object id = null)
		{
			var taergetElement = Find<TSenderType>(id).FirstOrDefault();
			if (taergetElement == null)
			{
				Public<TSenderType>(default(TValue), id);
				taergetElement = Find<TSenderType>(id).FirstOrDefault();
			}

			taergetElement.EventActions.Add(subscriber);
		}

		public static void UnSubscribe(object id)
		{
			var taergetElement = Find<OwnType>(id).FirstOrDefault();
			taergetElement?.EventActions.Clear();
		}

		public static void UnSubscribe<TSenderType>(object id)
		{
			var taergetElement = Find<TSenderType>(id).FirstOrDefault();
				taergetElement?.EventActions.Clear();
		}

		public static void Add(TValue value, object id = null)
		{
			throw new NotImplementedException();	
		}
		
		public static void Public(TValue value, object id = null)
		{
			var taergetElement = Find<OwnType>(id).FirstOrDefault();
			if (taergetElement == null)
			{
				Values.Add(new RegistryElement<TValue>(typeof(OwnType), typeof(TTargetType), value, id));
			}
			else
			{
				taergetElement.Value = value;
			}
		}

		public static void Add<TSenderType>(TValue value, object id = null)
		{
			throw new NotImplementedException();
		}
		
		public static void Public<TSenderType>(TValue value, object id = null)
		{
			var taergetElement = Find<TSenderType>(id).FirstOrDefault();
			if (taergetElement == null)
			{
				Values.Add(new RegistryElement<TValue>(typeof(TSenderType), typeof(TTargetType), value, id));
			}
			else
			{
				taergetElement.Value = value;
			}
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

		public static TValue Get<TSendrType>(object id = null)
		{
			var reslt = Find<TSendrType>(id).FirstOrDefault();

			if (reslt == null)
			{
				return default(TValue);
			}
			else
			{
				return reslt.Value;
			}
		}

		public static IEnumerable<TValue> GetValues<TSendrType>(object id=null)
		{
			var reslt = Find<TSendrType>(id);

			if (reslt == null)
			{
				return default(IEnumerable<TValue>);
			}
			else
			{
				return reslt.Select(el=>el.Value);
			}
		}

		public static IEnumerable<TValue> GetValues(object id =null)
		{
			var reslt = Find<OwnType>(id);

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
					
				       && el.Id == id || (el.Id == null && id == null) || (el.Id != null && el.Id.Equals(id));
			});
		}

		internal static IEnumerable<RegistryElement<TValue>> Find()
		{
			return Values;
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;

namespace DreamPlace.Lib.Rx
{
	internal class OwnSender
	{	
	}

	public enum PublicMode
	{
		AddNew,
		Replace
	}

	public static class Registry<TTargetType>
	{
		public static TTargetType GetValue()
		{
			var reslt = Registry<TTargetType, TTargetType>.Find();

			if (reslt == null)
			{
				throw new ArgumentException($"{typeof(TTargetType)} isn't published");
			}

			return reslt.FirstOrDefault().Value;
		}

		public static TTargetType GetValue(object id)
		{
			var reslt = Registry<TTargetType, TTargetType>.Find<OwnSender>(id);

			if (reslt == null)
			{
				throw new ArgumentException($"Нет реализации {typeof(TTargetType)}");
			}

			return reslt.FirstOrDefault().Value;
		}

		public static void PublicValue<TValue>(TValue value) where TValue:TTargetType
		{
			Registry<TTargetType, TTargetType>.PublicValue<OwnSender>(value);
		}

		public static void PublicValue<TValue>(TValue value, object id) where TValue : TTargetType
		{
			Registry<TTargetType, TTargetType>.PublicValue<OwnSender>(value, id);
		}


		public static void OnNext(RegistryEventArgs<TTargetType> e, object id)
		{
			var targetElement = Registry<TTargetType, TTargetType>.Find<OwnSender>(id).FirstOrDefault();
			targetElement?.EventActions.ForEach(l => l?.Invoke(e));
		}

		public static void OnNext(TTargetType e, object id)
		{
			OnNext(new RegistryEventArgs<TTargetType>(e), id);
		}

		public static void Subscribe(Action<RegistryEventArgs<TTargetType>> subscriber, object id = null)
		{
			var taergetElement = Registry<TTargetType, TTargetType>.Find<OwnSender>(id).FirstOrDefault();
			if (taergetElement == null)
			{
				Registry<TTargetType, TTargetType>.PublicValue(default(TTargetType), id);
				taergetElement = Registry<TTargetType, TTargetType>.Find<OwnSender>(id).FirstOrDefault();
			}

			taergetElement?.EventActions.Add(subscriber);
		}
	}

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
			var taergetElement = Find<OwnSender>(id).FirstOrDefault();
			if (taergetElement == null)
			{
				PublicValue<OwnSender>(default(TValue), id);
				taergetElement = Find<OwnSender>(id).FirstOrDefault();
			}
			
			taergetElement?.EventActions.Add(subscriber);
		}

		public static void Subscribe<TSenderType>(Action<RegistryEventArgs<TValue>> subscriber, object id = null)
		{
			var taergetElement = Find<TSenderType>(id).FirstOrDefault();
			if (taergetElement == null)
			{
				PublicValue<TSenderType>(default(TValue), id);
				taergetElement = Find<TSenderType>(id).FirstOrDefault();
			}

			taergetElement.EventActions.Add(subscriber);
		}

		public static void UnSubscribe(object id)
		{
			var taergetElement = Find<OwnSender>(id).FirstOrDefault();
			taergetElement?.EventActions.Clear();
		}

		public static void UnSubscribe<TSenderType>(object id)
		{
			var taergetElement = Find<TSenderType>(id).FirstOrDefault();
				taergetElement?.EventActions.Clear();
		}

		public static void PublicValue(TValue value, object id = null)
		{
			var taergetElement = Find<OwnSender>(id).FirstOrDefault();
			if (taergetElement == null)
			{
				Values.Add(new RegistryElement<TValue>(typeof(OwnSender), typeof(TTargetType), typeof(TValue), value, id));
			}
			else
			{
				taergetElement.Value = value;
			}
		}

		public static void PublicValue<TSenderType>(TValue value, object id = null)
		{
			var taergetElement = Find<TSenderType>(id).FirstOrDefault();
			if (taergetElement == null)
			{
				Values.Add(new RegistryElement<TValue>(typeof(TSenderType), typeof(TTargetType), typeof(TValue), value, id));
			}
			else
			{
				taergetElement.Value = value;
			}
		}

		public static TValue GetValue(object id = null)
		{
			var reslt = Find<OwnSender>(id).FirstOrDefault();

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
			var reslt = Find<OwnSender>(id);

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
			var targetElement = Find<OwnSender>(id).FirstOrDefault();
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
			   el?.SourceType == typeof(TSender)
			&& el.TargetType == typeof(TTargetType)
			&& el.ValueType == typeof(TValue)
			&& el.Id == id);
		}

		internal static IEnumerable<RegistryElement<TValue>> Find()
		{
			return Values;
		}
	}
}
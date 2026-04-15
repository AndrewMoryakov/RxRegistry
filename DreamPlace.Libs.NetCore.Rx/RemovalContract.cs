using System;
using System.Collections.Generic;
using System.Linq;

namespace DreamPlace.Libs.NetCore.Rx
{
	public class RemovalContract<TValue>
	{
		private readonly Action _onConfirm;

		internal RemovalContract(List<RegistryElement<TValue>> elements, Action onConfirm)
		{
			Values = elements.Select(e => e.Value).ToList().AsReadOnly();
			Ids = elements.Select(e => e.Id).ToList().AsReadOnly();
			Count = elements.Count;
			SubscriberCount = elements.Sum(e => e.EventActions.Count);
			_onConfirm = onConfirm;
		}

		/// <summary>Значения, которые будут удалены</summary>
		public IReadOnlyList<TValue> Values { get; private set; }

		/// <summary>Идентификаторы удаляемых элементов</summary>
		public IReadOnlyList<object> Ids { get; private set; }

		/// <summary>Количество элементов к удалению</summary>
		public int Count { get; private set; }

		/// <summary>Общее количество активных подписчиков на удаляемых элементах</summary>
		public int SubscriberCount { get; private set; }

		/// <summary>Был ли контракт подтверждён</summary>
		public bool IsConfirmed { get; private set; }

		/// <summary>Подтвердить удаление. Вызов необратим.</summary>
		/// <exception cref="InvalidOperationException">Контракт уже подтверждён</exception>
		public void Confirm()
		{
			if (IsConfirmed)
				throw new InvalidOperationException("Contract is already confirmed");

			_onConfirm();
			IsConfirmed = true;
		}
	}
}

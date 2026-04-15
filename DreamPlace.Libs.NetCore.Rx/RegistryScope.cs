using System;
using System.Collections.Generic;

namespace DreamPlace.Libs.NetCore.Rx
{
	public class RegistryScope : IDisposable
	{
		private readonly List<Action> _rollbackActions = new List<Action>();
		private bool _disposed;

		public void Public<TValue>(TValue value, object id = null)
		{
			Registry.Public(value, id);
			_rollbackActions.Add(() =>
			{
				var contract = Registry.Remove<TValue>(id);
				contract?.Confirm();
			});
		}

		public void Dispose()
		{
			if (_disposed) return;

			for (int i = _rollbackActions.Count - 1; i >= 0; i--)
				_rollbackActions[i]();

			_rollbackActions.Clear();
			_disposed = true;
		}
	}
}

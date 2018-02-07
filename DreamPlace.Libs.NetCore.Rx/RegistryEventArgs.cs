using System;

namespace DreamPlace.Libs.NetCore.Rx
{
	public enum ActionMode
	{
		Update,
		Delete
	}
	public class RegistryEventArgs<TValue> : EventArgs
	{
		public RegistryEventArgs(TValue e, ActionMode actionMode)
		{
			Value = e;
			Mode = actionMode;
		}

		public ActionMode Mode;
		public TValue Value { get; private set; }
	}
}

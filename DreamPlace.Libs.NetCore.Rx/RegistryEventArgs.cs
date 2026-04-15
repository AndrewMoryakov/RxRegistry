using System;

namespace DreamPlace.Libs.NetCore.Rx
{
	public enum ActionMode
	{
		Update,
		Delete
	}

	public struct RegistryEventArgs<TValue>
	{
		public RegistryEventArgs(TValue e, ActionMode actionMode = ActionMode.Update, object source = null)
		{
			Source = source;
			Value = e;
			Mode = actionMode;
		}

		public object Source;
		public ActionMode Mode;
		public TValue Value { get; private set; }
	}

	public struct RegistryEventArgs
	{
		public RegistryEventArgs(ActionMode actionMode = ActionMode.Update, object source = null)
		{
			Source = source;
			Mode = actionMode;
		}

		public object Source;
		public ActionMode Mode;
	}
}

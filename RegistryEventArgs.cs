﻿using System;

namespace DreamPlace.Lib.Rx
{
	public enum ActionMode
	{
		Update,
		Delete
	}
	public class RegistryEventArgs<TValue> : EventArgs
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
}

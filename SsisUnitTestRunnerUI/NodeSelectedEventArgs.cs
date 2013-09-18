using System;

namespace ssisUnitTestRunnerUI
{
    public class NodeSelectedEventArgs : EventArgs
    {
        public NodeSelectedEventArgs(object oldItem, object newItem)
        {
            OriginalItem = oldItem;
            NewItem = newItem;
        }

        public object NewItem { get; private set; }

        public object OriginalItem { get; private set; }
    }
}
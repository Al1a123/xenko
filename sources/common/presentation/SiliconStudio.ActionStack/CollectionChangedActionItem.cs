// Copyright (c) 2014 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace SiliconStudio.ActionStack
{
    /// <summary>
    /// A <see cref="DirtiableActionItem"/> representing a change in a collection.
    /// </summary>
    public class CollectionChangedActionItem : DirtiableActionItem
    {
        private readonly int index;
        private IList list;
        private IReadOnlyCollection<object> items;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionChangedActionItem"/> class.
        /// </summary>
        /// <param name="name">The name of this action item.</param>
        /// <param name="list">The collection that has been modified.</param>
        /// <param name="actionToUndo">The type of change done in the collection.</param>
        /// <param name="dirtiables">The <see cref="IDirtiable"/> objects that are affected by this action.</param>
        private CollectionChangedActionItem(string name, IList list, NotifyCollectionChangedAction actionToUndo, IEnumerable<IDirtiable> dirtiables)
            : base(name, dirtiables)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (actionToUndo == NotifyCollectionChangedAction.Reset) throw new ArgumentException("Reset is not supported by the undo stack.");
            ActionToUndo = actionToUndo;
            this.list = list;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionChangedActionItem"/> class.
        /// </summary>
        /// <param name="name">The name of this action item.</param>
        /// <param name="list">The collection that has been modified.</param>
        /// <param name="actionToUndo">The type of change done in the collection.</param>
        /// <param name="items">The items affected by the change, either removed or added.</param>
        /// <param name="index">The index at which the change occurred.</param>
        /// <param name="dirtiables">The <see cref="IDirtiable"/> objects that are affected by this action.</param>
        public CollectionChangedActionItem(string name, IList list, NotifyCollectionChangedAction actionToUndo, IReadOnlyCollection<object> items, int index, IEnumerable<IDirtiable> dirtiables)
            : this(name, list, actionToUndo, dirtiables)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            this.items = items;
            this.index = index;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionChangedActionItem"/> class.
        /// </summary>
        /// <param name="name">The name of this action item.</param>
        /// <param name="list">The collection that has been modified.</param>
        /// <param name="args">The arguments of an <see cref="INotifyCollectionChanged.CollectionChanged"/> event corresponding to the change.</param>
        /// <param name="dirtiables">The <see cref="IDirtiable"/> objects that are affected by this action.</param>
        public CollectionChangedActionItem(string name, IList list, NotifyCollectionChangedEventArgs args, IEnumerable<IDirtiable> dirtiables)
            : this(name, list, args.Action, dirtiables)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    items = args.NewItems.Cast<object>().ToArray();
                    index = args.NewStartingIndex;
                    break;
                case NotifyCollectionChangedAction.Move:
                    // Intentionally ignored, move in collection are not tracked
                    return;
                case NotifyCollectionChangedAction.Remove:
                    items = args.OldItems.Cast<object>().ToArray();
                    index = args.OldStartingIndex;
                    break;
                case NotifyCollectionChangedAction.Replace:
                    items = args.OldItems.Cast<object>().ToArray();
                    index = args.OldStartingIndex;
                    break;
                case NotifyCollectionChangedAction.Reset:
                    throw new NotSupportedException("Reset is not supported by the undo stack.");
                default:
                    items = new object[] { };
                    index = -1;
                    break;
            }
        }

        public NotifyCollectionChangedAction ActionToUndo { get; private set; }

        public int ItemCount => items.Count;

        /// <inheritdoc/>
        protected override void FreezeMembers()
        {
            list = null;
            items = null;
        }

        /// <inheritdoc/>
        protected override void UndoAction()
        {
            int i = 0;
            switch (ActionToUndo)
            {
                case NotifyCollectionChangedAction.Add:
                    ActionToUndo = NotifyCollectionChangedAction.Remove;
                    for (i = 0; i < items.Count; ++i)
                    {
                        list.RemoveAt(index);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    ActionToUndo = NotifyCollectionChangedAction.Add;
                    foreach (var item in items)
                    {
                        list.Insert(index + i, item);
                        ++i;
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    var replacedItems = new List<object>();
                    foreach (var item in items)
                    {
                        replacedItems.Add(list[index + i]);
                        list[index + i] = item;
                        ++i;
                    }
                    items = replacedItems;
                    break;
                case NotifyCollectionChangedAction.Move:
                    throw new NotSupportedException("Move is not supported by the undo stack.");
                case NotifyCollectionChangedAction.Reset:
                    throw new NotSupportedException("Reset is not supported by the undo stack.");
            }
        }

        /// <inheritdoc/>
        protected override void RedoAction()
        {
            // Once we have un-done, the previous value is updated so Redo is just Undoing the Undo
            UndoAction();
        }
    }
}
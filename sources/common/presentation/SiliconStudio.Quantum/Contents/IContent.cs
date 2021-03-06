﻿// Copyright (c) 2014 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.
using System;
using System.Collections.Generic;
using SiliconStudio.Core.Reflection;
using SiliconStudio.Quantum.References;

namespace SiliconStudio.Quantum.Contents
{
    /// <summary>
    /// Content of a <see cref="IGraphNode"/>.
    /// </summary>
    public interface IContent
    {
        /// <summary>
        /// Gets the <see cref="IContentNode"/> owning this content.
        /// </summary>
        IContentNode OwnerNode { get; }

        /// <summary>
        /// Gets the expected type of <see cref="Value"/>.
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        object Value { get; }

        /// <summary>
        /// Gets whether this content hold a primitive type value. If so, the node owning this content should have no children and modifying its value should not trigger any node refresh.
        /// </summary>
        /// <remarks>Types registered as primitive types in the <see cref="INodeBuilder"/> used to build this content are taken in account by this property.</remarks>
        bool IsPrimitive { get; }

        /// <summary>
        /// Gets or sets the type descriptor of this content
        /// </summary>
        ITypeDescriptor Descriptor { get; }

        /// <summary>
        /// Gets wheither this content holds a reference or is a direct value.
        /// </summary>
        bool IsReference { get; }

        /// <summary>
        /// Gets the reference hold by this content, if applicable.
        /// </summary>
        IReference Reference { get; }

        /// <summary>
        /// Gets all the indices in the value of this content, if it is a collection. Otherwise, this property returns null.
        /// </summary>
        IEnumerable<object> Indices { get; }

        /// <summary>
        /// Gets whether the <see cref="Reference"/> contained in this content should lead to the creation of model node for the referenced object.
        /// </summary>
        bool ShouldProcessReference { get; }

        /// <summary>
        /// Raised before the <see cref="Value"/> of this content changes and before the <see cref="Changing"/> event is raised.
        /// </summary>
        event EventHandler<ContentChangeEventArgs> PrepareChange;

        /// <summary>
        /// Raised after the <see cref="Value"/> of this content has changed and after the <see cref="Changed"/> event is raised.
        /// </summary>
        event EventHandler<ContentChangeEventArgs> FinalizeChange;

        /// <summary>
        /// Raised just before the <see cref="Value"/> of this content changes.
        /// </summary>
        event EventHandler<ContentChangeEventArgs> Changing;

        /// <summary>
        /// Raised when the <see cref="Value"/> of this content has changed.
        /// </summary>
        event EventHandler<ContentChangeEventArgs> Changed;

        /// <summary>
        /// Retrieves the value of this content or one of its item if it holds a collection.
        /// </summary>
        /// <param name="index">The index to use to retrieve the value, if applicable. index should be <c>null</c> otherwise.</param>
        object Retrieve(object index = null);

        /// <summary>
        /// Retrieves the value of this content or one of its item if it holds a collection.
        /// </summary>
        /// <param name="value">The value of this content.</param>
        /// <param name="index">The index to use to retrieve the value, if applicable. index should be <c>null</c> otherwise.</param>
        object Retrieve(object value, object index);

        /// <summary>
        /// Updates the <see cref="Value"/> property of this content with the given value, at the given index if applicable.
        /// </summary>
        /// <param name="newValue">The new value to set.</param>
        /// <param name="index">The index where to update the value, if applicable. index should be <c>null</c> otherwise.</param>
        void Update(object newValue, object index = null);

        /// <summary>
        /// Adds a new item at the given index to this content, assuming the content is a collection.
        /// </summary>
        /// <param name="itemIndex">The index at which the new item must be added.</param>
        /// <param name="newItem">The new item to add to the collection.</param>
        void Add(object itemIndex, object newItem);

        /// <summary>
        /// Adds a new item to this content, assuming the content is a collection.
        /// </summary>
        /// <param name="newItem">The new item to add to the collection.</param>
        void Add(object newItem);

        /// <summary>
        /// Removes an item from this content, assuming the content is a collection.
        /// </summary>
        /// <param name="itemIndex">The index from which the item must be removed.</param>
        /// <param name="item"></param>
        void Remove(object itemIndex, object item);

        // TODO: implement clear if needed
        //void Clear();
    }
}
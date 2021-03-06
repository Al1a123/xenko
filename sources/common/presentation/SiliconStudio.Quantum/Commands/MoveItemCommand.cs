using System;
using SiliconStudio.Core.Annotations;
using SiliconStudio.Core.Reflection;
using SiliconStudio.Quantum.Contents;

namespace SiliconStudio.Quantum.Commands
{
    // TODO: this command is very similar to RenameStringKeyCommand - try to factorize them
    public class MoveItemCommand : NodeCommandBase
    {
        public const string CommandName = "MoveItem";

        /// <inheritdoc/>
        public override string Name => CommandName;

        /// <inheritdoc/>
        public override CombineMode CombineMode => CombineMode.AlwaysCombine;

        /// <inheritdoc/>
        public override bool CanAttach(ITypeDescriptor typeDescriptor, MemberDescriptorBase memberDescriptor)
        {
            if (memberDescriptor != null)
            {
                var attrib = TypeDescriptorFactory.Default.AttributeRegistry.GetAttribute<MemberCollectionAttribute>(memberDescriptor.MemberInfo);
                if (attrib != null && attrib.ReadOnly)
                    return false;
            }

            var collectionDescriptor = typeDescriptor as CollectionDescriptor;
            return collectionDescriptor != null && collectionDescriptor.HasInsert;
        }

        public override void Execute(IContent content, object index, object parameter)
        {
            var indices = (Tuple<int, int>)parameter;
            var sourceIndex = indices.Item1;
            var targetIndex = indices.Item2;
            var value = content.Retrieve(sourceIndex);
            content.Remove(sourceIndex, value);
            content.Add(targetIndex, value);
        }
    }
}
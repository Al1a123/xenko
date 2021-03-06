﻿// Copyright (c) 2014 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SiliconStudio.Presentation.ViewModel;
using SiliconStudio.Quantum;

namespace SiliconStudio.Presentation.Quantum
{
    public class CombinedNodeCommandWrapper : NodeCommandWrapperBase
    {
        private readonly IReadOnlyCollection<ModelNodeCommandWrapper> commands;

        public CombinedNodeCommandWrapper(IViewModelServiceProvider serviceProvider, string name, IReadOnlyCollection<ModelNodeCommandWrapper> commands)
            : base(serviceProvider)
        {
            if (commands == null) throw new ArgumentNullException(nameof(commands));
            if (commands.Count == 0) throw new ArgumentException(@"The collection of commands to combine is empty", nameof(commands));
            if (commands.Any(x => !ReferenceEquals(x.NodeCommand, commands.First().NodeCommand))) throw new ArgumentException(@"The collection of commands to combine cannot contain different node commands", nameof(commands));
            this.commands = commands;
            Name = name;
        }

        public override string Name { get; }

        public override CombineMode CombineMode => CombineMode.DoNotCombine;

        public override async Task Invoke(object parameter)
        {
            ActionStack?.BeginTransaction();
            commands.First().NodeCommand.StartCombinedInvoke();
            await Task.WhenAll(commands.Select(x => x.Invoke(parameter)));
            commands.First().NodeCommand.EndCombinedInvoke();
            ActionStack?.EndTransaction($"Executed {Name}");
        }
    }
}

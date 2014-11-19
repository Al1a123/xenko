﻿// <auto-generated>
// Do not edit this file yourself!
//
// This code was generated by Paradox Shader Mixin Code Generator.
// To generate it yourself, please install SiliconStudio.Paradox.VisualStudio.Package .vsix
// and re-save the associated .pdxfx.
// </auto-generated>

using System;
using SiliconStudio.Core;
using SiliconStudio.Paradox.Effects;
using SiliconStudio.Paradox.Graphics;
using SiliconStudio.Paradox.Shaders;
using SiliconStudio.Core.Mathematics;
using Buffer = SiliconStudio.Paradox.Graphics.Buffer;


#line 3 "D:\Code\Paradox\sources\shaders\ShadowMap\ShadowMaps.pdxfx"
using SiliconStudio.Paradox.Engine;

#line 5
namespace SiliconStudio.Paradox.Effects.Modules
{

    #line 8
    internal partial class ShadowMapReceiverEffect  : IShaderMixinBuilder
    {
        public void Generate(ShaderMixinSourceTree mixin, ShaderMixinContext context)
        {

            #line 12
            context.Mixin(mixin, "ShadowMapCascadeBase");

            #line 14
            mixin.Mixin.AddMacro("SHADOWMAP_COUNT", context.GetParam(ShadowMapParameters.ShadowMapCount));

            #line 15
            mixin.Mixin.AddMacro("SHADOWMAP_CASCADE_COUNT", context.GetParam(ShadowMapParameters.ShadowMapCascadeCount));

            #line 17
            if (context.GetParam(ShadowMapParameters.FilterType) == ShadowMapFilterType.Nearest)

                #line 18
                context.Mixin(mixin, "ShadowMapFilterDefault");

            #line 19
            else 
#line 19
            if (context.GetParam(ShadowMapParameters.FilterType) == ShadowMapFilterType.PercentageCloserFiltering)

                #line 20
                context.Mixin(mixin, "ShadowMapFilterPcf");

            #line 21
            else 
#line 21
            if (context.GetParam(ShadowMapParameters.FilterType) == ShadowMapFilterType.Variance)

                #line 22
                context.Mixin(mixin, "ShadowMapFilterVsm");
        }

        [ModuleInitializer]
        internal static void __Initialize__()

        {
            ShaderMixinManager.Register("ShadowMapReceiverEffect", new ShadowMapReceiverEffect());
        }
    }

    #line 26
    internal partial class ShadowMapCaster  : IShaderMixinBuilder
    {
        public void Generate(ShaderMixinSourceTree mixin, ShaderMixinContext context)
        {

            #line 31
            context.CloneProperties();

            #line 31
            mixin.Mixin.CloneFrom(mixin.Parent.Mixin);

            #line 32
            context.Mixin(mixin, "ShadowMapCasterBase");

            #line 34
            if (context.GetParam(ShadowMapParameters.FilterType) == ShadowMapFilterType.Variance)

                #line 35
                context.Mixin(mixin, "ShadowMapCasterVsm");
        }

        [ModuleInitializer]
        internal static void __Initialize__()

        {
            ShaderMixinManager.Register("ShadowMapCaster", new ShadowMapCaster());
        }
    }

    #line 39
    internal partial class ShadowMapEffect  : IShaderMixinBuilder
    {
        public void Generate(ShaderMixinSourceTree mixin, ShaderMixinContext context)
        {

            {

                #line 43
                var __subMixin = new ShaderMixinSourceTree() { Name = "ShadowMapCaster", Parent = mixin };
                mixin.Children.Add(__subMixin);

                #line 43
                context.BeginChild(__subMixin);

                #line 43
                context.Mixin(__subMixin, "ShadowMapCaster");

                #line 43
                context.EndChild();
            }

            #line 45
            if (context.GetParam(ShadowMapParameters.ShadowMaps) == null || context.GetParam(ShadowMapParameters.ShadowMaps).Length == 0)

                #line 46
                return;

            #line 48
            context.Mixin(mixin, "ShadowMapReceiver");

            #line 53
            foreach(var ____1 in context.GetParam(ShadowMapParameters.ShadowMaps))

            {

                #line 53
                context.PushParameters(____1);

                {

                    #line 55
                    var __subMixin = new ShaderMixinSourceTree() { Parent = mixin };

                    #line 55
                    context.Mixin(__subMixin, "ShadowMapReceiverEffect");
                    mixin.Mixin.AddCompositionToArray("shadows", __subMixin.Mixin);
                }

                #line 53
                context.PopParameters();
            }
        }

        [ModuleInitializer]
        internal static void __Initialize__()

        {
            ShaderMixinManager.Register("ShadowMapEffect", new ShadowMapEffect());
        }
    }
}

﻿// Copyright (c) 2014-2016 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.

using SiliconStudio.Core;
using SiliconStudio.Core.Mathematics;

namespace SiliconStudio.Xenko.Particles
{
    [DataContract("ParticleTransform")]

    public class ParticleTransform
    {
        [DataMember(0)]
        [Display("Position inheritance")]
        public bool InheritPosition { get; set; } = true;

        [DataMember(1)]
        [Display("Position offset")]
        public Vector3 Position { get; set; } = new Vector3(0, 0, 0);

        [DataMember(2)]
        [Display("Rotation inheritance")]
        public bool InheritRotation { get; set; } = true;

        [DataMember(3)]
        [Display("Rotation offset")]
        public Quaternion Rotation { get; set; } = new Quaternion(0, 0, 0, 1);

        [DataMember(4)]
        [Display("Scale inheritance")]
        public bool InheritScale { get; set; } = true;

        [DataMember(5)]
        [Display("Scale offset")]
        public Vector3 Scale { get; set; } = new Vector3(1, 1, 1);

        [DataMember(5)]
        [Display("Uniform Scale")]
        public float ScaleUniform { get; set; } = 1f;

        // Order of these members should be *after* the fields they control (own offset and inherited field)
        // The visibility is controlled from EntityViewModel.UpdateObservableNode(...)
        [DataMember(10)]
        public bool DisplayParticlePosition = false;

        [DataMember(10)]
        public bool DisplayParticleRotation = false;

        [DataMember(10)]
        public bool DisplayParticleScale = false;

        [DataMember(10)]
        public bool DisplayParticleScaleUniform = false;

        [DataMemberIgnore]
        public Vector3 WorldPosition { get; private set; } = new Vector3(0, 0, 0);

        [DataMemberIgnore]
        public Quaternion WorldRotation { get; private set; } = new Quaternion(0, 0, 0, 1);

        [DataMemberIgnore]
        public Vector3 WorldScale { get; private set; } = new Vector3(1, 1, 1);


        public void SetParentTransform(ref Vector3 translation, ref Quaternion rotation, float scale)
        {
            var ownScale = Scale * ScaleUniform;
            WorldScale = (InheritScale) ? ownScale * scale : ownScale;

            WorldRotation = (InheritRotation) ? Rotation * rotation : Rotation;

            var offsetTranslation = Position * ((InheritScale) ? scale : 1f);

            if (InheritRotation)
            {
                rotation.Rotate(ref offsetTranslation);
            }

            WorldPosition = (InheritPosition) ? translation + offsetTranslation : offsetTranslation;
        }
    }
}


﻿// Copyright (c) 2014 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.

using System;
using System.ComponentModel;
using SiliconStudio.Core;
using SiliconStudio.Core.Annotations;
using SiliconStudio.Core.Collections;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko.Particles.BoundingShapes;
using SiliconStudio.Xenko.Particles.DebugDraw;

namespace SiliconStudio.Xenko.Particles
{
    [DataContract("ParticleSystem")]
    public class ParticleSystem : IDisposable
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ParticleSystem"/> is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        [DataMember(-10)]
        [DefaultValue(true)]
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Fixes local space location back to world space location. Used for debug drawing.
        /// </summary>
        /// <param name="translation">The locator's translation</param>
        /// <param name="rotation">The locator's quaternion rotation</param>
        /// <param name="scale">The locator's non-uniform scaling</param>
        /// <returns></returns>
        private bool ToWorldSpace(ref Vector3 translation, ref Quaternion rotation, ref Vector3 scale)
        {
            scale *= UniformScale;

            rotation *= Rotation;

            Rotation.Rotate(ref translation);
            translation *= UniformScale;
            translation += Translation;

            return true;
        }

        /// <summary>
        /// Tries to acquire and draw a debug shape for better feedback and visualization.
        /// </summary>
        /// <param name="debugDrawShape">The type of the debug shape (sphere, cone, etc.)</param>
        /// <param name="translation">The shape's translation</param>
        /// <param name="rotation">The shape's rotation</param>
        /// <param name="scale">The shape's non-uniform scaling</param>
        /// <returns><c>true</c> if debug shape can be displayed</returns>
        public bool TryGetDebugDrawShape(ref DebugDrawShape debugDrawShape, ref Vector3 translation, ref Quaternion rotation, ref Vector3 scale)
        {
            foreach (var particleEmitter in Emitters)
            {
                foreach (var initializer in particleEmitter.Initializers)
                {
                    if (initializer.TryGetDebugDrawShape(out debugDrawShape, out translation, out rotation, out scale))
                    {
                        // Convert to world space if local
                        if (particleEmitter.SimulationSpace == EmitterSimulationSpace.Local)
                            return ToWorldSpace(ref translation, ref rotation, ref scale);

                        return true;
                    }
                }

                foreach (var updater in particleEmitter.Updaters)
                {
                    if (updater.TryGetDebugDrawShape(out debugDrawShape, out translation, out rotation, out scale))
                    {
                        // Convert to world space if local
                        if (particleEmitter.SimulationSpace == EmitterSimulationSpace.Local)
                            return ToWorldSpace(ref translation, ref rotation, ref scale);

                        return true;
                    }
                }
            }

            if (BoundingShape == null)
                return false;

            if (BoundingShape.TryGetDebugDrawShape(out debugDrawShape, out translation, out rotation, out scale))
                return ToWorldSpace(ref translation, ref rotation, ref scale);

            return false;
        }

        /// <summary>
        /// Settings class which contains miscellaneous settings for the particle system
        /// </summary>
        /// <userdoc>
        /// Miscellaneous settings for the particle system. These settings are intended to be shared and are set during authoring of the particle system
        /// </userdoc>
        [DataMember(3)]
        [NotNull]
        [Display("Settings")]
        public ParticleSystemSettings Settings { get; set; } = new ParticleSystemSettings();

        /// <summary>
        /// AABB of this Particle System
        /// </summary>
        /// <userdoc>
        /// AABB (Axis-Aligned Bounding Box) used for fast culling and optimizations. Can be specified by the user. Leave it Null to disable culling.
        /// </userdoc>
        [DataMember(5)]
        [Display("Bounding Shape")]
        public BoundingShape BoundingShape { get; set; } = null;

        /// <summary>
        /// Gets the current AABB of the <see cref="ParticleSystem"/>
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public BoundingBox GetAABB()
        {
            return BoundingShape?.GetAABB(Translation, Rotation, UniformScale) ?? new BoundingBox(Translation, Translation);
        }

        private readonly SafeList<ParticleEmitter> emitters;
        /// <summary>
        /// List of Emitters in this <see cref="ParticleSystem"/>. Each Emitter has a separate <see cref="ParticlePool"/> (group) of Particles in it
        /// </summary>
        /// <userdoc>
        /// List of emitters in this particle system. Each Emitter has a separate particle pool (group) of particles in it
        /// </userdoc>
        [DataMember(10)]
        [Display("Emitters")]
        //[NotNullItems] // This attribute is not supported for non-derived classes
        [MemberCollection(CanReorderItems = true)]
        public SafeList<ParticleEmitter> Emitters
        {
            get
            {
                return emitters;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ParticleSystem()
        {
            emitters = new SafeList<ParticleEmitter>();
        }

        /// <summary>
        /// Translation of the ParticleSystem. Usually inherited directly from the ParticleSystemComponent or can be directly set.
        /// </summary>
        /// <userdoc>
        /// Translation of the ParticleSystem. Usually inherited directly from the ParticleSystemComponent or can be directly set.
        /// </userdoc>
        [DataMemberIgnore]
        public Vector3 Translation = new Vector3(0, 0, 0);

        /// <summary>
        /// Rotation of the ParticleSystem, expressed as a quaternion rotation. Usually inherited directly from the ParticleSystemComponent or can be directly set.
        /// </summary>
        /// <userdoc>
        /// Rotation of the ParticleSystem, expressed as a quaternion rotation. Usually inherited directly from the ParticleSystemComponent or can be directly set.
        /// </userdoc>
        [DataMemberIgnore]
        public Quaternion Rotation = new Quaternion(0, 0, 0, 1);

        /// <summary>
        /// Scale of the ParticleSystem. Only uniform scale is supported. Usually inherited directly from the ParticleSystemComponent or can be directly set.
        /// </summary>
        /// <userdoc>
        /// Scale of the ParticleSystem. Only uniform scale is supported. Usually inherited directly from the ParticleSystemComponent or can be directly set.
        /// </userdoc>
        [DataMemberIgnore]
        public float UniformScale = 1f;

        /// <summary>
        /// Updates the particles
        /// </summary>
        /// <param name="dt">Delta time - time, in seconds, elapsed since the last Update call to this particle system</param>
        /// <userdoc>
        /// Updates the particle system and all particles contained within. Delta time is the time, in seconds, which has passed since the last Update call.
        /// </userdoc>
        public void Update(float dt)
        {
            if (BoundingShape != null) BoundingShape.Dirty = true;

            // If the particle system is paused skip the rest of the update state
            if (isPaused)
            {
                foreach (var particleEmitter in Emitters)
                {
                    if (particleEmitter.Enabled)
                    {
                        particleEmitter.UpdatePaused(this);
                    }
                }

                return;
            }

            // If the particle system hasn't started yet do it now
            //  This includes warming up the system by simulating the emitters in background
            if (!hasStarted)
            {
                hasStarted = true;
                if (Settings.WarmupTime > 0)
                {
                    var remainingTime = Settings.WarmupTime;
                    var timeStep = 1f/30f;
                    while (remainingTime > 0)
                    {
                        var warmingUp = Math.Min(remainingTime, timeStep);

                        foreach (var particleEmitter in Emitters)
                        {
                            if (particleEmitter.Enabled)
                            {
                                particleEmitter.Update(warmingUp, this);
                            }
                        }

                        remainingTime -= warmingUp;
                    }
                    
                }
            }

            // Update all the emitters by delta time
            foreach (var particleEmitter in Emitters)
            {
                if (particleEmitter.Enabled)
                {
                    particleEmitter.Update(dt, this);
                }
            }            
        }

        /// <summary>
        /// Resets the particle system, resetting all values to their initial state
        /// </summary>
        public void ResetSimulation()
        {
            foreach (var particleEmitter in Emitters)
            {
                particleEmitter.ResetSimulation();
            }

            hasStarted = false;
        }

        /// <summary>
        /// isPaused shows if the simulation progresses by delta time every frame or no
        /// </summary>
        private bool isPaused;

        /// <summary>
        /// hasStarted shows if the simulation has started yet or no
        /// </summary>
        private bool hasStarted;

        /// <summary>
        /// Pauses the particle system simulation
        /// </summary>
        public void Pause()
        {
            isPaused = true;
        }

        /// <summary>
        /// Use to both start a new simulation or continue a paused one
        /// </summary>
        public void Play()
        {
            isPaused = false;
        }

        /// <summary>
        /// Stops the particle simulation by resetting it to its initial state and pausing it
        /// </summary>
        public void Stop()
        {
            ResetSimulation();
            isPaused = true;
        }



        #region Dispose
        private bool disposed;

        ~ParticleSystem()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;
            disposed = true;

            // Dispose unmanaged resources

            if (!disposing)
                return;

            // Dispose managed resources
            foreach (var particleEmitter in Emitters)
            {
                particleEmitter.Dispose();
            }

            Emitters.Clear();
        }
        #endregion Dispose

    }
}

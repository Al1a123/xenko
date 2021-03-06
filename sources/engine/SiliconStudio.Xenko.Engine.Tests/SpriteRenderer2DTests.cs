﻿// Copyright (c) 2014 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko.Games;
using SiliconStudio.Xenko.Graphics;
using SiliconStudio.Xenko.Rendering.Sprites;

namespace SiliconStudio.Xenko.Engine.Tests
{
    public class SpriteRenderer2DTests : EngineTestBase
    {
        private float currentTime;

        private readonly List<Entity> rotatingSprites = new List<Entity>();
        private List<Entity> entities = new List<Entity>();
        private Entity animatedBall;

        private const int ScreenWidth = 1024;
        private const int ScreenHeight = 780;

        public SpriteRenderer2DTests()
        {
            CurrentVersion = 3;
            GraphicsDeviceManager.PreferredBackBufferWidth = ScreenWidth;
            GraphicsDeviceManager.PreferredBackBufferHeight = ScreenHeight;
        }

        private Entity CreateSpriteEntity(SpriteSheet sheet, string frameName)
        {
            var entity = new Entity(frameName)
            {
                new SpriteComponent
                {
                    SpriteProvider = new SpriteFromSheet { Sheet = sheet },
                    CurrentFrame = sheet.FindImageIndex(frameName)
                }
            };

            entities.Add(entity);

            return entity;
        }

        protected override async Task LoadContent()
        {
            await base.LoadContent();

            var debugSheet = Content.Load<SpriteSheet>("DebugSpriteSheet");

            // current frame test
            animatedBall = CreateSpriteEntity(Content.Load<SpriteSheet>("BallSprite1"), "sphere1");
            animatedBall.Transform.Position = new Vector3(75, 75, 0);

            // normal reference one
            var normal = CreateSpriteEntity(debugSheet, "Normal");
            normal.Transform.Position = new Vector3(150, 300, 0);

            // color
            var color = CreateSpriteEntity(debugSheet, "Color");
            color.Transform.Position = new Vector3(0, 300, 0);
            color.Get<SpriteComponent>().Color = Color.Purple;

            // billboard
            var billboard = CreateSpriteEntity(debugSheet, "Billboard");
            billboard.Transform.Position = new Vector3(150, 150, 0);
            billboard.Get<SpriteComponent>().SpriteType = SpriteType.Billboard;

            // ratio
            var ratio = CreateSpriteEntity(debugSheet, "OtherRatio");
            ratio.Transform.Position = new Vector3(300, 150, 0);

            // Pre-multiplied Alpha
            var pAlpha = CreateSpriteEntity(debugSheet, "PAlpha");
            pAlpha.Transform.Position = new Vector3(400, 150, 0);
            pAlpha.Get<SpriteComponent>().PremultipliedAlpha = true;

            // Not Pre-multiplied alpha
            var npAlpha = CreateSpriteEntity(debugSheet, "NPAlpha");
            npAlpha.Transform.Position = new Vector3(550, 150, 0);
            npAlpha.Get<SpriteComponent>().PremultipliedAlpha = false;

            // depth test
            var onBack = CreateSpriteEntity(debugSheet, "OnBack");
            onBack.Transform.Position = new Vector3(0, 450, 0);
            var onFront = CreateSpriteEntity(debugSheet, "OnFront");
            onFront.Transform.Position = new Vector3(0, 550, 1);
            var noDepth = CreateSpriteEntity(debugSheet, "NoDepth");
            noDepth.Transform.Position = new Vector3(0, 650, 0);
            noDepth.Get<SpriteComponent>().IgnoreDepth = true;

            // create the rotating sprites
            rotatingSprites.Add(CreateSpriteEntity(debugSheet, "Center"));
            rotatingSprites.Add(CreateSpriteEntity(debugSheet, "TopLeft"));
            rotatingSprites.Add(CreateSpriteEntity(debugSheet, "OutOfImage"));

            // Invalid sprites
            CreateSpriteEntity(debugSheet, "NoTexture");
            CreateSpriteEntity(debugSheet, "NullWidth");
            CreateSpriteEntity(debugSheet, "NegativeHeight");

            // region out of bound
            var regionOutBound = CreateSpriteEntity(debugSheet, "ShiftedRegion");
            regionOutBound.Transform.Position = new Vector3(700, 250, 0);

            for (int i = 0; i < rotatingSprites.Count; i++)
                rotatingSprites[i].Transform.Position = new Vector3(ScreenWidth, ScreenHeight, i) / 2;

            // add all the entities to the scene
            foreach (var entity in entities)
                SceneSystem.SceneInstance.Scene.Entities.Add(entity);

            CameraComponent.UseCustomProjectionMatrix = true;
            CameraComponent.ProjectionMatrix = Matrix.OrthoOffCenterRH(0, ScreenWidth, 0, ScreenHeight, -10, 10);
        }

        protected override void RegisterTests()
        {
            base.RegisterTests();

            FrameGameSystem.TakeScreenshot();
            FrameGameSystem.Draw(() => UpdateSprites(0.6f)).TakeScreenshot();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            const float speed = 1 / 60f;
            currentTime += speed;

            if (!ScreenShotAutomationEnabled)
                UpdateSprites(currentTime);
        }

        private void UpdateSprites(float time)
        {
            animatedBall.Get<SpriteComponent>().CurrentFrame = (int)(time * 60);

            foreach (var entity in rotatingSprites)
            {
                entity.Transform.RotationEulerXYZ = new Vector3(0, 0, time);
            }
        }

        [Test]
        public void SpriteRender2DRun()
        {
            RunGameTest(new SpriteRenderer2DTests());
        }

        public static void Main()
        {
            using (var game = new SpriteRenderer2DTests())
                game.Run();
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Asteroid_Belt_Assault
{
    class PlayerManager
    {
        public Sprite playerSprite;
        public float playerSpeed = 320.0f;
        private Rectangle playerAreaLimit;
        private float timePlayed;

        public long PlayerScore = 0;
        public int LivesRemaining = 6;
        private bool destroyed = false;
        public static bool Dead = false;
        public bool isD4 = false;

        private Vector2 gunOffset = new Vector2(33, 10);
        private float shotTimer = 0.0f;
        private float minShotTimer = 0.2f;
        private int playerRadius = 15;
        public ShotManager PlayerShotManager;

        public int PowerupLevel = 1;

        public bool Destroyed {
            get { return this.destroyed; }
            set {
                    this.destroyed = value;
                PlayerManager.Dead = value;
            } }

        public PlayerManager(
            Texture2D texture,  
            Rectangle initialFrame,
            int frameCount,
            Rectangle screenBounds)
        {
            playerSprite = new Sprite(
                new Vector2(500, 500),
                texture,
                initialFrame,
                Vector2.Zero);

            PlayerShotManager = new ShotManager(
                texture,
                new Rectangle(0, 300, 5, 5),
                4,
                2,
                250f,
                screenBounds, false);

               
            playerAreaLimit =
                new Rectangle(
                    0,
                    screenBounds.Height / 2,
                    screenBounds.Width,
                    screenBounds.Height / 2);
                    
            for (int x = 1; x < frameCount; x++)
            {
                playerSprite.AddFrame(
                    new Rectangle(
                        initialFrame.X + (initialFrame.Width * x),
                        initialFrame.Y,
                        initialFrame.Width,
                        initialFrame.Height));
            }
            playerSprite.CollisionRadius = playerRadius;
        }

        private void FireShot()
        {
            if (shotTimer >= minShotTimer && !isD4) 
            {
                PlayerShotManager.FireShot(
                    playerSprite.Location + gunOffset,
                    new Vector2(0, -1),
                    true);
                shotTimer = 0.0f;

                if (PowerupLevel == 2)
                {

                    for (int i = -60; i <= 60; i += 30)
                    {
                        Vector2 newShot = new Vector2(i, -100);
                        newShot.Normalize();
                        PlayerShotManager.FireShot(
                            playerSprite.Location + gunOffset,
                            newShot,
                            true);
                    }
                }
            }

            if (isD4 && shotTimer >= minShotTimer)
            {
                shotTimer = 0.0f;
                for (int i = -60; i <= 60; i += 30)
                {
                    Vector2 newShotR = new Vector2(20, -20);
                    newShotR.Normalize();

                    PlayerShotManager.FireShot(
                        playerSprite.Location + gunOffset,
                        newShotR,
                        true);


                    Vector2 newShotL = new Vector2(-20, -20);
                    newShotL.Normalize();

                    PlayerShotManager.FireShot(
                       playerSprite.Location + gunOffset,
                       newShotL,
                       true);
                }

                if (PowerupLevel == 2)
                {

                    for (int i = -60; i <= 60; i += 30)
                    {
                        Vector2 newShot = new Vector2(i, -100);
                        newShot.Normalize();
                        PlayerShotManager.FireShot(
                            playerSprite.Location + gunOffset,
                            newShot,
                            true);
                    }
                }
            }
        }

        private void HandleKeyboardInput(KeyboardState keyState)
        {
            if (keyState.IsKeyDown(Keys.Up))
            {
                playerSprite.Velocity += new Vector2(0, -1);
            }

            if (keyState.IsKeyDown(Keys.Down))
            {
                playerSprite.Velocity += new Vector2(0, 1);
            }

            if (keyState.IsKeyDown(Keys.Left))
            {
                playerSprite.Velocity += new Vector2(-1, 0);
            }

            if (keyState.IsKeyDown(Keys.Right))
            {
                playerSprite.Velocity += new Vector2(1, 0);
            }

            if (keyState.IsKeyDown(Keys.Space))
            {
                FireShot();
            }
        }

        private void HandleGamepadInput(GamePadState gamePadState)
        {
            playerSprite.Velocity +=
                new Vector2(
                    gamePadState.ThumbSticks.Left.X,
                    -gamePadState.ThumbSticks.Left.Y);

            if (gamePadState.Buttons.A == ButtonState.Pressed)
            {
                FireShot();
            }
        }

        private void imposeMovementLimits()
        {
            Vector2 location = playerSprite.Location;

            if (location.X < playerAreaLimit.X)
                location.X = playerAreaLimit.X;

            if (location.X >
                (playerAreaLimit.Right - playerSprite.Source.Width))
                location.X =
                    (playerAreaLimit.Right - playerSprite.Source.Width);

            if (location.Y < playerAreaLimit.Y)
                location.Y = playerAreaLimit.Y;

            if (location.Y >
                (playerAreaLimit.Bottom - playerSprite.Source.Height))
                location.Y =
                    (playerAreaLimit.Bottom - playerSprite.Source.Height);

            playerSprite.Location = location;
        }

        public void Update(GameTime gameTime)
        {
            PlayerShotManager.Update(gameTime);

            if (!Destroyed)
            {
                playerSprite.Velocity = Vector2.Zero;

                shotTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                HandleKeyboardInput(Keyboard.GetState());
                HandleGamepadInput(GamePad.GetState(PlayerIndex.One));

                playerSprite.Velocity.Normalize();
                playerSprite.Velocity *= playerSpeed;

                playerSprite.Update(gameTime);
                imposeMovementLimits();
            }
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            PlayerShotManager.Draw(spriteBatch);
            

            if (!Destroyed)
            {
                
                //EffectManager.Effect("ShieldsUp").Trigger(playerSprite.Center + new Vector2(8, 0));
                EffectManager.Effect("ShipSmokeTrail").Trigger(playerSprite.Center + new Vector2(8, 28));
                playerSprite.Draw(spriteBatch);
            }
        }

    }
}

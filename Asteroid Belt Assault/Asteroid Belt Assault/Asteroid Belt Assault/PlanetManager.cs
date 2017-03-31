using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Asteroid_Belt_Assault
{
    class PlanetManager
    {
        private List<Sprite> planets = new List<Sprite>();
        private List<Rectangle> planetRects = new List<Rectangle>();
        private int screenWidth = 800;
        private int screenHeight = 600;
        private Random rand;
        int currentPlanet = 0;

        public static int seed = 0;

        public PlanetManager(
            int screenWidth,
            int screenHeight,
            Vector2 starVelocity,
            Texture2D texture)
        {
            seed += System.Environment.TickCount;
            rand = new Random(seed);

            planetRects.Add(new Rectangle(540, 230, 175, 175));
            planetRects.Add(new Rectangle(840, 140, 150, 150));


            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;

            foreach (Rectangle r in planetRects)
            {
                planets.Add(new Sprite(
                    new Vector2(rand.Next(0, screenWidth),
                        -200),
                    texture,
                    r,
                    Vector2.Zero));
            }

            currentPlanet = rand.Next(0, planets.Count);
            planets[currentPlanet].Velocity = new Vector2(0, 50);

        }

        public void Update(GameTime gameTime)
        {
            foreach (Sprite star in planets)
            {
                star.Update(gameTime);
                if (star.Location.Y > screenHeight)
                {

                    star.Location = new Vector2(
                        rand.Next(0, screenWidth), -200);
                    star.Velocity = Vector2.Zero;

                    currentPlanet = (currentPlanet + 1) % planets.Count;

                    planets[currentPlanet].Velocity = new Vector2(0, 50);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Sprite star in planets)
            {
                star.Draw(spriteBatch);
            }
        }

    }

}
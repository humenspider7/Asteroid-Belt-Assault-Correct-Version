using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Asteroid_Belt_Assault
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        enum GameStates { TitleScreen, DifficultySelect, Playing, PlayerDead, GameOver, Pause };
        GameStates gameState = GameStates.TitleScreen;
        Texture2D titleScreen;
        Texture2D spriteSheet;
        Texture2D planetSheet;
        Texture2D levelScreen;

        int difficultyLevel = 1;

        StarField starField;
        AsteroidManager asteroidManager;
        PlayerManager playerManager;
        EnemyManager enemyManager;
        ExplosionManager explosionManager;
        PlanetManager planetManager;
        PowerUpManager powerupManager;

        CollisionManager collisionManager;

        SpriteFont pericles14;

        private float playerDeathDelayTime = 10f;
        private float playerDeathTimer = 0f;
        private float titleScreenTimer = 0f;
        private float titleScreenDelayTime = 1f;

        private int playerStartingLives = 3;
        private Vector2 playerStartLocation = new Vector2(390, 550);
        private Vector2 scoreLocation = new Vector2(20, 10);
        private Vector2 livesLocation = new Vector2(20, 25);


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            titleScreen = Content.Load<Texture2D>(@"Textures\TitleScreen");
            spriteSheet = Content.Load<Texture2D>(@"Textures\spriteSheet");
            planetSheet = Content.Load<Texture2D>(@"Textures\PlanetSheet");
            levelScreen = Content.Load<Texture2D>(@"Textures\LevelSelect");

            planetManager = new PlanetManager(
                this.Window.ClientBounds.Width,
                this.Window.ClientBounds.Height,
                new Vector2(0, 40f),
                planetSheet);


            starField = new StarField(
                this.Window.ClientBounds.Width,
                this.Window.ClientBounds.Height,
                200,
                new Vector2(0, 30f),
                spriteSheet,
                new Rectangle(0, 450, 2, 2));

            asteroidManager = new AsteroidManager(
                10,
                spriteSheet,
                new Rectangle(0, 0, 50, 50),
                10,
                this.Window.ClientBounds.Width,
                this.Window.ClientBounds.Height);

            playerManager = new PlayerManager(
                spriteSheet,    
                new Rectangle(170, 265, 50, 85),    
                1,
                new Rectangle(
                    0,
                    0,
                    this.Window.ClientBounds.Width,
                    this.Window.ClientBounds.Height));

            enemyManager = new EnemyManager(
                spriteSheet,
                new Rectangle(0, 200, 50, 50),
                6,
                playerManager,
                new Rectangle(
                    0,
                    0,
                    this.Window.ClientBounds.Width,
                    this.Window.ClientBounds.Height));

            explosionManager = new ExplosionManager(
                spriteSheet,
                new Rectangle(0, 100, 50, 50),
                3,
                new Rectangle(0, 450, 2, 2));

            collisionManager = new CollisionManager(
                asteroidManager,
                playerManager,
                enemyManager,
                explosionManager);

            SoundManager.Initialize(Content);

            pericles14 = Content.Load<SpriteFont>(@"Fonts\Pericles14");


            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        private void resetGame()
        {
            playerManager.playerSprite.Location = playerStartLocation;
            foreach (Sprite asteroid in asteroidManager.Asteroids)
            {
                asteroid.Location = new Vector2(-500, -500);
            }
            enemyManager.Enemies.Clear();
            enemyManager.Active = true;
            playerManager.PlayerShotManager.Shots.Clear();
            enemyManager.EnemyShotManager.Shots.Clear();
            playerManager.Destroyed = false;
        }
        
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            switch (gameState)
            {
                case GameStates.TitleScreen:
                    titleScreenTimer +=
                        (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (titleScreenTimer >= titleScreenDelayTime)
                    {
                        if ((Keyboard.GetState().IsKeyDown(Keys.Space)) ||
                            (GamePad.GetState(PlayerIndex.One).Buttons.A ==
                            ButtonState.Pressed))
                        {
                            playerManager.LivesRemaining = playerStartingLives;
                            playerManager.PlayerScore = 0;
                            resetGame();
                            gameState = GameStates.DifficultySelect;
                        }
                    }
                    break;

                case GameStates.Pause:
                    KeyboardState kb = Keyboard.GetState();
                    if(kb.IsKeyDown(Keys.P))
                    {
                        gameState = GameStates.Playing;
                    }

                    break;
                case GameStates.DifficultySelect:
                    //WORK ON DIFFICULTY
                     kb = Keyboard.GetState();

                    if (kb.IsKeyDown(Keys.D1))
                    { 
                        //CHANGING DIFFICULTY LEVEL
                        difficultyLevel = 1;
                        gameState = GameStates.Playing;


                        //LIVES - 1, because the game includes 0 as a life.  Total lives are lives remaining + 1.  Fixed it in the display, so life 1 is your last life.
                        playerManager.LivesRemaining = 5;

                        //ENEMIES
                        enemyManager.MinShipsPerWave = 2;
                        enemyManager.MaxShipsPerWave = 4;

                        //ASTEROIDS
                        asteroidManager.minSpeed = 40;
                        asteroidManager.maxSpeed = 80;

                        //SCORES
                        collisionManager.enemyPointValue = 50;

                    }
                    else if (kb.IsKeyDown(Keys.D2))
                    {
                        difficultyLevel = 2;
                        gameState = GameStates.Playing;

                        //LIVES
                        playerManager.LivesRemaining = 2;

                        //ENEMIES
                        enemyManager.MinShipsPerWave = 6;
                        enemyManager.MaxShipsPerWave = 8;

                        //ASTEROIDS
                        asteroidManager.minSpeed = 80;
                        asteroidManager.maxSpeed = 160;

                        //SCORES
                        collisionManager.enemyPointValue = 100;

                    }

                    else if (kb.IsKeyDown(Keys.D3))
                    {
                        difficultyLevel = 3;
                        gameState = GameStates.Playing;
                       
                        //LIVES
                        playerManager.LivesRemaining = 1;

                        //ENEMIES
                        enemyManager.MinShipsPerWave = 12;
                        enemyManager.MaxShipsPerWave = 16;

                        //ASTEROIDS
                        asteroidManager.minSpeed = 160;
                        asteroidManager.maxSpeed = 200;

                        //SCORES
                        collisionManager.enemyPointValue = 200;
                    }

                    else if (kb.IsKeyDown(Keys.D4))
                    {
                        difficultyLevel = 4;
                        gameState = GameStates.Playing;

                        //LIVES
                        playerManager.LivesRemaining = 0;

                        //ENEMIES
                        enemyManager.MinShipsPerWave = 20;
                        enemyManager.MaxShipsPerWave = 24;

                        //ASTEROIDS
                        asteroidManager.minSpeed = 400;
                        asteroidManager.maxSpeed = 420;

                        //SCORES
                        collisionManager.enemyPointValue = 400;
                    }
                    break;

                case GameStates.Playing:

                    starField.Update(gameTime);
                    asteroidManager.Update(gameTime);
                    playerManager.Update(gameTime);
                    enemyManager.Update(gameTime);
                    explosionManager.Update(gameTime);
                    collisionManager.CheckCollisions();
                    planetManager.Update(gameTime);

                    if (playerManager.Destroyed)
                    {
                        playerDeathTimer = 0f;
                        enemyManager.Active = false;
                        playerManager.LivesRemaining--;
                        if (playerManager.LivesRemaining < 0)
                        {
                            gameState = GameStates.GameOver;
                        }
                        else
                        {
                            gameState = GameStates.PlayerDead;
                        }
                    }
                    //RESET FUNCTION
                    kb = Keyboard.GetState();
                    if (kb.IsKeyDown(Keys.R))
                    {
                        resetGame();
                        gameState = GameStates.DifficultySelect;
                        playerManager.PlayerScore = 0;
                     }
                    //PAUSE FUNCTION
                    if(kb.IsKeyDown(Keys.P))
                    {
                            gameState = GameStates.Pause;

                    }

                    //CHEAT CODES FOR LOSER
                    //Press 6 & 9 on Keypad to change lives to 69.
                    if (kb.IsKeyDown(Keys.D6) && kb.IsKeyDown(Keys.D9))
                    {
                        playerManager.LivesRemaining = 68;
                    }

                    //Change Player Speed when pressing 
                    if (kb.IsKeyDown(Keys.D1) && kb.IsKeyDown(Keys.D2))
                    {
                        playerManager.Destroyed = true;
                    }
                    break;

                case GameStates.PlayerDead:
                    playerDeathTimer +=
                        (float)gameTime.ElapsedGameTime.TotalSeconds;

                    starField.Update(gameTime);
                    asteroidManager.Update(gameTime);
                    enemyManager.Update(gameTime);
                    playerManager.PlayerShotManager.Update(gameTime);
                    explosionManager.Update(gameTime);
                    planetManager.Update(gameTime);


                    if (playerDeathTimer >= playerDeathDelayTime)
                    {
                        resetGame();
                        gameState = GameStates.Playing;
                    }
                    //RESET FUNCTION
                    kb = Keyboard.GetState();
                    if (kb.IsKeyDown(Keys.R))
                    {
                        resetGame();
                        gameState = GameStates.DifficultySelect;
                        playerManager.PlayerScore = 0;
                    }

                    break;

                case GameStates.GameOver:
                    playerDeathTimer +=
                        (float)gameTime.ElapsedGameTime.TotalSeconds;
                    starField.Update(gameTime);
                    asteroidManager.Update(gameTime);
                    enemyManager.Update(gameTime);
                    playerManager.PlayerShotManager.Update(gameTime);
                    explosionManager.Update(gameTime);
                    planetManager.Update(gameTime);

                    if (playerDeathTimer >= playerDeathDelayTime)
                    {
                        gameState = GameStates.TitleScreen;
                    }
                    //RESET FUNCTION
                    kb = Keyboard.GetState();
                    if (kb.IsKeyDown(Keys.R))
                    {
                        resetGame();
                        gameState = GameStates.DifficultySelect;
                        playerManager.PlayerScore = 0;
                    }
                    break;

            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            if (gameState == GameStates.TitleScreen)
            {
                spriteBatch.Draw(titleScreen,
                    new Rectangle(0, 0, this.Window.ClientBounds.Width,
                        this.Window.ClientBounds.Height),
                        Color.White);
            }

            if (gameState == GameStates.DifficultySelect)
            {
                spriteBatch.Draw(levelScreen,
                    new Rectangle(0, 0, this.Window.ClientBounds.Width,
                        this.Window.ClientBounds.Height),
                        Color.White);
            }

            if ((gameState == GameStates.Playing) ||
                (gameState == GameStates.PlayerDead) ||
                (gameState == GameStates.GameOver))
            {
                starField.Draw(spriteBatch);
                planetManager.Draw(spriteBatch);
                asteroidManager.Draw(spriteBatch);
                playerManager.Draw(spriteBatch);
                enemyManager.Draw(spriteBatch);
                explosionManager.Draw(spriteBatch);


                spriteBatch.DrawString(
                    pericles14,
                    "Score: " + playerManager.PlayerScore.ToString(),
                    scoreLocation,
                    Color.White);

                if (playerManager.LivesRemaining >= 0)
                {
                    spriteBatch.DrawString(
                        pericles14,
                        "Ships Remaining: " +
                            (playerManager.LivesRemaining + 1).ToString(),
                        livesLocation,
                        Color.White);
                }
            }

            if ((gameState == GameStates.GameOver))
            {
                spriteBatch.DrawString(
                    pericles14,
                    "G A M E  O V E R ! \nY O U  A R E  B A D!",
                    new Vector2(
                        this.Window.ClientBounds.Width / 2 -
                          pericles14.MeasureString("G A M E  O V E R !").X / 2,
                        50),
                    Color.White);
            }


            spriteBatch.End();

            base.Draw(gameTime);
        }

    }
}

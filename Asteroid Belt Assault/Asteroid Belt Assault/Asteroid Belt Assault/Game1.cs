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

        /*
         ISSUES

        Need to add Super Saiyan aura as a shield for the S3 Cheat.

        Also make shots faster when in S3.
        Add V shots for Difficulty 4.  Only shoots in V.

         */

        enum GameStates { TitleScreen, DifficultySelect, Playing, PlayerDead, GameOver, ccViewer, CreditsScreen, Pause };
        GameStates gameState = GameStates.TitleScreen;
        Texture2D titleScreen;
        Texture2D spriteSheet;
        Texture2D planetSheet;
        Texture2D levelScreen;
        Texture2D ccViewer;
        Texture2D credits;
        Texture2D weedBG;

        int difficultyLevel;

        StarField starField;
        AsteroidManager asteroidManager;
        PlayerManager playerManager;
        EnemyManager enemyManager;
        ExplosionManager explosionManager;
        PlanetManager planetManager;

        CollisionManager collisionManager;

        SpriteFont pericles14;

        private float playerDeathDelayTime = 3f; //THIS CONTROLS RESPAWN TIME.  Original is 10f.
        private float playerDeathTimer = 0f;
        private float titleScreenTimer = 0f;
        private float titleScreenDelayTime = 1f;
        private float playerTimePlayed = 0f;
        public bool isWeed = false;

        private int playerStartingLives = 3;
        private Vector2 playerStartLocation = new Vector2(390, 550);
        private Vector2 scoreLocation = new Vector2(20, 10);
        private Vector2 livesLocation = new Vector2(20, 25);
        private Vector2 timerLocation = new Vector2(20, 10);


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            //graphics.IsFullScreen = true;
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

            EffectManager.Initialize(this.graphics, this.Content);
            EffectManager.LoadContent();

            titleScreen = Content.Load<Texture2D>(@"Textures\TitleScreen");
            spriteSheet = Content.Load<Texture2D>(@"Textures\spriteSheet");
            planetSheet = Content.Load<Texture2D>(@"Textures\PlanetSheet");
            levelScreen = Content.Load<Texture2D>(@"Textures\LevelSelect");
            ccViewer = Content.Load<Texture2D>(@"Textures\CheatCodes");
            credits = Content.Load<Texture2D>(@"Textures\Credits");
            weedBG = Content.Load<Texture2D>(@"Textures\420");


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
            playerManager.PowerupLevel = 1;
            SoundManager.StopSong();

            starField = new StarField(
               this.Window.ClientBounds.Width,
               this.Window.ClientBounds.Height,
               200,
               new Vector2(0, 30f),
               spriteSheet,
               new Rectangle(0, 450, 2, 2));

            //Add something to stop audio playing here.
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

            EffectManager.Update(gameTime);

            // TODO: Add your update logic here
            KeyboardState kb = Keyboard.GetState();

            switch (gameState)
            {
                case GameStates.TitleScreen:
                    SoundManager.StopSong();
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


                case GameStates.CreditsScreen:
                    
                    if (kb.IsKeyDown(Keys.R))
                    {
                        gameState = GameStates.TitleScreen;
                    }

                    break;

                case GameStates.Pause:
                    if(kb.IsKeyDown(Keys.P))
                    {
                        gameState = GameStates.Playing;
                    }

                    break;
                case GameStates.DifficultySelect:
                    //WORK ON DIFFICULTY

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

                        for (int i = 0; i < 7; i++)
                        {
                            asteroidManager.AddAsteroid();
                        }


                    }
                    else if (kb.IsKeyDown(Keys.D2))
                    {
                        difficultyLevel = 2;
                        gameState = GameStates.Playing;
                        playerManager.LivesRemaining = 2;
                        enemyManager.MinShipsPerWave = 6;
                        enemyManager.MaxShipsPerWave = 8;
                        asteroidManager.minSpeed = 60;
                        asteroidManager.maxSpeed = 120;
                        collisionManager.enemyPointValue = 100;
                        for (int i = 0; i < 10; i++)
                        {
                            asteroidManager.AddAsteroid();
                            
                        }
                    }

                    else if (kb.IsKeyDown(Keys.D3))
                    {
                        difficultyLevel = 3;
                        gameState = GameStates.Playing;
                        playerManager.LivesRemaining = 1;
                        enemyManager.MinShipsPerWave = 12;
                        enemyManager.MaxShipsPerWave = 16;
                        asteroidManager.minSpeed = 100;
                        asteroidManager.maxSpeed = 140;
                        collisionManager.enemyPointValue = 200;
                        for (int i = 0; i < 13; i++)
                        {
                            asteroidManager.AddAsteroid();
                        }


                    }

                    else if (kb.IsKeyDown(Keys.D4))
                    {
                        difficultyLevel = 4;
                        playerManager.isD4 = true;
                        gameState = GameStates.Playing;
                        playerManager.LivesRemaining = 0;
                        enemyManager.MinShipsPerWave = 20;
                        enemyManager.MaxShipsPerWave = 24;
                        asteroidManager.minSpeed = 40;
                        asteroidManager.maxSpeed = 220;
                        collisionManager.enemyPointValue = 400;

                        playerManager.PowerupLevel = 4;

                        for (int i = 0; i < 1; i++)
                        {
                            asteroidManager.AddAsteroid();
                        }


                    }

                    else if (kb.IsKeyDown(Keys.D9)) //Survival Mode.  Score = time played.  No enemies.
                    {
                        difficultyLevel = 9;
                        gameState = GameStates.Playing;
                        playerManager.LivesRemaining = 1;
                        enemyManager.MinShipsPerWave = 0;
                        enemyManager.MaxShipsPerWave = 0;
                        asteroidManager.minSpeed = 200;
                        asteroidManager.maxSpeed = 300;
                        playerManager.playerSpeed = 400.0f;

                        for (int i = 0; i < 20; i++)
                        {
                            asteroidManager.AddAsteroid();
                        }

                    }



                    //Cheat Code Viewer
                    else if (kb.IsKeyDown(Keys.D5))
                    {
                        gameState = GameStates.ccViewer;

                    }

                    if (kb.IsKeyDown(Keys.Z))
                    {
                        gameState = GameStates.CreditsScreen;
                    }
                    break;

                case GameStates.ccViewer:
                    if (kb.IsKeyDown(Keys.R))
                    {
                        gameState = GameStates.DifficultySelect;
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
                        SoundManager.PlayPlayerDeath();
                        
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

                    if (kb.IsKeyDown(Keys.R))
                    {
                        resetGame();
                        gameState = GameStates.DifficultySelect;
                        playerManager.PlayerScore = 0;
                        playerManager.playerSpeed = 320.0f;
                        playerTimePlayed = 0f;
                        asteroidManager.Clear();
                        playerManager.PowerupLevel = 1;
                    }
                    //PAUSE FUNCTION
                    if(kb.IsKeyDown(Keys.P))
                    {
                            gameState = GameStates.Pause;

                    }

                    //CHEAT CODES FOR LOSERS

                    //Press 6 & 9 on Keypad to change lives to 69.
                    if (kb.IsKeyDown(Keys.D6) && kb.IsKeyDown(Keys.D9))
                    {
                        playerManager.LivesRemaining = 68;
                        
                        
                    }

                    //KYS
                    if (kb.IsKeyDown(Keys.K) && kb.IsKeyDown(Keys.Y) && kb.IsKeyDown(Keys.S))
                    {
                        playerManager.Destroyed = true;
                        SoundManager.PlayPlayerDeath();
                    }

                    //AND THIS IS TO GO EVEN FURTHER BEYOND! (Work on This)
                    if (kb.IsKeyDown(Keys.S) && kb.IsKeyDown(Keys.D3))
                    {

                        playerManager.playerSpeed = 480.0f;
                        asteroidManager.minSpeed = 60;
                        asteroidManager.maxSpeed = 120;
                        playerManager.PowerupLevel = 2;
                        SoundManager.PlayGoku();
                    }

                    //Smokemon
                    if(kb.IsKeyDown(Keys.D4) && kb.IsKeyDown(Keys.D2) && kb.IsKeyDown(Keys.D0))
                    {
                        isWeed = true;
                        SoundManager.PlaySmokemon();
                        //Plays Smokemon, but does not reset when you rest.

                        starField = new StarField(
                    this.Window.ClientBounds.Width,
                    this.Window.ClientBounds.Height,
                    200,
                    new Vector2(0, 30f),
                    weedBG,
                    new Rectangle(0, 0, 800, 600));
                    }

                    /*
                    Automatic difficulty adjustment based on your progress
                    if (difficultyLevel == 4 && playerManager.LivesRemaining > 1 && playerManager.LivesRemaining < 59)
                    {
                        asteroidManager.minSpeed = 170;
                        asteroidManager.maxSpeed = 270;
                    }
                    */
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
                        playerManager.playerSpeed = 320.0f;
                        playerTimePlayed = 0f;
                        asteroidManager.Clear();
                        playerManager.PowerupLevel = 1;
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
                        playerManager.playerSpeed = 320.0f;
                        playerTimePlayed = 0f;
                        asteroidManager.Clear();
                        playerManager.PowerupLevel = 1;
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

            if (gameState == GameStates.ccViewer)
            {
                spriteBatch.Draw(ccViewer,
                    new Rectangle(0, 0, this.Window.ClientBounds.Width,
                        this.Window.ClientBounds.Height),
                        Color.White);
            }

            if (gameState == GameStates.CreditsScreen)
            {
                spriteBatch.Draw(credits,
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


                if (difficultyLevel != 9)
                {
                    spriteBatch.DrawString(
                        pericles14,
                        "Score: " + playerManager.PlayerScore.ToString(),
                        scoreLocation,
                        Color.White);
                }
                if (playerManager.LivesRemaining >= 0)
                {
                    spriteBatch.DrawString(
                        pericles14,
                        "Ships Remaining: " +
                            (playerManager.LivesRemaining + 1).ToString(),
                        livesLocation,
                        Color.White);
                }

                if (difficultyLevel == 9)
                {
                    //Added a timer
                    playerTimePlayed +=
                        (float)gameTime.ElapsedGameTime.TotalSeconds;
                    spriteBatch.DrawString(
                        pericles14,
                        "Score: " +
                            ((int)playerTimePlayed * 100).ToString(),
                        timerLocation,
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
            EffectManager.Draw();

            base.Draw(gameTime);
        }

    }
}

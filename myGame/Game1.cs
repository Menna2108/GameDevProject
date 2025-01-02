using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using myGame.GameComponents;
using myGame.GameEntities;
using myGame.GameManagers;
using MyGame.GameScreens;
using System;
using System.Collections.Generic;

namespace myGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D backgroundTexture;
        private Texture2D heartTexture;
        private Texture2D coinTexture;
        private Texture2D rocketEnemyTexture;
        private Texture2D rocketBulletTexture;
        private Texture2D meteorTexture;

        private StartScreen startScreen;
        private PausePlayScreen pausePlayScreen;
        private GameOverScreen gameOverScreen;
        private Rocket rocket;

        private CoinManager coinManager;
        private EnemyManager enemyManager;
        private MeteorManager meteorManager;

        private SpriteFont font;

        private int playerHitCount = 0;
        private float backgroundPositionY = 0;
        private float backgroundSpeed = 2f;

        private const int MaxLives = 3;
        private int currentLives = MaxLives;

        private int currentLevel = 1;

        private List<Bullet> playerBullets;

        private CoinCollector coinCollector;

        private bool hasCollidedWithEnemy = false;

        private bool isLevelUpVisible = false;
        private float levelUpTimer = 0f;
        private const float levelUpDisplayDuration = 3f;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            backgroundTexture = Content.Load<Texture2D>("space");
            heartTexture = Content.Load<Texture2D>("hartje");
            coinTexture = Content.Load<Texture2D>("coin");
            rocketEnemyTexture = Content.Load<Texture2D>("Enemy1");
            rocketBulletTexture = Content.Load<Texture2D>("bullet");
            font = Content.Load<SpriteFont>("Fonts/coinFont");

            startScreen = new StartScreen(Content);
            pausePlayScreen = new PausePlayScreen(Content);
            gameOverScreen = new GameOverScreen(Content);
            gameOverScreen.Initialize(GraphicsDevice);

            Texture2D rocketTexture = Content.Load<Texture2D>("RocketSprite");
            rocket = new Rocket(rocketTexture, rocketBulletTexture, new Vector2(950, 850));
            coinManager = new CoinManager(coinTexture);

            // Observer toevoegen
            coinCollector = new CoinCollector();
            coinManager.AddObserver(coinCollector);
            coinManager.GenerateRandomCoins(5, GraphicsDevice.Viewport.Bounds, rocket.Position.Y);

            // Vijand raketten
            rocketEnemyTexture = Content.Load<Texture2D>("Enemy1");
            enemyManager = new EnemyManager(rocketEnemyTexture);

            // Meteoor
            meteorTexture = Content.Load<Texture2D>("meteor");
            meteorManager = new MeteorManager(meteorTexture);
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 1900;
            _graphics.PreferredBackBufferHeight = 950;
            _graphics.ApplyChanges();
            playerBullets = new List<Bullet>();

            base.Initialize();
            meteorManager.SetLevel(currentLevel, GraphicsDevice.Viewport.Bounds);
        }

        protected override void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();

            // Startscherm
            if (!GameManager.Instance.IsGameStarted)
            {
                startScreen.Update(mouseState, this);
                GameManager.Instance.IsGameStarted = startScreen.IsGameStarted;
                if (GameManager.Instance.IsGameStarted)
                {
                    RestartGame();
                }
                return;
            }

            // Game Over-scherm
            if (rocket.Health <= 0 || GameManager.Instance.IsGameOver)
            {
                gameOverScreen.Update(mouseState, out bool startNewGame, out bool exitGame);

                if (startNewGame)
                {
                    RestartGame();
                    GameManager.Instance.IsGameStarted = true;
                    GameManager.Instance.IsGameOver = false;
                }
                else if (exitGame)
                {
                    Exit();
                }
                return;
            }

            // Pauzescherm
            bool isPaused = GameManager.Instance.IsPaused;
            pausePlayScreen.Update(mouseState, ref isPaused);
            GameManager.Instance.IsPaused = isPaused;

            if (isPaused)
            {
                return;
            }

            // Update speler
            var keyboardState = Keyboard.GetState();
            rocket.Update(gameTime, keyboardState);

            if (keyboardState.IsKeyDown(Keys.Space))
            {
                Bullet newBullet = rocket.Shoot();
                if (newBullet != null)
                {
                    playerBullets.Add(newBullet);
                }
            }

            // Update spelerkogels
            for (int i = playerBullets.Count - 1; i >= 0; i--)
            {
                playerBullets[i].Update(gameTime);
                if (!playerBullets[i].IsActive)
                {
                    playerBullets.RemoveAt(i);
                }
            }

            // Achtergrond scrollen
            if (keyboardState.IsKeyDown(Keys.Up))
            {
                backgroundPositionY += backgroundSpeed * 1.5f;

                if (backgroundPositionY >= _graphics.PreferredBackBufferHeight)
                {
                    backgroundPositionY = 0;
                    coinManager.GenerateRandomCoins(5, GraphicsDevice.Viewport.Bounds, rocket.Position.Y);
                }
            }

            // Update coins
            coinManager.Update(gameTime, rocket.Bounds, backgroundPositionY, rocket.Position.Y);

            // Update vijanden
            enemyManager.Update(gameTime, rocket, playerBullets);

            // Controleer of de speler naar een nieuw niveau moet gaan
            if (coinCollector.TotalCoins >= 5 && currentLevel == 1)
            {
                currentLevel = 2;
                meteorManager.SetLevel(2, GraphicsDevice.Viewport.Bounds);  // Meteoren spawnen voor level 2
                rocket.IncreaseSpeed(2f);  // Verhoog raketsnelheid
                rocket.DecreaseShootCooldown(0.09f);
                isLevelUpVisible = true;
                levelUpTimer = 0f;
            }
            else if (coinCollector.TotalCoins >= 10 && currentLevel == 2)
            {
                currentLevel = 3;
                meteorManager.SetLevel(3, GraphicsDevice.Viewport.Bounds);  // Verwijder meteoren in level 3
                enemyManager.RemoveRocketEnemies();  // Verwijder raket vijanden in level 3
                isLevelUpVisible = true;
                levelUpTimer = 0f;
            }

            // Controleer of de speler geraakt is door vijandelijke raketten
            foreach (var enemy in enemyManager.GetEnemies())
            {
                if (enemy is RocketEnemy rocketEnemy && !hasCollidedWithEnemy)
                {
                    if (rocketEnemy.CheckPlayerHit(rocket.Bounds))
                    {
                        playerHitCount++;  // Verhoog de botsingsteller

                        if (playerHitCount >= 3)
                        {
                            rocket.LoseHealth();
                            currentLives--;
                            playerHitCount = 0;
                        }

                        // Stel de botsingservaring in
                        hasCollidedWithEnemy = true;
                    }
                }
            }

            // Reset de botsingstatus na elke update
            hasCollidedWithEnemy = false;
            meteorManager.Update(gameTime, rocket, enemyManager.GetEnemies());


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            // Achtergrond tekenen
            _spriteBatch.Draw(backgroundTexture,
                new Rectangle(0, (int)backgroundPositionY - _graphics.PreferredBackBufferHeight,
                _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), Color.White);
            _spriteBatch.Draw(backgroundTexture,
                new Rectangle(0, (int)backgroundPositionY,
                _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), Color.White);

            if (!GameManager.Instance.IsGameStarted)
            {
                startScreen.Draw(_spriteBatch);
            }
            else if (rocket.Health <= 0 || GameManager.Instance.IsGameOver)
            {
                gameOverScreen.Draw(_spriteBatch);
            }
            else
            {
                pausePlayScreen.Draw(_spriteBatch);

                if (GameManager.Instance.IsPaused)
                {
                    string pausedText = "Game Paused";
                    Vector2 pausedTextSize = font.MeasureString(pausedText);
                    Vector2 pausedTextPosition = new Vector2(
                        (_graphics.PreferredBackBufferWidth - pausedTextSize.X) / 2,
                        (_graphics.PreferredBackBufferHeight - pausedTextSize.Y) / 2
                    );
                    _spriteBatch.DrawString(font, pausedText, pausedTextPosition, Color.Red);
                }
                else
                {
                    rocket.Draw(_spriteBatch);
                    coinManager.Draw(_spriteBatch);

                    foreach (var bullet in playerBullets)
                    {
                        bullet.Draw(_spriteBatch);
                    }

                    enemyManager.Draw(_spriteBatch);
                    meteorManager.Draw(_spriteBatch);
                    DrawLevelUpScreen(_spriteBatch, gameTime);

                    // Hartjes tekenen
                    for (int i = 0; i < rocket.Health; i++)
                    {
                        _spriteBatch.Draw(
                            heartTexture,
                            new Vector2(10 + (i * (heartTexture.Width * 0.1f + 5)), 10),
                            null,
                            Color.White,
                            0f,
                            Vector2.Zero,
                            0.1f,
                            SpriteEffects.None,
                            0f
                        );
                    }

                    // Coins en level tonen
                    string coinText = $"Coins: {coinCollector.TotalCoins}";
                    string levelText = $"Level: {currentLevel}";
                    _spriteBatch.DrawString(font, coinText, new Vector2(950, 15), Color.Yellow);
                    _spriteBatch.DrawString(font, levelText, new Vector2(20, 60), Color.White);
                }
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void RestartGame()
        {
            rocket.Reset(new Vector2(950, 850));
            rocket.Health = MaxLives;

            currentLives = MaxLives;
            currentLevel = 1;
            coinCollector.Reset();
            coinManager.GenerateRandomCoins(5, GraphicsDevice.Viewport.Bounds, rocket.Position.Y);
            enemyManager.Reset();
            meteorManager.RestartMeteors(GraphicsDevice.Viewport.Bounds);
            meteorManager.SetLevel(1, GraphicsDevice.Viewport.Bounds);
            playerBullets.Clear();

            GameManager.Instance.IsGameStarted = true;
            GameManager.Instance.IsGameOver = false;
            GameManager.Instance.IsPaused = false;
        }

        private void DrawLevelUpScreen(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (isLevelUpVisible)
            {
                if (font != null) // Check if font is loaded
                {
                    string levelUpText = $"Level {currentLevel} - You leveled up!";
                    Vector2 levelUpTextSize = font.MeasureString(levelUpText);
                    Vector2 levelUpTextPosition = new Vector2(
                        (_graphics.PreferredBackBufferWidth - levelUpTextSize.X) / 2,
                        (_graphics.PreferredBackBufferHeight - levelUpTextSize.Y) / 2
                    );
                    spriteBatch.DrawString(font, levelUpText, levelUpTextPosition, Color.Green);

                    // Update the level-up timer based on gameTime
                    levelUpTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    // Check if the timer exceeds the display duration
                    if (levelUpTimer >= levelUpDisplayDuration)
                    {
                        isLevelUpVisible = false;
                    }
                }
                else
                {
                    Console.WriteLine("Font not loaded correctly!");
                }
            }
        }
    }
}

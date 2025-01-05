using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
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
        private Texture2D bossTexture;
        private Texture2D fireTexture;

        private StartScreen startScreen;
        private PausePlayScreen pausePlayScreen;
        private GameOverScreen gameOverScreen;
        private Rocket rocket;

        private CoinManager coinManager;
        private EnemyManager enemyManager;
        private MeteorManager meteorManager;
        private BossManager bossManager;

        private SpriteFont font;

        private int playerHitCount = 0;
        private float backgroundPositionY = 0;
        private float backgroundSpeed = 2f;

        private const int MaxLives = Rocket.MaxHealth;

        private int currentLevel = 1;

        private List<Bullet> playerBullets;

        private CoinCollector coinCollector;

        private bool hasCollidedWithEnemy = false;

        private bool isLevelUpVisible = false;
        private float levelUpTimer = 0f;
        private const float levelUpDisplayDuration = 3f;

        private GameWinScreen gameWinScreen;
        private bool hasPlayedWinSound = false;
        private bool hasPlayedGameOverSound = false;

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
            rocketBulletTexture = Content.Load<Texture2D>("bullet");
            fireTexture = Content.Load<Texture2D>("FireShoot");
            font = Content.Load<SpriteFont>("Fonts/coinFont");

            startScreen = new StartScreen(Content);
            pausePlayScreen = new PausePlayScreen(Content);
            gameOverScreen = new GameOverScreen(Content);
            gameOverScreen.Initialize(GraphicsDevice);
            gameWinScreen = new GameWinScreen(Content);
            gameWinScreen.Initialize(GraphicsDevice);

            Texture2D rocketTexture = Content.Load<Texture2D>("RocketSprite");
            rocket = new Rocket(rocketTexture, rocketBulletTexture, new Vector2(950, 850));

            // Initialiseer de AudioManager en laad de audio
            AudioManager.Instance.LoadContent(Content);

            // Observer toevoegen
            coinCollector = new CoinCollector();
            coinManager = new CoinManager(coinTexture, AudioManager.Instance);
            coinManager.AddObserver(coinCollector);
            coinManager.GenerateRandomCoins(5, GraphicsDevice.Viewport.Bounds, rocket.Position.Y);

            // Vijand raketten
            rocketEnemyTexture = Content.Load<Texture2D>("Enemy1");
            enemyManager = new EnemyManager(rocketEnemyTexture);

            // Meteoor
            meteorTexture = Content.Load<Texture2D>("meteor");
            meteorManager = new MeteorManager(meteorTexture);

            // Baas
            bossTexture = Content.Load<Texture2D>("Evil");
            bossManager = new BossManager(bossTexture, fireTexture);
        }

        protected override void Initialize()
        {
            // Scherminstellingen
            _graphics.PreferredBackBufferWidth = 1900;
            _graphics.PreferredBackBufferHeight = 950;
            _graphics.ApplyChanges();

            // Reset game state
            GameManager.Instance.Reset();

            // Speler-instellingen
            playerBullets = new List<Bullet>();

            // Vijanden en managers resetten
            bossManager = new BossManager(null, null); 
            bossManager.Initialize(GraphicsDevice.Viewport.Bounds);

            // Oproepen van de basisinitialisatie
            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();

            // Startscherm
            if (!GameManager.Instance.IsGameStarted)
            {
                startScreen.Update(mouseState, this);
                if (!GameManager.Instance.IsGameStarted && startScreen.IsGameStarted)
                {
                    GameManager.Instance.IsGameStarted = true;
                    RestartGame();
                    AudioManager.Instance.PlayGameSound(); 
                }
                return;
            }

            // Game Win-scherm
            if (GameManager.Instance.IsGameWon)
            {
                AudioManager.Instance.StopMusic();

                gameWinScreen.Update(mouseState, out bool startNewGame, out bool exitGame);

                if (!hasPlayedWinSound)
                {
                    AudioManager.Instance.PlayWinSound(); 
                    hasPlayedWinSound = true;
                }

                if (startNewGame)
                {
                    GameManager.Instance.IsGameWon = false;
                    RestartGame();
                    return;
                }
                else if (exitGame)
                {
                    Exit();
                }
                return;
            }

            // Game Over-scherm
            if (rocket.Health <= 0 || GameManager.Instance.IsGameOver)
            {
                gameOverScreen.Update(mouseState, out bool startNewGame, out bool exitGame);
                if (!hasPlayedGameOverSound)
                {
                    AudioManager.Instance.PlayGameOver(); 
                    hasPlayedGameOverSound = true;
                }

                if (startNewGame)
                {
                    GameManager.Instance.IsGameOver = false;
                    if (currentLevel == 3)
                    {
                        bossManager.Reset();
                        enemyManager.ClearAllEnemies();
                    }
                    RestartGame();
                    return;
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
            MediaPlayer.IsMuted = isPaused;

            if (isPaused) return;

            // Update speler
            var keyboardState = Keyboard.GetState();
            rocket.Update(gameTime, keyboardState);

            // Update vijanden alleen als het level kleiner is dan 3
            if (currentLevel < 3)
            {
                enemyManager.Update(gameTime, rocket, playerBullets);
            }
            else if (currentLevel == 3)
            {
                // Verwijder alle vijanden in level 3
                enemyManager.ClearAllEnemies();
                bossManager.Update(gameTime, rocket, playerBullets);
            }

            if (keyboardState.IsKeyDown(Keys.Space))
            {
                Bullet newBullet = rocket.Shoot();
                if (newBullet != null)
                {
                    playerBullets.Add(newBullet);
                    AudioManager.Instance.PlayShootSound(); 
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
            if (currentLevel < 3)
            {
                enemyManager.Update(gameTime, rocket, playerBullets);
                foreach (var enemy in enemyManager.GetDestroyedEnemies())
                {
                    AudioManager.Instance.PlayShootedSound(); 
                }
            }

            // Controleer op level-ups
            if (coinCollector.TotalCoins >= 10 && currentLevel == 1)
            {
                LevelUp(2);
            }
            else if (coinCollector.TotalCoins >= 20 && currentLevel == 2)
            {
                LevelUp(3);
            }

            // Controleer of de boss verslagen is en of de speler 30 coins heeft
            if (currentLevel == 3 && bossManager.IsBossDefeated && coinCollector.TotalCoins >= 30)
            {
                GameManager.Instance.IsGameWon = true;
            }

            // Controleer botsingen
            HandleCollisions();
            if (GameManager.Instance.IsGameStarted && rocket.Health == MaxLives && currentLevel == 1)
            {
                return;
            }

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
            else if (GameManager.Instance.IsGameWon)
            {
                gameWinScreen.Draw(_spriteBatch);
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

                    // Teken alleen de boss in level 3
                    if (currentLevel == 3)
                    {
                        bossManager.Draw(_spriteBatch);
                        // Teken de boss health onder de coins text
                        string bossHealthText = $"Boss Health: {bossManager.GetBossHealth()}";
                        _spriteBatch.DrawString(font, bossHealthText, new Vector2(950, 50), Color.Red);
                    }
                    else
                    {
                        enemyManager.Draw(_spriteBatch);
                    }

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
            // Reset game state manager eerst
            GameManager.Instance.Reset();

            // Reset rocket eerst
            rocket.Reset(new Vector2(950, 850));

            // Reset alle managers
            enemyManager.ClearAllEnemies();
            enemyManager.Reset();
            meteorManager.RestartMeteors(GraphicsDevice.Viewport.Bounds);
            bossManager.Reset();
            playerBullets.Clear();

            // Reset coins
            coinCollector.Reset();
            coinManager.GenerateRandomCoins(5, GraphicsDevice.Viewport.Bounds, rocket.Position.Y);

            // Reset game states
            hasPlayedWinSound = false;
            hasPlayedGameOverSound = false;
            isLevelUpVisible = false;
            hasCollidedWithEnemy = false;
            currentLevel = 1;
            backgroundPositionY = 0;

            // Reset enemy speed
            enemyManager.ResetEnemySpeed();

            // Set game state na alle resets
            GameManager.Instance.IsGameStarted = true;
            GameManager.Instance.IsGameWon = false;
            GameManager.Instance.IsGameOver = false;

            // Reset muziek als laatste
            AudioManager.Instance.StopMusic(); 
            AudioManager.Instance.PlayGameSound(); 
        }

        private void DrawLevelUpScreen(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (isLevelUpVisible)
            {
                if (font != null)
                {
                    string levelUpText = $"Level {currentLevel} - You leveled up!";
                    Vector2 levelUpTextSize = font.MeasureString(levelUpText);
                    Vector2 levelUpTextPosition = new Vector2(
                        (_graphics.PreferredBackBufferWidth - levelUpTextSize.X) / 2,
                        (_graphics.PreferredBackBufferHeight - levelUpTextSize.Y) / 2
                    );
                    spriteBatch.DrawString(font, levelUpText, levelUpTextPosition, Color.Green);
                    levelUpTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
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

        private void LevelUp(int newLevel)
        {
            currentLevel = newLevel;
            AudioManager.Instance.PlayLevelSound();
            isLevelUpVisible = true;
            levelUpTimer = 0f;

            if (newLevel == 2)
            {
                meteorManager.SetLevel(2, GraphicsDevice.Viewport.Bounds);
                rocket.IncreaseSpeed(2f);
                rocket.DecreaseShootCooldown(0.09f);
            }
            else if (newLevel == 3)
            {
                SetupLevel3();
                AudioManager.Instance.StopMusic();
                AudioManager.Instance.PlayBossFight(); 
            }
        }

        private void SetupLevel3()
        {
            // Reset alle enemies
            enemyManager.ClearAllEnemies();
            enemyManager.Reset();

            // Level 3 content
            meteorManager.SetLevel(3, GraphicsDevice.Viewport.Bounds);
            bossManager = new BossManager(bossTexture, rocketBulletTexture);
            bossManager.Initialize(GraphicsDevice.Viewport.Bounds);
            bossManager.SetLevel(3);

            // Power up mijn speler rocket
            rocket.IncreaseSpeed(1f);
            rocket.DecreaseShootCooldown(0.05f);
        }

        private void HandleCollisions()
        {
            if (rocket.Health <= 0) return;

            bool currentlyColliding = false;

            foreach (var enemy in enemyManager.GetEnemies())
            {
                if (enemy is RocketEnemy rocketEnemy)
                {
                    if (rocketEnemy.CheckPlayerHit(rocket.Bounds))
                    {
                        currentlyColliding = true;
                        if (!hasCollidedWithEnemy)
                        {
                            rocket.LoseHealth();
                            rocketEnemy.IsActive = false;
                            hasCollidedWithEnemy = true;
                        }
                        break;
                    }
                }
            }
            if (currentLevel == 3 && bossManager.CheckCollisionWithPlayerBounds(rocket.Bounds))
            {
                rocket.LoseHealth();
            }
            if (!currentlyColliding)
            {
                hasCollidedWithEnemy = false;
            }
        }
    }
}
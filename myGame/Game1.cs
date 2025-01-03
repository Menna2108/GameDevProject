using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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

        private const int MaxLives = 3;
        private int currentLives = MaxLives;

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


        // Sounds
        SoundEffect losingHeartSound;
        Song gameSound;
        SoundEffect shootSound;
        Song gameOver;
        SoundEffect winSound;
        SoundEffect coinSound;
        SoundEffect shootedSound;
        SoundEffect levelSound;

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
            font = Content.Load<SpriteFont>("Fonts/coinFont");

            startScreen = new StartScreen(Content);
            pausePlayScreen = new PausePlayScreen(Content);
            gameOverScreen = new GameOverScreen(Content);
            gameOverScreen.Initialize(GraphicsDevice);
            gameWinScreen = new GameWinScreen(Content);
            gameWinScreen.Initialize(GraphicsDevice);

            Texture2D rocketTexture = Content.Load<Texture2D>("RocketSprite");
            rocket = new Rocket(rocketTexture, rocketBulletTexture, new Vector2(950, 850));

            // Geluiden 
            gameSound = Content.Load<Song>("Audio/gameSound");
            shootSound = Content.Load<SoundEffect>("Audio/shoot");
            gameOver = Content.Load<Song>("Audio/over");
            losingHeartSound = Content.Load<SoundEffect>("Audio/losingHeart");
            coinSound = Content.Load<SoundEffect>("Audio/coin");
            shootedSound = Content.Load<SoundEffect>("Audio/shooted");
            levelSound = Content.Load<SoundEffect>("Audio/leveledUp");
            winSound = Content.Load<SoundEffect>("Audio/win");

            // Observer toevoegen
            coinCollector = new CoinCollector();
            coinManager = new CoinManager(coinTexture, coinSound);
            coinManager.AddObserver(coinCollector);
            coinManager.GenerateRandomCoins(5, GraphicsDevice.Viewport.Bounds, rocket.Position.Y);

            // Vijand raketten
            rocketEnemyTexture = Content.Load<Texture2D>("Enemy1");
            enemyManager = new EnemyManager(rocketEnemyTexture);

            // Meteoor
            meteorTexture = Content.Load<Texture2D>("meteor");
            meteorManager = new MeteorManager(meteorTexture);

            // baas
            bossTexture = Content.Load<Texture2D>("Evil"); 
            bossManager = new BossManager(bossTexture, rocketBulletTexture);

        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 1900;
            _graphics.PreferredBackBufferHeight = 950;
            _graphics.ApplyChanges();
            playerBullets = new List<Bullet>();

            bossManager = new BossManager(null, null);
            bossManager.Initialize(GraphicsDevice.Viewport.Bounds);

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
                    MediaPlayer.Play(gameSound);
                    MediaPlayer.IsRepeating = true;
                }
                return;
            }

            // Game Win-scherm
            if (coinCollector.TotalCoins >= 9)
            {
                if (!hasPlayedWinSound)
                {
                    MediaPlayer.Stop(); 
                    winSound.Play();    
                    hasPlayedWinSound = true;
                }

                GameManager.Instance.IsGameWon = true;
                gameWinScreen.Update(mouseState, out bool startNewGame, out bool exitGame);

                if (startNewGame)
                {
                    RestartGame();
                    GameManager.Instance.IsGameWon = false;
                    hasPlayedWinSound = false;
                    MediaPlayer.Play(gameSound); 
                    MediaPlayer.IsRepeating = true;
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
                if (!hasPlayedGameOverSound)
                {
                    MediaPlayer.Play(gameOver);
                    MediaPlayer.IsRepeating = false;
                    hasPlayedGameOverSound = true;
                }

                gameOverScreen.Update(mouseState, out bool startNewGame, out bool exitGame);

                if (startNewGame)
                {
                    RestartGame();
                    GameManager.Instance.IsGameOver = false;
                    hasPlayedGameOverSound = false;
                    MediaPlayer.Play(gameSound);
                    MediaPlayer.IsRepeating = true;
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

            if (keyboardState.IsKeyDown(Keys.Space))
            {
                Bullet newBullet = rocket.Shoot();
                if (newBullet != null)
                {
                    playerBullets.Add(newBullet);
                    shootSound.Play();
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
            foreach (var enemy in enemyManager.GetDestroyedEnemies())
            {
                shootedSound.Play();
            }

            // Controleer op level-ups
            if (coinCollector.TotalCoins >= 3 && currentLevel == 1)
            {
                LevelUp(2);
            }
            else if (coinCollector.TotalCoins >= 6 && currentLevel == 2)
            {
                LevelUp(3);
            }

            // Controleer botsingen
            HandleCollisions();

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
                    if (currentLevel == 3)
                    {
                        bossManager.Draw(_spriteBatch);
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
            bossManager.Reset();
            hasPlayedWinSound = false;

            GameManager.Instance.IsGameStarted = true;
            GameManager.Instance.IsGameOver = false;
            GameManager.Instance.IsPaused = false;

            MediaPlayer.Play(gameSound); 
            MediaPlayer.IsRepeating = true;
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
            levelSound.Play();
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
                GameManager.Instance.CurrentLevel = 3;
                meteorManager.SetLevel(3, GraphicsDevice.Viewport.Bounds);
                enemyManager.RemoveRocketEnemies();
                bossManager.SetLevel(3);
            }
        }

        private void HandleCollisions()
        {
            foreach (var enemy in enemyManager.GetEnemies())
            {
                if (enemy is RocketEnemy rocketEnemy && !hasCollidedWithEnemy)
                {
                    if (rocketEnemy.CheckPlayerHit(rocket.Bounds))
                    {
                        playerHitCount++;
                        if (playerHitCount >= 3)
                        {
                            rocket.LoseHealth();
                            currentLives--;
                            losingHeartSound?.Play();
                            playerHitCount = 0;
                        }
                        hasCollidedWithEnemy = true;
                    }
                }
            }
            hasCollidedWithEnemy = false;
        }
    }
}

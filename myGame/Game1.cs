using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using myGame.GameEntities;
using myGame.GameManagers;
using myGame.GameScreens;
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

        private StartScreen startScreen;
        private PausePlayScreen pausePlayScreen;
        private Rocket rocket;

        private CoinManager coinManager;
        private EnemyManager enemyManager;

        private SpriteFont font;

        private float backgroundPositionY = 0;
        private float backgroundSpeed = 2f;

        private const int MaxLives = 3;
        private int currentLives = MaxLives;

        private int currentLevel = 1;

        private List<Bullet> playerBullets;

        private CoinCollector coinCollector;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 1900;
            _graphics.PreferredBackBufferHeight = 950;
            _graphics.ApplyChanges();
            playerBullets = new List<Bullet>();

            base.Initialize();
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

            Texture2D rocketTexture = Content.Load<Texture2D>("RocketSprite");
            rocket = new Rocket(rocketTexture, rocketBulletTexture, new Vector2(950, 850));
            coinManager = new CoinManager(coinTexture);

            // Observer toevoegen
            coinCollector = new CoinCollector();
            coinManager.AddObserver(coinCollector);
            coinManager.GenerateRandomCoins(5, GraphicsDevice.Viewport.Bounds, rocket.Position.Y);

            // EnemyManager
            enemyManager = new EnemyManager(rocketEnemyTexture, rocketBulletTexture);
        }

        protected override void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();

            bool isGameStarted = GameManager.Instance.IsGameStarted;

            if (!isGameStarted)
            {
                startScreen.Update(mouseState, ref isGameStarted, this);
                GameManager.Instance.IsGameStarted = isGameStarted;
                return;
            }

            bool isPaused = GameManager.Instance.IsPaused;
            pausePlayScreen.Update(mouseState, ref isPaused);
            GameManager.Instance.IsPaused = isPaused;

            if (isPaused)
            {
                return;
            }

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
            enemyManager.Update(gameTime, rocket.Bounds, playerBullets, () => currentLives--);

            // Controleer of het spel voorbij is
            if (currentLives <= 0)
            {
                GameManager.Instance.IsGameStarted = false;
                // komt nog
            }

            // Controleer op level progressie
            if (coinCollector.TotalCoins >= 20 && currentLevel != 3)
            {
                currentLevel = 3;
                // Schakel naar Level 3 logica (Boss)
            }
            else if (coinCollector.TotalCoins >= 10 && currentLevel != 2)
            {
                currentLevel = 2;
                // Schakel naar Level 2 logica (meteor)
            }

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
            else
            {
                // Pauzescherm en pauzetekst
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

                    // Spelerkogels tekenen
                    foreach (var bullet in playerBullets)
                    {
                        bullet.Draw(_spriteBatch);
                    }

                    // Vijanden tekenen
                    enemyManager.Draw(_spriteBatch);

                    // Hartjes tekenen
                    for (int i = 0; i < currentLives; i++)
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

                    // Verzamelde coins tonen
                    string coinText = $"Coins: {coinCollector.TotalCoins}";
                    Vector2 textSize = font.MeasureString(coinText);
                    Vector2 textPosition = new Vector2(
                        (_graphics.PreferredBackBufferWidth / 2) - (textSize.X / 2),
                        10
                    );
                    _spriteBatch.DrawString(font, coinText, textPosition, Color.Yellow);

                    // Huidige level tonen
                    string levelText = $"Level: {currentLevel}";
                    Vector2 levelTextSize = font.MeasureString(levelText);
                    _spriteBatch.DrawString(
                        font,
                        levelText,
                        new Vector2(20, 60),
                        Color.White
                    );
                }
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

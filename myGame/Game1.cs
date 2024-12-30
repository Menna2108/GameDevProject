using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using myGame.GameEntities;
using myGame.GameManagers;
using myGame.GameScreens;
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

        private StartScreen startScreen;
        private PausePlayScreen pausePlayScreen;
        private Rocket rocket;

        private CoinManager coinManager;

        private SpriteFont font;

        private float backgroundPositionY = 0;
        private float backgroundSpeed = 2f;

        private const int MaxLives = 3;

        private List<Bullet> bullets;

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
            bullets = new List<Bullet>();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            backgroundTexture = Content.Load<Texture2D>("space");
            heartTexture = Content.Load<Texture2D>("hartje");
            coinTexture = Content.Load<Texture2D>("coin");
            font = Content.Load<SpriteFont>("Fonts/coinFont");

            startScreen = new StartScreen(Content);
            pausePlayScreen = new PausePlayScreen(Content);

            Texture2D rocketTexture = Content.Load<Texture2D>("RocketSprite"); 
            Texture2D bulletTexture = Content.Load<Texture2D>("bullet");

            rocket = new Rocket(rocketTexture, bulletTexture, new Vector2(950, 850));

            coinManager = new CoinManager(coinTexture);
            coinManager.GenerateRandomCoins(5, GraphicsDevice.Viewport.Bounds, rocket.Position.Y);
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
                    bullets.Add(newBullet);
                }
            }

            // Update kogels
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                bullets[i].Update(gameTime);
                if (!bullets[i].IsActive)
                {
                    bullets.RemoveAt(i);
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

                    // Kogels tekenen
                    foreach (var bullet in bullets)
                    {
                        bullet.Draw(_spriteBatch);
                    }

                    // Hartjes tekenen
                    for (int i = 0; i < MaxLives; i++)
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
                    string coinText = $"Coins: {coinManager.CollectedCoins}";
                    Vector2 textSize = font.MeasureString(coinText);
                    Vector2 textPosition = new Vector2(
                        (_graphics.PreferredBackBufferWidth / 2) - (textSize.X / 2),
                        10
                    );
                    _spriteBatch.DrawString(font, coinText, textPosition, Color.Yellow);
                }
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

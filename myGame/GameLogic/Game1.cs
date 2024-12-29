using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using myGame.GameScreens;
using myGame.GameEntities;

namespace myGame.GameLogic
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D backgroundTexture;
        private Texture2D heartTexture;

        private StartScreen startScreen;
        private PausePlayScreen pausePlayScreen;
        private Rocket rocket;

        private bool isGameStarted = false;
        private bool isGamePaused = false; 
        private float backgroundPositionY = 0;
        private float backgroundSpeed = 2f;

        private const int MaxLives = 3;

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

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            backgroundTexture = Content.Load<Texture2D>("space");
            heartTexture = Content.Load<Texture2D>("hartje");

            startScreen = new StartScreen(Content);
            pausePlayScreen = new PausePlayScreen(Content);

            rocket = new Rocket(Content, new Vector2(950, 850));
        }

        protected override void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();

            if (!isGameStarted)
            {
                startScreen.Update(mouseState, ref isGameStarted, this);
                return;
            }

            pausePlayScreen.Update(mouseState, ref isGamePaused);

            if (isGamePaused)
            {
                return; 
            }

            rocket.Update(gameTime, Keyboard.GetState());

            if (rocket.IsMovingUp())
            {
                backgroundPositionY += backgroundSpeed * 1.5f;
            }

            if (backgroundPositionY >= _graphics.PreferredBackBufferHeight)
            {
                backgroundPositionY = 0;
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

            if (!isGameStarted)
            {
                startScreen.Draw(_spriteBatch);
            }
            else
            {
                rocket.Draw(_spriteBatch);

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

                // Pauzeknop tekenen
                pausePlayScreen.Draw(_spriteBatch);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

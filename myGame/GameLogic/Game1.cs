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
        private StartScreen startScreen;
        private Rocket rocket;

        private bool isGameStarted = false;
        private float backgroundPositionY = 0;
        private float backgroundSpeed = 2f;

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
            startScreen = new StartScreen(Content);
            rocket = new Rocket(Content);
        }

        protected override void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();

            if (!isGameStarted)
            {
                startScreen.Update(gameTime, mouseState, ref isGameStarted, this);
                return;
            }

            rocket.Update(gameTime, Keyboard.GetState());

            if (rocket.IsMovingUp())
            {
                backgroundPositionY += backgroundSpeed;
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
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

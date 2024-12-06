using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace myGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D backgroundTexture;
        private Texture2D rocketSprite;
        private Vector2 rocketPosition;
        private float rocketSpeed = 4f;
        private float rocketScale = 0.16f;
        private float backgroundSpeed = 2f;
        private float backgroundPositionY = 0;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            rocketPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight - 100);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            backgroundTexture = Content.Load<Texture2D>("space");
            rocketSprite = Content.Load<Texture2D>("RocketSprite");
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();

            // Beweging van mijn raket
            if (keyboardState.IsKeyDown(Keys.Up) && rocketPosition.Y > 0)  
            {
                rocketPosition.Y -= rocketSpeed;
                backgroundPositionY += backgroundSpeed;  
            }

            // Collision detection voor omlaag, links en rechts
            if (keyboardState.IsKeyDown(Keys.Down) && rocketPosition.Y + rocketSprite.Height * rocketScale < _graphics.PreferredBackBufferHeight)
            {
                rocketPosition.Y += rocketSpeed;
            }

            if (keyboardState.IsKeyDown(Keys.Left) && rocketPosition.X > 0)
            {
                rocketPosition.X -= rocketSpeed; 
            }

            if (keyboardState.IsKeyDown(Keys.Right) && rocketPosition.X + rocketSprite.Width * rocketScale < _graphics.PreferredBackBufferWidth)
            {
                rocketPosition.X += rocketSpeed; 
            }

            // Achtergrond reset naar boven wanneer deze onderaan het scherm komt
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

            // Achtergrond schalen
            _spriteBatch.Draw(backgroundTexture, new Rectangle(0, (int)backgroundPositionY - _graphics.PreferredBackBufferHeight, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), Color.White);
            _spriteBatch.Draw(backgroundTexture, new Rectangle(0, (int)backgroundPositionY, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), Color.White);

            // Mijn raket tekenen
            _spriteBatch.Draw(rocketSprite, rocketPosition, null, Color.White, 0f, Vector2.Zero, rocketScale, SpriteEffects.None, 0f);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

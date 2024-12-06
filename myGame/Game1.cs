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

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            rocketPosition = new Vector2(100, 100);
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

            // Beweging van mijn raket met pijltoetsen
            if (keyboardState.IsKeyDown(Keys.Up))
                rocketPosition.Y -= rocketSpeed;  
            if (keyboardState.IsKeyDown(Keys.Down))
                rocketPosition.Y += rocketSpeed;  
            if (keyboardState.IsKeyDown(Keys.Left))
                rocketPosition.X -= rocketSpeed; 
            if (keyboardState.IsKeyDown(Keys.Right))
                rocketPosition.X += rocketSpeed;  

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            // Achtergrond schalen
            _spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), Color.White);

            // Raket tekenen
            _spriteBatch.Draw(rocketSprite, rocketPosition, null, Color.White, 0f, Vector2.Zero, rocketScale, SpriteEffects.None, 0f);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

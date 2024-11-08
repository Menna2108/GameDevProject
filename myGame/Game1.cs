using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace myGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Laad het hartje in en configureer de schaal en afstand
        private Texture2D heart;
        private Vector2 heartPosition;
        private float heartScale = 0.1f; // Maak het hartje kleiner (30% van originele grootte)
        private int numberOfHearts = 3; // Aantal harten

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            heartPosition = new Vector2(10, 10); // Startpositie linksboven
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            heart = Content.Load<Texture2D>("hartje"); // Zorg dat de naam klopt
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            // Teken de harten naast elkaar
            for (int i = 0; i < numberOfHearts; i++)
            {
                Vector2 position = new Vector2(heartPosition.X + i * (heart.Width * heartScale + 5), heartPosition.Y);
                _spriteBatch.Draw(heart, position, null, Color.White, 0f, Vector2.Zero, heartScale, SpriteEffects.None, 0f);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

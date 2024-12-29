using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace myGame.GameEntities
{
    public class Rocket
    {
        private Texture2D texture;
        private Vector2 position;
        private float speed = 4f;
        private float scale = 0.16f;

        public Rocket(Microsoft.Xna.Framework.Content.ContentManager content, Vector2 startPosition)
        {
            texture = content.Load<Texture2D>("RocketSprite");
            position = startPosition;
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState)
        {
            if (keyboardState.IsKeyDown(Keys.Up) && position.Y > 0)
                position.Y -= speed;

            if (keyboardState.IsKeyDown(Keys.Down) && position.Y + texture.Height * scale < 950)
                position.Y += speed;

            if (keyboardState.IsKeyDown(Keys.Left) && position.X > 0)
                position.X -= speed;

            if (keyboardState.IsKeyDown(Keys.Right) && position.X + texture.Width * scale < 1900)
                position.X += speed;
        }

        public bool IsMovingUp()
        {
            return Keyboard.GetState().IsKeyDown(Keys.Up);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
    }
}

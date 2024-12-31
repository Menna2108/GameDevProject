using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace myGame.GameEntities
{
    public class Bullet
    {
        private Texture2D texture;
        private Vector2 position;
        private float speed = 10f;
        private float scale = 0.1f;

        public bool IsActive { get; private set; } = true;

        public Rectangle Bounds => new Rectangle(
            (int)position.X,
            (int)position.Y,
            texture.Width,
            texture.Height
        );

        public Bullet(Texture2D texture, Vector2 startPosition)
        {
            this.texture = texture;
            position = startPosition;
        }
        public void Deactivate()
        {
            IsActive = false;
        }
        public void Update(GameTime gameTime)
        {
            position.Y -= speed;

            if (position.Y < 0)
                IsActive = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive)
            {
                spriteBatch.Draw(
                    texture,
                    position,
                    null,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    scale,
                    SpriteEffects.None,
                    0f
                );
            }
        }
    }
}
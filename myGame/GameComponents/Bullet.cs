using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace myGame.GameComponents
{
    public class Bullet
    {
        private Texture2D texture;
        private Vector2 position;
        private Vector2 direction;
        private float speed = 10f;
        private float scale = 0.1f;
        private bool isBossBullet;

        public bool IsActive { get; set; } = true;

        public Rectangle Bounds => new Rectangle(
            (int)position.X,
            (int)position.Y,
            (int)(texture.Width * scale),
            (int)(texture.Height * scale)
        );

        public Bullet(Texture2D texture, Vector2 startPosition, bool isBossBullet = false)
        {
            this.texture = texture;
            this.position = startPosition;
            this.isBossBullet = isBossBullet;
            // Standaard richting: omhoog voor speler, omlaag voor boss
            this.direction = isBossBullet ? Vector2.UnitY : -Vector2.UnitY;
        }

        public void SetDirection(Vector2 newDirection)
        {
            direction = newDirection;
            direction.Normalize();
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Update(GameTime gameTime)
        {
            position += direction * speed;

            // Deactiveer de bullet als deze buiten het scherm gaat
            if (position.Y < -50 || position.Y > 1000)
                IsActive = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive)
            {
                // Boss bullets kunnen we een andere kleur geven
                Color bulletColor = isBossBullet ? Color.Red : Color.White;

                spriteBatch.Draw(
                    texture,
                    position,
                    null,
                    bulletColor,
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
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myGame.GameComponents
{
    public class Fire
    {
        private Texture2D texture;
        private Vector2 position;
        private Vector2 direction;
        private float speed = 8f;  
        private float scale = 0.2f;  

        public bool IsActive { get; set; } = true;

        public Rectangle Bounds => new Rectangle(
            (int)position.X,
            (int)position.Y,
            (int)(texture.Width * scale),
            (int)(texture.Height * scale)
        );

        public Fire(Texture2D texture, Vector2 startPosition)
        {
            this.texture = texture;
            this.position = startPosition;
            this.direction = Vector2.UnitY; 
        }

        public void Update(GameTime gameTime)
        {
            position += direction * speed;
            if (position.Y > 1000)
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
                    Color.Red, 
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

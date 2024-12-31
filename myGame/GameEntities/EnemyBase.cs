using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myGame.GameEntities
{
    public abstract class EnemyBase
    {
        protected Texture2D texture;
        protected Vector2 position;
        protected float speed;
        protected float scale;

        public bool IsActive { get; protected set; } = true;

        public Rectangle Bounds => new Rectangle(
            (int)position.X,
            (int)position.Y,
            (int)(texture.Width * scale),
            (int)(texture.Height * scale)
        );

        public EnemyBase(Texture2D texture, Vector2 startPosition, float speed, float scale)
        {
            this.texture = texture;
            this.position = startPosition;
            this.speed = speed;
            this.scale = scale;
        }

        public abstract void Update(GameTime gameTime);

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

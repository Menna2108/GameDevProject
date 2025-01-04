using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using myGame.GameComponents;

namespace myGame.GameEntities
{
    public class BossEnemy : EnemyBase
    {
            private Rectangle screenBounds;
            public Vector2 Position { get; internal set; }

        public BossEnemy(Texture2D texture, Texture2D bulletTexture, Vector2 startPosition, float speed, float scale)
                : base(texture, startPosition, speed, scale)
            {
                this.IsActive = true;
                screenBounds = new Rectangle(0, 0, 1900, 950);
            }

            public void SetScreenBounds(Rectangle bounds)
            {
                screenBounds = bounds;
            }

            public override void Update(GameTime gameTime)
            {
                if (!IsActive) return;

                // Super simpele test beweging: alleen naar beneden
                position.Y += speed;

                // Reset positie als de boss te ver naar beneden gaat
                if (position.Y > screenBounds.Height)
                {
                    position.Y = 0;
                }
            }

            public new void Draw(SpriteBatch spriteBatch)
            {
                if (IsActive)
                {
                    base.Draw(spriteBatch);
                }
            }
        }
    }

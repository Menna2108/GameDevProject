using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace myGame.GameEntities
{
    public class RocketEnemy : EnemyBase
    {
        private int playerHitCount = 0;

        public RocketEnemy(Texture2D texture, Vector2 startPosition, float speed, float scale)
            : base(texture, startPosition, speed, scale)
        {
        }

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Beweeg de vijand in zigzag 
            position.Y += speed;

            if (position.Y % 100 < 50)
            {
                position.X -= 1f; 
            }
            else
            {
                position.X += 1f; 
            }

            if (position.Y > 950)
            {
                IsActive = false;
            }
        }

        public new void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        // Controleer of de vijand de speler heeft geraakt
        public bool CheckPlayerHit(Rectangle playerBounds)
        {
            if (Bounds.Intersects(playerBounds))
            {
                playerHitCount++;
                if (playerHitCount >= 3)
                {
                    playerHitCount = 0;
                    return true; 
                }
            }
            return false;
        }

    }
}

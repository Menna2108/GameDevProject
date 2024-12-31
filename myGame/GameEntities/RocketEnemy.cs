using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myGame.GameEntities
{
    public class RocketEnemy : EnemyBase
    {
        private Texture2D bulletTexture;
        private List<Bullet> bullets;
        private float shootCooldown = 2f;
        private float shootTimer;
        private int playerHitCount = 0;

        public RocketEnemy(Texture2D texture, Texture2D bulletTexture, Vector2 startPosition, float speed, float scale)
            : base(texture, startPosition, speed, scale)
        {
            this.bulletTexture = bulletTexture;
            bullets = new List<Bullet>();
            shootTimer = shootCooldown;
        }

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Bewegen naar beneden
            position.Y += speed;

            // Schieten
            shootTimer -= deltaTime;
            if (shootTimer <= 0)
            {
                shootTimer = shootCooldown;
                Vector2 bulletStartPosition = new Vector2(
                    position.X + (texture.Width * scale) / 2 - 5,
                    position.Y + (texture.Height * scale)
                );
                bullets.Add(new Bullet(bulletTexture, bulletStartPosition));
            }

            // Update kogels
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                bullets[i].Update(gameTime);
                if (!bullets[i].IsActive)
                {
                    bullets.RemoveAt(i);
                }
            }

            // Verwijderen als buiten scherm
            if (position.Y > 950)
            {
                IsActive = false;
            }
        }

        public new void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            foreach (var bullet in bullets)
            {
                bullet.Draw(spriteBatch);
            }
        }

        public List<Bullet> GetBullets() => bullets;

        public bool ProcessPlayerHit()
        {
            playerHitCount++;
            if (playerHitCount >= 3)
            {
                playerHitCount = 0;
                return true;
            }
            return false;
        }
    }
    
}

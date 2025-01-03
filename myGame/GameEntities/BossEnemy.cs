using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using myGame.GameComponents;

namespace myGame.GameEntities
{
    public class BossEnemy : EnemyBase
    {
        private Texture2D bulletTexture;
        private List<Bullet> bullets;
        private float shootTimer;
        private const float ShootCooldown = 1.5f;
        private Vector2 targetPosition;
        private int health = 3;
        private float invulnerabilityTimer;
        private const float InvulnerabilityDuration = 0.5f;
        private Vector2 playerPosition;  

        public bool IsInvulnerable => invulnerabilityTimer > 0;
        public List<Bullet> Bullets => bullets;
        public int Health => health;

        public Vector2 Position { get; internal set; }

        private Rectangle screenBounds;

        public BossEnemy(Texture2D texture, Texture2D bulletTexture, Vector2 startPosition, float speed, float scale)
            : base(texture, startPosition, speed, scale)
        {
            this.bulletTexture = bulletTexture;
            this.bullets = new List<Bullet>();
            this.shootTimer = ShootCooldown;
            this.targetPosition = startPosition;
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

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (invulnerabilityTimer > 0)
            {
                invulnerabilityTimer -= deltaTime;
            }

            Vector2 direction = targetPosition - position;
            if (direction != Vector2.Zero)
            {
                direction.Normalize();
                position += direction * speed * deltaTime;

                position.X = MathHelper.Clamp(position.X, 0, screenBounds.Width - texture.Width * scale);
                position.Y = MathHelper.Clamp(position.Y, 0, screenBounds.Height / 2); // Stay in upper half
            }

            shootTimer -= deltaTime;
            if (shootTimer <= 0)
            {
                Shoot();
                shootTimer = ShootCooldown;
            }

            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                bullets[i].Update(gameTime);
                if (!bullets[i].IsActive)
                {
                    bullets.RemoveAt(i);
                }
            }
        }


        private void Shoot()
        {
            Vector2 bulletPosition = new Vector2(
                position.X + (texture.Width * scale) / 2,
                position.Y + (texture.Height * scale)
            );

            Bullet bullet = new Bullet(bulletTexture, bulletPosition, true);

            Vector2 directionToPlayer = playerPosition - bulletPosition;
            if (directionToPlayer != Vector2.Zero)
            {
                directionToPlayer.Normalize();
                bullet.SetDirection(directionToPlayer);
            }

            bullets.Add(bullet);
        }


        public void UpdatePlayerPosition(Vector2 newPlayerPosition)
        {
            playerPosition = newPlayerPosition;
            targetPosition = new Vector2(
                newPlayerPosition.X,
                200  
            );
        }

        public void TakeDamage()
        {
            if (!IsInvulnerable)
            {
                health--;
                invulnerabilityTimer = InvulnerabilityDuration;
            }
        }

        public new void Draw(SpriteBatch spriteBatch)
        {
            if (!IsInvulnerable || (int)(invulnerabilityTimer * 10) % 2 == 0)
            {
                base.Draw(spriteBatch);
            }

            foreach (var bullet in bullets)
            {
                bullet.Draw(spriteBatch);
            }
        }
    }
}
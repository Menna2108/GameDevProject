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
        private Texture2D fireTexture;
        private float shootTimer;
        private const float ShootDelay = 1f;
        private float movementTimer;
        private float moveDirection = 1;
        private const float MovementSpeed = 6f;
        private List<Fire> bossProjectiles;
        private Random random;
        public int Health { get; private set; } = 3;

        public List<Fire> Projectiles => bossProjectiles;

        public BossEnemy(Texture2D texture, Texture2D fireTexture, Vector2 startPosition, float speed, float scale)
            : base(texture, startPosition, speed, scale)
        {
            this.fireTexture = fireTexture;
            this.IsActive = true;
            this.bossProjectiles = new List<Fire>();
            this.random = new Random();
            this.movementTimer = 0;
        }

        public void SetScreenBounds(Rectangle bounds)
        {
            screenBounds = bounds;
        }

        public override void Update(GameTime gameTime)
        {
            if (!IsActive) return;

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Horizontale beweging
            position.X += moveDirection * MovementSpeed;

            // Keer om bij de randen van het scherm
            if (position.X <= 0 || position.X >= screenBounds.Width - texture.Width * scale)
            {
                moveDirection *= -1;
            }


            // Schermgrenzen
            position.X = MathHelper.Clamp(position.X, 0, screenBounds.Width - texture.Width * scale);
            position.Y = 50;

            // Schiet logica
            shootTimer -= deltaTime;
            if (shootTimer <= 0)
            {
                ShootFire();
                shootTimer = ShootDelay;
            }

            // Update
            for (int i = bossProjectiles.Count - 1; i >= 0; i--)
            {
                bossProjectiles[i].Update(gameTime);
                if (!bossProjectiles[i].IsActive)
                {
                    bossProjectiles.RemoveAt(i);
                }
            }
        }
        public void TakeDamage()
        {
            Health--;
            if (Health <= 0)
            {
                IsActive = false;
            }
        }


        private void ShootFire()
        {
            Vector2 firePos = new Vector2(
               position.X + ((texture.Width * scale) - (fireTexture.Width * 0.2f)) / 2,
               position.Y + (texture.Height * scale)
            );

            Fire fire = new Fire(fireTexture, firePos);
            bossProjectiles.Add(fire);
        }

        public new void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive)
            {
                base.Draw(spriteBatch);
                foreach (var fire in bossProjectiles)
                {
                    fire.Draw(spriteBatch);
                }
            }
        }

        public bool CheckCollisionWithPlayer(Rectangle playerBounds)
        {
            foreach (var fire in bossProjectiles)
            {
                if (fire.IsActive && fire.Bounds.Intersects(playerBounds))
                {
                    fire.IsActive = false;
                    return true;
                }
            }
            return false;
        }
        public bool CheckCollisionWithBullets(List<Bullet> playerBullets)
        {
            foreach (var bullet in playerBullets)
            {
                if (bullet.IsActive && Bounds.Intersects(bullet.Bounds))
                {
                    bullet.Deactivate();
                    TakeDamage();
                    return true;
                }
            }
            return false;
        }
       

        public void Reset()
        {
            bossProjectiles.Clear();
            shootTimer = ShootDelay;
            movementTimer = 0;
            moveDirection = 1;
            position = new Vector2(screenBounds.Width / 2, 100);
        }
    }
}
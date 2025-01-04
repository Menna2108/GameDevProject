using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using myGame.GameComponents;
using myGame.GameManagers;
using System;
using System.Collections.Generic;

namespace myGame.GameEntities
{
    public class Rocket
    {
        private Texture2D texture;
        private Vector2 position;
        private float speed = 4f;
        private float scale = 0.16f;

        private Texture2D bulletTexture;
        private float shootCooldown = 0.5f;
        private float shootTimer = 0f;

        private int health;
        public const int MaxHealth = 3;
        public int Health
        {
            get => health;
            set
            {
                health = Math.Max(0, Math.Min(value, MaxHealth));
                if (health <= 0)
                {
                    GameManager.Instance.IsGameOver = true;
                }
            }
        }
        public bool CanShoot => shootTimer <= 0;

        public Vector2 Position => position;

        public Vector2 Center => new Vector2(
            position.X + (texture.Width * scale) / 2,
            position.Y
        );

        public Rectangle Bounds => new Rectangle(
            (int)position.X,
            (int)position.Y,
            (int)(texture.Width * scale),
            (int)(texture.Height * scale)
        );

        public Rocket(Texture2D texture, Texture2D bulletTexture, Vector2 startPosition)
        {
            this.texture = texture;
            this.bulletTexture = bulletTexture;
            this.position = startPosition;
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (keyboardState.IsKeyDown(Keys.Up) && position.Y > 0)
                position.Y -= speed;

            if (keyboardState.IsKeyDown(Keys.Down) && position.Y + texture.Height * scale < 950)
                position.Y += speed;

            if (keyboardState.IsKeyDown(Keys.Left) && position.X > 0)
                position.X -= speed;

            if (keyboardState.IsKeyDown(Keys.Right) && position.X + texture.Width * scale < 1900)
                position.X += speed;

            if (shootTimer > 0)
                shootTimer -= deltaTime;
        }

        public Bullet Shoot()
        {
            if (bulletTexture == null)
                throw new InvalidOperationException("Bullet texture is not loaded!");

            if (CanShoot)
            {
                shootTimer = shootCooldown;
                Vector2 bulletStartPosition = new Vector2(
                    Center.X - (bulletTexture.Width * 0.05f),
                    position.Y - (bulletTexture.Height * 0.1f)
                );

                return new Bullet(bulletTexture, bulletStartPosition);
            }

            return null;
        }

        public void Draw(SpriteBatch spriteBatch)
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

        public void Reset(Vector2 startPosition)
        {
            position = startPosition;
            shootTimer = 0;
            Health = MaxHealth;
        }
        public void LoseHealth()
        {
            Health--;
            if (Health <= 0)
            {
                GameManager.Instance.IsGameOver = true;
            }
        }
        public void CheckCollisionWithMeteors(List<MeteorEnemy> meteors)
        {
            foreach (var meteor in meteors)
            {
                if (Bounds.Intersects(meteor.Bounds)) 
                {
                    LoseHealth();
                    meteor.IsActive = false;  
                }
            }
        }
        public void IncreaseSpeed(float increment)
        {
            speed += increment;
        }

        public void DecreaseShootCooldown(float decrement)
        {
            shootCooldown = Math.Max(0.1f, shootCooldown - decrement);  
        }
        public void SetHealth(int value)
        {
            Health = value;
        }

    }

}
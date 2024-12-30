using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

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
        private ContentManager content;
        private Vector2 vector2;

        // Eigenschap: kan de rocket schieten?
        public bool CanShoot => shootTimer <= 0;

        // Eigenschap: positie van de rocket
        public Vector2 Position => position;

        // Eigenschap: center van de rocket, handig voor bullet startpositie
        public Vector2 Center => new Vector2(
            position.X + (texture.Width * scale) / 2,
            position.Y
        );

        // Eigenschap: bounds van de rocket (voor botsingsdetectie)
        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(
                    (int)position.X,
                    (int)position.Y,
                    (int)(texture.Width * scale),
                    (int)(texture.Height * scale)
                );
            }
        }

        
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
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace myGame.GameEntities
{
    public class RocketEnemy : EnemyBase
    {
        private List<Vector2> bullets;
        private float shootCooldown = 1.5f; // Snellere schietsnelheid
        private float shootTimer;
        private int playerHitCount = 0;

        public RocketEnemy(Texture2D texture, Vector2 startPosition, float speed, float scale)
            : base(texture, startPosition, speed, scale)
        {
            bullets = new List<Vector2>();
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

                // Puntje (coördinaat) toevoegen
                Vector2 bulletPosition = new Vector2(
                    position.X + (texture.Width * scale) / 2,
                    position.Y + (texture.Height * scale)
                );
                bullets.Add(bulletPosition);
            }

            // Update puntjes
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                bullets[i] = new Vector2(bullets[i].X, bullets[i].Y + 5f); // Bewegen naar beneden
                if (bullets[i].Y > 950) // Buiten scherm
                {
                    bullets.RemoveAt(i);
                }
            }

            // Verwijderen als vijand buiten scherm
            if (position.Y > 950)
            {
                IsActive = false;
            }
        }

        public new void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            // Teken de puntjes
            foreach (var bullet in bullets)
            {
                spriteBatch.Draw(
                    texture: null,
                    position: bullet,
                    sourceRectangle: null,
                    color: Color.Red,
                    rotation: 0f,
                    origin: Vector2.Zero,
                    scale: new Vector2(3f, 3f), // Maak het punt klein
                    effects: SpriteEffects.None,
                    layerDepth: 0f
                );
            }
        }

        public List<Vector2> GetBullets() => bullets;

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

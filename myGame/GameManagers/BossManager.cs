using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using myGame.GameComponents;
using myGame.GameEntities;
using System.Collections.Generic;

namespace myGame.GameManagers
{
    public class BossManager
    {
        private BossEnemy boss;
        private bool isLevel3;
        private bool isGameWon;

        public bool IsGameWon => isGameWon;

        public BossManager(Texture2D bossTexture, Texture2D bulletTexture)
        {
            Vector2 startPosition = new Vector2(950, 100);
            boss = new BossEnemy(bossTexture, bulletTexture, startPosition, 3f, 0.3f);
            isLevel3 = false;
            isGameWon = false;
        }
        public void Initialize(Rectangle screenBounds)
        {
            if (boss != null)
            {
                boss.SetScreenBounds(screenBounds);
                boss.IsActive = false; // Start inactive until level 3
            }
        }

        public void Update(GameTime gameTime, Rocket playerRocket, List<Bullet> playerBullets)
        {
            if (!isLevel3 || boss == null) return;
            boss.Update(gameTime);

            // Check player bullet collisions
            foreach (var bullet in playerBullets)
            {
                if (bullet.IsActive && boss.Bounds.Intersects(bullet.Bounds))
                {
                    boss.TakeDamage();
                    bullet.IsActive = false;

                    if (boss.Health <= 0)
                    {
                        isGameWon = true;
                        return;
                    }
                }
            }

            // Check boss bullet collisions with player
            foreach (var bullet in boss.Bullets)
            {
                if (bullet.IsActive && playerRocket.Bounds.Intersects(bullet.Bounds))
                {
                    playerRocket.LoseHealth();
                    bullet.IsActive = false;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!isLevel3 || boss == null) return;
            boss.Draw(spriteBatch);
        }

        public void SetLevel(int level)
        {
            isLevel3 = (level == 3);
            if (isLevel3 && boss != null)
            {
                boss.IsActive = true;
                boss.Position = new Vector2(950, 100); 
            }
        }


        public void Reset()
        {
            if (boss != null)
            {
                boss.IsActive = false;
            }
            isGameWon = false;
            isLevel3 = false;
        }
        public void UpdatePlayerPosition(Vector2 playerPosition)
        {
            if (boss != null)
            {
                boss.UpdatePlayerPosition(playerPosition);
            }
        }
    }
}
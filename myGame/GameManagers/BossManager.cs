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
        public bool IsBossDefeated => boss != null && !boss.IsActive;

        public BossManager(Texture2D bossTexture, Texture2D fireTexture)
        {
            Vector2 startPosition = new Vector2(950, 100);
            if (bossTexture != null && fireTexture != null)
            {
                boss = new BossEnemy(bossTexture, fireTexture, startPosition, 3f, 0.3f);
            }
            isLevel3 = false;
        }

        public void Initialize(Rectangle screenBounds)
        {
            if (boss != null)
            {
                boss.SetScreenBounds(screenBounds);
                boss.IsActive = false; 
            }
        }

        public void Update(GameTime gameTime, Rocket playerRocket, List<Bullet> playerBullets)
        {
            if (!isLevel3 || boss == null) return;

            boss.Update(gameTime);

            // Controleer of de boss geraakt wordt door spelerkogels
            if (boss.CheckCollisionWithBullets(playerBullets))
            {
                boss.TakeDamage();
            }

            // Controleer of de speler geraakt wordt door boss-projectielen
            if (boss.CheckCollisionWithPlayer(playerRocket.Bounds))
            {
                playerRocket.LoseHealth();
            }

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (isLevel3 && boss != null && boss.IsActive)
            {
                boss.Draw(spriteBatch);
            }
        }

        public void SetLevel(int level)
        {
            isLevel3 = (level == 3);
            if (boss != null)
            {
                boss.IsActive = isLevel3;
                if (isLevel3)
                {
                    boss.Reset();
                }
            }
        }
        public void TakeDamage()
        {
            if (boss != null && boss.IsActive)
            {
                boss.TakeDamage();
            }
        }
        public bool CheckCollisionWithPlayer(Rectangle playerBounds)
        {
            if (boss != null && boss.IsActive)
            {
                return boss.CheckCollisionWithPlayer(playerBounds);
            }
            return false;
        }

        public bool CheckCollisionWithPlayerBounds(Rectangle playerBounds)
        {
            if (boss != null && boss.IsActive)
            {
                return boss.Bounds.Intersects(playerBounds);
            }
            return false;
        }

        public void Reset()
        {
            if (boss != null)
            {
                boss.IsActive = false;
                boss.Reset(); 
            }
            isLevel3 = false;
        }
        public int GetBossHealth()
        {
            if (boss != null && boss.IsActive)
            {
                return boss.Health;
            }
            return 0;
        }
    }
}
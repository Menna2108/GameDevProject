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


        public BossManager(Texture2D bossTexture, Texture2D bulletTexture)
        {
            // Boss in het midden van het scherm maken
            Vector2 startPosition = new Vector2(950, 100);
            boss = new BossEnemy(bossTexture, bulletTexture, startPosition, 3f, 0.3f);
            isLevel3 = false;
        }

        public void Initialize(Rectangle screenBounds)
        {
            if (boss != null)
            {
                boss.SetScreenBounds(screenBounds);
                boss.IsActive = false; // Start inactief tot level 3
            }
        }

        public void Update(GameTime gameTime, Rocket playerRocket, List<Bullet> playerBullets)
        {
            if (!isLevel3 || boss == null) return;

            // Update boss positie
            boss.Update(gameTime);

            // Check voor botsingen met speler kogels
            //foreach (var bullet in playerBullets)
            //{
            //    if (bullet.IsActive && boss.Bounds.Intersects(bullet.Bounds))
            //    {
            //        boss.TakeDamage();
            //        bullet.IsActive = false;
            //    }
            //}
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
                    boss.Position = new Vector2(950, 100);
                }
            }
        }

        public void Reset()
        {
            if (boss != null)
            {
                boss.IsActive = false;
            }
            isLevel3 = false;
        }
    }
}

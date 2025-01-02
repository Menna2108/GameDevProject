using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using myGame.GameEntities;
using System;
using System.Collections.Generic;

namespace myGame.GameManagers
{
    public class MeteorManager
    {
        private List<MeteorEnemy> meteors;
        private Texture2D meteorTexture;
        private bool isLevel2;
        private float spawnTimer;
        private const float spawnInterval = 1.5f; 
        private Random random;

        public MeteorManager(Texture2D texture)
        {
            meteors = new List<MeteorEnemy>();
            meteorTexture = texture;
            isLevel2 = false;
            random = new Random();
        }

        public void Update(GameTime gameTime, Rocket playerRocket, List<EnemyBase> enemies)
        {
            if (!isLevel2) return;
            spawnTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (spawnTimer <= 0)
            {
                SpawnMeteor();
                spawnTimer = spawnInterval;
            }

            for (int i = meteors.Count - 1; i >= 0; i--)
            {
                meteors[i].Update(gameTime);

                if (!meteors[i].IsActive)
                {
                    meteors.RemoveAt(i);
                    continue;
                }

                if (meteors[i].Bounds.Intersects(playerRocket.Bounds))
                {
                    if (playerRocket.Health > 0)
                    {
                        playerRocket.Health--;
                    }
                    meteors[i].IsActive = false;
                }
                for (int j = enemies.Count - 1; j >= 0; j--)
                {
                    // Check of de meteoor met een vijand botst
                    if (meteors[i].Bounds.Intersects(enemies[j].Bounds))
                    {
                        // Verwijder vijand
                        enemies[j].IsActive = false;  

                        break; 
                    }
                }
            }
        }

        private void SpawnMeteor()
        {
            float speed = 3f; 
            float scale = 0.15f; 

            Vector2 position = new Vector2(
                random.Next(50, 1850), 
                -50 
            );

            meteors.Add(new MeteorEnemy(meteorTexture, position, speed, scale));
        }

        public void SetLevel(int level, Rectangle screenBounds)
        {
            isLevel2 = (level == 2);
            if (isLevel2)
            {
                spawnTimer = 0;
            }
            else
            {
                meteors.Clear();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!isLevel2) return;

            foreach (var meteor in meteors)
            {
                meteor.Draw(spriteBatch);
            }
        }

        public void RestartMeteors(Rectangle screenBounds)
        {
            meteors.Clear();
            spawnTimer = 0;
        }

        public List<MeteorEnemy> GetMeteors()
        {
            return meteors;
        }
    }
}

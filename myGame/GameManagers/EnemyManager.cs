using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using myGame.GameEntities;
using System;
using System.Collections.Generic;
using myGame.GameComponents;

namespace myGame.GameManagers
{
    public class EnemyManager
    {
        private List<EnemyBase> enemies;
        private Texture2D rocketEnemyTexture;
        private Texture2D rocketBulletTexture;
        private int spawnCooldown = 2;
        private float spawnTimer;

        public EnemyManager(Texture2D rocketEnemyTexture, Texture2D rocketBulletTexture)
        {
            this.rocketEnemyTexture = rocketEnemyTexture;
            this.rocketBulletTexture = rocketBulletTexture;
            enemies = new List<EnemyBase>();
        }


        public void Update(GameTime gameTime, Rectangle playerBounds, List<Bullet> playerBullets, Action onPlayerHit)
        {
            spawnTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Spawn nieuwe vijanden
            if (spawnTimer <= 0)
            {
                spawnTimer = spawnCooldown;
                SpawnRocketEnemy();
            }

            // Update vijanden
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                enemies[i].Update(gameTime);

                if (!enemies[i].IsActive)
                {
                    enemies.RemoveAt(i);
                    continue;
                }

                // Logica voor vijandelijke kogels
                if (enemies[i] is RocketEnemy rocketEnemy)
                {
                    foreach (var bulletPosition in rocketEnemy.GetBullets())
                    {
                        if (playerBounds.Contains(bulletPosition.ToPoint()))
                        {
                            if (rocketEnemy.ProcessPlayerHit())
                            {
                                onPlayerHit();
                            }
                        }
                    }
                }

                // Controle op botsing met spelerkogels
                for (int j = playerBullets.Count - 1; j >= 0; j--)
                {
                    if (enemies[i].Bounds.Intersects(playerBullets[j].Bounds))
                    {
                        enemies.RemoveAt(i);
                        playerBullets.RemoveAt(j);
                        break;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var enemy in enemies)
            {
                enemy.Draw(spriteBatch);
            }
        }

        private void SpawnRocketEnemy()
        {
            Vector2 startPosition = new Vector2(Random.Shared.Next(50, 1850), -50);
            float scale = 0.15f; 
            enemies.Add(new RocketEnemy(rocketEnemyTexture, startPosition, 2f, scale));
        }

        public void Reset()
        {
            enemies.Clear();
            spawnTimer = 0; 
        }
    }
}

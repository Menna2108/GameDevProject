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
        private float spawnTimer;
        private float spawnCooldown = 1f;
        private int enemiesToSpawn = 1; 
        private float enemySpeedIncrement = 0.05f; 
        private float currentEnemySpeed = 2f; 
        private int maxEnemiesInLevel1 = 50; 

        public EnemyManager(Texture2D rocketEnemyTexture)
        {
            this.rocketEnemyTexture = rocketEnemyTexture;
            enemies = new List<EnemyBase>();
        }

        public void Update(GameTime gameTime, Rocket playerRocket, List<Bullet> playerBullets)
        {
            spawnTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Spawn vijanden
            if (spawnTimer <= 0)
            {
                spawnTimer = spawnCooldown;

                for (int i = 0; i < enemiesToSpawn; i++)
                {
                    SpawnRocketEnemy();
                }

                IncreaseEnemySpeed();
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

                // Controleer of de vijand de speler heeft geraakt
                if (enemies[i] is RocketEnemy rocketEnemy)
                {
                    if (rocketEnemy.CheckPlayerHit(playerRocket.Bounds))
                    {
                        // Verlies een hart bij botsing
                        if (playerRocket.Health > 0)
                        {
                            playerRocket.Health--;
                        }

                        // Stop de vijand na de botsing
                        enemies[i].IsActive = false;
                    }
                }

                // Controleer of kogels de vijand raken
                foreach (var bullet in playerBullets)
                {
                    if (bullet.Bounds.Intersects(enemies[i].Bounds))
                    {
                        enemies[i].IsActive = false;  
                        bullet.IsActive = false;      
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
            enemies.Add(new RocketEnemy(rocketEnemyTexture, startPosition, currentEnemySpeed, scale));
        }

        private void IncreaseEnemySpeed()
        {
            currentEnemySpeed += enemySpeedIncrement;
        }

        public void Reset()
        {
            enemies.Clear();
            spawnTimer = 0;
            currentEnemySpeed = 2f; 
        }

        public List<EnemyBase> GetEnemies()
        {
            return enemies;
        }
    }


}

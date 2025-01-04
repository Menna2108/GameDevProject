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
        private float baseSpeed = 2f; 
        private float currentSpeed; 
        private int maxEnemiesInLevel1 = 50;
        private List<EnemyBase> destroyedEnemies = new List<EnemyBase>();
        private bool isActive = true;

        public EnemyManager(Texture2D rocketEnemyTexture)
        {
            this.rocketEnemyTexture = rocketEnemyTexture;
            enemies = new List<EnemyBase>();
            this.currentSpeed = baseSpeed;
        }

        public void SetActive(bool active)
        {
            isActive = active;
        }

        public void ResetEnemySpeed()
        {
            currentSpeed = baseSpeed;
        }

        public void IncreaseEnemySpeed(float multiplier)
        {
            currentSpeed = baseSpeed * multiplier;
        }

        public void Update(GameTime gameTime, Rocket playerRocket, List<Bullet> playerBullets)
        {
            if (!isActive) return; 

            spawnTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            // stoppen bij level 3
            if (GameManager.Instance.CurrentLevel == 3)
            {
                return;
            }

            // Spawn vijanden
            if (spawnTimer <= 0)
            {
                spawnTimer = spawnCooldown;

                for (int i = 0; i < enemiesToSpawn; i++)
                {
                    SpawnEnemy();
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
                        destroyedEnemies.Add(enemies[i]);
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

        private void SpawnEnemy()
        {
            Random rand = new Random();
            float xPos = rand.Next(0, 1800); 
            Vector2 position = new Vector2(xPos, -50);

            RocketEnemy enemy = new RocketEnemy(rocketEnemyTexture, position, currentSpeed, 0.2f);
            enemies.Add(enemy);
        }

        private void IncreaseEnemySpeed()
        {
            currentSpeed += enemySpeedIncrement;
        }

        public void Reset()
        {
            enemies.Clear();
            spawnTimer = 0;
            currentSpeed = baseSpeed;
        }

        public List<EnemyBase> GetEnemies()
        {
            return enemies;
        }

        public void ClearAllEnemies()
        {
            enemies.Clear();
        }

        public List<EnemyBase> GetDestroyedEnemies()
        {
            var tempDestroyed = new List<EnemyBase>(destroyedEnemies);
            destroyedEnemies.Clear();
            return tempDestroyed;
        }
    }
}
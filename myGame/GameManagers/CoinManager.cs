using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using myGame.GameEntities;
using myGame.GameInterfaces;
using System;
using System.Collections.Generic;

namespace myGame.GameManagers
{
    public class CoinManager
    {
        private Texture2D coinTexture;
        private List<Coin> coins = new List<Coin>();
        private List<ICoinCollector> observers = new List<ICoinCollector>();

        private int viewportHeight;
        private int viewportWidth;

        public CoinManager(Texture2D texture)
        {
            coinTexture = texture;
        }

        // Observers toevoegen
        public void AddObserver(ICoinCollector observer)
        {
            observers.Add(observer);
        }

        // Coin-verzamelaars notificeren
        private void NotifyCoinCollected()
        {
            foreach (var observer in observers)
            {
                observer.OnCoinCollected();
            }
        }

        public void GenerateRandomCoins(int count, Rectangle bounds, float maxYPosition)
        {
            viewportWidth = bounds.Width;
            viewportHeight = bounds.Height;

            coins.Clear();
            for (int i = 0; i < count; i++)
            {
                int x = Random.Shared.Next(bounds.Left, bounds.Right - (int)(coinTexture.Width * 0.1f));
                int y = Random.Shared.Next(bounds.Top, Math.Max(bounds.Top + 1, (int)maxYPosition));

                coins.Add(new Coin(new Vector2(x, y)));
            }
        }

        public void Update(GameTime gameTime, Rectangle playerBounds, float backgroundOffset, float maxYPosition)
        {
            // Checken of de speler coins oppakt
            for (int i = coins.Count - 1; i >= 0; i--)
            {
                if (playerBounds.Intersects(coins[i].Bounds))
                {
                    coins.RemoveAt(i);
                    NotifyCoinCollected(); 
                }
            }

            if (backgroundOffset >= viewportHeight)
            {
                GenerateRandomCoins(5, new Rectangle(0, 0, viewportWidth, viewportHeight), maxYPosition);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var coin in coins)
            {
                coin.Draw(spriteBatch, coinTexture);
            }
        }
    }
}

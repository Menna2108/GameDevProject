using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using myGame.GameEntities;
using System;
using System.Collections.Generic;

namespace myGame.GameManagers
{
    public class CoinManager
    {
        private Texture2D coinTexture;
        private List<Coin> coins = new List<Coin>();
        public int CollectedCoins { get; private set; } = 0;

        private int viewportHeight;
        private int viewportWidth;

        public CoinManager(Texture2D texture)
        {
            coinTexture = texture;
        }

        // Methode om willekeurige coins te genereren, met maxYPosition als limiet
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

        // Update-methode om interacties en hergeneratie van coins te verwerken
        public void Update(GameTime gameTime, Rectangle playerBounds, float backgroundOffset, float maxYPosition)
        {
            // Check of de speler coins oppakt
            for (int i = coins.Count - 1; i >= 0; i--)
            {
                if (playerBounds.Intersects(coins[i].Bounds))
                {
                    coins.RemoveAt(i);
                    CollectedCoins++;
                }
            }

            // Als de achtergrond is gereset, genereer nieuwe coins boven de raket
            if (backgroundOffset >= viewportHeight)
            {
                GenerateRandomCoins(5, new Rectangle(0, 0, viewportWidth, viewportHeight), maxYPosition);
            }
        }

        // Tekent alle coins op het scherm
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var coin in coins)
            {
                coin.Draw(spriteBatch, coinTexture);
            }
        }
    }
}

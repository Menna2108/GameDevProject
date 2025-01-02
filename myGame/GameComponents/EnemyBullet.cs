using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myGame.GameComponents
{
    public class EnemyBullet
    {
        public Vector2 Position { get; private set; }
        public float Speed { get; private set; } = 5f;
        public int Size { get; private set; } = 5; // Grootte van het puntje
        public bool IsActive { get; private set; } = true;

        public Rectangle Bounds => new Rectangle(
            (int)Position.X,
            (int)Position.Y,
            Size,
            Size
        );

        public EnemyBullet(Vector2 startPosition)
        {
            Position = startPosition;
        }

        public void Update()
        {
            Position = new Vector2(Position.X, Position.Y + Speed);

            // Deactiveer als buiten scherm
            if (Position.Y > 1080) // Schermhoogte aanpassen indien nodig
                IsActive = false;
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D pixelTexture)
        {
            if (IsActive)
            {
                spriteBatch.Draw(
                    pixelTexture,
                    Bounds,
                    Color.Red // De kleur van het puntje
                );
            }
        }
        public void Deactivate()
        {
            IsActive = false;
        }
    }
}

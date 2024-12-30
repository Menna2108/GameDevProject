using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace myGame.GameEntities
{
    public class Coin
    {
        private const float Scale = 0.1f; 
        public Vector2 Position { get; private set; }
        public Rectangle Bounds => new Rectangle(
            (int)Position.X,
            (int)Position.Y,
            (int)(32 * Scale),  
            (int)(32 * Scale)   
        );

        public Coin(Vector2 position)
        {
            Position = position;
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            spriteBatch.Draw(
                texture,
                Position,
                null,
                Color.White,
                0f,                  
                Vector2.Zero,        
                Scale,               
                SpriteEffects.None,
                0f                   
            );
        }
    }
}

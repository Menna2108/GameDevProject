using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MyGame.GameComponents
{
    public class Button
    {
        private Texture2D texture;
        private Vector2 position;
        private float scale;
        private bool isGrowing;
        private float ScalingSpeed = 0.01f;
        private float maxScale;
        private float minScale;
        private bool animate;

        public Button(Texture2D texture, Vector2 position, float initialScale = 1f, bool animate = true, float minScale = 0.8f, float maxScale = 1f, float speed = 0.01f)
        {
            this.texture = texture;
            this.position = position;
            this.scale = initialScale;
            this.isGrowing = true;
            this.animate = animate;
            this.minScale = minScale;
            this.maxScale = maxScale;
            this.ScalingSpeed = speed;
        }

        public Rectangle GetBounds()
        {
            return new Rectangle(
                (int)(position.X - (texture.Width * scale) / 2),
                (int)(position.Y - (texture.Height * scale) / 2),
                (int)(texture.Width * scale),
                (int)(texture.Height * scale)
            );
        }

        public void Update()
        {
            if (animate) 
            {
                if (isGrowing)
                {
                    scale += ScalingSpeed;
                    if (scale >= maxScale)
                        isGrowing = false;
                }
                else
                {
                    scale -= ScalingSpeed;
                    if (scale <= minScale)
                        isGrowing = true;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                texture,
                position,
                null,
                Color.White,
                0f,
                new Vector2(texture.Width / 2, texture.Height / 2),
                scale,
                SpriteEffects.None,
                0f
            );
        }
    }
}

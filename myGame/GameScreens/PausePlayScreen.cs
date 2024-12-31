using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace myGame.GameScreens
{
    public class PausePlayScreen
    {
        private Texture2D pauseButtonTexture;
        private Texture2D playButtonTexture;
        private Vector2 buttonPosition;

        private const float ButtonScale = 0.3f;
        private const float PlayButtonAdjustment = 0.92f;
        private bool isPaused = false;

        // Offsets om knoppen visueel uit te lijnen -> omdat de originele hadden verschillende grotte
        private Vector2 pauseButtonOffset = new Vector2(0, 0);
        private Vector2 playButtonOffset = new Vector2(5, 5);

        public PausePlayScreen(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            pauseButtonTexture = content.Load<Texture2D>("pause");
            playButtonTexture = content.Load<Texture2D>("play");

            // Positie instellen
            buttonPosition = new Vector2(1750, 2);
        }

        public void Update(MouseState mouseState, ref bool isGamePaused)
        {
            // Bounds voor Pause/Play-knop
            Rectangle buttonBounds = new Rectangle(
                (int)buttonPosition.X,
                (int)buttonPosition.Y,
                (int)(pauseButtonTexture.Width * ButtonScale),
                (int)(pauseButtonTexture.Height * ButtonScale)
            );

            // Klik-detectie
            if (mouseState.LeftButton == ButtonState.Pressed && buttonBounds.Contains(mouseState.Position))
            {
                isPaused = !isPaused;
                isGamePaused = isPaused;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (isPaused)
            {
                spriteBatch.Draw(playButtonTexture, buttonPosition + playButtonOffset, null, Color.White, 0f,
                    Vector2.Zero, ButtonScale * PlayButtonAdjustment, SpriteEffects.None, 0f);
            }
            else
            {
                spriteBatch.Draw(pauseButtonTexture, buttonPosition + pauseButtonOffset, null, Color.White, 0f,
                    Vector2.Zero, ButtonScale, SpriteEffects.None, 0f);
            }
        }
    }
}
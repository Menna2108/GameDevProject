using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace myGame.GameScreens
{
    public class StartScreen
    {
        private Texture2D startButtonTexture;
        private Texture2D exitButtonTexture;
        private Vector2 startButtonPosition;
        private Vector2 exitButtonPosition;

        private float buttonScale = 1f;
        private bool isGrowing = true;

        public StartScreen(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            startButtonTexture = content.Load<Texture2D>("start");
            exitButtonTexture = content.Load<Texture2D>("exit");

            startButtonPosition = new Vector2(800, 400);
            exitButtonPosition = new Vector2(800, 500);
        }

        public void Update(GameTime gameTime, MouseState mouseState, ref bool isGameStarted, Game game)
        {
            if (isGrowing)
            {
                buttonScale += 0.01f;
                if (buttonScale >= 1.2f)
                    isGrowing = false;
            }
            else
            {
                buttonScale -= 0.01f;
                if (buttonScale <= 1.0f)
                    isGrowing = true;
            }

            Rectangle startButtonBounds = new Rectangle(
                (int)(startButtonPosition.X - (startButtonTexture.Width * buttonScale) / 2),
                (int)(startButtonPosition.Y - (startButtonTexture.Height * buttonScale) / 2),
                (int)(startButtonTexture.Width * buttonScale),
                (int)(startButtonTexture.Height * buttonScale)
            );

            Rectangle exitButtonBounds = new Rectangle(
                (int)(exitButtonPosition.X - exitButtonTexture.Width / 2),
                (int)(exitButtonPosition.Y - exitButtonTexture.Height / 2),
                exitButtonTexture.Width,
                exitButtonTexture.Height
            );

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if (startButtonBounds.Contains(mouseState.Position))
                {
                    isGameStarted = true;
                }
                else if (exitButtonBounds.Contains(mouseState.Position))
                {
                    game.Exit();
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(startButtonTexture, startButtonPosition, null, Color.White, 0f,
                new Vector2(startButtonTexture.Width / 2, startButtonTexture.Height / 2), buttonScale, SpriteEffects.None, 0f);

            spriteBatch.Draw(exitButtonTexture, exitButtonPosition, null, Color.White, 0f,
                new Vector2(exitButtonTexture.Width / 2, exitButtonTexture.Height / 2), 1f, SpriteEffects.None, 0f);
        }
    }
}

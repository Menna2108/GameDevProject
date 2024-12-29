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
        private float exitButtonScale = 0.4f;
        private bool isGrowing = true;

        // Factor voor langzamere scaling van de Exit-knop
        private const float ExitScalingFactor = 0.5f;

        public StartScreen(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            startButtonTexture = content.Load<Texture2D>("start");
            exitButtonTexture = content.Load<Texture2D>("exit");

            // Posities instellen
            startButtonPosition = new Vector2(875, 400);
            exitButtonPosition = new Vector2(875, 600);
        }

        public void Update(MouseState mouseState, ref bool isGameStarted, Game game)
        {
            if (isGrowing)
            {
                buttonScale += 0.01f;
                exitButtonScale += 0.01f * ExitScalingFactor;

                if (buttonScale >= 1.2f)
                    isGrowing = false;
            }
            else
            {
                buttonScale -= 0.01f;
                exitButtonScale -= 0.01f * ExitScalingFactor;

                if (buttonScale <= 1.0f)
                    isGrowing = true;
            }

            // Bounds voor Start-knop
            Rectangle startButtonBounds = new Rectangle(
                (int)(startButtonPosition.X - (startButtonTexture.Width * buttonScale) / 2),
                (int)(startButtonPosition.Y - (startButtonTexture.Height * buttonScale) / 2),
                (int)(startButtonTexture.Width * buttonScale),
                (int)(startButtonTexture.Height * buttonScale)
            );

            // Bounds voor Exit-knop
            Rectangle exitButtonBounds = new Rectangle(
                (int)(exitButtonPosition.X - (exitButtonTexture.Width * exitButtonScale) / 2),
                (int)(exitButtonPosition.Y - (exitButtonTexture.Height * exitButtonScale) / 2),
                (int)(exitButtonTexture.Width * exitButtonScale),
                (int)(exitButtonTexture.Height * exitButtonScale)
            );

            // Klik-detectie
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
                new Vector2(exitButtonTexture.Width / 2, exitButtonTexture.Height / 2), exitButtonScale, SpriteEffects.None, 0f);
        }
    }
}

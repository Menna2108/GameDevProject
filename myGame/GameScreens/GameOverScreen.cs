using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MyGame.GameScreens
{
    public class GameOverScreen
    {
        private Texture2D newGameButtonTexture;
        private Texture2D exitButtonTexture;
        private Texture2D gameOverTexture;

        private Vector2 newGameButtonPosition;
        private Vector2 exitButtonPosition;
        private Vector2 gameOverPosition;

        private float buttonScale = 0.8f;
        private float exitButtonScale = 0.8f;
        private bool isGrowing = true;

        private const float ScalingSpeed = 0.01f;
        private const float MaxScale = 1f;
        private const float MinScale = 0.8f;

        public GameOverScreen(ContentManager content)
        {
            newGameButtonTexture = content.Load<Texture2D>("restart");
            exitButtonTexture = content.Load<Texture2D>("exit");
            gameOverTexture = content.Load<Texture2D>("gameover"); 
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            int screenWidth = graphicsDevice.Viewport.Width;
            int screenHeight = graphicsDevice.Viewport.Height;

            int buttonWidth = (int)(newGameButtonTexture.Width * 0.8f);
            int buttonHeight = (int)(newGameButtonTexture.Height * 0.8f);

            newGameButtonPosition = new Vector2(
                (screenWidth / 2) - (buttonWidth / 2) - 100, 
                screenHeight / 2
            );

            exitButtonPosition = new Vector2(
                (screenWidth / 2) + (buttonWidth / 2) + 100, 
                screenHeight / 2
            );

            gameOverPosition = new Vector2(
                (screenWidth - gameOverTexture.Width) / 2,
                (screenHeight / 2) - gameOverTexture.Height - 50 
            );
        }

        public void Update(MouseState mouseState, out bool startNewGame, out bool exitGame)
        {
            startNewGame = false;
            exitGame = false;

            // Animatie 
            if (isGrowing)
            {
                buttonScale += ScalingSpeed;
                exitButtonScale += ScalingSpeed;

                if (buttonScale >= MaxScale)
                {
                    isGrowing = false;
                }
            }
            else
            {
                buttonScale -= ScalingSpeed;
                exitButtonScale -= ScalingSpeed;

                if (buttonScale <= MinScale)
                {
                    isGrowing = true;
                }
            }

            // Bounds voor New Game-knop
            Rectangle newGameButtonBounds = new Rectangle(
                (int)(newGameButtonPosition.X - (newGameButtonTexture.Width * buttonScale) / 2),
                (int)(newGameButtonPosition.Y - (newGameButtonTexture.Height * buttonScale) / 2),
                (int)(newGameButtonTexture.Width * buttonScale),
                (int)(newGameButtonTexture.Height * buttonScale)
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
                if (newGameButtonBounds.Contains(mouseState.Position))
                {
                    startNewGame = true;
                }
                else if (exitButtonBounds.Contains(mouseState.Position))
                {
                    exitGame = true;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                gameOverTexture,
                gameOverPosition,
                Color.White
            );

            // Draw New Game button
            spriteBatch.Draw(
                newGameButtonTexture,
                newGameButtonPosition,
                null,
                Color.White,
                0f,
                new Vector2(newGameButtonTexture.Width / 2, newGameButtonTexture.Height / 2),
                buttonScale,
                SpriteEffects.None,
                0f
            );

            // Draw Exit button
            spriteBatch.Draw(
                exitButtonTexture,
                exitButtonPosition,
                null,
                Color.White,
                0f,
                new Vector2(exitButtonTexture.Width / 2, exitButtonTexture.Height / 2),
                exitButtonScale,
                SpriteEffects.None,
                0f
            );
        }
    }
}

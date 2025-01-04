using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyGame.GameComponents;

namespace MyGame.GameScreens
{
    public class GameOverScreen
    {
        private Button newGameButton;
        private Button exitButton;
        private Texture2D gameOverTexture;
        private Vector2 gameOverPosition;
        public GameOverScreen(ContentManager content)
        {
            gameOverTexture = content.Load<Texture2D>("gameover");
            newGameButton = new Button(content.Load<Texture2D>("restart"), new Vector2(650, 500), 0.8f);
            exitButton = new Button(content.Load<Texture2D>("exit"), new Vector2(1350, 500), 0.8f);

        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            int screenWidth = graphicsDevice.Viewport.Width;
            int screenHeight = graphicsDevice.Viewport.Height;

            gameOverPosition = new Vector2(
                (screenWidth - gameOverTexture.Width) / 2,
                (screenHeight / 2) - gameOverTexture.Height - 50
            );
        }

        public void Update(MouseState mouseState, out bool startNewGame, out bool exitGame)
        {
            startNewGame = false;
            exitGame = false;

            newGameButton.Update();
            exitButton.Update();

            // Klik-detectie
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if (newGameButton.GetBounds().Contains(mouseState.Position))
                {
                    startNewGame = true;
                }
                else if (exitButton.GetBounds().Contains(mouseState.Position))
                {
                    exitGame = true;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(gameOverTexture, gameOverPosition, Color.White);
            newGameButton.Draw(spriteBatch);
            exitButton.Draw(spriteBatch);
        }
    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyGame.GameComponents;

namespace MyGame.GameScreens
{
    public class StartScreen
    {
        private Button startButton;
        private Button exitButton;

        public bool IsGameStarted { get; private set; } = false;

        public StartScreen(ContentManager content)
        {
            startButton = new Button(content.Load<Texture2D>("start"), new Vector2(875, 400));
            exitButton = new Button(content.Load<Texture2D>("exit"), new Vector2(875, 600), 0.4f, true, 0.4f, 0.5f, 0.005f);


        }

        public void Update(MouseState mouseState, Game game)
        {
            startButton.Update();
            exitButton.Update();

            // Klik-detectie
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if (startButton.GetBounds().Contains(mouseState.Position))
                {
                    IsGameStarted = true;
                }
                else if (exitButton.GetBounds().Contains(mouseState.Position))
                {
                    game.Exit();
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            startButton.Draw(spriteBatch);
            exitButton.Draw(spriteBatch);
        }
    }
}

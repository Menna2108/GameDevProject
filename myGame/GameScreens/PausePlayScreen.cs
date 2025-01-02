using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyGame.GameComponents;

namespace MyGame.GameScreens
{
    public class PausePlayScreen
    {
        private Button pauseButton;
        private Button playButton;
        private bool isPaused = false;

        public PausePlayScreen(ContentManager content)
        {
            pauseButton = new Button(content.Load<Texture2D>("pause"), new Vector2(1825, 60), 0.3f, false);
            playButton = new Button(content.Load<Texture2D>("play"), new Vector2(1825, 56), 0.28f, false);

        }

        public void Update(MouseState mouseState, ref bool isGamePaused)
        {
            pauseButton.Update();
            playButton.Update();

            // Klik-detectie
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if (pauseButton.GetBounds().Contains(mouseState.Position) || playButton.GetBounds().Contains(mouseState.Position))
                {
                    isPaused = !isPaused;
                    isGamePaused = isPaused;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (isPaused)
                playButton.Draw(spriteBatch);
            else
                pauseButton.Draw(spriteBatch);
        }
    }
}

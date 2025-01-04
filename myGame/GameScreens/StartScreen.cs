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
        private SpriteFont font;

        public bool IsGameStarted { get; private set; } = false;

        public StartScreen(ContentManager content)
        {
            startButton = new Button(content.Load<Texture2D>("start"), new Vector2(920, 600));
            exitButton = new Button(content.Load<Texture2D>("exit"), new Vector2(920, 750), 0.4f, true, 0.4f, 0.5f, 0.005f);
            font = content.Load<SpriteFont>("Fonts/coinFont");
        }

        public void Update(MouseState mouseState, Game game)
        {
            startButton.Update();
            exitButton.Update();

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
            // Draw welcome text
            string welcomeText = "Welcome to Survive the Space!";
            string controlsText = "Use Arrow Keys to move and Space to shoot";
            string surviveText = "Let's survive this!";
            string bossWarningText = "Warning: Touching the boss will instantly defeat you due to his powerful aura!";

            Vector2 welcomePosition = new Vector2(700, 50);
            Vector2 controlsPosition = new Vector2(650, 200);
            Vector2 survivePosition = new Vector2(800, 300);
            Vector2 bossWarningPosition = new Vector2(400, 400);

            spriteBatch.DrawString(font, welcomeText, welcomePosition, Color.Yellow);
            spriteBatch.DrawString(font, controlsText, controlsPosition, Color.White);
            spriteBatch.DrawString(font, surviveText, survivePosition, Color.Green);
            spriteBatch.DrawString(font, bossWarningText, bossWarningPosition, Color.Red);

            startButton.Draw(spriteBatch);
            exitButton.Draw(spriteBatch);
        }
    }
}
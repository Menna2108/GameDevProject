using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace myGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D backgroundTexture;
        private Texture2D rocketSprite;
        private Texture2D startButtonTexture;
        private Texture2D heartTexture; // Hartjes Texture

        private Vector2 rocketPosition;
        private Vector2 startButtonPosition; // Positie van de knop
        private Rectangle startButtonBounds; // Bounds voor detectie van klikken

        private float rocketSpeed = 4f;
        private float rocketScale = 0.16f;
        private float backgroundSpeed = 2f;
        private float backgroundPositionY = 0;

        private bool isGameStarted = false;

        private float buttonOscillation = 0f; 
        private float buttonOscillationSpeed = 1f; 

        private float heartScale = 0.1f; 

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            rocketPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight - 100);
            startButtonPosition = new Vector2((_graphics.PreferredBackBufferWidth - 300) / 2, (_graphics.PreferredBackBufferHeight - 150) / 2); // Centraal gepositioneerd
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            backgroundTexture = Content.Load<Texture2D>("space");
            rocketSprite = Content.Load<Texture2D>("RocketSprite");
            startButtonTexture = Content.Load<Texture2D>("start_button");
            heartTexture = Content.Load<Texture2D>("hartje"); 

            // Definieer de knop-bounds
            startButtonBounds = new Rectangle((int)startButtonPosition.X, (int)startButtonPosition.Y, 300, 150); // Breder én hoger
        }

        protected override void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();

            if (!isGameStarted)
            {
                // Start Button Beweging
                buttonOscillation += buttonOscillationSpeed;
                startButtonPosition.Y += (float)(Math.Sin(buttonOscillation * 0.05) * 2);

                // Update bounds voor knop
                startButtonBounds = new Rectangle((int)startButtonPosition.X, (int)startButtonPosition.Y, 300, 150); // Update grootte
                // Controleer of er op de knop is geklikt
                if (mouseState.LeftButton == ButtonState.Pressed && startButtonBounds.Contains(mouseState.Position))
                {
                    isGameStarted = true;  
                }
                return; 
            }

            var keyboardState = Keyboard.GetState();

            // Beweging van mijn raket
            if (keyboardState.IsKeyDown(Keys.Up) && rocketPosition.Y > 0)
            {
                rocketPosition.Y -= rocketSpeed;
                backgroundPositionY += backgroundSpeed;
            }

            // Collision detection voor omlaag, links en rechts
            if (keyboardState.IsKeyDown(Keys.Down) && rocketPosition.Y + rocketSprite.Height * rocketScale < _graphics.PreferredBackBufferHeight)
            {
                rocketPosition.Y += rocketSpeed;
            }

            if (keyboardState.IsKeyDown(Keys.Left) && rocketPosition.X > 0)
            {
                rocketPosition.X -= rocketSpeed;
            }

            if (keyboardState.IsKeyDown(Keys.Right) && rocketPosition.X + rocketSprite.Width * rocketScale < _graphics.PreferredBackBufferWidth)
            {
                rocketPosition.X += rocketSpeed;
            }

            if (backgroundPositionY >= _graphics.PreferredBackBufferHeight)
            {
                backgroundPositionY = 0;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            // Achtergrond tekenen
            _spriteBatch.Draw(backgroundTexture,
                new Rectangle(0, (int)backgroundPositionY - _graphics.PreferredBackBufferHeight,
                _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), Color.White);
            _spriteBatch.Draw(backgroundTexture,
                new Rectangle(0, (int)backgroundPositionY,
                _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), Color.White);

            if (!isGameStarted)
            {
                // Teken de start button
                _spriteBatch.Draw(startButtonTexture, startButtonBounds, Color.White);
            }
            else
            {
                // Teken de raket
                _spriteBatch.Draw(rocketSprite, rocketPosition, null, Color.White, 0f, Vector2.Zero, rocketScale, SpriteEffects.None, 0f);

                // Teken hartjes
                for (int i = 0; i < 3; i++)
                {
                    _spriteBatch.Draw(
                        heartTexture,
                        new Vector2(10 + (i * (heartTexture.Width * heartScale + 5)), 10),
                        null,
                        Color.White,
                        0f,
                        Vector2.Zero,
                        heartScale,
                        SpriteEffects.None,
                        0f
                    );
                }
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace myGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D hartje; // De afbeelding van het hartje (hartje.png)
        private List<Vector2> heartPositions; // Lijst voor de posities van de hartjes

        private int numHearts = 3; // Aantal levens/hartjes

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Initialiseer de lijst met hartjes
            heartPositions = new List<Vector2>();

            // Stel de startpositie in voor het eerste hartje
            int startX = 20; // Begin X-positie van de hartjes
            int startY = 20; // Y-positie van de hartjes

            // Voeg de posities van de hartjes toe aan de lijst
            for (int i = 0; i < numHearts; i++)
            {
                heartPositions.Add(new Vector2(startX + i * 50, startY));
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Laad de afbeelding "hartje.png" vanuit de Content-map
            hartje = Content.Load<Texture2D>("hartje");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Hier kun je logica toevoegen om een leven te verliezen of toe te voegen

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            // Teken elk hartje op de corresponderende positie in de lijst
            foreach (var position in heartPositions)
            {
                _spriteBatch.Draw(hartje, position, Color.White);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace myGame.GameEntities
{
    public class MeteorEnemy : EnemyBase
    {
        public MeteorEnemy(Texture2D texture, Vector2 startPosition, float speed, float scale)
            : base(texture, startPosition, speed, scale)
        {
        }

        public override void Update(GameTime gameTime)
        {
            // Beweeg simpelweg naar beneden
            position.Y += speed;

            // Deactiveer als het buiten het scherm is
            if (position.Y > 850)
            {
                IsActive = false;
            }
        }
    }
}

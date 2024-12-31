using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myGame.GameEntities
{
    internal class Meteoor : EnemyBase
    {
        public Meteoor(Texture2D texture, Vector2 startPosition, float speed, float scale)
            : base(texture, startPosition, speed, scale)
        {
        }

        public override void Update(GameTime gameTime)
        {
            position.Y += speed;

            if (position.Y > 950)
            {
                IsActive = false;
            }
        }
    }
}

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myGame.GameEntities
{
    internal class Boss : EnemyBase
    {
        private int health = 3;

        public Boss(Texture2D texture, Vector2 startPosition, float speed, float scale)
            : base(texture, startPosition, speed, scale)
        {
        }

        public override void Update(GameTime gameTime)
        {
            // blijf
        }

        public bool TakeDamage()
        {
            health--;
            if (health <= 0)
            {
                IsActive = false;
                return true; 
            }
            return false;
        }
    }
}

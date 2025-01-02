using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myGame.GameManagers
{
    public static class TextureManager
    {
        private static Texture2D pixelTexture;

        public static void Initialize(GraphicsDevice graphicsDevice)
        {
            pixelTexture = new Texture2D(graphicsDevice, 1, 1);
            pixelTexture.SetData(new[] { Color.White });
        }

        public static Texture2D GetPixel()
        {
            return pixelTexture;
        }
    }
}

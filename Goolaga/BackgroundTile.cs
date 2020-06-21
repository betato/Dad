using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goolaga
{
    class BackgroundTile
    {
        public BackgroundTile(TextureId textureID, Vector2 pos)
        {
            texture = TextureManager.GetTexture(textureID);
            Pos = pos;
            Size = texture.Bounds.Size.ToVector2();
        }

        public Vector2 Pos { get; private set; }
        public Vector2 Size { get; private set; }
        private Texture2D texture;

        public void Draw(SpriteBatch spriteBatch, Vector2 cameraLocation)
        {
            Rectangle destRect = new Rectangle(
                (int)((Pos.X - cameraLocation.X) * TextureManager.scale),
                (int)((Pos.Y - cameraLocation.Y) * TextureManager.scale),
                texture.Width * (int)TextureManager.scale,
                texture.Height * (int)TextureManager.scale);

            spriteBatch.Draw(texture, destRect, Color.White);
        }
    }
}

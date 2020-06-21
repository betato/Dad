using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goolaga
{
    abstract class Entity
    {
        public Vector2 Pos
        {
            get { return pos; }
        }

        public Vector2 Size
        {
            get { return size; }
        }

        protected Vector2 pos;
        protected Vector2 size;
        private Texture2D texture;
        private bool[,] transparency;
        private bool transparencyLoaded;

        protected void SetTexture(TextureId textureID)
        {
            texture = TextureManager.GetTexture(textureID);
            transparencyLoaded = TextureManager.HasTransparency(textureID);
            if (transparencyLoaded)
            {
                transparency = TextureManager.GetTransparency(textureID);
                size = texture.Bounds.Size.ToVector2();
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 cameraLocation)
        {
            Rectangle destRect = new Rectangle(
                (int)((pos.X - cameraLocation.X) * TextureManager.scale),
                (int)((pos.Y - cameraLocation.Y) * TextureManager.scale),
                texture.Width * (int)TextureManager.scale, 
                texture.Height * (int)TextureManager.scale);

            spriteBatch.Draw(texture, destRect, Color.White);
        }

        public Vector2 Center()
        {
            return new Vector2(pos.X + size.X / 2, pos.Y + size.Y / 2);
        }

        public bool InBounds(Vector2 pos, Vector2 size)
        {
            return this.pos.X < pos.X + size.X &&
                this.pos.X + this.size.X > pos.X &&
                this.pos.Y < pos.X + size.Y &&
                this.pos.Y + this.size.Y > pos.Y;
        }

        public bool Colliding(Entity other)
        {
            if (!transparencyLoaded)
                return false; // No transparency loaded - default no collision

            int xLeft = (int)Math.Max(this.pos.X, other.pos.X);
            int xRight = (int)Math.Min(this.pos.X + this.size.X, other.pos.X + other.size.X);
            int yTop = (int)Math.Max(this.pos.Y, other.pos.Y);
            int yBottom = (int)Math.Min(this.pos.Y + this.size.Y, other.pos.Y + other.size.Y);

            if (xLeft > xRight || yTop > yBottom)
                 return false; // Bounds do not overlap

            int thisX = (int)this.Pos.X;
            int thisY = (int)this.Pos.Y;
            int otherX = (int)other.Pos.X;
            int otherY = (int)other.Pos.Y;

            // Loop through pixels in intersecting region
            for (int y = yTop; y < yBottom; y++)
            {
                for (int x = xLeft; x < xRight; x++)
                {
                    if(!this.transparency[x - thisX, y - thisY] && !other.transparency[x - otherX, y - otherY])
                    {
                        return true;
                    }
                }
            }
            return false; // No pixel collision
        }
    }
}

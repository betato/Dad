using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goolaga
{
    enum TextureId
    {
        // Debug square
        Test,

        // Bullets
        BulletPlayer,
        BulletEnemy,

        // Backgrounds
        Background1,

        // Player
        Player,
        PlayerEx1,
        PlayerEx2,
        PlayerEx3,
        PlayerEx4,
        PlayerEx5,
        PlayerEx6,

        // Uhmno
        Uhmno,
        UhmnoEx1,
        UhmnoEx2,
        UhmnoEx3,
        UhmnoEx4,
        UhmnoEx5,

        // Bwumb
        Bwumb,
        BwumbEx1,
        BwumbEx2,
        BwumbEx3,
        BwumbEx4,
        BwumbEx5,
    }

    static class TextureManager
    {
        public static readonly float scale = 2.0f;
        public static ContentManager ContentManager { get; set; }
        private static Dictionary<TextureId, Texture2D> textures = new Dictionary<TextureId, Texture2D>();
        private static Dictionary<TextureId, bool[,]> transparencies = new Dictionary<TextureId, bool[,]>();

        public static void Initialize(ContentManager contentManager)
        {
            ContentManager = contentManager;
            LoadTextures();
        }

        private static void LoadTextures()
        {
            LoadTexture(TextureId.Test, @"Test", true);
            LoadTexture(TextureId.BulletPlayer, @"PlayerBullet", true);
            LoadTexture(TextureId.BulletEnemy, @"EnemyBullet", true);

            // Backgrounds
            LoadTexture(TextureId.Background1, @"background\Bgtest3", false);

            // Player
            LoadTexture(TextureId.Player, @"player\Player", true);
            LoadTexture(TextureId.PlayerEx1, @"player\PlayerExplosion1", false);
            LoadTexture(TextureId.PlayerEx2, @"player\PlayerExplosion2", false);
            LoadTexture(TextureId.PlayerEx3, @"player\PlayerExplosion3", false);
            LoadTexture(TextureId.PlayerEx4, @"player\PlayerExplosion4", false);
            LoadTexture(TextureId.PlayerEx5, @"player\PlayerExplosion5", false);
            LoadTexture(TextureId.PlayerEx6, @"player\PlayerExplosion6", false);

            // Uhmno
            LoadTexture(TextureId.Uhmno, @"uhmno\Uhmno", true);
            LoadTexture(TextureId.UhmnoEx1, @"uhmno\UhmnoExplosion1", false);
            LoadTexture(TextureId.UhmnoEx2, @"uhmno\UhmnoExplosion2", false);
            LoadTexture(TextureId.UhmnoEx3, @"uhmno\UhmnoExplosion3", false);
            LoadTexture(TextureId.UhmnoEx4, @"uhmno\UhmnoExplosion4", false);
            LoadTexture(TextureId.UhmnoEx5, @"uhmno\UhmnoExplosion5", false);

            // Bwumb
            LoadTexture(TextureId.Bwumb,    @"bwumb\Bwumb", true);
            LoadTexture(TextureId.BwumbEx1, @"bwumb\BwumbExplosion1", false);
            LoadTexture(TextureId.BwumbEx2, @"bwumb\BwumbExplosion2", false);
            LoadTexture(TextureId.BwumbEx3, @"bwumb\BwumbExplosion3", false);
            LoadTexture(TextureId.BwumbEx4, @"bwumb\BwumbExplosion4", false);
            LoadTexture(TextureId.BwumbEx5, @"bwumb\BwumbExplosion5", false);
        }

        private static void LoadTexture(TextureId id, string resourceName, bool genTransparency)
        {
            // Load texture
            Texture2D texture = ContentManager.Load<Texture2D>(resourceName);
            textures.Add(id, texture);

            // Create transparency array
            if (genTransparency)
            {
                Color[] pixels = new Color[texture.Width * texture.Height];
                texture.GetData(pixels);
                bool[,] transparency = new bool[texture.Width, texture.Height];
                for (int y = 0; y < texture.Height; y++)
                {
                    for (int x = 0; x < texture.Width; x++)
                    {
                        transparency[x, y] = pixels[y * texture.Width + x].A == 0;
                    }
                }
                transparencies.Add(id, transparency);
            }
        }

        public static Texture2D GetTexture(TextureId id)
        {
            return textures[id];
        }

        public static bool HasTransparency(TextureId id)
        {
            return transparencies.ContainsKey(id);
        }

        public static bool[,] GetTransparency(TextureId id)
        {
            return transparencies[id];
        }
    }
}

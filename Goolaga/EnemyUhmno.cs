using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goolaga
{
    class EnemyUhmno : Enemy
    {
        public EnemyUhmno(Vector2 pos) : base(192, 2048, 512, 4)
        {
            SetTexture(TextureId.Uhmno);
            explosionTextures = new TextureId[5];
            explosionTextures[0] = TextureId.UhmnoEx1;
            explosionTextures[1] = TextureId.UhmnoEx2;
            explosionTextures[2] = TextureId.UhmnoEx3;
            explosionTextures[3] = TextureId.UhmnoEx4;
            explosionTextures[4] = TextureId.UhmnoEx5;
            this.pos = pos;
        }

        protected override void Update(float timeDelta, float cameraX, float cameraSpeed, Player player,
            ref List<Bullet> playerBullets, ref List<Bullet> enemyBullets, ref List<Enemy> enemies)
        {
            base.Update(timeDelta, cameraX, cameraSpeed, player,
                ref playerBullets, ref enemyBullets, ref enemies);

            if (player.Dead)
            {
                // Continue to end of screen
                Move(timeDelta, new Vector2(-1, 0));
                return;
            }

            Vector2 accel;
            accel = player.Center() - Center();
            Move(timeDelta, accel);
        }
    }
}

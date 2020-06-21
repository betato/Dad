using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goolaga
{
    class EnemyBwumb : Enemy
    {
        private float fireTimer = 0;
        private const float fireInterval = 0.7f;

        private readonly Vector2 gun1pos = new Vector2(5, 13);
        private readonly Vector2 gun2pos = new Vector2(5, 48);

        public EnemyBwumb(Vector2 pos) : base(96, 1024, 256, 6)
        {
            SetTexture(TextureId.Bwumb);
            explosionTextures = new TextureId[5];
            explosionTextures[0] = TextureId.BwumbEx1;
            explosionTextures[1] = TextureId.BwumbEx2;
            explosionTextures[2] = TextureId.BwumbEx3;
            explosionTextures[3] = TextureId.BwumbEx4;
            explosionTextures[4] = TextureId.BwumbEx5;
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

            // Move vertically in line with player
            Vector2 accel = new Vector2(0, player.Center().Y - Center().Y);
            accel.Normalize();

            if (Pos.X - player.Pos.X > 600)
            {
                // Move towards
                accel.X = -1;
            }
            else if (Pos.X - player.Pos.X < 300)
            {
                // Move away
                accel.X = 1;
            }
            Move(timeDelta, accel);

            foreach (Bullet bullet in playerBullets)
            {
                
            }
            
            fireTimer += timeDelta;
            if (fireTimer >= fireInterval && Math.Abs(player.Center().Y - Center().Y) < 100)
            {
                enemyBullets.Add(new Bullet(pos + gun1pos, new Vector2(), new Vector2(-1, 0), false));
                enemyBullets.Add(new Bullet(pos + gun2pos, new Vector2(), new Vector2(-1, 0), false));
                fireTimer = 0;
            }
        }
    }
}

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goolaga
{
    class Bullet : Entity
    {
        private const float velScalar = 512;
        private Vector2 velocity;

        public Bullet(Vector2 pos, Vector2 startVelocity, Vector2 direction, bool playerBullet)
        {
            if (playerBullet)
                SetTexture(TextureId.BulletPlayer);
            else
                SetTexture(TextureId.BulletEnemy);

            this.pos = pos;
            direction.Normalize();
            velocity = startVelocity + velScalar * direction;
        }

        public void Update(float timeDelta)
        {
            pos += velocity * timeDelta;
        }
    }
}

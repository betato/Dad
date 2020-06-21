using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goolaga
{
    abstract class Ship : Entity
    {
        // Movement
        protected Vector2 vel = new Vector2(0, 0);
        public float MaxVel { get; private set; } = 256;
        public float MovementAccelScalar { get; private set; } = 2048;
        public float FrictionAccelScalar { get; private set; } = 512;

        // Explosion
        public bool Dead { get; set; }
        protected TextureId[] explosionTextures;
        public float ExplosionLength { get; protected set; } = 0.4f;
        public float ExplosionTime { get; private set; } = 0;

        public Ship(float maxVelocity, float acceleration, float frictionAcceleration, float explosionLength)
        {
            MaxVel = maxVelocity;
            MovementAccelScalar = acceleration;
            FrictionAccelScalar = frictionAcceleration;
            this.ExplosionLength = explosionLength;
        }

        protected abstract void Update(float timeDelta, float cameraX, float cameraSpeed, Player player,
            ref List<Bullet> playerBullets, ref List<Bullet> enemyBullets, ref List<Enemy> enemies);

        public void BaseUpdate(float timeDelta, float cameraX, float cameraSpeed, Player player, 
            ref List<Bullet> playerBullets, ref List<Bullet> enemyBullets, ref List<Enemy> enemies)
        {
            if (Dead)
            {
                ExplosionTime += timeDelta;
                SetTexture(explosionTextures[
                    Math.Min((int)(ExplosionTime * explosionTextures.Length / ExplosionLength), explosionTextures.Length - 1)]);
                // Allow ship to drift to a stop
                Move(timeDelta, new Vector2(0, 0));
            }
            else
            {
                Update(timeDelta, cameraX, cameraSpeed, player, ref playerBullets, ref enemyBullets, ref enemies);
            }
        }

        // Accel is the acceleration direction
        protected void Move(float timeDelta, Vector2 accel)
        {
            // Clamp acceleration
            if (accel.Length() > 0.0f)
            {
                accel.Normalize();
                accel *= MovementAccelScalar;
            }

            // Apply frictional acceleration
            if (vel.Length() > 0.0f)
            {
                if (vel.Length() < FrictionAccelScalar * timeDelta)
                {
                    // Friction will stop the player
                    vel = new Vector2(0, 0);
                }
                else
                {
                    // Friction deceleration
                    Vector2 fricAccel = new Vector2(vel.X, vel.Y);
                    fricAccel.Normalize();
                    accel -= fricAccel * FrictionAccelScalar;
                }
            }

            // Update and clamp velocity
            vel += accel * timeDelta;
            if (vel.Length() > MaxVel)
            {
                vel.Normalize();
                vel *= MaxVel;
            }

            // Update position
            pos += vel * timeDelta;
        }
    }
}

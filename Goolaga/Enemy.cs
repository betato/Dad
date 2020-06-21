using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goolaga
{
    abstract class Enemy : Ship
    {
        public int ScoreValue { get; private set; }

        public Enemy(float maxVelocity, float acceleration, float frictionAcceleration, int scoreValue) : 
            base(maxVelocity, acceleration, frictionAcceleration, 0.4f) { this.ScoreValue = scoreValue; }

        protected override void Update(float timeDelta, float cameraX, float cameraSpeed, Player player,
            ref List<Bullet> playerBullets, ref List<Bullet> enemyBullets, ref List<Enemy> enemies)
        {            
            foreach (Bullet bullet in playerBullets)
            {
                if (Colliding(bullet))
                {
                    Dead = true;
                    playerBullets.Remove(bullet);
                    return;
                }
            }
        }
    }
}

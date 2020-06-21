using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goolaga
{
    class Player : Ship
    {
        private Vector2 bounds;
        KeyboardState currentKeyboard, previousKeyboard;
        private Vector2 centeredPos;

        private readonly Vector2 gun1pos = new Vector2(44, 12);
        private readonly Vector2 gun2pos = new Vector2(44, 49);

        public Player(Vector2 centeredPos, Vector2 bounds, float cameraX) : base(256, 2048, 512, 1.0f)
        {
            SetTexture(TextureId.Player);
            explosionTextures = new TextureId[6];
            explosionTextures[0] = TextureId.PlayerEx1;
            explosionTextures[1] = TextureId.PlayerEx2;
            explosionTextures[2] = TextureId.PlayerEx3;
            explosionTextures[3] = TextureId.PlayerEx4;
            explosionTextures[4] = TextureId.PlayerEx5;
            explosionTextures[5] = TextureId.PlayerEx6;

            this.centeredPos = centeredPos - size / 2;
            StayOnScreen(cameraX);

            this.bounds = bounds;
            currentKeyboard = Keyboard.GetState();
        }

        public void StayOnScreen(float cameraX)
        {
            pos.Y = centeredPos.Y - size.Y / 2;
            pos.X = cameraX + centeredPos.X - size.X / 2;
        }

        private void UpdateMovement(float timeDelta, float cameraX, float cameraSpeed)
        {
            Vector2 accel = new Vector2(0, 0);
            
            if (currentKeyboard.IsKeyDown(Keys.D))
                accel.X = 1;
            if (currentKeyboard.IsKeyDown(Keys.A))
                accel.X -= 1;
            if (currentKeyboard.IsKeyDown(Keys.S))
                accel.Y = 1;
            if (currentKeyboard.IsKeyDown(Keys.W))
                accel.Y -= 1;

            Move(timeDelta, accel);

            // Check bounds
            if (pos.X < cameraX)
            {
                pos.X = cameraX;
                // Set speed to zero only if the player is driving into the wall
                // This prevents the ship from getting stuck
                if (vel.X < 0)
                    vel.X = 0;
            }
            else if (pos.X + size.X > bounds.X + cameraX)
            {
                pos.X = bounds.X + cameraX - size.X;
                vel.X = cameraSpeed;
            }
            if (pos.Y < 0)
            {
                pos.Y = 0;
                vel.Y = 0;
            }
            else if (pos.Y + size.Y > bounds.Y)
            {
                pos.Y = bounds.Y - size.Y;
                vel.Y = 0;
            }
        }

        private void UpdateShooting(ref List<Bullet> playerBullets)
        {
            if (currentKeyboard.IsKeyDown(Keys.Space) && !previousKeyboard.IsKeyDown(Keys.Space))
            {
                playerBullets.Add(new Bullet(pos + gun1pos, new Vector2(), new Vector2(1, 0), true));
                playerBullets.Add(new Bullet(pos + gun2pos, new Vector2(), new Vector2(1, 0), true));
            }
        }

        private void UpdateDeath(ref List<Bullet> enemyBullets, ref List<Enemy> enemies)
        {
            foreach (Bullet bullet in enemyBullets)
            {
                if (Colliding(bullet))
                {
                    Dead = true;
                    enemyBullets.Remove(bullet);
                    return;
                }
            }
            foreach (Enemy enemy in enemies)
            {
                if (Colliding(enemy) && enemy.Dead == false)
                {
                    enemy.Dead = true;
                    Dead = true;
                    return;
                }
            }
        }

        protected override void Update(float timeDelta, float cameraX, float cameraSpeed, Player player, 
            ref List<Bullet> playerBullets, ref List<Bullet> enemyBullets, ref List<Enemy> enemies)
        {
            // Get input
            previousKeyboard = currentKeyboard;
            currentKeyboard = Keyboard.GetState();

            UpdateMovement(timeDelta, cameraX, cameraSpeed);
            UpdateShooting(ref playerBullets);
            UpdateDeath(ref enemyBullets, ref enemies);
        }
    }
}

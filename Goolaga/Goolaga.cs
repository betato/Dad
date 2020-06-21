using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Goolaga
{
    public class Goolaga : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private const float cameraSpeed = 64;
        private Vector2 cameraLocation;
        private Vector2 gameSize;

        private List<BackgroundTile> backgroundScroll = new List<BackgroundTile>();
        private List<Enemy> enemies = new List<Enemy>();
        private List<Bullet> playerBullets = new List<Bullet>();
        private List<Bullet> enemyBullets = new List<Bullet>();
        private Player player;

        private int score = 0;
        private SpriteFont labelFont;
        private SpriteFont titleFont;
        float gameTimer = 0;
        readonly Random rng = new Random();

        enum GameState
        {
            NewGame,
            Playing,
            GameOver
        }
        GameState gameState = GameState.NewGame;

        public Goolaga()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        
        protected override void Initialize()
        {
            base.Initialize();
            cameraLocation = new Vector2(0, 0);

            int displayWidth = GraphicsDevice.DisplayMode.Width;
            int displayHeight = GraphicsDevice.DisplayMode.Height;

            graphics.PreferredBackBufferWidth = displayWidth;
            graphics.PreferredBackBufferHeight = displayHeight;
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();
            gameSize = new Vector2(displayWidth / TextureManager.scale, displayHeight / TextureManager.scale);
            
            // Load first tiles
            for (int i = 0; i < 4; i++)
            {
                Texture2D tex = TextureManager.GetTexture(TextureId.Background1);
                Vector2 location = new Vector2(tex.Width * i, 0);
                backgroundScroll.Add(new BackgroundTile(TextureId.Background1, location));
            }

            player = new Player(
                new Vector2(gameSize.X / 4, gameSize.Y / 2),
                new Vector2(gameSize.X, gameSize.Y),
                cameraLocation.X);
        }
        
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            TextureManager.Initialize(Content);
            labelFont = Content.Load<SpriteFont>(@"font\LabelFont");
            titleFont = Content.Load<SpriteFont>(@"font\TitleFont");
        }
        
        protected override void UnloadContent()
        {
            
        }

        private void StartGame()
        {
            score = 0;

            player = new Player(
                new Vector2(gameSize.X / 4, gameSize.Y / 2),
                new Vector2(gameSize.X, gameSize.Y),
                cameraLocation.X);

            playerBullets.Clear();
            enemyBullets.Clear();
            enemies.Clear();

            gameState = GameState.Playing;
        }

        private void UpdatePlayer(float timeDelta)
        {
            if (gameState == GameState.Playing)
            {
                if (player.ExplosionTime < player.ExplosionLength)
                {
                    player.BaseUpdate(timeDelta, cameraLocation.X, cameraSpeed, player, ref playerBullets, ref enemyBullets, ref enemies);
                }
                else if (playerBullets.Count == 0)
                {
                    foreach (Enemy enemy in enemies)
                    {
                        if (enemy.Dead)
                            return; // Wait for enemies to finish exploding
                    }
                    gameState = GameState.GameOver;
                }
            }
            else
            {
                player.StayOnScreen(cameraLocation.X);
            }
        }

        private void AddEnemies(float timeDelta)
        {
            int rngProb = Math.Max((int)(1.0f / timeDelta), 1);
            int scoreFactor = 1 + score / 100;
            rngProb /= scoreFactor;
            if (rng.Next(2 * rngProb) == 0)
                enemies.Add(new EnemyUhmno(new Vector2(gameSize.X + cameraLocation.X, rng.Next((int)gameSize.Y))));
            if (rng.Next(7 * rngProb) == 0)
                enemies.Add(new EnemyBwumb(new Vector2(gameSize.X + cameraLocation.X, rng.Next((int)gameSize.Y))));
        }

        private void UpdateBullets(float timeDelta)
        {
            for (int i = playerBullets.Count - 1; i >= 0; i--)
            {
                if (playerBullets[i].InBounds(cameraLocation, gameSize))
                    playerBullets[i].Update(timeDelta);
                else
                    playerBullets.RemoveAt(i);
            }
            for (int i = enemyBullets.Count - 1; i >= 0; i--)
            {
                if (enemyBullets[i].InBounds(cameraLocation, gameSize))
                    enemyBullets[i].Update(timeDelta);
                else
                    enemyBullets.RemoveAt(i);
            }
        }

        private void UpdateBackground(float timeDelta)
        {
            // TODO: Add more tileable background textures
            BackgroundTile firstTile = backgroundScroll[0];
            if (firstTile.Pos.X + firstTile.Size.X < cameraLocation.X)
            {
                backgroundScroll.RemoveAt(0);
                BackgroundTile lastTile = backgroundScroll[backgroundScroll.Count - 1];
                Vector2 location = new Vector2(lastTile.Pos.X + lastTile.Size.X, 0);
                backgroundScroll.Add(new BackgroundTile(TextureId.Background1, location));
            }
        }

        private void UpdateEnemies(float timeDelta)
        {
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                enemies[i].BaseUpdate(timeDelta, cameraLocation.X, cameraSpeed, player, ref playerBullets, ref enemyBullets, ref enemies);
                if (enemies[i].ExplosionTime >= enemies[i].ExplosionLength)
                {
                    // Enemy has finished exploding. Remove and increase score
                    score += enemies[i].ScoreValue;
                    enemies.RemoveAt(i);
                    continue;
                }
                // Delete out-of-bounds enemies
                if (enemies[i].Pos.X + enemies[i].Size.X < cameraLocation.X)
                {
                    enemies.RemoveAt(i);
                }
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && gameState != GameState.Playing)
                StartGame();
            float timeDelta = (float)(gameTime.ElapsedGameTime.TotalSeconds);

            // Update background scroll
            UpdateBackground(timeDelta);

            // Increase score every second
            if (gameState == GameState.Playing)
            {
                gameTimer += timeDelta;
                if (gameTimer >= 1)
                {
                    gameTimer %= 1;
                    score++;
                }
            }

            // Update camera
            cameraLocation.X += cameraSpeed * timeDelta;

            // Update player
            UpdatePlayer(timeDelta);

            // Update bullets
            UpdateBullets(timeDelta);

            // Add enemies
            if (gameState == GameState.Playing)
                AddEnemies(timeDelta);

            // Update enimies
            UpdateEnemies(timeDelta);

            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);

            // Background
            foreach (BackgroundTile tile in backgroundScroll)
            {
                tile.Draw(spriteBatch, cameraLocation);
            }

            // Bullets
            foreach (Bullet bullet in playerBullets)
            {
                bullet.Draw(spriteBatch, cameraLocation);
            }
            foreach (Bullet bullet in enemyBullets)
            {
                bullet.Draw(spriteBatch, cameraLocation);
            }

            // Player
            if (player.ExplosionTime < player.ExplosionLength)
                player.Draw(spriteBatch, cameraLocation);

            // Enemies
            foreach (Enemy enemy in enemies)
            {
                enemy.Draw(spriteBatch, cameraLocation);
            }

            // Text
            if (gameState == GameState.GameOver)
            {
                string gameOverString = "  GAME OVER";
                string scoreString = string.Format("You scored {0} points", score);

                Vector2 gameOverSize = titleFont.MeasureString(gameOverString);
                Vector2 scoreSize = labelFont.MeasureString(gameOverString);

                spriteBatch.DrawString(titleFont, gameOverString, (gameSize * TextureManager.scale - gameOverSize) / 2, Color.White);
                spriteBatch.DrawString(labelFont, scoreString, (gameSize * TextureManager.scale - scoreSize) / 2 + new Vector2(0, 60), Color.White);
            }
            else if (gameState == GameState.Playing)
            {
                spriteBatch.DrawString(labelFont, string.Format("Score: {0}", score), new Vector2(15, 10), Color.White);
            }
            else if (gameState == GameState.NewGame)
            {
                string str = "PRESS SPACE\n    TO START";
                Vector2 textSize = titleFont.MeasureString(str);
                spriteBatch.DrawString(titleFont, str, (gameSize * TextureManager.scale - textSize) / 2, Color.White);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}

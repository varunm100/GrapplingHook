using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SimplePlatformer
{
    class Player
    {
        public Texture2D playerTexture;
        public Vector2 position;
        public Vector2 scale;
        public Vector2 velocity;
        public Rectangle collider;

        public Player(Texture2D _playerTexture, Vector2 _position, Vector2 _scale, Vector2 _velocity)
        {
            this.playerTexture = _playerTexture;
            this.position = _position;
            this.scale = _scale;
            this.velocity = _velocity;
        }

        public void updateCollider()
        {
            collider = new Rectangle((int)position.X, (int)position.Y, (int)playerTexture.Width*(int)scale.X, (int)playerTexture.Height*(int)scale.Y);
        }

        public void updatePosition(GameTime gameTime)
        {
            position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public void checkJump(float jumpHeight, Keys jumpKey)
        {
            if (Keyboard.GetState().IsKeyDown(jumpKey))
            {
                position.Y += jumpHeight;
            }
        }
    }
}

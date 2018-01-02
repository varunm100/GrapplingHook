using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace SimplePlatformer
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        World mainWorld;
        Body mainBody;
        DrawablePhysicsObject floor;
        DrawablePhysicsObject platform1;
        const float unitToPixel = 100.0f;
        const float pixelToUnit = 1 / unitToPixel;
        float moveSpeed = 25;
        float jumpHeight = 250;
        float retractSpeed = 100f;
        Texture2D texture;
        KeyboardState oldState;

        Joint swingJoint;

        float windowWidth;
        float windowHeight;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            windowWidth = GraphicsDevice.Viewport.Bounds.Width;
            windowHeight = GraphicsDevice.Viewport.Bounds.Height;

            spriteBatch = new SpriteBatch(GraphicsDevice);

            texture = this.Content.Load<Texture2D>("crate");
            mainWorld = new World(new Vector2(0, 9.8f));
            Vector2 size = new Vector2(10, 10);
            mainBody = BodyFactory.CreateRectangle(mainWorld, size.X * pixelToUnit, size.Y * pixelToUnit, (float)0.1);
            mainBody.Position = new Vector2(100*pixelToUnit, 0);
            mainBody.BodyType = BodyType.Dynamic;
            
            floor = new DrawablePhysicsObject(mainWorld, texture, new Vector2(100f, 100.0f), 1000);
            //floor.Position = new Vector2(GraphicsDevice.Viewport.Width / 2.0f, GraphicsDevice.Viewport.Height - 50);
            floor.Position = new Vector2(GraphicsDevice.Viewport.Width / 2.0f, 10);
            floor.body.BodyType = BodyType.Static;

            platform1 = new DrawablePhysicsObject(mainWorld, texture, new Vector2(300f, 300f), 1000);
            platform1.Position = new Vector2(0, windowHeight);
            platform1.body.BodyType = BodyType.Static;
            //mainBody.Position = new Vector2((GraphicsDevice.Viewport.Width / 2.0f) * pixelToUnit, (GraphicsDevice.Viewport.Height / 2) * pixelToUnit);
            //JointFactory.CreateDistanceJoint(mainWorld, floor.body, mainBody);
            //JointFactory.CreateDistanceJoint(mainWorld, mainBody, floor.body, CoordinateHelper.ToWorld(floor.body.Position), CoordinateHelper.ToWorld(floor.body.Position));
            //JointFactory.CreateRopeJoint(mainWorld, mainBody, floor.body, CoordinateHelper.ToWorld(floor.body.Position), CoordinateHelper.ToWorld(floor.body.Position));
        }

        protected override void UnloadContent()
        {
            
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            mainWorld.Step((float)gameTime.ElapsedGameTime.TotalSeconds);

            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyUp(Keys.Space) && oldState.IsKeyDown(Keys.Space))
            {
                mainBody.LinearVelocity += new Vector2(0, -jumpHeight) * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                mainBody.LinearVelocity += new Vector2(-moveSpeed, 0)*(float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (keyboardState.IsKeyUp(Keys.Left) && oldState.IsKeyDown(Keys.Left))
            {
                mainBody.LinearVelocity = Vector2.Zero;
            }

            if (keyboardState.IsKeyUp(Keys.Right) && oldState.IsKeyDown(Keys.Right))
            {
                mainBody.LinearVelocity = Vector2.Zero;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                mainBody.LinearVelocity += new Vector2(moveSpeed, 0) * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (mainBody.Position.X < 0)
            {
                mainBody.ApplyTorque(0.05f);
                mainBody.ApplyForce(new Vector2(0.05f, 0));
            }

            if (mainBody.Position.X*unitToPixel > windowWidth)
            {
                mainBody.ApplyTorque(0.05f);
                mainBody.ApplyForce(new Vector2(-0.05f, 0));
            }

            if (mainBody.Position.Y < 0)
            {
                mainBody.ApplyForce(Vector2.UnitY * 0.0f);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.R))
            {
                mainBody.SetTransform(new Vector2(100 * pixelToUnit, 0),0);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
            {
                if (swingJoint == null)
                {
                    swingJoint = JointFactory.CreateRopeJoint(mainWorld, mainBody, floor.body, CoordinateHelper.ToWorld(floor.body.Position), CoordinateHelper.ToWorld(floor.body.Position));
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                if (swingJoint != null)
                {
                    this.mainWorld.RemoveJoint(swingJoint);
                    mainWorld.Step((float)gameTime.ElapsedGameTime.TotalSeconds);
                    swingJoint = null;
                    Vector2 NewPlayerPosition = floor.body.Position - mainBody.Position;
                    mainBody.Position+=new Vector2(NewPlayerPosition.X/retractSpeed,NewPlayerPosition.Y/retractSpeed);
                    swingJoint = JointFactory.CreateRopeJoint(mainWorld, mainBody, floor.body, CoordinateHelper.ToWorld(floor.body.Position), CoordinateHelper.ToWorld(floor.body.Position));
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                if (swingJoint != null)
                {
                    this.mainWorld.RemoveJoint(swingJoint);
                    mainWorld.Step((float)gameTime.ElapsedGameTime.TotalSeconds);
                    swingJoint = null;
                    Vector2 NewPlayerPosition = floor.body.Position - mainBody.Position;
                    mainBody.Position -= new Vector2(NewPlayerPosition.X/retractSpeed, NewPlayerPosition.Y/retractSpeed);
                    swingJoint = JointFactory.CreateRopeJoint(mainWorld, mainBody, floor.body, CoordinateHelper.ToWorld(floor.body.Position), CoordinateHelper.ToWorld(floor.body.Position));
                    
                }
            }

            if (Keyboard.GetState().IsKeyUp(Keys.LeftShift))
            {
                if (swingJoint != null)
                {
                    this.mainWorld.RemoveJoint(swingJoint);
                    mainWorld.Step((float)gameTime.ElapsedGameTime.TotalSeconds);
                    swingJoint = null;
                }
            }

            oldState = keyboardState;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            Vector2 position = mainBody.Position * unitToPixel;
            Vector2 scale = new Vector2(50 / (float)texture.Width, 50 / (float)texture.Height);
            spriteBatch.Draw(texture, position, null, Color.White, mainBody.Rotation, new Vector2(texture.Width / 2.0f, texture.Height / 2.0f), scale, SpriteEffects.None, 0);
            floor.Draw(spriteBatch);
            platform1.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

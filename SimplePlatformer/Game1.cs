using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Threading;

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
        float moveSpeed = 12;
        float jumpHeight = 250;
        float retractSpeed = 100f;
        float cameraPlayerBuffer = 225f;
        Texture2D texture;
        KeyboardState oldState;
        bool swinging = false;
        List<DrawablePhysicsObject> hookList = new List<DrawablePhysicsObject>();
        const int initialNumHooks = 5;
        int numHooks = initialNumHooks;
        float currentX = 0.0f;
        float moveVelocity = 125f;
        float maxAngularVelocity = 8f;
        float targetRopeX = 1000f;
        float targetRopeY = 5f;
        Vector2 currentHookPosition;
        Vector2 ropeOrigin;
        Vector2 ropePosition;
        Texture2D ropeTexture;
        Texture2D characterSprite;
        Texture2D hotLava;

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

            texture = this.Content.Load<Texture2D>("box-texture");
            characterSprite = this.Content.Load<Texture2D>("crate");
            hotLava = this.Content.Load<Texture2D>("fire");
            ropeTexture = this.Content.Load<Texture2D>("rope-sprite");
            mainWorld = new World(new Vector2(0, 20f));
            Vector2 size = new Vector2(10, 10);
            mainBody = BodyFactory.CreateRectangle(mainWorld, size.X * pixelToUnit, size.Y * pixelToUnit, (float)0.1);
            mainBody.Position = new Vector2(100*pixelToUnit, 0);
            mainBody.BodyType = BodyType.Dynamic;
            
            floor = new DrawablePhysicsObject(mainWorld, hotLava, new Vector2(700.0f*(numHooks-1), 100.0f), 1000);
            floor.Position = new Vector2(GraphicsDevice.Viewport.Width / 2.0f, GraphicsDevice.Viewport.Height - 50);
            floor.body.BodyType = BodyType.Static;

            for (int i = 1; i < (numHooks+1); i++)
            {
                hookList.Add(new DrawablePhysicsObject(mainWorld, this.Content.Load<Texture2D>("hook-sprite"), new Vector2(100f, 50f), 1000));
                hookList[hookList.Count - 1].Position = new Vector2((GraphicsDevice.Viewport.Width)*(i/2),10);
                hookList[hookList.Count - 1].body.BodyType = BodyType.Static;
                hookList[hookList.Count - 1].body.Rotation = 180;
            }

            platform1 = new DrawablePhysicsObject(mainWorld, texture, new Vector2(300f, 300f), 1000);
            platform1.Position = new Vector2(0, windowHeight);
            platform1.body.BodyType = BodyType.Static;
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

            ropeOrigin = new Vector2(ropeTexture.Bounds.Center.X, ropeTexture.Bounds.Center.Y);

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

            if ((mainBody.Position.Y * unitToPixel) > windowHeight-107)
            {
                mainBody.SetTransform(new Vector2(100 * pixelToUnit, 0), 0);
                findHook.stopNull = true;
            }

            if (mainBody.AngularVelocity >= maxAngularVelocity && mainBody.AngularVelocity > 0)
            {
                mainBody.AngularVelocity = 5.5f;
            }
            if (mainBody.AngularVelocity <= -maxAngularVelocity && mainBody.AngularVelocity < 0)
            {
                mainBody.AngularVelocity = -5.5f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.R))
            {
                mainBody.SetTransform(new Vector2(100 * pixelToUnit, 0),0);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
            {
                if (swingJoint == null)
                {
                    swingJoint = JointFactory.CreateRopeJoint(mainWorld, mainBody, findHook.findClosestHook(hookList, mainBody).body, CoordinateHelper.ToWorld(findHook.findClosestHook(hookList, mainBody).body.Position), CoordinateHelper.ToWorld(findHook.findClosestHook(hookList, mainBody).body.Position));
                    mainBody.Rotation = 0;
                    swinging = true;
                    currentHookPosition = findHook.findClosestHook(hookList, mainBody).body.Position;
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                
                if (swingJoint != null)
                {
                    this.mainWorld.RemoveJoint(swingJoint);
                    mainWorld.Step((float)gameTime.ElapsedGameTime.TotalSeconds);
                    swingJoint = null;
                    Vector2 NewPlayerPosition = findHook.findClosestHook(hookList, mainBody).body.Position - mainBody.Position;
                    mainBody.Position+=new Vector2(NewPlayerPosition.X/retractSpeed,NewPlayerPosition.Y/retractSpeed);
                    swingJoint = JointFactory.CreateRopeJoint(mainWorld, mainBody, findHook.findClosestHook(hookList, mainBody).body, CoordinateHelper.ToWorld(findHook.findClosestHook(hookList, mainBody).body.Position), CoordinateHelper.ToWorld(findHook.findClosestHook(hookList, mainBody).body.Position));
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                if (swingJoint != null)
                {
                    this.mainWorld.RemoveJoint(swingJoint);
                    mainWorld.Step((float)gameTime.ElapsedGameTime.TotalSeconds);
                    swingJoint = null;
                    Vector2 NewPlayerPosition = findHook.findClosestHook(hookList, mainBody).body.Position - mainBody.Position;
                    mainBody.Position -= new Vector2(NewPlayerPosition.X/retractSpeed, NewPlayerPosition.Y/retractSpeed);
                    swingJoint = JointFactory.CreateRopeJoint(mainWorld, mainBody, findHook.findClosestHook(hookList, mainBody).body, CoordinateHelper.ToWorld(findHook.findClosestHook(hookList, mainBody).body.Position), CoordinateHelper.ToWorld(findHook.findClosestHook(hookList, mainBody).body.Position));
                }
            }

            if (Keyboard.GetState().IsKeyUp(Keys.LeftShift))
            {
                if (swingJoint != null)
                {
                    this.mainWorld.RemoveJoint(swingJoint);
                    mainWorld.Step((float)gameTime.ElapsedGameTime.TotalSeconds);
                    swingJoint = null;
                    swinging = false;
                }
            }
            
            if (Keyboard.GetState().IsKeyDown(Keys.T))
            {
                mainBody.Rotation = 0;
            }
            oldState = keyboardState;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            currentX += moveVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, transformMatrix: Matrix.CreateTranslation(new Vector3((mainBody.Position.X * unitToPixel) - cameraPlayerBuffer, 0, 0) * -1));
            //spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, transformMatrix: Matrix.CreateTranslation(new Vector3(currentX, 0, 0) * -1));
            Vector2 position = mainBody.Position * unitToPixel;
            Vector2 scale = new Vector2(50 / (float)texture.Width, 50 / (float)texture.Height);
            spriteBatch.Draw(characterSprite, position, null, Color.White, mainBody.Rotation, new Vector2(texture.Width / 2.0f, texture.Height / 2.0f), scale, SpriteEffects.None, 0);
            floor.Draw(spriteBatch);
            foreach(var i in hookList)
            {
                i.Draw(spriteBatch);
            }
            platform1.Draw(spriteBatch);
            if (swinging && currentHookPosition != null)
            {
                spriteBatch.Draw(ropeTexture, position: position,rotation: (float)findHook.getAngle(mainBody.Position, currentHookPosition), scale: new Vector2(Vector2.Distance(mainBody.Position, findHook.findClosestHook(hookList, mainBody).body.Position)*unitToPixel / ropeTexture.Width, targetRopeY / ropeTexture.Height));
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

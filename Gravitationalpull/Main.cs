using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using MonoGame.Extended;
using System.Linq;
using System.Collections.Generic;

namespace Gravitationalpull {
    public class Main : Game {
        public static SpriteFont font { get; private set; }
        public static GraphicsDeviceManager graphics { get; private set; }
        public static readonly Random random = new Random();

        private SpriteBatch spriteBatch;
        private KeyboardState prevKeyboardState;
        private MouseState prevMouseState;

        private List<Mover> movers;
        private List<Attractor> attractors;

        /* Settings */
        public static readonly int height = 1080;
        public static readonly int width = 1920;
        public static float gravity = 10.0f;
        private readonly int moversAmount = 100;
        private readonly float minMass = 0.5f; //5;
        private readonly float maxMass = 5;

        public Main() {
            Main.graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            Main.graphics.PreferredBackBufferHeight = Main.height;
            Main.graphics.PreferredBackBufferWidth = Main.width;
        }

        protected override void Initialize() {
            base.Initialize();

            this.IsMouseVisible = true;

            // Reset lists
            this.movers = new List<Mover>();
            this.attractors = new List<Attractor>();

            // Create attractor
            float mass = 2;
            float radius = mass * 8;
            this.attractors.Add(new Attractor(mass,
                Main.width / 2,
                Main.height / 2,
                radius));

            // Create attractors - circle
            //for (int a = 0; a < 360; a += 60) {
            //    float r = 500; // Radius
            //    float angle = new Angle(a, AngleType.Degree).Radians;
            //    float x = r * (float)Math.Sin(angle) + attractors[0].position.X;
            //    float y = r * (float)Math.Cos(angle) + attractors[0].position.Y;

            //    this.attractors.Add(new Attractor(mass,
            //        x,
            //        y,
            //        radius));
            //}

            // Create movers - random
            this.movers = new List<Mover>(); // Reset
            for (int i = 0; i < this.moversAmount; i++) {
                float _mass = RandomExtensions.NextSingle(random, this.minMass, this.maxMass);
                float x = RandomExtensions.NextSingle(random, 0, Main.width);
                float y = RandomExtensions.NextSingle(random, 0, Main.height);
                this.movers.Add(new Mover(_mass, x, y, _mass * 8));
            }

            // Create movers - in circle
            //int degrees = 360;
            //int stepSize = 8;
            //int ringsOffset = 5;
            //for (int i = 0; i < 50; i++) { // Amount of rings
            //    for (int a = 0; a < degrees; a += stepSize) { // Ring size
            //        float r = 250 + (ringsOffset * i); // Radius
            //        float angle = new Angle(a, AngleType.Degree).Radians;
            //        float x = r * (float)Math.Sin(angle) + attractors[0].position.X;
            //        float y = r * (float)Math.Cos(angle) + attractors[0].position.Y;

            //        float _mass = RandomExtensions.NextSingle(random, this.minMass, this.maxMass);
            //        _mass -= i * 0.1f;
            //        this.movers.Add(new Mover(_mass, x, y, _mass * 8));
            //    }
            //}
        }

        protected override void LoadContent() {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Main.font = Content.Load<SpriteFont>("font");
        }

        protected override void UnloadContent() {
            
        }

        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (Keyboard.GetState().IsKeyUp(Keys.R) && this.prevKeyboardState.IsKeyDown(Keys.R))
                this.Initialize();
            if (Mouse.GetState().LeftButton == ButtonState.Released && this.prevMouseState.LeftButton == ButtonState.Pressed) {
                float mass = RandomExtensions.NextSingle(random, 0.5f, 3);
                this.movers.Add(new Mover(mass, Mouse.GetState().Position.ToVector2().X, Mouse.GetState().Position.ToVector2().Y, mass * 8));
            }
            if (Mouse.GetState().RightButton == ButtonState.Released && this.prevMouseState.RightButton == ButtonState.Pressed) {
                float mass = 5;
                this.attractors.Add(new Attractor(mass, Mouse.GetState().Position.ToVector2().X, Mouse.GetState().Position.ToVector2().Y, mass * 8));
            }

            foreach (Mover mover in this.movers) {
                // Calculate gravity according to mass
                //Vector2 gravity = new Vector2(0, Main.gravity * mover.mass);
                //mover.ApplyForce(gravity);

                // Calculate attraction from mover to attractor
                foreach (Attractor attractor in this.attractors) {
                    Vector2 force = attractor.Attract(mover);
                    mover.ApplyForce(force);
                }

                // Make mover move
                mover.Update();
            }

            base.Update(gameTime);
            this.prevKeyboardState = Keyboard.GetState();
            this.prevMouseState = Mouse.GetState();
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);
            //GraphicsDevice.Clear(new Color(45, 52, 54));

            foreach (Mover mover in this.movers) {
                mover.Draw(spriteBatch);
            }

            foreach (Attractor attractor in this.attractors) {
                //attractor.Draw(spriteBatch);
            }

            spriteBatch.Begin();

            spriteBatch.DrawString(Main.font, "Press R to restart", new Vector2(5, 5), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

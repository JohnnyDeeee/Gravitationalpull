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
        public static readonly int width = 1280;
        public static readonly int height = 1024;
        public static float gravity = 10.0f;
        private int moversAmount;
        private float minMass;
        private float maxMass;
        private bool debug;
        private bool hideHud;

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

            //this.InitializeRandom();
            this.InitializeCircle();
        }

        private void InitializeRandom() {
            this.moversAmount = 100;
            this.minMass = 0.5f;
            this.maxMass = 5f;

            // Create attractor
            float mass = 2;
            float radius = mass * 8;
            this.attractors.Add(new Attractor(mass,
                Main.width / 2,
                Main.height / 2,
                radius));

            // Create movers - random
            this.movers = new List<Mover>(); // Reset
            for (int i = 0; i < this.moversAmount; i++) {
                float _mass = RandomExtensions.NextSingle(random, this.minMass, this.maxMass);
                float x = RandomExtensions.NextSingle(random, 0, Main.width);
                float y = RandomExtensions.NextSingle(random, 0, Main.height);
                this.movers.Add(new Mover(_mass, x, y, _mass * 8));
            }
        }

        private void InitializeCircle() {
            this.minMass = 0.1f;
            // Ignoring maxMass, mass = minMass

            // Create attractor
            float mass = 2;
            float radius = mass * 8;
            this.attractors.Add(new Attractor(mass,
                Main.width / 2,
                Main.height / 2,
                radius));

            // Create attractors - circle
            for (int a = 90; a < 360; a += 180) {
                float r = 50; // Radius
                float angle = new Angle(a, AngleType.Degree).Radians;
                float x = r * (float)Math.Sin(angle) + attractors[0].position.X;
                float y = r * (float)Math.Cos(angle) + attractors[0].position.Y;

                this.attractors.Add(new Attractor(mass,
                    x,
                    y,
                    radius));
            }

            // Create movers - in circle
            int degrees = 360;
            int stepSize = 8/2;
            int ringsOffset = 5;
            int ringsAmount = 30;
            for (int i = 0; i < ringsAmount; i++) { // Amount of rings
                for (int a = 0; a < degrees; a += stepSize) { // Ring size
                    float r = 250 + (ringsOffset * i); // Radius
                    float angle = new Angle(a, AngleType.Degree).Radians;
                    float x = r * (float)Math.Sin(angle) + attractors[0].position.X;
                    float y = r * (float)Math.Cos(angle) + attractors[0].position.Y;

                    float _mass = this.minMass;
                    _mass -= i * 0.1f; // Decrease mass on each outer ring
                    _mass = MathHelper.Clamp(_mass, 0.1f, this.minMass); // Mass = size, so if the mass is the low we wont see the mover
                    this.movers.Add(new Mover(_mass, x, y, _mass * 8));
                }
            }
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
            if (Keyboard.GetState().IsKeyUp(Keys.D) && this.prevKeyboardState.IsKeyDown(Keys.D))
                this.debug = !this.debug;
            if (Keyboard.GetState().IsKeyUp(Keys.Space) && this.prevKeyboardState.IsKeyDown(Keys.Space))
                this.hideHud = !this.hideHud;
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
                    //Vector2 force = attractor.Attract(mover, 5f, 15f); // For random init
                    Vector2 force = attractor.Attract(mover, 50f, 50f); // For circle init
                    //Vector2 force = attractor.AttractAndRepel(mover, 500f);
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

            if(this.debug)
                foreach (Attractor attractor in this.attractors) {
                    attractor.Draw(spriteBatch);
                }

            spriteBatch.Begin();

            if (!hideHud) {
                spriteBatch.DrawString(Main.font, "Press R to restart", new Vector2(5, 5), Color.White);
                spriteBatch.DrawString(Main.font, "Press D to debug", new Vector2(5, 25), Color.White);
                spriteBatch.DrawString(Main.font, "Press SPACE to hide this text", new Vector2(5, 45), Color.White);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

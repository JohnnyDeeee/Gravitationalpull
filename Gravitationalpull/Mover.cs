using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;

namespace Gravitationalpull {
    public class Mover {
        public Vector2 position { get; private set; }
        public Vector2 velocity { get; private set; }
        public Vector2 acceleration { get; private set; }
        public float radius { get; private set; }
        public float mass { get; private set; }
        public Color color { get; private set; }

        public Mover(float mass, float x, float y, float radius) {
            this.mass = mass;
            this.position = new Vector2(x, y);
            this.radius = radius;
            this.velocity = Vector2.Zero;
            this.acceleration = Vector2.Zero;

            this.color = new Color((int)RandomExtensions.NextSingle(Main.random, 1, 255),
                (int)RandomExtensions.NextSingle(Main.random, 1, 255),
                (int)RandomExtensions.NextSingle(Main.random, 1, 255));
        }

        // Newton's 2nd law: F = M * A
        // or A = F / M
        public void ApplyForce(Vector2 force) {
            Vector2 _force = Vector2.Divide(force, this.mass);
            this.acceleration += _force;
        }

        public void Update() {
            this.velocity += this.acceleration;
            this.position += this.velocity;
            this.acceleration *= 0; // Clear the acceleration
        }

        public void Draw(SpriteBatch batch) {
            batch.Begin();

            //batch.DrawRectangle(this.position, new Size2(this.radius, this.radius), Color.Red, this.radius);
            batch.DrawCircle(this.position, this.radius, 32, this.color, this.radius);

            //batch.DrawString(Main.font, $"Y = {this.position.Y}", new Vector2(this.position.X, this.position.Y - 15), Color.White);
            //batch.DrawString(Main.font, $"{this.mass}", new Vector2(this.position.X, this.position.Y), Color.White);

            batch.End();
        }

        public Vector2 Attract(Mover mover, float minMagnitude, float maxMagnitude) {
            Vector2 force = Vector2.Subtract(this.position, mover.position);
            float magnitude = (float)Math.Sqrt(Math.Pow(force.X, 2) + Math.Pow(force.Y, 2)); // Distance

            magnitude = MathHelper.Clamp(magnitude, minMagnitude, maxMagnitude);

            force.Normalize();
            float strength = (Main.gravity * this.mass * mover.mass) / (float)(Math.Pow(magnitude, 2));
            force *= strength;

            return force;
        }

        public Vector2 AttractAndRepel(Mover mover, float maxMagnitude) {
            Vector2 force = Vector2.Subtract(this.position, mover.position);
            float magnitude = (float)Math.Sqrt(Math.Pow(force.X, 2) + Math.Pow(force.Y, 2)); // Distance

            magnitude -= mover.radius; // Makes sure the movers dont collide with the attractor

            magnitude = MathHelper.Clamp(magnitude, this.radius, maxMagnitude); // User our radius as a min distance

            float strengthModifier = -1 + (((magnitude - this.radius) * 0.01f) * 2); // // StrengthModifier that gives -1 when mover is closest and 1 when furthest
            strengthModifier *= 5; // Make it a bit stronger

            force.Normalize();
            float strength = (Main.gravity * this.mass * mover.mass) / (float)(Math.Pow(magnitude, 2));
            force *= strength * strengthModifier; // Multiply by strengthModifier to change the direction of the force when the mover is close

            return force;
        }
    }
}

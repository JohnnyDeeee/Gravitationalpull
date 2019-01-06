using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gravitationalpull {
    public class Attractor {
        public float mass { get; private set; }
        public Vector2 position { get; private set; }
        public float radius { get; private set; }

        public Attractor(float mass, float x, float y, float radius) {
            this.mass = mass;
            this.position = new Vector2(x, y);
            this.radius = radius;
        }

        public void Draw(SpriteBatch batch) {
            batch.Begin();

            batch.DrawCircle(this.position, this.radius, 32, Color.Black, this.radius);

            batch.End();
        }

        public Vector2 Attract(Mover mover) {
            Vector2 force = Vector2.Subtract(this.position, mover.position);
            float magnitude = (float)Math.Sqrt(Math.Pow(force.X, 2) + Math.Pow(force.Y, 2)); // Distance

            magnitude = MathHelper.Clamp(magnitude, 5, 15);
            //magnitude = 50;

            force.Normalize();
            float strength = (Main.gravity * this.mass * mover.mass) / (float)(Math.Pow(magnitude, 2));
            force *= strength;

            return force;
        }
    }
}

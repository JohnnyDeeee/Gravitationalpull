﻿using Microsoft.Xna.Framework;
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

            batch.DrawCircle(this.position, this.radius, 32, Color.Red, this.radius);

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

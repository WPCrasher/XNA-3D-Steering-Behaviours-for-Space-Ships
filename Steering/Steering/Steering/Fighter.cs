using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;


namespace Steering
{
   
    public class Fighter:Entity
    {
        public Vector3 TargetPos = Vector3.Zero;
        private Fighter target = null;
        public Vector3 offset;
        private Path path = new Path();
        string modelName;

        public string ModelName
        {
            get { return modelName; }
            set { modelName = value; }
        }

        internal Path Path
        {
            get { return path; }
            set { path = value; }
        }

        
        public Fighter Target
        {
            get { return target; }
            set { target = value; }
        }
        private Fighter leader = null;

        public Fighter Leader
        {
            get { return leader; }
            set { leader = value; }
        }

        SteeringBehaviours steeringBehaviours;
        public float maxSpeed = Params.GetFloat("max_speed");
        bool drawAxis;
        List<Vector3> feelers = new List<Vector3>();

        // The acceleration is smoothed
        Vector3 acceleration;

        public List<Vector3> Feelers
        {
            get { return feelers; }
            set { feelers = value; }
        }

        public bool DrawAxis
        {
            get { return drawAxis; }
            set { drawAxis = value; }
        }

        bool drawFeelers;

        float roll = 0.0f;

        public bool DrawFeelers
        {
            get { return drawFeelers; }
            set { drawFeelers = value; }
        }

        internal SteeringBehaviours SteeringBehaviours
        {
            get { return steeringBehaviours; }
            set { steeringBehaviours = value; }
        }

        public Fighter()
        {
            worldTransform = Matrix.Identity;
            Position = new Vector3(0, 10, 0);
            Look = new Vector3(0, 0, -1);
            Right = new Vector3(1, 0, 0);
            Up = new Vector3(0, 1, 0);
            globalUp = new Vector3(0, 1, 0);
            steeringBehaviours = new SteeringBehaviours(this);
            drawAxis = false;
            Solid = true;
            modelName = "boa";
        }

        public override void LoadContent() 
        {            
            model = XNAGame.Instance.Content.Load<Model>(modelName);
            worldTransform = Matrix.CreateWorld(Position, Look, Up);

            foreach (ModelMesh mesh in model.Meshes)
            {
                BoundingSphere = BoundingSphere.CreateMerged(BoundingSphere, mesh.BoundingSphere);
            }

        }

        public override void UnloadContent()
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            float timeDelta = (float) gameTime.ElapsedGameTime.TotalSeconds;
            float smoothRate;
            steeringBehaviours.timeDelta = timeDelta;
            force = steeringBehaviours.calculate();
            SteeringBehaviours.checkNaN(force);
            Vector3 newAcceleration = force / Mass;

            if (timeDelta > 0)
            {
                smoothRate = Utilities.Clip(9 * timeDelta, 0.15f, 0.4f) / 2.0f;
                Utilities.BlendIntoAccumulator(smoothRate, newAcceleration, ref acceleration);
            }

            velocity += acceleration * timeDelta;
            float speed = velocity.Length();
            if (speed > maxSpeed)
            {

                velocity.Normalize();
                velocity *= maxSpeed;
            }
            Position += velocity * timeDelta;            

            // the length of this global-upward-pointing vector controls the vehicle's
            // tendency to right itself as it is rolled over from turning acceleration
            Vector3 globalUp = new Vector3(0, 0.2f, 0);
            // acceleration points toward the center of local path curvature, the
            // length determines how much the vehicle will roll while turning
            Vector3 accelUp = acceleration * 0.05f;
            // combined banking, sum of UP due to turning and global UP
            Vector3 bankUp = accelUp + globalUp;
            // blend bankUp into vehicle's UP basis vector
            smoothRate = timeDelta * 3;
            Vector3 tempUp = Up;
            Utilities.BlendIntoAccumulator(smoothRate, bankUp, ref tempUp);
            Up = tempUp;
            Up.Normalize();

            if (speed > 0.0001f)
            {
                Look = velocity;
                Look.Normalize();
                if (Look.Equals(Right))
                {
                    Right = Vector3.Right;
                }
                else
                {
                    Right = Vector3.Cross(Look, Up);

                    Right.Normalize();

                    SteeringBehaviours.checkNaN(ref Right, Vector3.Right);
                    Up = Vector3.Cross(Right, Look);
                    Up.Normalize();
                    SteeringBehaviours.checkNaN(ref Up, Vector3.Up);
                }
                // Apply damping
                velocity *= 0.99f;
            }
            
            if (Look != basis)
            {
                
                float angle = (float)Math.Acos(Vector3.Dot(basis, Look));                
                Vector3 axis = Vector3.Cross(basis, Look);

                quaternion = Quaternion.CreateFromAxisAngle(axis, angle);
                quaternion.Normalize();
                
                worldTransform.Up = Up;
                worldTransform.Forward = Look;
                worldTransform.Right = Right;
                worldTransform = Matrix.CreateWorld(Position, Look, Up);
                checkNan(worldTransform);
            }
            else
            {
                worldTransform = Matrix.CreateTranslation(Position);
            }
            drawAxis = false;
        }

        private void checkNan(Matrix worldTransform)
        {
            if (float.IsNaN(worldTransform.M21))
            {
                System.Console.WriteLine("NAN!!");
            }

        }
        public override void Draw(GameTime gameTime)
        {
            /*
            SpriteFont spriteFont = XNAGame.Instance.SpriteFont;
            XNAGame.Instance.SpriteBatch.DrawString(spriteFont, "Pos: " + pos.X + " " + pos.Y + " " + pos.Z, new Vector2(10, 10), Color.White);
            XNAGame.Instance.SpriteBatch.DrawString(spriteFont, "Look: " + look.X + " " + look.Y + " " + look.Z, new Vector2(10, 30), Color.White);
            XNAGame.Instance.SpriteBatch.DrawString(spriteFont, "Right: " + right.X + " " + right.Y + " " + right.Z, new Vector2(10, 50), Color.White);
            XNAGame.Instance.SpriteBatch.DrawString(spriteFont, "Up: " + up.X + " " + up.Y + " " + up.Z, new Vector2(10, 70), Color.White);
            XNAGame.Instance.SpriteBatch.DrawString(spriteFont, "Roll: " + roll, new Vector2(10, 110), Color.White);
            */
            // Draw the mesh
            if (model != null)
            {
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.PreferPerPixelLighting = true;
                        effect.World = worldTransform;
                        effect.Projection = XNAGame.Instance.Camera.getProjection();
                        effect.View = XNAGame.Instance.Camera.getView();
                    }
                    mesh.Draw();
                }
            }

            if (drawAxis)
            {
                Line.DrawLine(Position, Position + (Look * 10), Color.White);
                Line.DrawLine(Position, Position + (Up * 10), Color.Red);
                Line.DrawLine(Position, Position + (Right * 10), Color.Blue);                                
            }

            if (drawFeelers)
            {
                foreach (Vector3 feeler in feelers)
                {
                    Line.DrawLine(Position, feeler, Color.Chartreuse);                    
                }
            }
        }
    }
}

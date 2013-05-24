using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Steering.Scenario
{
    public class FlockingScenario:Scenario
    {

        public override string Description()
        {
            return "Flocking Demo";
        }

        public override void SetUp()
        {
            Params.Load("flocking.properties");
            List<Entity> children = XNAGame.Instance.Children;
            //Ground ground = new Ground();
            //children.Add(ground);
            //XNAGame.Instance.Ground = ground;
            Fighter bigFighter = new EliteFighter();
            bigFighter.ModelName = "python";
            bigFighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.obstacle_avoidance);
            bigFighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.wander);
            bigFighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.pursuit);
            bigFighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.sphere_constrain);
            bigFighter.scale = 10.0f;
            children.Add(bigFighter);

            float range = Params.GetFloat("world_range");
            Fighter fighter = null;
            for (int i = 0; i < Params.GetFloat("num_boids"); i++)
            {
                Vector3 pos = Utilities.RandomPosition(range);

                fighter = new EliteFighter();
                fighter.ModelName = "ferdelance";
                fighter.Position = pos;
                fighter.Target = bigFighter;
                fighter.SteeringBehaviours.turnOffAll();
                fighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.separation);
                fighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.cohesion);
                fighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.alignment);
                fighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.wander);
                fighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.sphere_constrain);
                fighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.obstacle_avoidance);
                children.Add(fighter);
            }

            int numObstacles = 10;
            float dist = (range * 2) / numObstacles;
            for (float x = -range; x < range; x += dist)
            {
                for (float z = -range; z < range; z += dist)
                {
                    Obstacle o = new Obstacle(20);
                    o.Position = new Vector3(x, 0, z);
                    o.Color = new Vector3((float)Utilities.RNG.NextDouble(), (float)Utilities.RNG.NextDouble(), (float)Utilities.RNG.NextDouble());
                    o.ShouldDraw = true;
                    children.Add(o);
                }
            }

            bigFighter.Target = fighter;

            Fighter camFighter = new EliteFighter();
            Vector3 offset = new Vector3(0, 0, 10);
            fighter.ModelName = "cobramk3";
            camFighter.Position = fighter.Position + offset;
            camFighter.offset = offset;
            camFighter.Leader = fighter;
            camFighter.SteeringBehaviours.turnOffAll();
            camFighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.offset_pursuit);
            fighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.sphere_constrain);
            XNAGame.Instance.Children.Add(camFighter);

            XNAGame.Instance.CamFighter = camFighter;
            Camera camera = XNAGame.Instance.Camera;
            camera.Position = new Vector3(0.0f, 60.0f, 200.0f);

            foreach (Entity child in children)
            {
                child.LoadContent();
            }
        }

    

        public override void TearDown()
        {
            
        }
    }
}

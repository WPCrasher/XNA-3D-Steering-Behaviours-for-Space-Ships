using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Steering.Scenario
{
    public class ObstacleAvoidanceScenario:Scenario
    {
        public override string Description()
        {
            return "Obstacle Avoidance Demo";
        }

        public override void SetUp()
        {
            Params.Load("default.properties");
            List<Entity> children = XNAGame.Instance.Children;
            Fighter leader = new Fighter();
            leader.ModelName = "viper";
            leader.Position = new Vector3(10, 20, 20);            
            leader.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.arrive);
            leader.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.obstacle_avoidance);
            leader.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.separation);
            leader.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.wall_avoidance);
            leader.TargetPos = new Vector3(0, 100, -450);
            children.Add(leader);
            XNAGame.Instance.Leader = leader;

            // Add some Obstacles

            Obstacle o = new Obstacle(4);
            o.Position = new Vector3(0, 10, -10);
            children.Add(o);

            o = new Obstacle(17);
            o.Position = new Vector3(-10, 16, -80);
            children.Add(o);

            o = new Obstacle(10);
            o.Position = new Vector3(10, 15, -120);
            children.Add(o);

            o = new Obstacle(12);
            o.Position = new Vector3(5, -10, -150);
            children.Add(o);

            o = new Obstacle(20);
            o.Position = new Vector3(-2, 5, -200);
            children.Add(o);

            o = new Obstacle(10);
            o.Position = new Vector3(-25, -20, -250);
            children.Add(o);

            o = new Obstacle(10);
            o.Position = new Vector3(20, -20, -250);
            children.Add(o);

            o = new Obstacle(35);
            o.Position = new Vector3(-10, -30, -300);
            children.Add(o);

            // Now make a fleet
            int fleetSize = 5;
            float xOff = 6;
            float zOff = 6;
            for (int i = 2; i < fleetSize; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    float z = (i - 1) * +zOff;
                    Fighter fleet = new Fighter();
                    fleet.Leader = leader;
                    fleet.ModelName = "cobramk1";
                    fleet.offset = new Vector3((xOff * (-i / 2.0f)) + (j * xOff), 0, z);
                    fleet.Position = leader.Position + fleet.offset;
                    fleet.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.offset_pursuit);
                    fleet.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.separation);
                    fleet.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.wall_avoidance);
                    fleet.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.obstacle_avoidance);
                    children.Add(fleet);
                }
            }

            Fighter camFighter = new Fighter();
            camFighter.Leader = leader;
            camFighter.Position = new Vector3(0, 15, fleetSize * zOff);
            camFighter.offset = new Vector3(0, 5, fleetSize * zOff);
            camFighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.offset_pursuit);
            camFighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.wall_avoidance);
            camFighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.obstacle_avoidance);
            XNAGame.Instance.CamFighter = camFighter;
            children.Add(camFighter);


            Ground ground = new Ground();
            children.Add(ground);
            XNAGame.Instance.Ground = ground;
            foreach (Entity child in children)
            {
                child.Position.Y += 100;
                child.LoadContent();
            }
        }



        public override void TearDown()
        {
            
        }
    }
}

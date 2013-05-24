using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Steering.Scenario
{
    class WanderScenario:Scenario
    {
        public override string Description()
        {
            return "Wander Demo";
        }

        public override void SetUp()
        {
            Params.Load("default.properties");
            List<Entity> children = XNAGame.Instance.Children;
            Fighter leader = new Fighter();
            leader.Position = new Vector3(10, 120, 20);
            leader.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.wander);
            leader.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.obstacle_avoidance);
            leader.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.wall_avoidance);
            children.Add(leader);

            Fighter camFighter = new Fighter();
            camFighter.Leader = leader;
            camFighter.Position = new Vector3(10, 120, 0);
            camFighter.offset = new Vector3(0, 5, 10);
            camFighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.offset_pursuit);
            camFighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.wall_avoidance);
            camFighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.obstacle_avoidance);
            XNAGame.Instance.CamFighter = camFighter;
            children.Add(camFighter);

            Ground ground = new Ground();
            children.Add(ground);
            XNAGame.Instance.Ground = ground;

            XNAGame.Instance.Camera.Position = new Vector3(10, 120, 50);
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

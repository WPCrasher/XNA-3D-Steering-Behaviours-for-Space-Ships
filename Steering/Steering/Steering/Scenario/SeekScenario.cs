using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Steering.Scenario
{
    public class SeekScenario:Scenario
    {
        public override string Description()
        {
            return "Seek Demo";
        }

        public override void SetUp()
        {
            Params.Load("default.properties");
            List<Entity> children = XNAGame.Instance.Children;
            Fighter leader = new Fighter();
            leader.Position = new Vector3(-10, 20, 20);
            leader.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.seek);
            leader.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.obstacle_avoidance);
            leader.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.wall_avoidance);
            leader.TargetPos = new Vector3(10, 30, 20);
            children.Add(leader);
            XNAGame.Instance.Leader = leader;

            Fighter camFighter = new Fighter();
            camFighter.Leader = leader;
            camFighter.offset = new Vector3(0, 5, 10);
            camFighter.Position = leader.Position + camFighter.offset;
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
                child.LoadContent();
            }
        }



        public override void TearDown()
        {

        }
    }
}

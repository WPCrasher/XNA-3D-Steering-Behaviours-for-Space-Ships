using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Steering.Scenario
{
    class PursuitScenario:Scenario
    {
        public override string Description()
        {
            return "Pursuit Demo";
        }

        public override void SetUp()
        {
            Params.Load("default.properties");
            List<Entity> children = XNAGame.Instance.Children;

            Ground ground = new Ground();
            children.Add(ground);
            XNAGame.Instance.Ground = ground;

            Fighter fighter = new Fighter();
            fighter.ModelName = "cobramk1";
            fighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.arrive);
            fighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.obstacle_avoidance);
            fighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.wall_avoidance);
            fighter.Position = new Vector3(2, 20, -50);
            fighter.TargetPos = fighter.Position * 2;
            XNAGame.Instance.Leader = fighter;
            children.Add(fighter);

            Fighter fighter1 = new Fighter();
            fighter1.ModelName = "viper";
            fighter1.Target = fighter;
            fighter1.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.pursuit);
            fighter1.Position = new Vector3(-20, 20, -20);
            children.Add(fighter1);
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

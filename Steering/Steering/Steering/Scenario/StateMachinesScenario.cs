using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Steering.Scenario
{
    class StateMachinesScenario:Scenario
    {
        public override string Description()
        {
            return "Finite State Machines Demo";
        }

        public override void SetUp()
        {
            Params.Load("default.properties");
            List<Entity> children = XNAGame.Instance.Children;
            Ground ground = new Ground();
            children.Add(ground);
            XNAGame.Instance.Ground = ground;
            AIFighter aiFighter = new AIFighter();
            aiFighter.Position = new Vector3(-20, 50, 50);
            aiFighter.maxSpeed = 16.0f;
            aiFighter.SwicthState(new IdleState(aiFighter));
            aiFighter.Path.DrawPath = true;
            children.Add(aiFighter);

            Fighter fighter = new Fighter();
            fighter.ModelName = "ship2";
            fighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.arrive);
            fighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.obstacle_avoidance);
            fighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.wall_avoidance);
            fighter.Position = new Vector3(10, 50, 0);
            fighter.TargetPos = aiFighter.Position + new Vector3(-50, 0, -80);
            children.Add(fighter);

            Fighter camFighter = new Fighter();
            camFighter.Leader = fighter;
            camFighter.offset = new Vector3(0, 5, 10);
            camFighter.Position = fighter.Position + camFighter.offset;
            camFighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.offset_pursuit);
            camFighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.wall_avoidance);
            camFighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.obstacle_avoidance);
            XNAGame.Instance.CamFighter = camFighter;
            children.Add(camFighter);

            XNAGame.Instance.Leader = fighter;
            Camera camera = XNAGame.Instance.Camera;
            camera.Position = new Vector3(0.0f, 60.0f, 100.0f);

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

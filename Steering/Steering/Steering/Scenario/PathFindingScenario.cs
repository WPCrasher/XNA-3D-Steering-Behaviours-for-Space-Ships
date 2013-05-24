using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Steering.Scenario
{
    public class PathFindingScenario:Scenario
    {
        private PathFinder pathFinder;
        KeyboardState oldState;
        Vector3 targetPos;
        Vector3 startPos;
        Fighter fighter;

        public override string Description()
        {
            return "Path Finding Demo";
        }

        public override void SetUp()
        {
            Params.Load("default.properties");

            targetPos = new Vector3(20, 0, -150);
            startPos = new Vector3(10, 0, 20);

            List<Entity> children = XNAGame.Instance.Children;
            fighter = new Fighter();
            fighter.ModelName = "fighter";
            fighter.Position = startPos;
            children.Add(fighter);
            Obstacle obstacle = new Obstacle(10);
            obstacle.Position = new Vector3(15, 0, -10);
            children.Add(obstacle);

            obstacle = new Obstacle(12);
            obstacle.Position = new Vector3(5, 0, -50);
            children.Add(obstacle);

            obstacle = new Obstacle(5);
            obstacle.Position = new Vector3(15, 0, -70);
            children.Add(obstacle);

            pathFinder = new PathFinder();
            
            Path path = pathFinder.FindPath(fighter.Position, targetPos);
            path.DrawPath = true;
            path.Looped = false;
            fighter.Path = path;
            XNAGame.Instance.Leader = fighter;
            fighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.follow_path);

            Fighter camFighter = new Fighter();
            camFighter.Leader = fighter;
            camFighter.offset = new Vector3(0, 0, 5);
            camFighter.Position = fighter.Position + camFighter.offset;
            camFighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.offset_pursuit);
            camFighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.wall_avoidance);
            camFighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.obstacle_avoidance);
            XNAGame.Instance.CamFighter = camFighter;
            children.Add(camFighter);

            foreach (Entity child in children)
            {
                child.LoadContent();
            }

            oldState = Keyboard.GetState();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);

            KeyboardState newState = Keyboard.GetState();
            bool recalculate = false;
            if (newState.IsKeyDown(Keys.P))
            {
                if (!oldState.IsKeyDown(Keys.P))
                {
                    pathFinder.Smooth = !pathFinder.Smooth;
                    recalculate = true;
                }
            }

            if (recalculate)
            {
                if (fighter.Path != null)
                {
                    fighter.Path.DrawPath = false;
                }

                Path path = pathFinder.FindPath(startPos, targetPos);
                fighter.Path = path;
                fighter.Path.DrawPath = true;
                fighter.Path.Next = 0;
            }
            oldState = newState;
        }

        public override void TearDown()
        {
            
        }
    }
}

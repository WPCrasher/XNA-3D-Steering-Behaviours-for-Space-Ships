using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Steering.Scenario
{
    public abstract class Scenario
    {
        public abstract string Description();
        public abstract void SetUp();

        public virtual void Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                Vector3 newTargetPos = XNAGame.Instance.Camera.Position + (XNAGame.Instance.Camera.Look * 100.0f);
                //newTargetPos.Y = 8;
                XNAGame.Instance.Leader.TargetPos = newTargetPos;

            }

            if (mouseState.RightButton == ButtonState.Pressed)
            {
                Vector3 newTargetPos = XNAGame.Instance.Camera.Position;
                XNAGame.Instance.Leader.TargetPos = newTargetPos;

            }
        }
        public abstract void TearDown();        
    }
}

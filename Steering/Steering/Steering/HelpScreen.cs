using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Steering
{
    class HelpScreen
    {
        SpriteFont spriteFont;
        string[] extras = {"C: Toggle camera following" 
                          , "P: Toggle Path Smoothing"
                          , "O: Toggle 3D Path Finding"
                          };

        public HelpScreen()
        {
            spriteFont = XNAGame.Instance.Content.Load<SpriteFont>("Verdana");
        }

        public void Draw()
        {
            Vector2 top = new Vector2(20, 0);
            float gap = 30.0f;
            int key = 1;
            Vector2 position = top;
            XNAGame.Instance.SpriteBatch.DrawString(spriteFont, "F1: Toggle Help", position, Color.White);
            foreach (Scenario.Scenario scenario in XNAGame.Instance.Scenarios)
            {
                string message = "F" + (key + 1) + ": " + scenario.Description();                
                position.Y = gap * (key ++);
                XNAGame.Instance.SpriteBatch.DrawString(spriteFont, message, position, Color.White);
            }
            foreach (string message in extras)
            {
                position.Y = gap * (key++);
                XNAGame.Instance.SpriteBatch.DrawString(spriteFont, message, position, Color.White);
            }
        }
    }
}

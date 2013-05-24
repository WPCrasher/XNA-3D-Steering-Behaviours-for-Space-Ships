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
using System.Threading;
using Steering.Scenario;


namespace Steering
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class XNAGame : Microsoft.Xna.Framework.Game
    {
        static XNAGame instance = null;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Fighter camFighter;
        KeyboardState oldState;
        List<Scenario.Scenario> scenarios = new List<Scenario.Scenario>();
        HelpScreen helpScreen;

        public List<Scenario.Scenario> Scenarios
        {
            get { return scenarios; }
            set { scenarios = value; }
        }
        int scenarioID = 0;
        bool drawHelpScreen = true;

        public Fighter CamFighter
        {
            get { return camFighter; }
            set { camFighter = value; }
        }

        private Ground ground;
        public Ground Ground
        {
            get { return ground; }
            set { ground = value; }
        }        
        bool useCamFighter = false;

        
        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
            set { spriteBatch = value; }
        }
        private Camera camera;
        List<Entity> children = new List<Entity>();

        private Queue<Entity> updateQueue = new Queue<Entity>();

        public List<Entity> Children
        {
            get { return children; }
            set { children = value; }
        }
        private Fighter fighter;

        public Fighter Leader
        {
            get { return fighter; }
            set { fighter = value; }
        }

        public static XNAGame Instance
        {
            get
            {
                return instance;
            }
        }

        Space space;

        public Space Space
        {
            get { return space; }
            set { space = value; }
        }

        public XNAGame()
        {
            instance = this;
            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
            graphics.PreferMultiSampling = true;
            graphics.SynchronizeWithVerticalRetrace = true;
            graphics.ApplyChanges();
            graphics.IsFullScreen = false;
            Content.RootDirectory = "Content";

        }
        
        protected override void Initialize()
        {
       
                        
            // TODO: Add your initialization logic here
            camera = new Camera();

            camera.Position = new Vector3(2, 20, 50);
            int midX = GraphicsDeviceManager.DefaultBackBufferHeight / 2;
            int midY = GraphicsDeviceManager.DefaultBackBufferWidth / 2;
            Mouse.SetPosition(midX, midY);
            children.Add(camera);
            Params.Load("default.properties");
            space = new Space();
            oldState = Keyboard.GetState();

            scenarios.Add(new SeekScenario());
            scenarios.Add(new ArriveScenario());
            scenarios.Add(new PursuitScenario());
            scenarios.Add(new WanderScenario());
            scenarios.Add(new StateMachinesScenario());
            scenarios.Add(new ObstacleAvoidanceScenario());
            scenarios.Add(new FlockingScenario());
            scenarios.Add(new PathFindingScenario());

            helpScreen = new HelpScreen();

            ClearWorld();
            scenarios[scenarioID].SetUp();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()

        {

            Vector2 a = new Vector2(10, 10);
            Vector2 b = new Vector2(20, 20);

            a = b;
            b.X = 30;
            b.Y = 30;
            
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            foreach (Entity child in children)
            {
                child.LoadContent();
            }            
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            foreach (Entity child in children)
            {
                child.UnloadContent();
            }
        }

        public void ClearWorld()
        {
            for (int i = children.Count - 1; i >= 0; i--)
            {
                if (children[i] != camera)
                {
                    children.Remove(children[i]);
                }
            }
            camera.Position = new Vector3(2, 20, 50);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            
            KeyboardState newState = Keyboard.GetState();
            if (newState.IsKeyDown(Keys.C))
            {
                if (!oldState.IsKeyDown(Keys.C))
                {
                    useCamFighter = !useCamFighter;
                }
            }

            if (newState.IsKeyDown(Keys.F1))
            {
                if (!oldState.IsKeyDown(Keys.F1))
                {
                    drawHelpScreen = !drawHelpScreen;
                }
            }

            for (int i = 0; i < scenarios.Count; i++)
            {
                if (newState.IsKeyDown(Keys.F2 + i))
                {                    
                    scenarioID = i;
                    ClearWorld();
                    scenarios[scenarioID].SetUp();
                }
            }

            scenarios[scenarioID].Update(gameTime);
            
            if (useCamFighter)
            {
                camera.Position = camFighter.Position;
                camera.Look = camFighter.Look;
                camera.Up = camFighter.Up;
                camera.Right = camFighter.Right;
            }
            else
            {
                camera.Up = Vector3.Up;
                camera.Right = Vector3.Cross(Camera.Look, Camera.Up);
            }

            for (int i = children.Count - 1; i >= 0; i--)
            {
                if (children[i].Alive == false)
                {
                    children.Remove(children[i]);
                }
                else
                {
                    children[i].Update(gameTime);
                }
            }

            oldState = newState;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            // Allows the game to exit
            KeyboardState keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            float timeDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            foreach (Entity child in children)
            {
                DepthStencilState state = new DepthStencilState();
                state.DepthBufferEnable = true;
                GraphicsDevice.DepthStencilState = state;

                if (child != camFighter)
                {
                    child.Draw(gameTime);
                }
            }
            // Draw any lines
            Line.Draw();

            if (drawHelpScreen)
            {
                helpScreen.Draw();
            }
            
            spriteBatch.End();
            base.Draw(gameTime);
        }

        public Camera Camera
        {
            get
            {
                return camera;
            }
            set
            {
                camera = value;
            }
        }

        public GraphicsDeviceManager GraphicsDeviceManager
        {
            get
            {
                return graphics;
            }
        }
    }
}

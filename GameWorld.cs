using Kaiju.Command;
using Kaiju.ComponentPattern;
using Kaiju.State;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Kaiju
{
    /// <summary>
    /// Enum for whether the players are using a controller or keyboard as input
    /// </summary>
    public enum InputType
    {
        Keyboard,
        GamePad
    }

    /// <summary>
    /// GameWorld is the main game world class and the core loop logic of the game
    /// Handles initialization, content loading, updates, drawing and so on
    /// Made by: All
    /// </summary>
    public class GameWorld : Game
    {
        private static GameWorld instance;

        public static GameWorld Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameWorld();
                }
                return instance;
            }
        }

        public GraphicsDeviceManager _graphics;
        public GraphicsDeviceManager Graphics { get { return _graphics; } }
        private SpriteBatch _spriteBatch;

        private List<GameObject> gameObjects = new List<GameObject>();
        private List<GameObject> newGameObjects = new List<GameObject>();
        private List<GameObject> destroyedGameObjects = new List<GameObject>();
        private List<GameObject> UIObjects = new List<GameObject>();

        public GameObject player1Go;
        public Player player1;
        public GameObject player2Go;
        public Player player2;

        public string SelectedCharacterNameP1 { get; set; }
        public string SelectedCharacterNameP2 { get; set; }

        public string SelectedPlayerProfileP1 { get; set; }
        public string SelectedPlayerProfileP2 { get; set; }

        public GameObject stageGo;
        public Stage stage;
        

        private IGameState currentState;

        public Camera camera;

        private bool p1GamepadConnected;
        private bool p2GamepadConnected;

        public float DeltaTime { get; private set; }

        /// <summary>
        /// Contructor used to set specific variables on launch
        /// such as window size and fullscreen
        /// </summary>
        private GameWorld()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferHeight = 1440;
            _graphics.PreferredBackBufferWidth = 2560;
            _graphics.ApplyChanges();
            //_graphics.ToggleFullScreen();
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            p1GamepadConnected = GamePad.GetState(PlayerIndex.One).IsConnected;
            p2GamepadConnected = GamePad.GetState(PlayerIndex.Two).IsConnected;
        }

        /// <summary>
        /// Initializes the game world and sets the initial game state
        /// </summary>
        protected override void Initialize()
        {

            currentState = new MenuState(this); // starter scenen
            currentState.OnControllerConnectionChanged(p1GamepadConnected, p2GamepadConnected);

            base.Initialize();
        }

        /// <summary>
        /// Loads all required game content
        /// </summary>
        protected override void LoadContent()
        {
            

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            

        }

        /// <summary>
        /// Main update loop of the game
        /// </summary>
        /// <param name="gameTime">Makes the game frameindependent</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (Keyboard.GetState().IsKeyDown(Keys.M))
            {
                Graphics.PreferredBackBufferWidth++;
                Graphics.PreferredBackBufferHeight++;
            }

            bool currentP1Connected = GamePad.GetState(PlayerIndex.One).IsConnected;
            bool currentP2Connected = GamePad.GetState(PlayerIndex.Two).IsConnected;

            if (currentP1Connected != p1GamepadConnected || currentP2Connected != p2GamepadConnected)
            {
                p1GamepadConnected = currentP1Connected;
                p2GamepadConnected = currentP2Connected;

                if (currentState is IGameState gameState)
                {
                    gameState.OnControllerConnectionChanged(p1GamepadConnected, p2GamepadConnected);
                }
            }

            DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            foreach (var gameObject in gameObjects)
            {
                gameObject.Update();
            }

            foreach (var ui in UIObjects)
            {
                ui.Update();
            }

            
            InputHandler.Instance.Execute();
            CheckCollision();
            Cleanup();

            if (currentState != null)
            {
                currentState.Update(gameTime);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Draws and renders all gameobjects, UI
        /// </summary>
        /// <param name="gameTime">Makes the game frameindependent</param>
        protected override void Draw(GameTime gameTime)
        {
            // if currentState is not null it will use the states BackgoundColor.
            // If the currentState is null it will default to CornflowerBlue (it is like an if-else statement)
            GraphicsDevice.Clear(currentState?.DefaultBackgroundColor ?? Color.CornflowerBlue);

            if (currentState is BattleState)
            {
                Matrix transform = Matrix.CreateTranslation(-camera.GetTopLeft().X, -camera.GetTopLeft().Y, 0);
                _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, transform);
            }
            else
            {
                _spriteBatch.Begin();
            }

            currentState.Draw(_spriteBatch);
            
            foreach (var gameObject in gameObjects)
            {
                gameObject.Draw(_spriteBatch);
            }
            
            _spriteBatch.End();


            _spriteBatch.Begin();


            foreach (var ui in UIObjects)
            {
                ui.Draw(_spriteBatch);
            }

            if (currentState != null)
            {
                //currentState.Draw(_spriteBatch);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Detects and handles collision between all active gameobjects
        /// </summary>
        public void CheckCollision()
        {
            HashSet<(GameObject, GameObject)> handledCollisions = new();
            foreach (GameObject go1 in gameObjects)
            {
                foreach (GameObject go2 in gameObjects)
                {
                    if (go1 == go2 || handledCollisions.Contains((go1, go2)))
                    {
                        continue;
                    }
                    Collider col1 = go1.GetComponent<Collider>() as Collider;
                    Collider col2 = go2.GetComponent<Collider>() as Collider;

                    if (col1 != null && col2 != null && col1.CollisionBox.Intersects(col2.CollisionBox))
                    {
                        bool handledCollision = false;

                        foreach (RectangleData rects1 in col1.PixelPerfectRectangles)
                        {
                            foreach (RectangleData rects2 in col2.PixelPerfectRectangles)
                            {
                                if (rects1.Rectangle.Intersects(rects2.Rectangle))
                                {
                                    Debug.WriteLine($"Collision with {go1} and {go2}");
                                    handledCollision = true;
                                    break;
                                }
                            }
                            if (rects1.Rectangle.Intersects(col2.CollisionBox) && col2.isAttack && !(col2.Owner == go1.GetComponent<Player>() as Player || col2.Owner == go1.GetComponent<AI>() as AI))
                            {
                                handledCollision = true;
                            }
                            if (handledCollision)
                            {
                                break;
                            }
                        }
                        if (handledCollision)
                        {
                            go1.OnCollisionEnter(col2);
                            handledCollisions.Add((go1, go2));
                        }
                    }
                }
            }
        }
        public bool CheckCollision(Collider col)
        {

            foreach (GameObject go2 in gameObjects)
            {
                if (go2.GetComponent<Stage>() as Stage == null)
                {
                    continue;
                }
                Collider col2 = go2.GetComponent<Collider>() as Collider;

                if (col != null && col2 != null && col.CollisionBox.Intersects(col2.CollisionBox))
                {
                    return true;
                }
            }
            return false;
        }
        public bool CheckCollision(Rectangle rect)
        {
            foreach (GameObject go2 in gameObjects)
            {
                if (go2.GetComponent<Stage>() as Stage == null)
                {
                    continue;
                }
                Collider col2 = go2.GetComponent<Collider>() as Collider;

                if (col2 != null && rect.Intersects(col2.CollisionBox))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Instantiates new gameobjects
        /// </summary>
        /// <param name="gameObjectToInstantiate">Gameobject to instantiate</param>
        public void Instantiate(GameObject gameObjectToInstantiate)
        {
            newGameObjects.Add(gameObjectToInstantiate);
        }

        /// <summary>
        /// Destroys gameobjects
        /// </summary>
        /// <param name="gameObjectToDestroy">Gameobject to destroy</param>
        public void Destroy(GameObject gameObjectToDestroy)
        {
            destroyedGameObjects.Add(gameObjectToDestroy);
        }

        /// <summary>
        /// Adds pending gameobjects to the gameobjec list
        /// and deletes gameobjects to be destroyed
        /// </summary>
        public void Cleanup()
        {
            for (int i = 0; i < newGameObjects.Count; i++)
            {
                gameObjects.Add(newGameObjects[i]);
                newGameObjects[i].Awake();
            }
            for (int i = 0; i < newGameObjects.Count; i++)
            {
                newGameObjects[i].Start();
            }
            for (int i = 0; i < destroyedGameObjects.Count; i++)
            {
                gameObjects.Remove(destroyedGameObjects[i]);
            }
            destroyedGameObjects.Clear();
            newGameObjects.Clear();
        }

        /// <summary>
        /// Loads and contructs an animation from a set of sprites
        /// </summary>
        /// <param name="animationName">The name of the animation</param>
        /// <param name="spriteNames">Array of the sprite names</param>
        /// <param name="fps">The frames per second of the animation</param>
        /// <param name="heldAnimation">Whether the animation should be held</param>
        /// <returns></returns>
        public Animation BuildAnimation(string animationName, string[] spriteNames, int fps, bool heldAnimation)
        {
            Texture2D[] sprites = new Texture2D[spriteNames.Length];

            for (int i = 0; i < sprites.Length; i++)
            {
                sprites[i] = GameWorld.Instance.Content.Load<Texture2D>(spriteNames[i]);
            }

            Animation animation = new Animation(animationName, sprites, fps, heldAnimation);

            return animation;
        }

        /// <summary>
        /// changes the current state to the new state
        /// </summary>
        /// <param name="newState"> the new state the current state will change into</param>
        public void ChangeGameState(IGameState newState)
        {
            if (currentState != null)
            {
                currentState.Exit();
            }
            currentState = newState;

            if (currentState is IGameState gameState)
            {
                gameState.OnControllerConnectionChanged(p1GamepadConnected, p2GamepadConnected);
            }
        }

        /// <summary>
        /// Adds a UI object to the world and initializes it
        /// </summary>
        /// <param name="uiObject">The UI Gameobject to add</param>
        public void AddUIObject(GameObject uiObject)
        {
            UIObjects.Add(uiObject);
            uiObject.Awake();
            uiObject.Start();

            Debug.WriteLine($"UIObject added: {uiObject}");
        }

        /// <summary>
        /// Removes a UI object from the world and marks it for destruction
        /// </summary>
        /// <param name="uiObjectToDestroy">The UI Gameobject to remove</param>
        public void DestroyUIObject(GameObject uiObjectToDestroy)
        {
            if (UIObjects.Contains(uiObjectToDestroy))
            {
                UIObjects.Remove(uiObjectToDestroy);
                Destroy(uiObjectToDestroy);
            }
        }
    }
}

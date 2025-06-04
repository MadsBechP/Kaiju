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
    public enum InputType
    {
        Keyboard,
        GamePad
    }

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

        public GameObject stageGo;
        public Stage stage;
        private Texture2D background;

        private IGameState currentState;

        public Camera camera;

        private bool p1GamepadConnected;
        private bool p2GamepadConnected;

        public float DeltaTime { get; private set; }
        private GameWorld()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferHeight = 1440;
            _graphics.PreferredBackBufferWidth = 2560;
            _graphics.ApplyChanges();
            _graphics.ToggleFullScreen();
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            p1GamepadConnected = GamePad.GetState(PlayerIndex.One).IsConnected;
            p2GamepadConnected = GamePad.GetState(PlayerIndex.Two).IsConnected;
        }

        protected override void Initialize()
        {

            currentState = new BattleState(this); // starter scenen

            base.Initialize();
        }

        protected override void LoadContent()
        {
            background = Content.Load<Texture2D>("City");

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            camera = new Camera();

        }

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

            camera.MoveToward((float)gameTime.ElapsedGameTime.TotalMilliseconds);
            InputHandler.Instance.Execute();
            CheckCollision();
            Cleanup();

            if (currentState != null)
            {
                currentState.Update(gameTime);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // if currentState is not null it will use the states BackgoundColor.
            // If the currentState is null til will default to CornflowerBlue (it is like an if-else statement)
            GraphicsDevice.Clear(currentState?.BackgoundColor ?? Color.CornflowerBlue);

            Matrix transform = Matrix.CreateTranslation(-camera.GetTopLeft().X, -camera.GetTopLeft().Y, 0);
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, transform);

            //draw background
            _spriteBatch.Draw(background,
                new Rectangle(
                    (int)Math.Round(camera.Center.X) - GraphicsDevice.Viewport.Width / 2,
                    (int)Math.Round(camera.Center.Y) - GraphicsDevice.Viewport.Height / 2,
                    GraphicsDevice.Viewport.Width,
                    GraphicsDevice.Viewport.Height),
                Color.White);


            foreach (var gameObject in gameObjects)
            {
                gameObject.Draw(_spriteBatch);
            }
            player1.DrawShield(_spriteBatch);
            player2.DrawShield(_spriteBatch);
            _spriteBatch.End();


            _spriteBatch.Begin();


            foreach (var ui in UIObjects)
            {
                ui.Draw(_spriteBatch);
            }

            if (currentState != null)
            {
                currentState.Draw(_spriteBatch);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

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

        public void Instantiate(GameObject gameObjectToInstantiate)
        {
            newGameObjects.Add(gameObjectToInstantiate);
        }

        public void Destroy(GameObject gameObjectToDestroy)
        {
            destroyedGameObjects.Add(gameObjectToDestroy);
        }

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

        public void AddUIObject(GameObject uiObject)
        {
            UIObjects.Add(uiObject);
            uiObject.Awake();
            uiObject.Start();

            Debug.WriteLine($"UIObject added: {uiObject}");
        }

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

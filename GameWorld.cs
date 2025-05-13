using DesignPatterns.ComponentPattern;
using Kaiju.Command;
using Kaiju.ComponentPattern;
using Kaiju.ComponentPattern.Characters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;

namespace Kaiju
{
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

        private GraphicsDeviceManager _graphics;
        public GraphicsDeviceManager Graphics { get { return _graphics; } }
        private SpriteBatch _spriteBatch;

        private List<GameObject> gameObjects = new List<GameObject>();
        private List<GameObject> newGameObjects = new List<GameObject>();
        private List<GameObject> destroyedGameObjects = new List<GameObject>();

        public GameObject player1Go;
        public Player player1;
        public GameObject player2Go;
        public Player player2;


        public float DeltaTime { get; private set; }
        private GameWorld()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.ToggleFullScreen();
            Window.AllowUserResizing = true;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            player1Go = new GameObject();
            player1 = player1Go.AddComponent<Player>();
            player1Go.AddComponent<SpriteRenderer>();
            player1Go.AddComponent<Collider>();
            player1Go.AddComponent<Animator>();
            player1.chr = player1Go.AddComponent<Godzilla>();
            gameObjects.Add(player1Go);

            player2Go = new GameObject();
            player2 = player2Go.AddComponent<AI>();
            player2Go.AddComponent<SpriteRenderer>();
            player2Go.AddComponent<Collider>();
            player2Go.AddComponent<Animator>();
            player2.chr = player2Go.AddComponent<Gigan>();
            gameObjects.Add(player2Go);

            foreach (var gameObject in gameObjects)
            {
                gameObject.Awake();
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            foreach (var gameObject in gameObjects)
            {
                gameObject.Start();
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            InputHandler.Instance.Execute();
            foreach (var gameObject in gameObjects)
            {
                gameObject.Update();
            }
            CheckCollision();
            Cleanup();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, null);
            foreach (var gameObject in gameObjects)
            {
                gameObject.Draw(_spriteBatch);
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

        public void Instantiate(GameObject gameObjectToInstantiate)
        {
            newGameObjects.Add(gameObjectToInstantiate);
        }

        public void Destroy(GameObject gameObjectToDestroy)
        {
            destroyedGameObjects.Add(gameObjectToDestroy);
        }

        private void Cleanup()
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

        public Animation BuildAnimation(string animationName, string[] spriteNames)
        {
            Texture2D[] sprites = new Texture2D[spriteNames.Length];

            for (int i = 0; i < sprites.Length; i++)
            {
                sprites[i] = GameWorld.Instance.Content.Load<Texture2D>(spriteNames[i]);
            }

            Animation animation = new Animation(animationName, sprites, 5);

            return animation;
        }
    }
}

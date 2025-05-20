using Kaiju.Command;
using Kaiju.ComponentPattern;
using Kaiju.ComponentPattern.Characters;
using Kaiju.Observer;
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
        private List<GameObject> UIObjects = new List<GameObject>();

        public GameObject player1Go;
        public Player player1;
        public GameObject player2Go;
        public Player player2;

        public GameObject stageGo;
        public Stage stage;
       
        private InputHandler inputHandler = InputHandler.Instance;
        private Camera camera;

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
            player1.collider = player1Go.AddComponent<Collider>(player1);
            player1Go.AddComponent<Animator>();
            player1.chr = player1Go.AddComponent<Godzilla>();
            gameObjects.Add(player1Go);

            player2Go = new GameObject();
            player2 = player2Go.AddComponent<Player>();
            player2Go.AddComponent<SpriteRenderer>();
            player2Go.AddComponent<Collider>();
            player2.collider = player2Go.AddComponent<Collider>(player2);
            player2Go.AddComponent<Animator>();
            player2.chr = player2Go.AddComponent<Gigan>();
            gameObjects.Add(player2Go);

            stageGo = new GameObject();
            stageGo.AddComponent<SpriteRenderer>();
            stageGo.AddComponent<Collider>();
            stageGo.AddComponent<Stage>();
            gameObjects.Add(stageGo);


            GameObject timerGo = new GameObject();
            timerGo.AddComponent<Timer>();
            
            UIObjects.Add(timerGo);
            timerGo.Awake();

            foreach (var gameObject in gameObjects)
            {
                gameObject.Awake();
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            
            Texture2D player1Profile = Content.Load<Texture2D>("GZProfile");
            Texture2D player2Profile = Content.Load<Texture2D>("GZProfile");
            string name1 = "null";
            string name2 = "null";
            switch (player1.chr)
            {
                case Godzilla:
                    {
                        player1Profile = Content.Load<Texture2D>("GZProfile");
                        name1 = "Godzilla";
                        break;
                    }
                case Gigan:
                    {
                        player1Profile = Content.Load<Texture2D>("GiganProfile");
                        name1 = "Gigan";
                        break;
                    }
            }
            switch (player2.chr)
            {
                case Godzilla:
                    {
                        player2Profile = Content.Load<Texture2D>("GZProfile");
                        name2 = "Godzilla";
                        break;
                    }
                case Gigan:
                    {
                        player2Profile = Content.Load<Texture2D>("GiganProfile");
                        name2 = "Gigan";
                        break;
                    }
            }




            GameObject player1DamageMeterGo = new GameObject();
            var playerDamageMeter = player1DamageMeterGo.AddComponent<DamageMeter>();
            playerDamageMeter.Setup(
                name1,
                player1Profile,
                new Vector2((Graphics.PreferredBackBufferWidth / 2) - 750, Graphics.PreferredBackBufferHeight - 185), // damageFontPos
                new Vector2((Graphics.PreferredBackBufferWidth / 2) - 735, Graphics.PreferredBackBufferHeight - 80), // namePos
                new Vector2((Graphics.PreferredBackBufferWidth / 2) - 1000,Graphics.PreferredBackBufferHeight - 250), // hudPos
                new Vector2((Graphics.PreferredBackBufferWidth / 2) - 950, Graphics.PreferredBackBufferHeight - 200) // profilePos
               );

            GameObject player2DamageMeterGo = new GameObject();
            var player2DamageMeter = player2DamageMeterGo.AddComponent<DamageMeter>();
            player2DamageMeter.Setup(
                name2,
                player2Profile,
                new Vector2((Graphics.PreferredBackBufferWidth / 2) + 790, Graphics.PreferredBackBufferHeight - 185), // damageFontPos
                new Vector2((Graphics.PreferredBackBufferWidth / 2) + 780, Graphics.PreferredBackBufferHeight - 80), // namePos
                new Vector2((Graphics.PreferredBackBufferWidth / 2) + 550, Graphics.PreferredBackBufferHeight - 250), // hudPos
                new Vector2((Graphics.PreferredBackBufferWidth / 2) + 610, Graphics.PreferredBackBufferHeight - 200) // profilePos
               );

            UIObjects.Add(player1DamageMeterGo);
            UIObjects.Add(player2DamageMeterGo);

            player1DamageMeterGo.Awake();
            player2DamageMeterGo.Awake();

            
            playerDamageMeter.SetSubject(player1);
            player2DamageMeter.SetSubject(player2);

            //AIDamageMeter.Updated();
            //playerDamageMeter.Updated();

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            foreach (var gameObject in gameObjects)
            {
                gameObject.Start();
            }
            foreach (var gameObject in UIObjects)
            {
                gameObject.Start();
            }
            camera = new Camera(_graphics);

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

            DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            InputHandler.Instance.Execute();

            foreach (var gameObject in gameObjects)
            {
                gameObject.Update();
            }
            foreach (var gameObject in UIObjects)
            {
                gameObject.Update();
            }

            camera.MoveToward((float)gameTime.ElapsedGameTime.TotalMilliseconds);
            CheckCollision();
            Cleanup();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Matrix transform = Matrix.CreateTranslation(-camera.GetTopLeft().X, -camera.GetTopLeft().Y, 0);
            //transform += Matrix.CreateScale(0.5f);
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, transform);
            foreach (var gameObject in gameObjects)
            {
                gameObject.Draw(_spriteBatch);
            }
            _spriteBatch.End();

            _spriteBatch.Begin();
            foreach (var gameObject in UIObjects)
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

                    foreach (RectangleData rects1 in col.PixelPerfectRectangles)
                    {
                        foreach (RectangleData rects2 in col2.PixelPerfectRectangles)
                        {
                            if (rects1.Rectangle.Intersects(rects2.Rectangle))
                            {
                            }
                        }
                        
                    }
                    
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

       
    }
}

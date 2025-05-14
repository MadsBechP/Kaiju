using Kaiju.Command;
using Kaiju.ComponentPattern;
using Kaiju.Observer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Kaiju
{
    public class GameWorld : Game, ISubject
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

        public GameObject playerGo;
        public Player player;

       
        private InputHandler inputHandler = InputHandler.Instance;

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
            playerGo = new GameObject();
            player = playerGo.AddComponent<Player>();
            playerGo.AddComponent<SpriteRenderer>();
            gameObjects.Add(playerGo);

            GameObject timerGo = new GameObject();
            timerGo.AddComponent<Timer>();
            gameObjects.Add(timerGo);


            foreach (var gameObject in gameObjects)
            {
                gameObject.Awake();
            }
            inputHandler.AddUpdateCommand(Keys.A, new MoveCommand(player, new Vector2(-1, 0)));
            inputHandler.AddUpdateCommand(Keys.D, new MoveCommand(player, new Vector2(1, 0)));
            inputHandler.AddButtonDownCommand(Keys.Space, new JumpCommand(player));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            var playerProfile = Content.Load<Texture2D>("GZProfile");

            GameObject playerDamageMeterGo = new GameObject();
            var playerDamageMeter = playerDamageMeterGo.AddComponent<DamageMeter>();
            playerDamageMeter.Setup(
                "GZ",
                playerProfile,
                new Vector2((Graphics.PreferredBackBufferWidth / 2) - 750, Graphics.PreferredBackBufferHeight - 185), // damageFontPos
                new Vector2((Graphics.PreferredBackBufferWidth / 2) - 735, Graphics.PreferredBackBufferHeight - 80), // namePos
                new Vector2((Graphics.PreferredBackBufferWidth / 2) - 1000,Graphics.PreferredBackBufferHeight - 250), // hudPos
                new Vector2((Graphics.PreferredBackBufferWidth / 2) - 950, Graphics.PreferredBackBufferHeight - 200) // profilePos
               );

            var AIProfile = Content.Load<Texture2D>("GZProfile");

            GameObject AIDamageMeterGo = new GameObject();
            var AIDamageMeter = AIDamageMeterGo.AddComponent<DamageMeter>();
            AIDamageMeter.Setup(
                "AI-GZ",
                AIProfile,
                new Vector2((Graphics.PreferredBackBufferWidth / 2) + 790, Graphics.PreferredBackBufferHeight - 185), // damageFontPos
                new Vector2((Graphics.PreferredBackBufferWidth / 2) + 780, Graphics.PreferredBackBufferHeight - 80), // namePos
                new Vector2((Graphics.PreferredBackBufferWidth / 2) + 550, Graphics.PreferredBackBufferHeight - 250), // hudPos
                new Vector2((Graphics.PreferredBackBufferWidth / 2) + 600, Graphics.PreferredBackBufferHeight - 200) // profilePos
               );

            gameObjects.Add(AIDamageMeterGo);
            gameObjects.Add(playerDamageMeterGo);
            AIDamageMeterGo.Awake();
            playerDamageMeterGo.Awake();
            AIDamageMeter.Updated();
            playerDamageMeter.Updated();

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
            inputHandler.Execute();
            foreach (var gameObject in gameObjects)
            {
                gameObject.Update();
            }
            Cleanup();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            foreach (var gameObject in gameObjects)
            {
                gameObject.Draw(_spriteBatch);
            }
            _spriteBatch.End();

            base.Draw(gameTime);
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

        private List<IObserver> observers = new List<IObserver>();
        public void Attach(IObserver observer)
        {
            observers.Add(observer);
        }

        public void Detach(IObserver observer)
        {
            observers.Remove(observer);
        }

        public void Notify()
        {
            foreach (IObserver observer in observers)
            {
                observer.Updated();
            }
        }
    }
}

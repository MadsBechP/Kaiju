using Kaiju.Command;
using Kaiju.ComponentPattern;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

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

            GameObject damageMeterGo = new GameObject();
            damageMeterGo.AddComponent<DamageMeter>();
            gameObjects.Add(damageMeterGo);
                        

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
    }
}

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

        public GameObject player1Go;
        public Player player1;
        public GameObject player2Go;
        public Player player2;

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
            player1Go = new GameObject();
            player1 = player1Go.AddComponent<Player>();
            player1Go.AddComponent<SpriteRenderer>();
            gameObjects.Add(player1Go);

            player2Go = new GameObject();
            player2 = player2Go.AddComponent<AI>();
            player2Go.AddComponent<SpriteRenderer>();
            gameObjects.Add(player2Go);

            foreach (var gameObject in gameObjects)
            {
                gameObject.Awake();
            }
            inputHandler.AddUpdateCommand(Keys.A, new MoveCommand(player1, new Vector2(-1, 0)));
            inputHandler.AddUpdateCommand(Keys.D, new MoveCommand(player1, new Vector2(1, 0)));
            inputHandler.AddButtonDownCommand(Keys.W, new JumpCommand(player1));
            inputHandler.AddUpdateCommand(Keys.Left, new MoveCommand(player2, new Vector2(-1, 0)));
            inputHandler.AddUpdateCommand(Keys.Right, new MoveCommand(player2, new Vector2(1, 0)));
            inputHandler.AddButtonDownCommand(Keys.Up, new JumpCommand(player2));

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

            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, null);
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

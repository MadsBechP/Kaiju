using Kaiju.Command;
using Kaiju.ComponentPattern;
using Kaiju.ComponentPattern.Characters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Kaiju.State
{
    /// <summary>
    /// BattleState manages the battle scene. 
    /// This include creating the scene, handling the victory conditions, and setting up players, timer and HUD.
    /// Made by Emilie
    /// </summary>
    public class BattleState : IGameState
    {
        private GameWorld game;
        private Texture2D background;
        private Texture2D player1Profile;
        private Texture2D player2Profile;
        string name1 = "null";
        string name2 = "null";
        private Timer timer;

        private List<GameObject> stateObjects = new List<GameObject>();
        private List<GameObject> UIObjects = new List<GameObject>();

        public Color DefaultBackgroundColor => Color.Gray;

        /// <summary>
        /// Constructor
        /// Initialize a new instance of the BattleState class.
        /// Sets up the stage, players, timer and HUD.
        /// </summary>
        /// <param name="game">The current GameWorld instance</param>
        public BattleState(GameWorld game)
        {
            this.game = game;

            InputHandler.Instance.ClearBindings(); // so that it won't try and create binding that are already there

            CreateStage();
            CreatePlayers();
            CreateTimer();
            LoadContent();
            HUDSetup();

            GameWorld.Instance.camera = new Camera();

            foreach (var obj in stateObjects)
            {
                game.Instantiate(obj);
            }
            game.Cleanup();
        }

        /// <summary>
        /// Called every frame to update game logic and check win conditions.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            bool player1Dead = game.player1.Lives <= 0;
            bool player2Dead = game.player2.Lives <= 0;

            if (timer != null && timer.TimeRanOut || player1Dead || player2Dead)
            {
                float player1Lives = game.player1.Lives;
                float player2Lives = game.player2.Lives;

                int p1KosGiven = 3 - game.player2.Lives;
                int p1KosTaken = 3 - game.player1.Lives;

                int p2KosGiven = 3 - game.player1.Lives;
                int p2KosTaken = 3 - game.player2.Lives;

                string char1 = name1;
                string char2 = name2;

                if (player1Lives == player2Lives)
                {
                    DatabaseManager.Instance.RecordMatchResult($"{game.SelectedPlayerProfileP1}", false, true, char1, p1KosGiven, p1KosTaken);
                    DatabaseManager.Instance.RecordMatchResult($"{game.SelectedPlayerProfileP2}", false, true, char2, p2KosGiven, p2KosTaken);

                    game.ChangeGameState(new VictoryState(game, "", true));
                }
                else if (player1Lives > player2Lives)
                {
                    DatabaseManager.Instance.RecordMatchResult($"{game.SelectedPlayerProfileP1}", true, false, char1, p1KosGiven, p1KosTaken);
                    DatabaseManager.Instance.RecordMatchResult($"{game.SelectedPlayerProfileP2}", false, false, char2, p2KosGiven, p2KosTaken);

                    game.ChangeGameState(new VictoryState(game, $"{name1}", false));
                }
                else
                {
                    DatabaseManager.Instance.RecordMatchResult($"{game.SelectedPlayerProfileP1}", false, false, char1, p1KosGiven, p1KosTaken);
                    DatabaseManager.Instance.RecordMatchResult($"{game.SelectedPlayerProfileP2}", true, false, char2, p2KosGiven, p2KosTaken);

                    game.ChangeGameState(new VictoryState(game, $"{name2}", false));
                }
            }
            GameWorld.Instance.camera.MoveToward((float)gameTime.ElapsedGameTime.TotalMilliseconds);
        }

        /// <summary>
        /// Loads the content for player HUD based on the selected character.
        /// </summary>
        private void LoadContent()
        {
            background = game.Content.Load<Texture2D>("City");
            switch (game.player1.chr)
            {
                case Godzilla:
                    {
                        player1Profile = game.Content.Load<Texture2D>("GZProfile");
                        name1 = "Godzilla";
                        break;
                    }
                case Gigan:
                    {
                        player1Profile = game.Content.Load<Texture2D>("GiganProfile");
                        name1 = "Gigan";
                        break;
                    }
            }
            switch (game.player2.chr)
            {
                case Godzilla:
                    {
                        player2Profile = game.Content.Load<Texture2D>("GZProfile");
                        name2 = "Godzilla";
                        break;
                    }
                case Gigan:
                    {
                        player2Profile = game.Content.Load<Texture2D>("GiganProfile");
                        name2 = "Gigan";
                        break;
                    }
            }
        }

        /// <summary>
        /// Sets up the HUD elements for both players.
        /// Adds them to the UI game object list.
        /// Sets them to subjects (the observer). 
        /// </summary>
        private void HUDSetup()
        {
            var width = game.GraphicsDevice.Viewport.Width;
            var height = game.GraphicsDevice.Viewport.Height;

            GameObject player1DamageMeterGo = new GameObject();
            var playerDamageMeter = player1DamageMeterGo.AddComponent<DamageMeter>();

            playerDamageMeter.Setup(
                name1,
                player1Profile,
                new Vector2(width * 0.145f, height * 0.895f), // damageFontPos
                new Vector2(width * 0.14f, height * 0.956f), // namePos
                new Vector2(width * 0.05f, height * 0.85f), // hudPos
                new Vector2(width * 0.067f, height * 0.88f) // profilePos
               );

            GameObject player2DamageMeterGo = new GameObject();
            var player2DamageMeter = player2DamageMeterGo.AddComponent<DamageMeter>();
            player2DamageMeter.Setup(
                name2,
                player2Profile,
                new Vector2(width * 0.844f, height * 0.895f), // damageFontPos 
                new Vector2(width * 0.845f, height * 0.956f), // namePos
                new Vector2(width * 0.75f, height * 0.85f), // hudPos
                new Vector2(width * 0.775f, height * 0.883f) // profilePos
               );


            game.AddUIObject(player1DamageMeterGo);
            game.AddUIObject(player2DamageMeterGo);

            UIObjects.Add(player1DamageMeterGo);
            UIObjects.Add(player2DamageMeterGo);

            playerDamageMeter.SetSubject(game.player1);
            player2DamageMeter.SetSubject(game.player2);
        }

        /// <summary>
        /// Creates both player objects based on character selection.
        /// </summary>
        public void CreatePlayers()
        {
            // Player 1
            game.player1Go = new GameObject();
            if (game.SelectedPlayerProfileP1 == "AI")
            {
                game.player1 = game.player1Go.AddComponent<AI>();
            }
            else
            {
                game.player1 = game.player1Go.AddComponent<Player>();
            }
            game.player1.InputType = InputType.Keyboard;
            game.player1.GamePadIndex = PlayerIndex.One;
            game.player1Go.AddComponent<SpriteRenderer>();
            game.player1.collider = game.player1Go.AddComponent<Collider>();
            game.player1.stageCollider = game.player1Go.AddComponent<Collider>(game.player1);
            game.player1Go.AddComponent<Animator>();
            game.player1.chr = CreateCharacter(game.player1Go, game.SelectedCharacterNameP1, out name1, true);
            
            stateObjects.Add(game.player1Go);

            // Player 2
            game.player2Go = new GameObject();
            if (game.SelectedPlayerProfileP2 == "AI")
            {
                game.player2 = game.player2Go.AddComponent<AI>();
            }
            else
            {
                game.player2 = game.player2Go.AddComponent<Player>();
            }
            game.player2.InputType = InputType.Keyboard;
            game.player2.GamePadIndex = PlayerIndex.Two;
            game.player2Go.AddComponent<SpriteRenderer>();
            game.player2.collider = game.player2Go.AddComponent<Collider>();
            game.player2.stageCollider = game.player2Go.AddComponent<Collider>(game.player2);
            game.player2Go.AddComponent<Animator>();
            game.player2.chr = CreateCharacter(game.player2Go, game.SelectedCharacterNameP2, out name2, false);

            stateObjects.Add(game.player2Go);
        }

        /// <summary>
        /// Creates the timer object and adds it to the UI.
        /// </summary>
        public void CreateTimer()
        {
            GameObject timerGo = new GameObject();
            timer = timerGo.AddComponent<Timer>();
            game.AddUIObject(timerGo);
            UIObjects.Add(timerGo);

        }

        /// <summary>
        /// Creates the stage with its components
        /// </summary>
        public void CreateStage()
        {
            game.stageGo = new GameObject();
            game.stageGo.AddComponent<SpriteRenderer>();
            game.stageGo.AddComponent<Collider>();
            game.stageGo.AddComponent<Stage>();
            stateObjects.Add(game.stageGo);
        }

        /// <summary>
        /// Creates the character component for a player, based on the selected name.
        /// It also returns the characters name.
        /// </summary>
        /// <param name="go"> the players gameobject </param>
        /// <param name="name"> the name of the selected character </param>
        /// <param name="characterName"> sets to the actual name of the character</param>
        /// <returns>the created character component</returns>
        private Character CreateCharacter(GameObject go, string name, out string characterName, bool isPlayer1)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = isPlayer1 ? "Godzilla" : "Gigan";
            }

            switch (name)
            {
                case "Godzilla":
                    characterName = "Godzilla";
                    return go.AddComponent<Godzilla>();
                case "Gigan":
                    characterName = "Gigan";
                    return go.AddComponent<Gigan>();
                case "Random":
                    return GetRandomCharacter(go, out characterName);
                default:
                    characterName = "Unknown";
                    return go.AddComponent<Godzilla>();
            }
        }
        
        private static Random rnd = new Random();
        /// <summary>
        /// Randomly chooses between character and return the chosen one.
        /// </summary>
        /// <param name="go">the gameobject to attach the character to.</param>
        /// <param name="name">gives and sets the actual name of the randomly selected character.</param>
        /// <returns>the created character component.</returns>
        private Character GetRandomCharacter(GameObject go, out string name)
        {
            if (rnd.Next(2) == 0)
            {
                name = "Godzilla";
                return go.AddComponent<Godzilla>();
            }
            else
            {
                name = "Gigan";
                return go.AddComponent<Gigan>();
            }
        }

        /// <summary>
        /// Draws the background and any custom effects (like shields).
        /// </summary>
        /// <param name="spriteBatch">used to draw</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background,
                new Rectangle(
                    (int)Math.Round(GameWorld.Instance.camera.Center.X) - GameWorld.Instance.GraphicsDevice.Viewport.Width / 2,
                    (int)Math.Round(GameWorld.Instance.camera.Center.Y) - GameWorld.Instance.GraphicsDevice.Viewport.Height / 2,
                    GameWorld.Instance.GraphicsDevice.Viewport.Width,
                    GameWorld.Instance.GraphicsDevice.Viewport.Height),
                Color.White);

            GameWorld.Instance.player1.DrawShield(spriteBatch);
            GameWorld.Instance.player2.DrawShield(spriteBatch);
        }

        /// <summary>
        /// When exiting, cleans up all objects created during the battle.
        /// </summary>
        public void Exit()
        {
            foreach (var obj in stateObjects)
            {
                game.Destroy(obj);
            }
            stateObjects.Clear();

            foreach (var ui in UIObjects)
            {
                game.DestroyUIObject(ui);
            }
            UIObjects.Clear();
        }

        /// <summary>
        /// Changes the controls of the players depeding on if a controller is connected or not
        /// </summary>
        /// <param name="p1Connected">Bool if player one has a controller connected</param>
        /// <param name="p2Connected">Bool if player two has a controller connected</param>
        public void OnControllerConnectionChanged(bool p1Connected, bool p2Connected)
        {
            if (p1Connected)
            {
                game.player1.InputType = InputType.GamePad;
            }
            else
            {
                game.player1.InputType = InputType.Keyboard;
            }
            if (p2Connected)
            {
                game.player2.InputType = InputType.GamePad;
            }
            else
            {
                game.player2.InputType = InputType.Keyboard;
            }

            InputHandler.Instance.ClearBindings();

            game.player1.chr.SetControls();
            game.player2.chr.SetControls();
        }
    }
}

using Kaiju.Command;
using Kaiju.ComponentPattern;
using Kaiju.ComponentPattern.Characters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Kaiju.State
{    
    public class BattleState : IGameState
    {
        private GameWorld game;
        private Texture2D player1Profile;
        private Texture2D player2Profile;
        string name1 = "null";
        string name2 = "null";

        private List<GameObject> stateObjects = new List<GameObject>();

        public Microsoft.Xna.Framework.Color BackgoundColor => Microsoft.Xna.Framework.Color.LightSlateGray;

        public BattleState(GameWorld game)
        {
            this.game = game;

            InputHandler.Instance.ClearBindings();
            CreatePlayers();
            CreateTimer();
            LoadContent();
            HUDSetup();
        }
        public void Update(GameTime gameTime)
        {
           
        }

        private void LoadContent()
        {
            
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
        private void HUDSetup()
        {
            GameObject player1DamageMeterGo = new GameObject();
            var playerDamageMeter = player1DamageMeterGo.AddComponent<DamageMeter>();
            playerDamageMeter.Setup(
                name1,
                player1Profile,
                new Vector2((game.Graphics.PreferredBackBufferWidth / 2) - 750, game.Graphics.PreferredBackBufferHeight - 185), // damageFontPos
                new Vector2((game.Graphics.PreferredBackBufferWidth / 2) - 735, game.Graphics.PreferredBackBufferHeight - 80), // namePos
                new Vector2((game.Graphics.PreferredBackBufferWidth / 2) - 1000, game.Graphics.PreferredBackBufferHeight - 250), // hudPos
                new Vector2((game.Graphics.PreferredBackBufferWidth / 2) - 950, game.Graphics.PreferredBackBufferHeight - 200) // profilePos
               );

            GameObject player2DamageMeterGo = new GameObject();
            var player2DamageMeter = player2DamageMeterGo.AddComponent<DamageMeter>();
            player2DamageMeter.Setup(
                name2,
                player2Profile,
                new Vector2((game.Graphics.PreferredBackBufferWidth / 2) + 790, game.Graphics.PreferredBackBufferHeight - 185), // damageFontPos
                new Vector2((game.Graphics.PreferredBackBufferWidth / 2) + 780, game.Graphics.PreferredBackBufferHeight - 80), // namePos
                new Vector2((game.Graphics.PreferredBackBufferWidth / 2) + 550, game.Graphics.PreferredBackBufferHeight - 250), // hudPos
                new Vector2((game.Graphics.PreferredBackBufferWidth / 2) + 610, game.Graphics.PreferredBackBufferHeight - 200) // profilePos
               );


            game.Instantiate(player1DamageMeterGo);
            game.Instantiate(player2DamageMeterGo);

            stateObjects.Add(player1DamageMeterGo);
            stateObjects.Add(player2DamageMeterGo);

            player1DamageMeterGo.Awake();
            player2DamageMeterGo.Awake();


            playerDamageMeter.SetSubject(game.player1);
            player2DamageMeter.SetSubject(game.player2);
        }

        public void CreatePlayers()
        {
            game.player1Go = new GameObject();
            game.player1 = game.player1Go.AddComponent<Player>();
            game.player1Go.AddComponent<SpriteRenderer>();
            game.player1Go.AddComponent<Collider>();
            game.player1Go.AddComponent<Animator>();
            game.player1.chr = game.player1Go.AddComponent<Godzilla>();
            game.Instantiate(game.player1Go);
            stateObjects.Add(game.player1Go);

            game.player2Go = new GameObject();
            game.player2 = game.player2Go.AddComponent<Player>();
            game.player2Go.AddComponent<SpriteRenderer>();
            game.player2Go.AddComponent<Collider>();
            game.player2Go.AddComponent<Animator>();
            game.player2.chr = game.player2Go.AddComponent<Gigan>();
            game.Instantiate(game.player2Go);
            stateObjects.Add(game.player2Go);
        }
        public void CreateTimer()
        {
            GameObject timerGo = new GameObject();
            timerGo.AddComponent<Timer>();
            game.Instantiate(timerGo);
            stateObjects.Add(timerGo);
        }

        public void Draw(SpriteBatch spriteBatch)
        {           
        }

        public void Exit()
        {
            foreach (var o in stateObjects)
            {
                game.Destroy(o);
            }
            stateObjects.Clear();
        }
    }
}

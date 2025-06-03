using Kaiju.Command;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaiju.State
{
    /// <summary>
    /// This class holds the logic of the Menu screen.
    /// In the Menu, the player choose which character they want to play as,
    /// and is able to create a player profile that saves the count of the players
    /// win and loses.
    /// </summary>
    public class MenuState : IGameState
    {
        public Color DefaultBackgroundColor => Color.DarkBlue;

        private GameWorld game;
        private SpriteFont promptFont;
        private string promptText;
        private Vector2 promptPosition;

        private List<CharacterProfile> profiles = new List<CharacterProfile>();
        
        private int selectedIndexP1;
        private int selectedIndexP2;
        private bool player1Confirmed = false;
        private bool player2Confirmed = false;

        private Texture2D gzTexture;
        private Texture2D ggTexture;
        private Texture2D rndTexture;

        private Texture2D highlightTexture;

        private Texture2D playerProfileTexture;
        private Vector2 ppOrigin;
        private Vector2 p1ProfilePos;
        private Vector2 p2ProfilePos;
        
        private SpriteFont playerProfileFont;
        private string p1Name;
        private string p2Name;
        private Vector2 p1NamePos;
        private Vector2 p2NamePos;

        private bool selectingProfileP1 = false;
        private bool selectingProfileP2 = false;
        


        public MenuState (GameWorld game)
        {
            this.game = game;
            DatabaseManager.Instance.Initialize();

            var w = game.GraphicsDevice.Viewport.Width;
            var h = game.GraphicsDevice.Viewport.Height;

            // The text
            promptFont = game.Content.Load<SpriteFont>("promptFont");
            promptText = "Choose Your Kaiju";
            promptPosition = new Vector2(game.GraphicsDevice.Viewport.Width * 0.50f, game.GraphicsDevice.Viewport.Height * 0.10f);

            playerProfileFont = game.Content.Load<SpriteFont>("Menu\\PlayerProfileFont");
            p1Name = "Test";
            p2Name = "Test";

            // Load texture
            gzTexture = game.Content.Load<Texture2D>("Menu\\GZMenuProfile");
            ggTexture = game.Content.Load<Texture2D>("Menu\\GGMenuProfile");
            rndTexture = game.Content.Load<Texture2D>("Menu\\RandomMenuProfile");

            highlightTexture = game.Content.Load<Texture2D>("Pixel");

            playerProfileTexture = game.Content.Load<Texture2D>("playerHUD");

            // Placements            
            profiles.Add(new CharacterProfile("Godzilla", gzTexture, new Vector2(w * 0.28f, h * 0.5f), false));
            profiles.Add(new CharacterProfile("Random", rndTexture, new Vector2(w * 0.50f, h * 0.5f), false));
            profiles.Add(new CharacterProfile("Gigan", ggTexture, new Vector2(w * 0.73f, h * 0.5f), true));

            ppOrigin = new Vector2(playerProfileTexture.Width / 2, playerProfileTexture.Height / 2);
            p1ProfilePos = new Vector2(w * 0.30f, h * 0.80f);
            p2ProfilePos = new Vector2(w * 0.75f, h * 0.80f);

            p1NamePos = new Vector2(w * 0.315f, h * 0.825f);
            p2NamePos = new Vector2(w * 0.765f, h * 0.825f);

            // Input commands for player 1
            InputHandler.Instance.ClearBindings();
            InputHandler.Instance.AddButtonDownCommand(Keys.A, new ChangeSelectionCommand(-1, this, true));
            InputHandler.Instance.AddButtonDownCommand(Keys.D, new ChangeSelectionCommand(1, this, true));
            InputHandler.Instance.AddButtonDownCommand(Keys.LeftControl, new ConfirmSelectionCommand(this, true));
            // Input commands for player 2
            InputHandler.Instance.AddButtonDownCommand(Keys.Left, new ChangeSelectionCommand(-1, this, false));
            InputHandler.Instance.AddButtonDownCommand(Keys.Right, new ChangeSelectionCommand(1, this, false));
            InputHandler.Instance.AddButtonDownCommand(Keys.Enter, new ConfirmSelectionCommand(this, false));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw title text
            Vector2 promOrigin = promptFont.MeasureString(promptText) / 2;
            spriteBatch.DrawString(promptFont, promptText, promptPosition, Color.OrangeRed, 0, promOrigin, 1, SpriteEffects.None, 1);

            //player profile
            spriteBatch.Draw(playerProfileTexture, p1ProfilePos, null, Color.White, 0, ppOrigin, 1.4f, SpriteEffects.None, 1);
            spriteBatch.Draw(playerProfileTexture, p2ProfilePos, null, Color.White, 0, ppOrigin, 1.4f, SpriteEffects.None, 1);

            Vector2 p1NameOrigin = playerProfileFont.MeasureString(p1Name) / 2;
            Vector2 p2NameOrigin = playerProfileFont.MeasureString(p2Name) / 2;
            spriteBatch.DrawString(playerProfileFont, p1Name, p1NamePos, Color.White, 0, p1NameOrigin, 1, SpriteEffects.None, 1);
            spriteBatch.DrawString(playerProfileFont, p2Name, p2NamePos, Color.White, 0, p1NameOrigin, 1, SpriteEffects.None, 1);


            //character profiles + highlights
            for (int i = 0; i < profiles.Count; i++)
            {
                CharacterProfile profile = profiles[i];
                SpriteEffects effect = profile.FlipTexture ? SpriteEffects.FlipHorizontally : SpriteEffects.None; // checks if the flip bool is set to true. If yes then it will flip horizontally, but if no then nothing will happen
                int thickness = 10;

                // The highlight for P1
                if (i == selectedIndexP1 && !player1Confirmed)
                {
                    
                    Rectangle higlightRec1 = new Rectangle(
                        (int)(profile.Position.X - profile.Origin.X - thickness),
                        (int)(profile.Position.Y - profile.Origin.Y - thickness),
                        profile.Texture.Width + thickness * 2,
                        profile.Texture.Height + thickness * 2
                        );
                    spriteBatch.Draw(highlightTexture, higlightRec1, Color.Red);
                }
                // The highlight for P2
                if (i == selectedIndexP2 && !player2Confirmed)
                {
                    Rectangle higlightRec2 = new Rectangle(
                        (int)(profile.Position.X - profile.Origin.X - thickness),
                        (int)(profile.Position.Y - profile.Origin.Y - thickness),
                        profile.Texture.Width + thickness * 2,
                        profile.Texture.Height + thickness * 2
                        );
                    spriteBatch.Draw(highlightTexture, higlightRec2, Color.Blue);
                }
                
                // Draw the profile pictures
                spriteBatch.Draw(profile.Texture, profile.Position, null, Color.White, 0, profile.Origin, 1, effect, 1);
            }
            
           
        }        
        
        public void Update(GameTime gameTime)
        {
            InputHandler.Instance.Execute();

            if(player1Confirmed && player2Confirmed)
            {
                game.SelectedCharacterNameP1 = profiles[selectedIndexP1].Name;
                game.SelectedCharacterNameP2 = profiles[selectedIndexP2].Name;
                game.ChangeGameState(new BattleState(game));
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Q))
            {
                game.ChangeGameState(new BattleState(game));
            }
        }

        public void Exit()
        {

        }

        public void ChangeSelection(int direction, bool isPlayer1)
        {
            if(isPlayer1 && !player1Confirmed)
            {
                selectedIndexP1 += direction;
                if (selectedIndexP1 < 0) { selectedIndexP1 = profiles.Count - 1; }
                if (selectedIndexP1 >= profiles.Count) { selectedIndexP1 = 0; }
            }
            
            if(!isPlayer1 && !player2Confirmed)
            {
                selectedIndexP2 += direction;
                if (selectedIndexP2 < 0) { selectedIndexP2 = profiles.Count - 1; }
                if (selectedIndexP2 >= profiles.Count) { selectedIndexP2 = 0; }
            }
            
        } 
        public void ConfirmSelection(bool isPlayer1)
        {
            if (isPlayer1)
            {
                player1Confirmed = true;
            }
            else
            {
                player2Confirmed = true;
            }
        }
        
    }
}

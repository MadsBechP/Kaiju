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
    public class ProfileState : IGameState, ISelectable
    {
        private GameWorld game;
        private bool isPlayer1;
        private SpriteFont textFont;
        private List<string> profiles;
        private int selectedIndex = 0;
        public Color DefaultBackgroundColor => Color.Beige;

        public ProfileState(GameWorld game, bool isPlayer1)
        {
            this.game = game;
            this.isPlayer1 = isPlayer1;
            textFont = game.Content.Load<SpriteFont>("promptFont");

            profiles = DatabaseManager.Instance.ListAllPlayerNames();

            InputHandler.Instance.ClearBindings();
            if (isPlayer1)
            {
                //InputHandler.Instance.AddButtonDownCommand(Keys.W, ChangeSelectionCommand)
            }
        }

        public void Update(GameTime gameTime)
        {
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // tegn listen af profiler ud
        }

        public void Exit()
        {
            
        }

        public void ChangeSelection(int direction, bool isPlayer1)
        {
            throw new NotImplementedException();
        }

        public void ConfirmSelection(bool isPlayer1)
        {
            throw new NotImplementedException();
        }
    }
}

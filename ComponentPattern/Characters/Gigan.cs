using Kaiju.Command;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Kaiju.ComponentPattern.Characters
{
    /// <summary>
    /// Character class for Gigan
    /// Used to setup animations, input and character specific details for cleaner code
    /// Inherits from Character
    /// Made by: Mads
    /// </summary>
    public class Gigan : Character
    {
        /// <summary>
        /// Constuctor 
        /// </summary>
        /// <param name="gameObject">specifies which gameobject it is tied to</param>
        public Gigan(GameObject gameObject) : base(gameObject)
        {
        }

        /// <summary>
        /// Sets up sprites, animations and controls
        /// </summary>
        public override void Start()
        {
            base.Start();
            Player player = (Player)gameObject.GetComponent<Player>();
            sr.SetSprite("GG_Sprites\\GG_Walk\\GG_Walk_01");
            gameObject.Transform.Scale = new Vector2(3f, 3f);

            ani.AddAnimation(GameWorld.Instance.BuildAnimation("Idle", new string[] { "GG_Sprites\\GG_Walk\\GG_Walk_01" }, 5, false));
            ani.AddAnimation(GameWorld.Instance.BuildAnimation("Walk", new string[] {
                "GG_Sprites\\GG_Walk\\GG_Walk_01",
                "GG_Sprites\\GG_Walk\\GG_Walk_02",
                "GG_Sprites\\GG_Walk\\GG_Walk_03",
                "GG_Sprites\\GG_Walk\\GG_Walk_04"}, 5, true));
            ani.AddAnimation(GameWorld.Instance.BuildAnimation("Hit", new string[] {
                "GG_Sprites\\GG_Big_Hit\\GG_Big_Hit_01",
                "GG_Sprites\\GG_Big_Hit\\GG_Big_Hit_02"}, 2, false));
            ani.AddAnimation(GameWorld.Instance.BuildAnimation("LPunch", new string[] {
                "GG_Sprites\\GG_Punch_L\\GG_Punch_L_01",
                "GG_Sprites\\GG_Punch_L\\GG_Punch_L_02"}, 5, false));
            ani.AddAnimation(GameWorld.Instance.BuildAnimation("RPunch", new string[] {
                "GG_Sprites\\GG_Punch_R\\GG_Punch_R_01",
                "GG_Sprites\\GG_Punch_R\\GG_Punch_R_02"}, 5, false));
            ani.AddAnimation(GameWorld.Instance.BuildAnimation("Kick", new string[] {
                "GG_Sprites\\GG_Kick\\GG_Kick_01",
                "GG_Sprites\\GG_Kick\\GG_Kick_02"}, 5, false));
            ani.AddAnimation(GameWorld.Instance.BuildAnimation("Beam", new string[] {
                "GG_Sprites\\GG_Beam\\GG_Beam_01",
                "GG_Sprites\\GG_Beam\\GG_Beam_02",
                "GG_Sprites\\GG_Beam\\GG_Beam_03",
                "GG_Sprites\\GG_Beam\\GG_Beam_04",
                "GG_Sprites\\GG_Beam\\GG_Beam_05", }, 8, false));
            ani.AddAnimation(GameWorld.Instance.BuildAnimation("SawStill", new string[] {
                "GG_Sprites\\GG_Saw_Still\\GG_Saw_Still_01",
                "GG_Sprites\\GG_Saw_Still\\GG_Saw_Still_02",
                "GG_Sprites\\GG_Saw_Still\\GG_Saw_Still_03", }, 5, false));
            ani.AddAnimation(GameWorld.Instance.BuildAnimation("SawCont", new string[] {
                "GG_Sprites\\GG_Saw_Still\\GG_Saw_Still_02",
                "GG_Sprites\\GG_Saw_Still\\GG_Saw_Still_03", }, 5, false));
            ani.AddAnimation(GameWorld.Instance.BuildAnimation("SawMove", new string[] {
                "GG_Sprites\\GG_Saw_Move\\GG_Saw_Move_01",
                "GG_Sprites\\GG_Saw_Move\\GG_Saw_Move_02",
                "GG_Sprites\\GG_Saw_Move\\GG_Saw_Move_03",
                "GG_Sprites\\GG_Saw_Move\\GG_Saw_Move_04",}, 5, true));
            ani.AddAnimation(GameWorld.Instance.BuildAnimation("Block", new string[] {
                "GG_Sprites\\GG_Saw_Still\\GG_Saw_Still_03" }, 5, true));


            if (player != null)
            {
                if (player.InputType == InputType.Keyboard)
                {
                    if (gameObject == GameWorld.Instance.player1Go)
                    {
                        InputHandler.Instance.AddUpdateCommand(Keys.A, new MoveCommand(GameWorld.Instance.player1, new Vector2(-1, 0)));
                        InputHandler.Instance.AddUpdateCommand(Keys.D, new MoveCommand(GameWorld.Instance.player1, new Vector2(1, 0)));
                        InputHandler.Instance.AddButtonDownCommand(Keys.W, new JumpCommand(GameWorld.Instance.player1));
                        InputHandler.Instance.AddButtonDownCommand(Keys.F, new AttackCommand(GameWorld.Instance.player1, 4));
                        InputHandler.Instance.AddButtonDownCommand(Keys.G, new AttackCommand(GameWorld.Instance.player1, 5));
                        InputHandler.Instance.AddButtonDownCommand(Keys.H, new AttackCommand(GameWorld.Instance.player1, 6));
                        InputHandler.Instance.AddUpdateCommand(Keys.Q, new SpecialCommand(GameWorld.Instance.player1, 2));
                        InputHandler.Instance.AddUpdateCommand(Keys.LeftShift, new BlockCommand(GameWorld.Instance.player1));
                    }
                    else if (gameObject == GameWorld.Instance.player2Go)
                    {
                        InputHandler.Instance.AddUpdateCommand(Keys.Left, new MoveCommand(GameWorld.Instance.player2, new Vector2(-1, 0)));
                        InputHandler.Instance.AddUpdateCommand(Keys.Right, new MoveCommand(GameWorld.Instance.player2, new Vector2(1, 0)));
                        InputHandler.Instance.AddButtonDownCommand(Keys.Up, new JumpCommand(GameWorld.Instance.player2));
                        InputHandler.Instance.AddButtonDownCommand(Keys.OemComma, new AttackCommand(GameWorld.Instance.player2, 4));
                        InputHandler.Instance.AddButtonDownCommand(Keys.OemPeriod, new AttackCommand(GameWorld.Instance.player2, 5));
                        InputHandler.Instance.AddButtonDownCommand(Keys.OemMinus, new AttackCommand(GameWorld.Instance.player2, 6));
                        InputHandler.Instance.AddUpdateCommand(Keys.RightShift, new SpecialCommand(GameWorld.Instance.player2, 2));
                        InputHandler.Instance.AddUpdateCommand(Keys.RightControl, new BlockCommand(GameWorld.Instance.player2));
                    }
                }
                else if (player.InputType == InputType.GamePad)
                {
                    var index = player.GamePadIndex;
                    InputHandler.Instance.AddUpdateCommand(index, Buttons.LeftThumbstickLeft, new MoveCommand(player, new Vector2(-1, 0)));
                    InputHandler.Instance.AddUpdateCommand(index, Buttons.LeftThumbstickRight, new MoveCommand(player, new Vector2(1, 0)));
                    InputHandler.Instance.AddButtonDownCommand(index, Buttons.A, new JumpCommand(player));
                    InputHandler.Instance.AddButtonDownCommand(index, Buttons.X, new AttackCommand(player, 4));
                    InputHandler.Instance.AddButtonDownCommand(index, Buttons.B, new AttackCommand(player, 5));
                    InputHandler.Instance.AddButtonDownCommand(index, Buttons.Y, new AttackCommand(player, 6));
                    InputHandler.Instance.AddUpdateCommand(index, Buttons.LeftTrigger, new SpecialCommand(player, 2));
                    InputHandler.Instance.AddUpdateCommand(index, Buttons.RightTrigger, new BlockCommand(player));
                }
            }
        }

        /// <summary>
        /// Sets the horizontal facing direction of the characters sprite
        /// Godzillas and Gigans sprites face different directions so this is used to have them flip opposite of each other for example when spriteeffect.fliphorizontally is used
        /// </summary>
        /// <param name="x">if true it faces to the right</param>
        public override void FaceRight(bool x)
        {
            sr.SetFlipHorizontal(x);
        }
    }

}

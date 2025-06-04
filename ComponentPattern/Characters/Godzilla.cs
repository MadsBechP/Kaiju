using Kaiju.Command;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Kaiju.ComponentPattern.Characters
{
    /// <summary>
    /// Character class for Godzilla
    /// Used to setup animations, input and character specific details for cleaner code
    /// Inherits from Character
    /// Made by: Mads
    /// </summary>
    public class Godzilla : Character
    {
        /// <summary>
        /// Constuctor 
        /// </summary>
        /// <param name="gameObject">specifies which gameobject it is tied to</param>
        public Godzilla(GameObject gameObject) : base(gameObject)
        {
        }

        /// <summary>
        /// Sets up sprites, animations and controls
        /// </summary>
        public override void Start()
        {
            base.Start();
            Player player = (Player)gameObject.GetComponent<Player>();
            sr.SetSprite("GZ_Sprites\\GZ_Walk\\GZ_Walk_01");
            gameObject.Transform.Scale = new Vector2(3f, 3f);

            ani.AddAnimation(GameWorld.Instance.BuildAnimation("Idle", new string[] { "GZ_Sprites\\GZ_Walk\\GZ_Walk_01" }, 5, false));
            ani.AddAnimation(GameWorld.Instance.BuildAnimation("Walk", new string[] {
                "GZ_Sprites\\GZ_Walk\\GZ_Walk_01",
                "GZ_Sprites\\GZ_Walk\\GZ_Walk_02",
                "GZ_Sprites\\GZ_Walk\\GZ_Walk_03",
                "GZ_Sprites\\GZ_Walk\\GZ_Walk_04",
                "GZ_Sprites\\GZ_Walk\\GZ_Walk_05",
                "GZ_Sprites\\GZ_Walk\\GZ_Walk_06",
                "GZ_Sprites\\GZ_Walk\\GZ_Walk_07",
                "GZ_Sprites\\GZ_Walk\\GZ_Walk_08"}, 5, true));
            ani.AddAnimation(GameWorld.Instance.BuildAnimation("Hit", new string[] {
                "GZ_Sprites\\GZ_Hit\\GZ_Hit_01",
                "GZ_Sprites\\GZ_Hit\\GZ_Hit_02",
                "GZ_Sprites\\GZ_Hit\\GZ_Hit_03",
                "GZ_Sprites\\GZ_Hit\\GZ_Hit_04"}, 4, false));
            ani.AddAnimation(GameWorld.Instance.BuildAnimation("LPunch", new string[] {
                "GZ_Sprites\\GZ_Punch_L\\GZ_Punch_L_01",
                "GZ_Sprites\\GZ_Punch_L\\GZ_Punch_L_02"}, 5, false));
            ani.AddAnimation(GameWorld.Instance.BuildAnimation("RPunch", new string[] {
                "GZ_Sprites\\GZ_Punch_R\\GZ_Punch_R_01",
                "GZ_Sprites\\GZ_Punch_R\\GZ_Punch_R_02"}, 5, false));
            ani.AddAnimation(GameWorld.Instance.BuildAnimation("LKick", new string[] {
                "GZ_Sprites\\GZ_Kick_L\\GZ_Kick_L_01",
                "GZ_Sprites\\GZ_Kick_L\\GZ_Kick_L_02"}, 5, false));
            ani.AddAnimation(GameWorld.Instance.BuildAnimation("RKick", new string[] {
                "GZ_Sprites\\GZ_Kick_R\\GZ_Kick_R_01",
                "GZ_Sprites\\GZ_Kick_R\\GZ_Kick_R_02"}, 5, false));
            ani.AddAnimation(GameWorld.Instance.BuildAnimation("TailSwipe", new string[] {
                "GZ_Sprites\\GZ_Crouch\\GZ_Crouch_01",
                "GZ_Sprites\\GZ_Crouch\\GZ_Crouch_02",
                "GZ_Sprites\\GZ_Crouch\\GZ_Crouch_03",
                "GZ_Sprites\\GZ_Crouch\\GZ_Crouch_04",
                "GZ_Sprites\\GZ_Crouch\\GZ_Crouch_03",
                "GZ_Sprites\\GZ_Crouch\\GZ_Crouch_02",
                "GZ_Sprites\\GZ_Crouch\\GZ_Crouch_01"}, 5, false));
            ani.AddAnimation(GameWorld.Instance.BuildAnimation("Special", new string[] {
                "GZ_Sprites\\GZ_Breath\\GZ_Breath_01",
                "GZ_Sprites\\GZ_Breath\\GZ_Breath_02",
                "GZ_Sprites\\GZ_Breath\\GZ_Breath_03",
                "GZ_Sprites\\GZ_Breath\\GZ_Breath_04",
                "GZ_Sprites\\GZ_Breath\\GZ_Breath_05",
                "GZ_Sprites\\GZ_Breath\\GZ_Breath_06",
                "GZ_Sprites\\GZ_Breath\\GZ_Breath_05",
                "GZ_Sprites\\GZ_Breath\\GZ_Breath_06",
                "GZ_Sprites\\GZ_Breath\\GZ_Breath_05",
                "GZ_Sprites\\GZ_Breath\\GZ_Breath_06",
                "GZ_Sprites\\GZ_Breath\\GZ_Breath_05",
                "GZ_Sprites\\GZ_Breath\\GZ_Breath_06",}, 5, false));
            ani.AddAnimation(GameWorld.Instance.BuildAnimation("Block", new string[] {
                "GZ_Sprites\\GZ_Crouch\\GZ_Crouch_02"}, 5, true));
        }

        /// <summary>
        /// Sets the controls for the character based on player and inputtype
        /// </summary>
        public override void SetControls()
        {
            Player player = (Player)gameObject.GetComponent<Player>();

            if (player.InputType == InputType.Keyboard)
            {
                if (this.gameObject == GameWorld.Instance.player1Go)
                {
                    InputHandler.Instance.AddUpdateCommand(Keys.A, new MoveCommand(GameWorld.Instance.player1, new Vector2(-1, 0)));
                    InputHandler.Instance.AddUpdateCommand(Keys.D, new MoveCommand(GameWorld.Instance.player1, new Vector2(1, 0)));
                    InputHandler.Instance.AddButtonDownCommand(Keys.W, new JumpCommand(GameWorld.Instance.player1));
                    InputHandler.Instance.AddButtonDownCommand(Keys.F, new AttackCommand(GameWorld.Instance.player1, 1));
                    InputHandler.Instance.AddButtonDownCommand(Keys.G, new AttackCommand(GameWorld.Instance.player1, 2));
                    InputHandler.Instance.AddButtonDownCommand(Keys.H, new AttackCommand(GameWorld.Instance.player1, 3));
                    InputHandler.Instance.AddUpdateCommand(Keys.Q, new SpecialCommand(GameWorld.Instance.player1, 1));
                    InputHandler.Instance.AddUpdateCommand(Keys.LeftShift, new BlockCommand(GameWorld.Instance.player1));

                }
                else if (this.gameObject == GameWorld.Instance.player2Go)
                {
                    InputHandler.Instance.AddUpdateCommand(Keys.Left, new MoveCommand(GameWorld.Instance.player2, new Vector2(-1, 0)));
                    InputHandler.Instance.AddUpdateCommand(Keys.Right, new MoveCommand(GameWorld.Instance.player2, new Vector2(1, 0)));
                    InputHandler.Instance.AddButtonDownCommand(Keys.Up, new JumpCommand(GameWorld.Instance.player2));
                    InputHandler.Instance.AddButtonDownCommand(Keys.OemComma, new AttackCommand(GameWorld.Instance.player2, 1));
                    InputHandler.Instance.AddButtonDownCommand(Keys.OemPeriod, new AttackCommand(GameWorld.Instance.player2, 2));
                    InputHandler.Instance.AddButtonDownCommand(Keys.OemMinus, new AttackCommand(GameWorld.Instance.player2, 3));
                    InputHandler.Instance.AddUpdateCommand(Keys.RightShift, new SpecialCommand(GameWorld.Instance.player2, 1));
                    InputHandler.Instance.AddUpdateCommand(Keys.RightControl, new BlockCommand(GameWorld.Instance.player2));
                }
            }
            else if (player.InputType == InputType.GamePad)
            {
                var index = player.GamePadIndex;
                InputHandler.Instance.AddUpdateCommand(index, Buttons.LeftThumbstickLeft, new MoveCommand(player, new Vector2(-1, 0)));
                InputHandler.Instance.AddUpdateCommand(index, Buttons.LeftThumbstickRight, new MoveCommand(player, new Vector2(1, 0)));
                InputHandler.Instance.AddButtonDownCommand(index, Buttons.A, new JumpCommand(player));
                InputHandler.Instance.AddButtonDownCommand(index, Buttons.X, new AttackCommand(player, 1));
                InputHandler.Instance.AddButtonDownCommand(index, Buttons.B, new AttackCommand(player, 2));
                InputHandler.Instance.AddButtonDownCommand(index, Buttons.Y, new AttackCommand(player, 3));
                InputHandler.Instance.AddUpdateCommand(index, Buttons.LeftTrigger, new SpecialCommand(player, 1));
                InputHandler.Instance.AddUpdateCommand(index, Buttons.RightTrigger, new BlockCommand(player));
            }
        }

        /// <summary>
        /// Sets the horizontal facing direction of the characters sprite
        /// Godzillas and Gigans sprites face different directions so this is used to have them flip opposite of each other for example when spriteeffect.fliphorizontally is used
        /// </summary>
        /// <param name="x">if true it faces to the right</param>
        public override void FaceRight(bool x)
        {
            sr.SetFlipHorizontal(!x);
        }
    }
}

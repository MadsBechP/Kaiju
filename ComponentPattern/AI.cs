using Kaiju.Command;
using Kaiju.State;
using Kaiju.State.AIStates;
using Microsoft.Xna.Framework;
using System.Windows.Forms;

namespace Kaiju.ComponentPattern
{
    public class AI : Player
    {
        private IState<AI> currentState;




        public ICommand left;
        public ICommand right;
        public ICommand jump;
        public ICommand punch;
        public ICommand kick;
        public ICommand swipebeam;
        public ICommand special;
        public ICommand shield;



        public Player opponent { get; private set; }



        public AI(GameObject gameObject) : base(gameObject)
        {
        }

        public override void Start()
        {

            base.Start();
            left = new MoveCommand(this, new Vector2(-1, 0));
            right = new MoveCommand(this, new Vector2(1, 0));
            jump = new JumpCommand(this);
            punch = new AttackCommand(this, 1);

            gameObject.Transform.Scale = new Vector2(2f, 2f);


            if (this.gameObject == GameWorld.Instance.player1Go)
            {
                opponent = GameWorld.Instance.player2;

            }
            if (this.gameObject == GameWorld.Instance.player2Go)
            {
                opponent = GameWorld.Instance.player1;
            }
            ChangeGameState(new IdleState());
            speed = 300;
        }
        public override void Update()
        {
            base.Update();
            currentState.Execute();

            Rectangle overVoidCheckLeft = new Rectangle(stageCollider.CollisionBox.X - 100, stageCollider.CollisionBox.Y, stageCollider.CollisionBox.Width, stageCollider.CollisionBox.Height + 1000);
            Rectangle overVoidCheckRight = new Rectangle(stageCollider.CollisionBox.X + 100, stageCollider.CollisionBox.Y, stageCollider.CollisionBox.Width, stageCollider.CollisionBox.Height + 1000);
            if (!GameWorld.Instance.CheckCollision(overVoidCheckLeft) || !GameWorld.Instance.CheckCollision(overVoidCheckRight))
            {
                ChangeGameState(new RecoveryState());
            }

        }
        public void ChangeGameState(IState<AI> newState)
        {
            if (currentState != null)
            {
                currentState.Exit();
            }
            currentState = newState;
            currentState.Enter(this);
        }
    }
}

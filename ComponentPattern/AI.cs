using Kaiju.Command;
using Kaiju.ComponentPattern.Characters;
using Kaiju.State;
using Kaiju.State.AIStates;
using Microsoft.Xna.Framework;
using System.Threading;
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

        public float atkCooldown;



        public Player opponent { get; private set; }



        public AI(GameObject gameObject) : base(gameObject)
        {
        }

        public override void Awake()
        {
            base.Awake();
            left = new MoveCommand(this, new Vector2(-1, 0));
            right = new MoveCommand(this, new Vector2(1, 0));
            jump = new JumpCommand(this);
            if (chr is Godzilla)
            {
                punch = new AttackCommand(this, 2);
                kick = new AttackCommand(this, 2);
                swipebeam = new AttackCommand(this, 3);
                special = new SpecialCommand(this, 1);

            }
            else if (chr is Gigan)
            {
                punch = new AttackCommand(this, 4);
                kick = new AttackCommand(this, 5);
                swipebeam = new AttackCommand(this, 6);
                special = new SpecialCommand(this, 2);
            }
                kick = new AttackCommand(this, 1);

            gameObject.Transform.Scale = new Vector2(2f, 2f);


            if (gameObject == GameWorld.Instance.player1Go)
            {
                opponent = GameWorld.Instance.player2;

            }
            if (gameObject == GameWorld.Instance.player2Go)
            {
                opponent = GameWorld.Instance.player1;
            }
            ChangeGameState(new IdleState());
            speed = 300;




        }
        public override void Start()
        {
            base.Start();
            Thread aiLogic = new(AILoop);
            aiLogic.IsBackground = true;
            aiLogic.Start();

        }
        public override void Update()
        {
            base.Update();
            currentState.Execute();

            if (atkCooldown > 0)
            {
                atkCooldown -= GameWorld.Instance.DeltaTime;
            }
        }

        private void AILoop()
        {
            // checks if the ai is at the edge 5 times every second. if it is, goes to recoverystate 
            while (true)
            {
                Thread.Sleep(200);

                Rectangle overVoidCheckLeft = new Rectangle(stageCollider.CollisionBox.X - 100, stageCollider.CollisionBox.Y, 1, stageCollider.CollisionBox.Height + 1000);
                Rectangle overVoidCheckRight = new Rectangle(stageCollider.CollisionBox.X + 100, stageCollider.CollisionBox.Y, 1, stageCollider.CollisionBox.Height + 1000);
                if (!GameWorld.Instance.CheckCollision(overVoidCheckLeft) || !GameWorld.Instance.CheckCollision(overVoidCheckRight))
                {
                    ChangeGameState(new RecoveryState());
                }



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

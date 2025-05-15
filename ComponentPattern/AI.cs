using Kaiju.Command;
using Kaiju.Observer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;

namespace Kaiju.ComponentPattern
{
    public class AI : Player, ISubject
    {
        ICommand left;
        ICommand right;
        ICommand jump;
        Player opponent;

        Vector2 Pos { get {return gameObject.Transform.Position; } }
        Vector2 OPos { get { return opponent.gameObject.Transform.Position; } }

        private List<IObserver> observers = new List<IObserver>();
        public int Damage { get; private set; }

        public AI(GameObject gameObject) : base(gameObject)
        {
            
        }

        public override void Start()
        {

            base.Start();
            left = new MoveCommand(this, new Vector2(-1, 0));
            right = new MoveCommand(this, new Vector2(1, 0));
            jump = new JumpCommand(this);

            gameObject.Transform.Scale = new Vector2(2f, 2f);


            if (this.gameObject == GameWorld.Instance.player1Go)
            {
                opponent = GameWorld.Instance.player2;

            }
            if (this.gameObject == GameWorld.Instance.player2Go)
            {
                opponent = GameWorld.Instance.player1;
            }
        }
        public override void Update()
        {
            base.Update();
            if (Pos.X < OPos.X - 200)
            {
                right.Execute();
            }
            else if (Pos.X > OPos.X + 200)
            {
                left.Execute();
            }
            if (Pos.Y > OPos.Y + 300)
            {
                jump.Execute();
            }
        }
        public override void OnCollisionEnter(Collider collider)
        {
            if (collider.isAttack)
            {
                gameObject.Transform.Position = new Vector2(GameWorld.Instance.Graphics.PreferredBackBufferWidth/2, GameWorld.Instance.Graphics.PreferredBackBufferHeight / 2);
                TakeDamage(5);
            }


        }

        public void TakeDamage(int amount)
        {
            Damage += amount;
            Debug.WriteLine($"{this} took damage! new value {Damage}"); // tjek om TakeDamage faktisk bliver kaldt
            Notify();
        }

        public void Attach(IObserver observer)
        {
            observers.Add(observer);
        }

        public void Detach(IObserver observer)
        {
            observers.Remove(observer);
        }

        public void Notify()
        {
            foreach (var observer in observers)
            {
                observer.Updated();
            }
        }
    }
}

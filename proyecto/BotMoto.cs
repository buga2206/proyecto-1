using System;
using System.Timers;

namespace proyecto
{
    public class BotMoto : Moto
    {
        private static readonly Random rand = new Random();

        public BotMoto(Node startNode, int trailValue, int velocidad, int tamañoEstela, int trailHeadValue)
            : base(startNode, trailValue, velocidad, tamañoEstela, trailHeadValue)
        {
        }

        protected override void OnMoveTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            ChangeDirection(GetRandomDirection());
            base.OnMoveTimerElapsed(sender, e);
        }

        private Direction GetRandomDirection()
        {
            int randomValue = rand.Next(100);

            if (randomValue < 25)
            {
                return Direction.Up;
            }
            else if (randomValue < 50)
            {
                return Direction.Down;
            }
            else if (randomValue < 75)
            {
                return Direction.Left;
            }
            else
            {
                return Direction.Right;
            }
        }

    }
}
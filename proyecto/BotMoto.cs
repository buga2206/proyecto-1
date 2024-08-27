using System;
using System.Timers;

namespace proyecto
{
    public class BotMoto : Moto
    {
        private readonly Grid gameGrid;
        private readonly double decisionThreshold;

        public BotMoto(Node startNode, int trailValue, Grid grid, double decisionThreshold, int velocidad, int tamañoEstela)
            : base(startNode, trailValue, velocidad, tamañoEstela)
        {
            this.gameGrid = grid;
            this.decisionThreshold = decisionThreshold;
        }

        protected override void OnMoveTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            Random rand = new Random();
            if (rand.NextDouble() < decisionThreshold)
            {
                ChangeDirection(RandomDirection());
            }

            base.OnMoveTimerElapsed(sender, e);
        }

        private Direction RandomDirection()
        {
            Random rand = new Random();
            Array directions = Enum.GetValues(typeof(Direction));
            return (Direction)directions.GetValue(rand.Next(directions.Length))!;
        }
    }
}

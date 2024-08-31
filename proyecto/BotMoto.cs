using System;
using System.Timers;

namespace proyecto
{
    public class BotMoto : Moto
    {
        private readonly Grid gameGrid;
        private readonly double decisionThreshold;
        private Random rand;

        public BotMoto(Node startNode, int trailValue, Grid grid, double decisionThreshold, int velocidad, int tamañoEstela, int trailHeadValue)
            : base(startNode, trailValue, velocidad, tamañoEstela, trailHeadValue)
        {
            this.gameGrid = grid;
            this.decisionThreshold = decisionThreshold;
            this.rand = new Random();
        }

        protected override void OnMoveTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            // Decisión de cambio de dirección
            if (rand.NextDouble() < decisionThreshold)
            {
                ChangeDirection(GetBestDirection());
            }

            base.OnMoveTimerElapsed(sender, e);
        }

        private Direction GetBestDirection()
        {
            // Aquí podrías implementar una lógica más avanzada para determinar la mejor dirección,
            // como buscar al jugador más cercano o evitar obstáculos.
            return RandomDirection();
        }

        private Direction RandomDirection()
        {
            Array directions = Enum.GetValues(typeof(Direction));
            return (Direction)directions.GetValue(rand.Next(directions.Length))!;
        }
    }
}

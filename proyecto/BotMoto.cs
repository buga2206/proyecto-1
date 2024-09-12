using System;
using System.Timers;

namespace proyecto
{
    public class BotMoto : Moto
    {
        private readonly Grid gameGrid;
        private readonly double decisionThreshold;
        private static readonly Random rand = new Random();  // Instancia única de Random

        public BotMoto(Node startNode, int trailValue, Grid grid, double decisionThreshold, int velocidad, int tamañoEstela, int trailHeadValue)
            : base(startNode, trailValue, velocidad, tamañoEstela, trailHeadValue)
        {
            this.gameGrid = grid;
            this.decisionThreshold = decisionThreshold;
        }

        // Manejador del temporizador para movimiento
        protected override void OnMoveTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            // Decidir si cambiar de dirección basado en un número aleatorio
            ChangeDirection(GetRandomDirection());

            // Llamar al método de movimiento de la clase base
            base.OnMoveTimerElapsed(sender, e);
        }

        // Escoge una dirección aleatoria simple
        private Direction GetRandomDirection()
        {
            int randomValue = rand.Next(100); // Genera un número aleatorio entre 0 y 99

            // Basado en el valor de randomValue, elegimos una dirección
            if (randomValue < 25)
            {
                return Direction.Up;   // 25% de probabilidad de ir hacia arriba
            }
            else if (randomValue < 50)
            {
                return Direction.Down; // 25% de probabilidad de ir hacia abajo
            }
            else if (randomValue < 75)
            {
                return Direction.Left; // 25% de probabilidad de ir a la izquierda
            }
            else
            {
                return Direction.Right; // 25% de probabilidad de ir a la derecha
            }
        }

    }
}
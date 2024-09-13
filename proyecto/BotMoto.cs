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
            ChangeDirection(GetRandomDirection());  // Cambia la dirección actual de la moto por una dirección aleatoria

            base.OnMoveTimerElapsed(sender, e); // Llama al método base para procesar el movimiento según la nueva dirección
        }

        private Direction GetRandomDirection()
        {
            int randomValue = rand.Next(100);

            // 25% de probabilidad de moverse hacia arriba
            if (randomValue < 25)
            {
                return Direction.Up;
            }
            // 25% de probabilidad de moverse hacia abajo
            else if (randomValue < 50)
            {
                return Direction.Down;
            }
            // 25% de probabilidad de moverse hacia la izquierda
            else if (randomValue < 75)
            {
                return Direction.Left;
            }
            // 25% de probabilidad de moverse hacia la derecha
            else
            {
                return Direction.Right;
            }
        }
    }
}
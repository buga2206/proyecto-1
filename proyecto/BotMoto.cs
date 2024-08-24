namespace proyecto
{
    public class BotMoto : Moto
    {
        private Random random;
        private Grid grid; // Necesitamos una referencia al grid para eliminar el bot
        private double turnProbability; // Probabilidad de giro

        public BotMoto(Node startNode, int trailValue, Grid grid, double turnProbability = 0.000001) : base(startNode, trailValue)
        {
            this.grid = grid;
            this.turnProbability = turnProbability; // Inicializar la probabilidad de giro
            random = new Random();
        }

        public bool MoveBot()
        {
            // Intentar mover el bot
            TryRandomTurn();

            // Mover el bot utilizando la lógica ya existente en Moto
            if (!Move())
            {
                // Si el movimiento falla (colisión), devolver false para indicar destrucción
                return false;
            }

            return true;
        }

        private void TryRandomTurn()
        {
            // Intentar girar solo si se cumple la probabilidad
            if (random.NextDouble() < turnProbability)
            {
                // Obtener todas las direcciones posibles
                List<Direction> possibleDirections = new List<Direction>();

                if (CanMoveTo(CurrentNode.Up))
                {
                    possibleDirections.Add(Direction.Up);
                }
                if (CanMoveTo(CurrentNode.Down))
                {
                    possibleDirections.Add(Direction.Down);
                }
                if (CanMoveTo(CurrentNode.Left))
                {
                    possibleDirections.Add(Direction.Left);
                }
                if (CanMoveTo(CurrentNode.Right))
                {
                    possibleDirections.Add(Direction.Right);
                }

                // Si hay direcciones posibles, elegir una aleatoriamente
                if (possibleDirections.Count > 0)
                {
                    Direction newDirection = possibleDirections[random.Next(possibleDirections.Count)];
                    ChangeDirection(newDirection); // Usamos el método de la clase base
                }
            }
        }

        private bool CanMoveTo(Node? node)
        {
            // Verifica si el nodo es transitable (no es null y no tiene estela ni obstáculo)
            return node != null && node.Value == 0;
        }
    }
}

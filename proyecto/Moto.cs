namespace proyecto
{
    public class Moto
    {
        public Node CurrentNode { get; private set; } // Nodo actual en el que se encuentra la moto
        public int TrailValue { get; private set; } // Valor que la moto deja en la estela (por ejemplo, 1)
        public Direction CurrentDirection { get; private set; } // Dirección actual de la moto
        private OwnLinkedList<Node> trailNodes; // Lista de nodos que forman la estela
        private const int MaxTrailLength = 3; // Longitud máxima de la estela

        public Moto(Node startNode, int trailValue)
        {
            CurrentNode = startNode;
            TrailValue = trailValue;
            CurrentDirection = Direction.Right; // Dirección inicial
            trailNodes = new OwnLinkedList<Node>();
        }

        public void ChangeDirection(Direction newDirection)
        {
            // Cambia la dirección de la moto, evitando giros de 180 grados
            bool canChange = (CurrentDirection == Direction.Left && newDirection != Direction.Right) ||
                             (CurrentDirection == Direction.Right && newDirection != Direction.Left) ||
                             (CurrentDirection == Direction.Up && newDirection != Direction.Down) ||
                             (CurrentDirection == Direction.Down && newDirection != Direction.Up);

            if (canChange)
            {
                CurrentDirection = newDirection;
            }
        }

        public bool Move()
        {
            // Mueve la moto a la siguiente posición según la dirección actual
            Node nextNode = null;

            switch (CurrentDirection)
            {
                case Direction.Up:
                    nextNode = CurrentNode.Up;
                    break;
                case Direction.Down:
                    nextNode = CurrentNode.Down;
                    break;
                case Direction.Left:
                    nextNode = CurrentNode.Left;
                    break;
                case Direction.Right:
                    nextNode = CurrentNode.Right;
                    break;
            }

            if (nextNode == null || nextNode.Value != 0)
            {
                // La moto ha chocado con una pared o con otra estela
                return false;
            }

            // Añadir el nodo actual a la lista de la estela
            trailNodes.AddFrist(CurrentNode);

            // Si la estela ha alcanzado su longitud máxima, elimina el último segmento
            if (trailNodes.Count() > MaxTrailLength)
            {
                Node tailNode = trailNodes[0];
                tailNode.Value = 0; // Eliminar la estela visualmente del grid
                trailNodes.RemoveAt(0); // Remover el último nodo de la lista
            }

            // Marca el nodo actual como parte de la estela
            CurrentNode.Value = TrailValue;

            // Actualiza la posición actual
            CurrentNode = nextNode;

            // Marca el nuevo nodo como la cabeza de la moto
            CurrentNode.Value = 6; // Valor 6 para la cabeza de la moto

            return true;
        }
    }

    // Enumeración para manejar las direcciones
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
}

using System;
using System.Timers;
using System.Diagnostics;

namespace proyecto
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    public class Moto
    {
        public Node CurrentNode { get; private set; }
        public int TrailValue { get; private set; }
        public int trailHead;
        public Direction CurrentDirection { get; private set; }
        private OwnLinkedList<Node> trailNodes;
        private int distanceTravelled;
        private const int MaxTrailLength = 3;

        public int Combustible { get; set; }
        public int Velocidad { get; private set; }
        public int TamañoEstela { get; set; }

        protected System.Timers.Timer moveTimer;

        public Moto(Node startNode, int trailValue, int velocidad, int tamañoEstela, int trailHeadValue, int combustible = 100)
        {
            CurrentNode = startNode ?? throw new ArgumentNullException(nameof(startNode), "Start node cannot be null");
            TrailValue = trailValue;
            trailHead = trailHeadValue;
            Velocidad = velocidad;
            TamañoEstela = tamañoEstela;
            Combustible = combustible;
            CurrentDirection = Direction.Right;
            trailNodes = new OwnLinkedList<Node>();

            // Inicializar el temporizador pero no iniciarlo inmediatamente
            moveTimer = new System.Timers.Timer(velocidad);
            moveTimer.Elapsed += OnMoveTimerElapsed;
            moveTimer.AutoReset = true;
        }

        protected virtual void OnMoveTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            if (Move())
            {
                if (Application.OpenForms.Count > 0)
                {
                    Form1? form = Application.OpenForms[0] as Form1;
                    if (form != null && !form.IsDisposed)
                    {
                        form.Invoke(new Action(() => form.RenderGrid()));
                    }
                }
            }
        }

        public void ChangeDirection(Direction newDirection)
        {
            if ((CurrentDirection == Direction.Left && newDirection != Direction.Right) ||
                (CurrentDirection == Direction.Right && newDirection != Direction.Left) ||
                (CurrentDirection == Direction.Up && newDirection != Direction.Down) ||
                (CurrentDirection == Direction.Down && newDirection != Direction.Up))
            {
                CurrentDirection = newDirection;
            }
        }

        public virtual bool Move()  // Método virtual para permitir la sobrescritura en BotMoto
        {
            Node? nextNode = GetNextNode();

            if (IsCollision(nextNode))
            {
                moveTimer.Stop();
                return false;
            }

            if (nextNode != null)
            {
                HandlePower(nextNode);
                UpdateTrail();
                MoveToNextNode(nextNode);
                UpdateCombustible();
            }

            return true;
        }

        private Node? GetNextNode()
        {
            return CurrentDirection switch
            {
                Direction.Up => CurrentNode.Up,
                Direction.Down => CurrentNode.Down,
                Direction.Left => CurrentNode.Left,
                Direction.Right => CurrentNode.Right,
                _ => null,
            };
        }

        private bool IsCollision(Node? nextNode)
        {
            if (nextNode == null)
                return true; // Colisión con los bordes del grid

            int nextNodeValue = nextNode.Value;

            // Colisiones con las cabezas de los bots o jugadores
            if (nextNodeValue >= 6 && nextNodeValue <= 15)
            {
                return true;
            }

            // Colisiones con las estelas de los bots o jugadores
            if (nextNodeValue >= 10 && nextNodeValue <= 17)
            {
                return true;
            }

            // No colisiona con ítems y poderes (valores 1-5)
            if (nextNodeValue >= 1 && nextNodeValue <= 5)
            {
                return false;
            }

            // Cualquier otro valor que no esté manejado explícitamente
            return false;
        }


        private void UpdateTrail()
        {
            trailNodes.AddFirst(CurrentNode);

            if (trailNodes.Count() > MaxTrailLength)
            {
                Node tailNode = trailNodes[MaxTrailLength];
                tailNode.Value = 0;
                trailNodes.RemoveAt(MaxTrailLength);
            }
        }

        private void MoveToNextNode(Node nextNode)
        {
            CurrentNode.Value = TrailValue;
            CurrentNode = nextNode!;
            CurrentNode.Value = trailHead;
        }

        private void HandlePower(Node nextNode)
        {
            if (nextNode.Value == 0)
            {
                return;
            }

            switch (nextNode.Value)
            {
                case 1:
                    Debug.WriteLine("Gas applied");
                    break;
                case 2:
                    Debug.WriteLine("Trail growth applied");
                    break;
                case 3:
                    Debug.WriteLine("Bomb applied");
                    break;
                case 4:
                    Debug.WriteLine("Velocity applied");
                    break;
                case 5:
                    Debug.WriteLine("Shield applied");
                    break;
            }

            nextNode.Value = 0;
        }

        private void UpdateCombustible()
        {
            distanceTravelled++;
            if (distanceTravelled >= 5)
            {
                Combustible--;
                distanceTravelled = 0;
            }
        }

        public void StartTimer()
        {
            moveTimer.Start();
        }

        public void StopTimer()
        {
            moveTimer.Stop();
            moveTimer.Dispose();
        }
    }
}

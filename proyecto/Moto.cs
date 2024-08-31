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
        private int MaxTrailLength;

        public int Combustible { get; set; }
        public int Velocidad { get; private set; }
        public int TamañoEstela { get; set; }

        protected System.Timers.Timer moveTimer;
        private OwnQueueList<Item> itemsQueue;

        public Moto(Node startNode, int trailValue, int velocidad, int tamañoEstela, int trailHeadValue, int combustible = 100)
        {
            CurrentNode = startNode ?? throw new ArgumentNullException(nameof(startNode), "Start node cannot be null");
            TrailValue = trailValue;
            trailHead = trailHeadValue;
            Velocidad = velocidad;
            TamañoEstela = tamañoEstela;
            MaxTrailLength = tamañoEstela;
            Combustible = combustible;
            CurrentDirection = Direction.Right;
            trailNodes = new OwnLinkedList<Node>();
            itemsQueue = new OwnQueueList<Item>();

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

        public virtual bool Move()
        {
            if (Combustible <= 0)
            {
                StopMovement();
                return false;
            }

            Node? nextNode = GetNextNode();

            if (IsCollision(nextNode))
            {
                moveTimer.Stop();
                return false;
            }

            if (nextNode != null)
            {
                HandleItem(nextNode);
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
            return nextNode == null || (nextNode.Value != 0 && nextNode.Value != 1 && nextNode.Value != 2 && nextNode.Value != 3 && nextNode.Value != 4 && nextNode.Value != 5);
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

        private void HandleItem(Node nextNode)
        {
            if (nextNode.Value == 0)
            {
                return;
            }

            Item collectedItem = GetItemFromValue(nextNode.Value);
            itemsQueue.Enqueue(collectedItem);
            ApplyItem(itemsQueue.Dequeue());

            nextNode.Value = 0;
        }

        private Item GetItemFromValue(int value)
        {
            return value switch
            {
                1 => new CeldaDeCombustible(),
                2 => new CrecimientoEstela(),
                3 => new Bomba(),
                _ => throw new ArgumentException("Tipo de ítem no reconocido")
            };
        }

        private void ApplyItem(Item item)
        {
            item.Aplicar(this);
        }

        public void ActualizarMaxTrailLength()
        {
            MaxTrailLength = TamañoEstela;
        }

        public void IncrementarCombustible(int cantidad)
        {
            Combustible = Math.Min(Combustible + cantidad, 100);  // Aumenta el combustible sin exceder 100
            Debug.WriteLine($"Combustible incrementado a: {Combustible}");
        }

        private void UpdateCombustible()
        {
            distanceTravelled++;
            if (distanceTravelled >= 5)
            {
                Combustible--;
                distanceTravelled = 0;

                if (Combustible <= 0)
                {
                    StopMovement();
                }
            }
        }

        private void StopMovement()
        {
            moveTimer.Stop();
            Debug.WriteLine("Moto detenida por falta de combustible");
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

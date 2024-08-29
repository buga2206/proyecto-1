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
        public Direction CurrentDirection { get; private set; }
        private OwnLinkedList<Node> trailNodes;
        private int distanceTravelled;
        private const int MaxTrailLength = 3;

        public int Combustible { get; set; }
        public int Velocidad { get; private set; }
        public int TamañoEstela { get; set; }

        protected System.Timers.Timer moveTimer;

        public Moto(Node startNode, int trailValue, int velocidad, int tamañoEstela, int combustible = 100)
        {
            CurrentNode = startNode ?? throw new ArgumentNullException(nameof(startNode), "Start node cannot be null");
            TrailValue = trailValue;
            Velocidad = velocidad;
            TamañoEstela = tamañoEstela;
            Combustible = combustible;
            CurrentDirection = Direction.Right;
            trailNodes = new OwnLinkedList<Node>();

            moveTimer = new System.Timers.Timer(velocidad);
            moveTimer.Elapsed += OnMoveTimerElapsed;
            moveTimer.AutoReset = true;
            moveTimer.Start();
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

        public bool Move()
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
            return nextNode == null || (nextNode.Value != 0 && nextNode.Value != 3 && nextNode.Value != 4 && nextNode.Value != 5 && nextNode.Value != 8 && nextNode.Value != 9);
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
            CurrentNode.Value = 6;
        }

        private void HandlePower(Node nextNode)
        {
            if (nextNode.Value == 0)
            {
                // Nodo vacío, no hacer nada
                return;
            }

            Debug.WriteLine($"Node value detected: {nextNode.Value}"); // Línea de depuración

            switch (nextNode.Value)
            {
                case 3:
                    ShowPowerMessage("Gas");
                    Debug.WriteLine("Gas detected and applied"); // Mensaje de depuración adicional
                    AplicarGas(); // Método para aplicar el efecto del gas
                    break;
                case 4:
                    ShowPowerMessage("Escudo");
                    break;
                case 5:
                    ShowPowerMessage("HiperVelocidad");
                    break;
                case 8:
                    ShowPowerMessage("Bomba");
                    break;
                case 9:
                    ShowPowerMessage("Crecimiento de Estela");
                    break;
                default:
                    Debug.WriteLine($"Unhandled node value: {nextNode.Value} at position [{CurrentNode.Value}]"); // Muestra el valor no manejado
                    break;
            }

            nextNode.Value = 0; // Resetea el nodo después de recoger el ítem/poder
        }



        private void AplicarGas()
        {
            Combustible = Math.Min(Combustible + 20, 100); // Incrementa el combustible hasta un máximo de 100
            Debug.WriteLine($"Nuevo nivel de combustible: {Combustible}");
        }




        private void ShowPowerMessage(string powerName)
        {
            if (Application.OpenForms.Count > 0)
            {
                Form1? form = Application.OpenForms[0] as Form1;
                form?.Invoke(new Action(() =>
                {
                    form.powerMessageLabel.Text = $"Poder encontrado: {powerName}";
                }));
            }
        }

        private void UpdateCombustible()
        {
            distanceTravelled++;
            if (distanceTravelled >= 5)
            {
                Combustible--;
                Debug.WriteLine($"el combistible es de {Combustible}");
                distanceTravelled = 0;
            }
        }

        public void StopTimer()
        {
            moveTimer?.Stop();
            moveTimer?.Dispose();
        }
    }
}

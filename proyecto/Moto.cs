using System;
using System.Timers;
using System.Diagnostics;

namespace proyecto
{
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
        public int NivelVelocidad { get; private set; }
        private bool tieneEscudo;
        private System.Timers.Timer? escudoTimer; // Especificar System.Timers.Timer y permitir nulabilidad
        private System.Timers.Timer? poderDelayTimer; // Especificar System.Timers.Timer y permitir nulabilidad

        protected System.Timers.Timer moveTimer; // Especificar System.Timers.Timer
        private OwnQueueList<Item> itemsQueue;
        private readonly OwnStackList<Poder> poderesStack;

        public Moto(Node startNode, int trailValue, int nivelVelocidad, int tamañoEstela, int trailHeadValue, int combustible = 100)
        {
            CurrentNode = startNode ?? throw new ArgumentNullException(nameof(startNode), "Start node cannot be null");
            TrailValue = trailValue;
            trailHead = trailHeadValue;
            NivelVelocidad = nivelVelocidad;
            Velocidad = GetVelocidadPorNivel(NivelVelocidad);
            TamañoEstela = tamañoEstela;
            MaxTrailLength = tamañoEstela;
            Combustible = combustible;
            CurrentDirection = Direction.Right;
            trailNodes = new OwnLinkedList<Node>();
            itemsQueue = new OwnQueueList<Item>();

            poderesStack = new OwnStackList<Poder>();
            InitializePoderDelayTimer();

            moveTimer = new System.Timers.Timer(Velocidad); // Especificar System.Timers.Timer
            moveTimer.Elapsed += OnMoveTimerElapsed;
            moveTimer.AutoReset = true;

            tieneEscudo = false;
        }

        private void InitializePoderDelayTimer()
        {
            poderDelayTimer = new System.Timers.Timer(1000); // Especificar System.Timers.Timer
            poderDelayTimer.Elapsed += OnPoderDelayElapsed;
            poderDelayTimer.AutoReset = false;
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
                HandlePoder(nextNode);
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
            if (tieneEscudo)
            {
                // Si tiene escudo, no colisiona con ningún nodo
                return nextNode == null;
            }
            // Sin escudo, colisiona con cualquier nodo que no sea vacío, gas, incremento de estela, bomba, hiper velocidad o escudo
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
            if (nextNode.Value >= 1 && nextNode.Value <= 3)
            {
                Item collectedItem = GetItemFromValue(nextNode.Value);
                itemsQueue.Enqueue(collectedItem);
                ApplyItem(itemsQueue.Dequeue());
                nextNode.Value = 0;
            }
        }

        private void HandlePoder(Node nextNode)
        {
            if (nextNode.Value >= 4 && nextNode.Value <= 5)
            {
                Poder collectedPoder = GetPoderFromValue(nextNode.Value);
                AplicarPoderConDelay(collectedPoder);
                nextNode.Value = 0;
            }
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

        private Poder GetPoderFromValue(int value)
        {
            return value switch
            {
                4 => new HiperVelocidad(),
                5 => new Escudo(),
                _ => throw new ArgumentException("Tipo de poder no reconocido")
            };
        }

        private void ApplyItem(Item item)
        {
            item.Aplicar(this);
        }

        public void AplicarPoderConDelay(Poder poder)
        {
            poderesStack.Push(poder);
            poderDelayTimer?.Start(); // Usar el temporizador con nulabilidad
        }

        private void OnPoderDelayElapsed(object? sender, ElapsedEventArgs e)
        {
            if (poderesStack.Count() > 0)
            {
                Poder poder = poderesStack.Pop();
                poder.Aplicar(this);
                Debug.WriteLine("Poder aplicado después de 1 segundo de delay");
            }
        }

        public void AplicarHiperVelocidad()
        {
            NivelVelocidad = Math.Min(NivelVelocidad + 4, 10);
            Velocidad = GetVelocidadPorNivel(NivelVelocidad);
            moveTimer.Interval = Velocidad;
        }

        public void ActivarEscudo(int duracion)
        {
            tieneEscudo = true;

            escudoTimer = new System.Timers.Timer(duracion * 1000); // Especificar System.Timers.Timer
            escudoTimer.Elapsed += (sender, e) => DesactivarEscudo();
            escudoTimer.AutoReset = false;
            escudoTimer.Start();
        }

        private void DesactivarEscudo()
        {
            tieneEscudo = false;
            escudoTimer?.Stop();
            escudoTimer?.Dispose();
        }

        public void ActualizarMaxTrailLength()
        {
            MaxTrailLength = TamañoEstela;
        }

        public void IncrementarCombustible(int cantidad)
        {
            Combustible = Math.Min(Combustible + cantidad, 100);
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

        private int GetVelocidadPorNivel(int nivelVelocidad)
        {
            if (nivelVelocidad < 1 || nivelVelocidad > 10)
            {
                throw new ArgumentOutOfRangeException(nameof(nivelVelocidad), "Nivel de velocidad no válido. Debe estar entre 1 y 10.");
            }

            return 1300 - (nivelVelocidad - 1) * 100;
        }
    }
}

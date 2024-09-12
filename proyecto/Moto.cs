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
        private System.Timers.Timer? escudoTimer;

        protected System.Timers.Timer moveTimer;
        private OwnQueueList<Item> itemsQueue;
        private readonly OwnStackList<Poder> poderesStack; // Mantiene la pila de poderes

        // Definimos un evento que se disparará cuando se recoja un poder
        public event Action? OnPoderCollected;

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

            poderesStack = new OwnStackList<Poder>(); // Inicializa la pila de poderes

            moveTimer = new System.Timers.Timer(Velocidad);
            moveTimer.Elapsed += OnMoveTimerElapsed;
            moveTimer.AutoReset = true;

            tieneEscudo = false;
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
            if (Combustible <= 0)
            {
                StopMovement();
                return false;
            }

            Node? nextNode = GetNextNode();

            if (IsCollision(nextNode))
            {
                moveTimer.Stop();
                ClearTrail();  // Clear the trail before stopping the moto
                return false;
            }

            if (nextNode != null)
            {
                HandleItem(nextNode);
                HandlePoder(nextNode); // Recolecta los poderes
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
                return nextNode == null;
            }

            if (nextNode == null || (nextNode.Value != 0 && nextNode.Value != 1 && nextNode.Value != 2 && nextNode.Value != 3 && nextNode.Value != 4 && nextNode.Value != 5))
            {
                // Collision detected, return true to stop the moto
                return true;
            }

            return false;
        }

        private void ClearTrail()
        {
            foreach (var node in trailNodes)
            {
                node.Value = 0; // Clear the trail in the grid
            }
            CurrentNode.Value = 0;
            trailNodes.Clear(); // Clear the trail in the linked list
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
                if (tieneEscudo)
                {
                    return;
                }
                else
                {
                    Item collectedItem = GetItemFromValue(nextNode.Value);
                    itemsQueue.Enqueue(collectedItem);
                    ApplyItem(itemsQueue.Dequeue());
                    nextNode.Value = 0;
                }
                
            }
        }

        // Manejamos la recolección de poderes y notificamos a Form1 para actualizar la UI
        private void HandlePoder(Node nextNode)
        {
            if (nextNode.Value >= 4 && nextNode.Value <= 5)
            {
                Poder collectedPoder = GetPoderFromValue(nextNode.Value);
                AddPoder(collectedPoder); // Agrega el poder a la pila
                nextNode.Value = 0;

                // Disparamos el evento para actualizar la interfaz
                OnPoderCollected?.Invoke();
                Debug.WriteLine("Poder recogido: " + collectedPoder);
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

        // Método agregado para manejar la adición de poderes a la pila
        public void AddPoder(Poder poder)
        {
            poderesStack.Push(poder);
            Debug.WriteLine($"Poder agregado a la pila: {poder.GetType().Name}");
        }

        private void ApplyItem(Item item)
        {
            item.Aplicar(this);
        }

        // Método para aplicar un poder con un delay de 1 segundo
        public void AplicarPoderConDelay(int selectedIndex)
        {
            // Creamos una pila auxiliar para sacar los poderes que están sobre el seleccionado
            OwnStackList<Poder> auxStack = new OwnStackList<Poder>();

            // Movemos los poderes que están sobre el seleccionado a la pila auxiliar
            for (int i = 0; i < PoderCount() - 1 - selectedIndex; i++)
            {
                auxStack.Push(poderesStack.Pop());
            }

            // Extraemos el poder seleccionado
            Poder selectedPoder = poderesStack.Pop();

            // Restauramos los poderes a la pila original
            while (auxStack.Count() > 0)
            {
                poderesStack.Push(auxStack.Pop());
            }

            // Aplicamos un delay de 1 segundo antes de aplicar el poder
            Task.Delay(1000).ContinueWith(_ =>
            {
                // Después del delay, aplicamos el poder
                selectedPoder.Aplicar(this);

                // Mostrar mensaje en consola para debug
                Debug.WriteLine("Poder aplicado después de 1 segundo de delay");

                // Aquí puedes agregar cualquier lógica adicional para actualizar la UI o el estado del juego
                OnPoderCollected?.Invoke(); // Evento para actualizar la UI, si es necesario
            });
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

            escudoTimer = new System.Timers.Timer(duracion * 1000);
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
            ClearTrail(); // Clear the trail when the moto stops
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
                throw new ArgumentException("El nivel de velocidad debe estar entre 1 y 10.");
            }

            return 900 - (nivelVelocidad - 1) * 100;
        }

        // Implementación del método PoderCount
        public int PoderCount()
        {
            return poderesStack.Count();
        }

        // Implementación del método GetPoderAt
        public Poder GetPoderAt(int index)
        {
            return poderesStack.Peek(index);
        }
    }
}

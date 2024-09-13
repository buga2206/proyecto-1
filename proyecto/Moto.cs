using System;
using System.Timers;
using System.Diagnostics;

namespace proyecto 
{
    public class Moto 
    {
        public Node CurrentNode; 
        public int TrailValue;
        public int trailHead; 
        public Direction CurrentDirection;
        private OwnLinkedList<Node> trailNodes;
        private int distanceTravelled;
        private int MaxTrailLength;
        public int Combustible;
        public int Velocidad; 
        public int TamañoEstela;
        public int NivelVelocidad; 
        private bool tieneEscudo; 
        private System.Timers.Timer? escudoTimer; 
        protected System.Timers.Timer moveTimer;
        private System.Timers.Timer? hipervelocidadTimer;
        private OwnQueueList<Item> itemsQueue;  
        private readonly OwnStackList<Poder> poderesStack;
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
            CurrentDirection = Direction.Down; 
            trailNodes = new OwnLinkedList<Node>();
            itemsQueue = new OwnQueueList<Item>(); 
            poderesStack = new OwnStackList<Poder>();
            hipervelocidadTimer = null;

            moveTimer = new System.Timers.Timer(Velocidad);
            moveTimer.Elapsed += OnMoveTimerElapsed;
            moveTimer.AutoReset = true; 
            tieneEscudo = false; 
        }

        protected virtual void OnMoveTimerElapsed(object? sender, ElapsedEventArgs e)  // Se ejecuta cada vez que el temporizador de movimiento expira
        {
            if (Move())
            {
                if (Application.OpenForms.Count > 0)  // Se verifica si hay formularios abiertos
                {
                    Form1? form = Application.OpenForms[0] as Form1;  // Se obtiene el formulario principal
                    if (form != null && !form.IsDisposed)  // Verifica que el formulario no esté cerrado o nulo
                    {
                        form.Invoke(new Action(() => form.RenderGrid()));  // Actualiza la interfaz gráfica para reflejar el nuevo estado de la moto
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
                ClearTrail();  
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
                Direction.Up => CurrentNode.Up,  // Si la dirección es arriba, retorna el nodo de arriba
                Direction.Down => CurrentNode.Down,  // Si la dirección es abajo, retorna el nodo de abajo
                Direction.Left => CurrentNode.Left,  // Si la dirección es a la izquierda, retorna el nodo de la izquierda
                Direction.Right => CurrentNode.Right,  // Si la dirección es a la derecha, retorna el nodo de la derecha
                _ => null,  // Si no hay dirección válida, retorna null
            };
        }

        private bool IsCollision(Node? nextNode) 
        {
            if (tieneEscudo)  // Si la moto tiene escudo, solo colisiona si no hay siguiente nodo
            {
                return nextNode == null;
            }

            if (nextNode == null || (nextNode.Value != 0 && nextNode.Value != 1 && nextNode.Value != 2 && nextNode.Value != 3 && nextNode.Value != 4 && nextNode.Value != 5)) 
            {
                return true; 
            }

            return false;
        }

        private void ClearTrail()
        {
            foreach (var node in trailNodes)  // Recorre cada nodo en la estela
            {
                node.Value = 0;  // Limpia el valor de cada nodo
            }
            CurrentNode.Value = 0;  // Limpia el valor del nodo actual
            trailNodes.Clear();  // Limpia la lista de nodos de la estela
        }

        private void HandleItem(Node nextNode) 
        {
            if (nextNode.Value >= 1 && nextNode.Value <= 3)  // Si el nodo contiene un ítem
            {
                if (tieneEscudo)  // Si la moto tiene escudo, no recoge el ítem, para evitar la bomba
                {
                    return;
                }
                else
                {
                    Item collectedItem = GetItemFromValue(nextNode.Value);  // Obtiene el ítem correspondiente al valor del nodo
                    itemsQueue.Enqueue(collectedItem);  // Agrega el ítem a la cola
                    ApplyItem(itemsQueue.Dequeue());  // Aplica el ítem
                    nextNode.Value = 0;  // Limpia el valor del nodo
                }
            }
        }

        private Item GetItemFromValue(int value) 
        {
            return value switch
            {
                1 => new CeldaDeCombustible(),  // Retorna una celda de combustible si el valor es 1
                2 => new CrecimientoEstela(),  // Retorna un ítem de crecimiento de estela si el valor es 2
                3 => new Bomba(),  // Retorna una bomba si el valor es 3
                _ => throw new ArgumentException("Tipo de ítem no reconocido"),  // Lanza una excepción si el valor no es válido
            };
        }

        private void ApplyItem(Item item)
        {
            Task.Delay(1000).ContinueWith(_ =>  // Aplica el Item después de 1 segundo de retraso
            {
                item.Aplicar(this);  // Aplica el Item a la moto

            });
        }

        private void HandlePoder(Node nextNode)
        {
            if (nextNode.Value >= 4 && nextNode.Value <= 5)  // Si el nodo contiene un poder
            {
                Poder collectedPoder = GetPoderFromValue(nextNode.Value);  // Obtiene el poder correspondiente al valor del nodo
                AddPoder(collectedPoder);  // Agrega el poder a la pila
                nextNode.Value = 0;  // Limpia el valor del nodo

                OnPoderCollected?.Invoke();  // Dispara el evento de poder recogido
            }
        }

        private Poder GetPoderFromValue(int value) 
        {
            return value switch
            {
                4 => new HiperVelocidad(),  // Retorna un poder de hiper velocidad si el valor es 4
                5 => new Escudo(),  // Retorna un poder de escudo si el valor es 5
                _ => throw new ArgumentException("Tipo de poder no reconocido"),  // Lanza una excepción si el valor no es válido
            };
        }

        public void AddPoder(Poder poder)
        {
            poderesStack.Push(poder);  // Agrega el poder a la pila
        }

        private void UpdateTrail() 
        {
            trailNodes.AddFirst(CurrentNode); 

            if (trailNodes.Count() > MaxTrailLength)  // Si la estela supera la longitud máxima
            {
                Node tailNode = trailNodes[MaxTrailLength];  // Obtiene el nodo al final de la estela
                tailNode.Value = 0;  // Limpia el valor del nodo al final
                trailNodes.RemoveAt(MaxTrailLength);  // Elimina el nodo del final de la estela
            }
        }

        private void MoveToNextNode(Node nextNode)
        {
            CurrentNode.Value = TrailValue;  // Establece el valor de la estela en el nodo actual
            CurrentNode = nextNode!;  // Mueve la moto al siguiente nodo
            CurrentNode.Value = trailHead;  // Establece el valor de la cabeza en el nuevo nodo
        }

        private void UpdateCombustible() 
        {
            distanceTravelled++;  // Incrementa la distancia recorrida

            if (distanceTravelled >= 5)  // Reduce el combustible cada 5 unidades de distancia
            {
                Combustible--;  // Disminuye el combustible
                distanceTravelled = 0;  // Reinicia la distancia recorrida

                if (Combustible <= 0)  // Si se acaba el combustible, detiene el movimiento
                {
                    StopMovement();
                }
            }
        }

        public void AplicarPoderConDelay(int selectedIndex)  
        {
            OwnStackList<Poder> auxStack = new OwnStackList<Poder>();  // Pila auxiliar para manejar los poderes

            for (int i = 0; i < PoderCount() - 1 - selectedIndex; i++)  // Desapila hasta llegar al poder seleccionado
            {
                auxStack.Push(poderesStack.Pop());
            }

            Poder selectedPoder = poderesStack.Pop();  // Obtiene el poder seleccionado

            while (auxStack.Count() > 0)  // Vuelve a apilar los poderes restantes
            {
                poderesStack.Push(auxStack.Pop());
            }

            Task.Delay(1000).ContinueWith(_ =>  // Aplica el poder después de 1 segundo de retraso
            {
                selectedPoder.Aplicar(this);  // Aplica el poder a la moto
                OnPoderCollected?.Invoke();  // Dispara el evento de poder recogido
            });
        }

        public void ActivarHiperVelocidad(int duracion)
        {
            NivelVelocidad = Math.Min(NivelVelocidad + 4, 10);  // Incrementa el nivel de velocidad
            Velocidad = GetVelocidadPorNivel(NivelVelocidad); 
            moveTimer.Interval = Velocidad;  

            // Si el temporizador ya estaba corriendo, lo reiniciamos
            if (hipervelocidadTimer != null)
            {
                hipervelocidadTimer.Stop();
                hipervelocidadTimer.Dispose();
            }

            hipervelocidadTimer = new System.Timers.Timer(duracion * 1000);
            hipervelocidadTimer.Elapsed += (sender, e) => DesactivarHiperVelocidad();  // Asociamos el evento para desactivar la hipervelocidad
            hipervelocidadTimer.AutoReset = false;  
            hipervelocidadTimer.Start(); 
        }

        private void DesactivarHiperVelocidad()
        {
            NivelVelocidad = Math.Max(NivelVelocidad - 4, 1);  // Reduce el nivel de velocidad al valor anterior (o 1 si es el mínimo)
            Velocidad = GetVelocidadPorNivel(NivelVelocidad);
            moveTimer.Interval = Velocidad; 

            hipervelocidadTimer?.Stop(); 
            hipervelocidadTimer?.Dispose(); 
            hipervelocidadTimer = null;  // Reinicia la referencia al temporizador
        }

        public void ActivarEscudo(int duracion)
        {
            tieneEscudo = true;  // Activa el escudo

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
            Combustible = Math.Min(Combustible + cantidad, 100);  // Incrementa el combustible, sin superar 100
        }

        private void StopMovement()
        {
            moveTimer.Stop();
            ClearTrail();
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

            return 1000 - (nivelVelocidad - 1) * 100;  // Calcula la velocidad
        }

        public int PoderCount() 
        {
            return poderesStack.Count();  // Retorna el número de poderes en la pila
        }

        public Poder GetPoderAt(int index) 
        {
            return poderesStack.Peek(index);  // Retorna el poder en la posición especificada
        }
    }
}
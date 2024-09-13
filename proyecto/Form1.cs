using System;
using System.Collections.Generic;
using System.Drawing;
using System.Timers;
using System.Windows.Forms;
using System.Diagnostics;

namespace proyecto
{
    public partial class Form1 : Form
    {
        private Grid gameGrid;
        private Moto player1;
        private List<BotMoto> bots;
        private const int GridRows = 40;
        private const int GridColumns = 60;
        private PictureBox[,] pictureBoxes;
        private Dictionary<int, Image> imageCache;
        private System.Timers.Timer startDelayTimer;
        private bool canMove;
        private List<PictureBox> poderesPictureBoxes;
        private int currentPoderIndex;

        public Form1()
        {
            InitializeComponent();

            this.DoubleBuffered = true; // Habilita el doble búfer para reducir el parpadeo en la interfaz gráfica

            gameGrid = new Grid(GridRows, GridColumns);
            pictureBoxes = new PictureBox[GridRows, GridColumns];
            bots = new List<BotMoto>();
            imageCache = LoadImageCache();

            InitializeGrid();
            CenterGrid();
            RenderGrid();

            InitializePlayer1();
            InitializeBots();
            GenerateItemsAndPowers();

            InitializeStartDelay();

            InitializePoderesPictureBoxes();

            this.KeyDown += new KeyEventHandler(Form1_KeyDown);
            this.Focus();
        }

        private Dictionary<int, Image> LoadImageCache()
        {

            return new Dictionary<int, Image>
            {
                { 0, Image.FromFile("Resources/emptySpace.png") },
                { 1, Image.FromFile("Resources/Items/gas.png") },
                { 2, Image.FromFile("Resources/Items/trailGrowth.png") },
                { 3, Image.FromFile("Resources/Items/bomb.png") },
                { 4, Image.FromFile("Resources/Powers/hiperVelocity.png") },
                { 5, Image.FromFile("Resources/Powers/shield.png") },
                { 6, Image.FromFile("Resources/BotsHead/bot1Head.png") },
                { 7, Image.FromFile("Resources/BotsHead/bot2Head.png") },
                { 8, Image.FromFile("Resources/BotsHead/bot3Head.png") },
                { 9, Image.FromFile("Resources/BotsHead/bot4Head.png") },
                { 10, Image.FromFile("Resources/BotsBody/bot1Body.png") },
                { 11, Image.FromFile("Resources/BotsBody/bot2Body.png") },
                { 12, Image.FromFile("Resources/BotsBody/bot3Body.png") },
                { 13, Image.FromFile("Resources/BotsBody/bot4Body.png") },
                { 14, Image.FromFile("Resources/PlayersHead/Player1Head.png") },
                { 15, Image.FromFile("Resources/PlayersBody/Player1Body.png") },
            };
        }


        private void InitializeGrid()
        {
            int cellSize = 15;  // Tamaño de cada celda
            int borderWidth = 1;  // Ancho del borde

            // Recorre todas las filas del grid
            for (int row = 0; row < GridRows; row++)
            {
                // Recorre todas las columnas del grid
                for (int col = 0; col < GridColumns; col++)
                {
                    // Panel que representará el borde de una celda
                    Panel borderPanel = new Panel
                    {
                        Width = cellSize + borderWidth * 2,
                        Height = cellSize + borderWidth * 2,
                        Location = new Point(col * (cellSize + borderWidth * 2), row * (cellSize + borderWidth * 2)), // Posición del panel dentro de la cuadrícula
                        BackColor = Color.MidnightBlue 
                    };

                    PictureBox cellBox = new PictureBox
                    {
                        Width = cellSize,
                        Height = cellSize,
                        Location = new Point(borderWidth, borderWidth),
                        BorderStyle = BorderStyle.None
                    };

                    // Se añade el PictureBox dentro del panel
                    borderPanel.Controls.Add(cellBox);

                    // Se guarda la referencia del PictureBox 
                    pictureBoxes[row, col] = cellBox;

                    // Añade el panel a la interfaz gráfica
                    this.Controls.Add(borderPanel);
                }
            }
        }

        private void CenterGrid() 
        {
            // Verifica si es nulo
            if (pictureBoxes == null || pictureBoxes.Length == 0)
            {
                return;
            }

            int panelWidth = pictureBoxes[0, 0].Width + 2; // Obtiene el ancho del panel
            int panelHeight = pictureBoxes[0, 0].Height + 2; // Obtiene el alto del panel

            int gridWidth = panelWidth * GridColumns; // Calcula el ancho total 
            int gridHeight = panelHeight * GridRows; // Calcula el alto total
            
            int startX = (this.ClientSize.Width - gridWidth) / 2; // Calcula la posición inicial (X)
            int startY = (this.ClientSize.Height - gridHeight) / 2; // Calcula la posición inicial (Y)

            for (int row = 0; row < GridRows; row++)
            {
                for (int col = 0; col < GridColumns; col++)
                {
                    Control borderPanel = pictureBoxes[row, col].Parent;
                    if (borderPanel != null)
                    {
                        borderPanel.Location = new Point(startX + col * panelWidth, startY + row * panelHeight); // Actualiza la posición para centrar el grid
                    }
                }
            }
        }

        public void RenderGrid() 
        {
            for (int row = 0; row < GridRows; row++)  
            {
                for (int col = 0; col < GridColumns; col++) 
                {
                    Node? currentNode = gameGrid.GetNode(row, col);  
                    if (currentNode == null) continue; 

                    PictureBox cellBox = pictureBoxes[row, col];  // Obtiene el PictureBox correspondiente a la posición (row, col)
                    if (cellBox == null)  // Si el PictureBox es nulo, lanza una excepción indicando donde
                    {
                        throw new InvalidOperationException($"PictureBox at [{row},{col}] is null in RenderGrid.");
                    }

                    // Verifica si la imagen del PictureBox es nula o si su etiqueta (Tag) es diferente al valor del nodo actual
                    if (cellBox.Image == null || cellBox.Image.Tag == null || (int)cellBox.Image.Tag != currentNode.Value)
                    {
                        cellBox.Image = imageCache[currentNode.Value];  // Asigna la imagen correspondiente al valor del nodo desde ImageCache
                        cellBox.Image.Tag = currentNode.Value;  // Establece la etiqueta (Tag) de la imagen como el valor actual del nodo
                    }
                }
            }
        }

        private void InitializePlayer1()  
        {
            int rowPosition = (GridRows / 8);  
            int colPosition = (GridColumns / 2);  

            Node? startNode = gameGrid.GetNode(rowPosition, colPosition);  

            if (startNode == null)  
            {
                throw new NullReferenceException("No se pudo iniciar el player 1");  
            }
            else 
            {
                player1 = new Moto(startNode, 15, 6, 3, 14); 
                player1.OnPoderCollected += UpdatePoderesUI;  // Asigna el evento OnPoderCollected del jugador 1 para actualizar la UI de poderes
            }
        }


        private void InitializeBots()  
        {
            int rowPositionBot1 = GridRows / 4; 
            int colPositionBot1 = GridColumns / 4; 

            int rowPositionBot2 = GridRows / 4; 
            int colPositionBot2 = 3 * (GridColumns / 4); 

            int rowPositionBot3 = 3 * (GridRows / 4); 
            int colPositionBot3 = GridColumns / 4; 

            int rowPositionBot4 = 3 * (GridRows / 4); 
            int colPositionBot4 = 3 * (GridColumns / 4); 

            Node? botStartNode1 = gameGrid.GetNode(rowPositionBot1, colPositionBot1);
            Node? botStartNode2 = gameGrid.GetNode(rowPositionBot2, colPositionBot2);
            Node? botStartNode3 = gameGrid.GetNode(rowPositionBot3, colPositionBot3);
            Node? botStartNode4 = gameGrid.GetNode(rowPositionBot4, colPositionBot4);

            if (botStartNode1 != null)
            {
                BotMoto bot1 = new BotMoto(botStartNode1, 6, 3, 3, 10);  
                bot1.ChangeDirection(Direction.Down);  
                bots.Add(bot1);  
            }


            if (botStartNode2 != null)
            {
                BotMoto bot2 = new BotMoto(botStartNode2, 7, 4, 3, 11);  
                bot2.ChangeDirection(Direction.Right); 
                bots.Add(bot2); 
            }

            if (botStartNode3 != null)
            {
                BotMoto bot3 = new BotMoto(botStartNode3, 8, 2, 3, 12);
                bot3.ChangeDirection(Direction.Up); 
                bots.Add(bot3); 
            }

            if (botStartNode4 != null)
            {
                BotMoto bot4 = new BotMoto(botStartNode4, 9, 5, 3, 13); 
                bot4.ChangeDirection(Direction.Left); 
                bots.Add(bot4);  
            }
        }

        private void GenerateItemsAndPowers() 
        {
            Random rand = new Random();  

            for (int row = 0; row < GridRows; row++) 
            {
                for (int col = 0; col < GridColumns; col++)  
                {
                    Node? currentNode = gameGrid.GetNode(row, col);  

                    if (currentNode != null && currentNode.Value == 0)
                    {
                        double itemProbability = rand.NextDouble();  // Genera un número aleatorio entre 0 y 1 para la probabilidad de generar un ítem

                        // Si la probabilidad es menor que 0.05 (5%), genera un ítem en esta celda
                        if (itemProbability < 0.05)
                        {
                            int itemType = rand.Next(1, 6);  // Genera un número aleatorio entre 1 y 5 para determinar el tipo de ítem

                            switch (itemType)
                            {
                                case 1:
                                    currentNode.Value = 1;  // Gas
                                    break;
                                case 2:
                                    currentNode.Value = 2;  // Incremento de estela
                                    break;
                                case 3:
                                    currentNode.Value = 3;  // Bomba
                                    break;
                                case 4:
                                    currentNode.Value = 4;  // Hiper velocidad
                                    break;
                                case 5:
                                    currentNode.Value = 5;  // Escudo
                                    break;
                            }
                        }
                    }
                }
            }
            RenderGrid(); 
        }


        private void InitializeStartDelay()  
        {
            canMove = false;  

            startDelayTimer = new System.Timers.Timer(10000);  

            startDelayTimer.Elapsed += OnStartDelayElapsed;  

            startDelayTimer.AutoReset = false;  

            startDelayTimer.Start();
        }

        private void InitializePoderesPictureBoxes() 
        {
            poderesPictureBoxes = new List<PictureBox>();  // Lista vacía de PictureBox para almacenar los poderes

            currentPoderIndex = -1;  // Inicializa el índice de poderes actuales en -1, indicando que no hay un poder activo

            Panel poderPanel = new Panel
            {
                Location = new Point(10, 10),  
                Size = new Size(35, 400), 
                BorderStyle = BorderStyle.FixedSingle 
            };

            this.Controls.Add(poderPanel);  // Añade el panel de poderes a los controles del formulario principal

            AddEmptyPictureBox(poderPanel);  // Llama a un método para agregar un PictureBox vacío dentro del panel de poderes
        }

        private void AddEmptyPictureBox(Panel panel) 
        {
            PictureBox poderBox = new PictureBox
            {
                Size = new Size(22, 22),  
                Location = new Point(5, poderesPictureBoxes.Count * 20), 
                BorderStyle = BorderStyle.FixedSingle, 
                SizeMode = PictureBoxSizeMode.CenterImage, 
                BackColor = Color.Transparent  
            };

            panel.Controls.Add(poderBox);  // Añade el PictureBox al panel de poderes

            poderesPictureBoxes.Add(poderBox);  // Añade el PictureBox a la lista de poderes
        }

        public void UpdatePoderesUI()
        {
            if (this.InvokeRequired)  // Verifica si la actualización de la UI debe ser invocada desde otro hilo 
            {
                this.Invoke(new Action(UpdatePoderesUI));  // Si es necesario, invoca el método en el hilo principal
                return;  // Retorna para evitar que el código continúe en el hilo actual
            }

            // Obtiene el panel que contiene los PictureBox de los poderes desde el primer PictureBox
            Panel poderPanel = (Panel)poderesPictureBoxes[0].Parent;

            // Si el jugador tiene más poderes que PictureBox disponibles, agrega más PictureBox vacíos al panel
            while (player1.PoderCount() > poderesPictureBoxes.Count)
            {
                AddEmptyPictureBox(poderPanel);  // Agrega un nuevo PictureBox vacío al panel de poderes
            }

            // Recorre cada poder del jugador y actualiza el PictureBox correspondiente con la imagen del poder
            for (int i = 0; i < player1.PoderCount(); i++)
            {
                Poder poder = player1.GetPoderAt(i);  // Obtiene el poder en la posición 'i' de la lista de poderes del jugador
                int poderValue = (poder is HiperVelocidad) ? 4 : 5;  // Determina el valor del poder (4 para HiperVelocidad, 5 para otros)
                poderesPictureBoxes[i].Image = imageCache[poderValue];  // Asigna la imagen correspondiente al poder en el PictureBox
            }

            // Limpia los PictureBox que no están siendo usados si hay más PictureBox que poderes activos
            for (int i = player1.PoderCount(); i < poderesPictureBoxes.Count; i++)
            {
                poderesPictureBoxes[i].Image = null;  // Elimina la imagen de los PictureBox no utilizados
            }
        }

        private void OnStartDelayElapsed(object? sender, ElapsedEventArgs e)
        {
            canMove = true;  

            player1.StartTimer();  

            foreach (var bot in bots)  
            {
                bot.StartTimer();  // Inicia el temporizador de cada bot, permitiendo que comiencen a moverse
            }

            this.Invoke(new Action(() => RenderGrid()));  // Invoca el método RenderGrid en el hilo principal para actualizar la cuadrícula visualmente
        }

        private void Form1_KeyDown(object? sender, KeyEventArgs e) 
        {
            if (canMove)  // Verifica si está permitido el movimiento
            {
                HandlePlayer1KeyDown(e);  // Llama al método que maneja el control del jugador 1 basado en la tecla presionada
                HandlePowerSelectionKeyDown(e);  // Llama al método que maneja la selección de poderes basado en la tecla presionada
            }
        }

        private void HandlePlayer1KeyDown(KeyEventArgs e) 
        {
            if (player1 != null)  // Verifica si el jugador 1 está inicializado
            {
                Direction newDirection = player1.CurrentDirection; 

                switch (e.KeyCode)
                {
                    case Keys.W:
                        newDirection = Direction.Up;  
                        break;
                    case Keys.S:
                        newDirection = Direction.Down;  
                        break;
                    case Keys.A:
                        newDirection = Direction.Left;  
                        break;
                    case Keys.D:
                        newDirection = Direction.Right;  
                        break;
                }

                // Verifica si el cambio de dirección es válido antes de aplicarlo
                if (IsValidDirectionChange(player1.CurrentDirection, newDirection))
                {
                    player1.ChangeDirection(newDirection);  // Cambia la dirección del jugador a la nueva dirección válida
                }
            }
        }

        private bool IsValidDirectionChange(Direction currentDirection, Direction newDirection)
        {
            // Verifica que no se permita moverse en la dirección opuesta directa para evitar un giro de 180 grados
            return (currentDirection == Direction.Left && newDirection != Direction.Right) ||
                   (currentDirection == Direction.Right && newDirection != Direction.Left) ||
                   (currentDirection == Direction.Up && newDirection != Direction.Down) ||
                   (currentDirection == Direction.Down && newDirection != Direction.Up);
        }


        private void HandlePowerSelectionKeyDown(KeyEventArgs e) 
        {
            switch (e.KeyCode)  
            {
                case Keys.Up:  
                    MovePowerSelectorUp();
                    break;
                case Keys.Down:  
                    MovePowerSelectorDown();
                    break;
                case Keys.Space:  
                    ApplySelectedPower();
                    break;
            }
        }

        private void MovePowerSelectorUp()
        {
            if (currentPoderIndex > 0) 
            {
                currentPoderIndex--;  // Decrementa el índice para seleccionar el poder anterior
                HighlightSelectedPower();  // Llama a un método para resaltar el poder seleccionado
            }
            else
            { 
                return;  
            }
        }

        private void MovePowerSelectorDown() 
        {
            if (currentPoderIndex < player1.PoderCount() - 1) 
            {
                currentPoderIndex++;  
                HighlightSelectedPower(); 
            }
            else
            {
                return;  
            }
        }

        private void ApplySelectedPower() 
        {
            if (currentPoderIndex >= 0 && player1.PoderCount() > 0)  // Verifica que haya un poder seleccionado y que el jugador tenga poderes
            {
                player1.AplicarPoderConDelay(currentPoderIndex);  // Aplica el poder con un retraso (delay)
            }
            else
            {
                return;  
            }
        }

        private void HighlightSelectedPower()  
        {
            for (int i = 0; i < poderesPictureBoxes.Count; i++)
            {
                if (i == currentPoderIndex) 
                {
                    poderesPictureBoxes[i].BackColor = Color.Cyan; 
                    poderesPictureBoxes[i].BorderStyle = BorderStyle.FixedSingle;  
                }
                else  
                {
                    poderesPictureBoxes[i].BackColor = Color.Transparent;  
                    poderesPictureBoxes[i].BorderStyle = BorderStyle.None;  
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)  
        {
            if (player1 != null)  // Si el jugador 1 está inicializado, detiene su temporizador
            {
                player1.StopTimer();
            }

            foreach (var bot in bots)  // Recorre todos los bots y detiene sus temporizadores
            {
                bot.StopTimer();
            }

            base.OnFormClosing(e);  // Llama al método base para manejar el cierre del formulario correctamente
        }

    }
}
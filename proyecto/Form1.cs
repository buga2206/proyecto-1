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
        private const int GridColumns = 40;
        private PictureBox[,] pictureBoxes;
        private Dictionary<int, Image> imageCache;

        private System.Timers.Timer startDelayTimer;
        private bool canMove;

        // Nueva lista de PictureBoxes para visualizar los poderes
        private List<PictureBox> poderesPictureBoxes;
        private int currentPoderIndex; // Índice del selector en la lista de poderes

        public Form1()
        {
            InitializeComponent();

            this.DoubleBuffered = true;

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

            // Inicialización de los PictureBoxes para los poderes
            InitializePoderesPictureBoxes();

            this.KeyDown += new KeyEventHandler(Form1_KeyDown);
            this.Focus();
        }

        // Método para inicializar los PictureBoxes de los poderes
        private void InitializePoderesPictureBoxes()
        {
            poderesPictureBoxes = new List<PictureBox>();
            currentPoderIndex = -1; // Inicialmente no hay poderes

            // Crear un panel en la interfaz para los poderes
            Panel poderPanel = new Panel
            {
                Location = new Point(10, 10), // Ajustar posición en la pantalla
                Size = new Size(35, 400), // Tamaño inicial
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(poderPanel);

            // Inicialmente agregamos un solo PictureBox vacío
            AddEmptyPictureBox(poderPanel);
            Debug.WriteLine("PictureBox inicializado con uno vacío");
        }

        // Método para agregar un PictureBox vacío
        private void AddEmptyPictureBox(Panel panel)
        {
            PictureBox poderBox = new PictureBox
            {
                Size = new Size(22, 22), // Tamaño de 15x15 píxeles
                Location = new Point(5, poderesPictureBoxes.Count * 20), // Espaciado dinámico
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.CenterImage, // Centrar la imagen
                BackColor = Color.Transparent // Fondo transparente
            };

            panel.Controls.Add(poderBox);
            poderesPictureBoxes.Add(poderBox);
        }

        // Actualiza las imágenes de los poderes recolectados en los PictureBoxes
        public void UpdatePoderesUI()
        {
            // Asegura que cualquier actualización de la UI se realice en el hilo principal
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(UpdatePoderesUI));
                return;
            }

            Debug.WriteLine("Actualizando los PictureBox de poderes...");

            Panel poderPanel = (Panel)poderesPictureBoxes[0].Parent;

            // Asegura que haya un PictureBox para cada poder en la pila
            while (player1.PoderCount() > poderesPictureBoxes.Count)
            {
                AddEmptyPictureBox(poderPanel); // Agrega un nuevo PictureBox si es necesario
            }

            // Actualizamos las imágenes de los poderes recolectados
            for (int i = 0; i < player1.PoderCount(); i++)
            {
                Poder poder = player1.GetPoderAt(i);
                int poderValue = (poder is HiperVelocidad) ? 4 : 5; // Asigna el valor de la imagen
                poderesPictureBoxes[i].Image = imageCache[poderValue];
                Debug.WriteLine($"Poder visualizado en PictureBox [{i}]: {poder.GetType().Name}");
            }

            // Limpia los PictureBox que no estén en uso
            for (int i = player1.PoderCount(); i < poderesPictureBoxes.Count; i++)
            {
                poderesPictureBoxes[i].Image = null;
                Debug.WriteLine($"PictureBox [{i}] vacío.");
            }
        }

        private void InitializeStartDelay()
        {
            canMove = false;
            startDelayTimer = new System.Timers.Timer(7000); // 7 segundos de retraso
            startDelayTimer.Elapsed += OnStartDelayElapsed;
            startDelayTimer.AutoReset = false;
            startDelayTimer.Start();
        }

        private void OnStartDelayElapsed(object? sender, ElapsedEventArgs e)
        {
            canMove = true;

            player1.StartTimer();  // Inicia el movimiento de la moto 1

            foreach (var bot in bots)
            {
                bot.StartTimer();  // Inicia el movimiento de los bots
            }

            // Asegura que se actualice la interfaz después de que se pueda mover
            this.Invoke(new Action(() => RenderGrid()));
        }

        private Dictionary<int, Image> LoadImageCache()
        {
            return new Dictionary<int, Image>
            {
                { 0, Image.FromFile("C:\\Users\\User\\desktop\\Datos 1\\proyecto-1\\proyecto-1\\proyecto\\Resources\\emptySpace.png") },
                { 1, Image.FromFile("C:\\Users\\User\\desktop\\Datos 1\\proyecto-1\\proyecto-1\\proyecto\\Resources\\Items\\gas.png") },
                { 2, Image.FromFile("C:\\Users\\User\\desktop\\Datos 1\\proyecto-1\\proyecto-1\\proyecto\\Resources\\Items\\trailGrowth.png") },
                { 3, Image.FromFile("C:\\Users\\User\\desktop\\Datos 1\\proyecto-1\\proyecto-1\\proyecto\\Resources\\Items\\bomb.png") },
                { 4, Image.FromFile("C:\\Users\\User\\desktop\\Datos 1\\proyecto-1\\proyecto-1\\proyecto\\Resources\\Powers\\hiperVelocity.png") },
                { 5, Image.FromFile("C:\\Users\\User\\desktop\\Datos 1\\proyecto-1\\proyecto-1\\proyecto\\Resources\\Powers\\shield.png") },
                { 6, Image.FromFile("C:\\Users\\User\\desktop\\Datos 1\\proyecto-1\\proyecto-1\\proyecto\\Resources\\BotsHead\\bot1Head.png") },
                { 7, Image.FromFile("C:\\Users\\User\\desktop\\Datos 1\\proyecto-1\\proyecto-1\\proyecto\\Resources\\BotsHead\\bot2Head.png") },
                { 8, Image.FromFile("C:\\Users\\User\\desktop\\Datos 1\\proyecto-1\\proyecto-1\\proyecto\\Resources\\BotsHead\\bot3Head.png") },
                { 9, Image.FromFile("C:\\Users\\User\\desktop\\Datos 1\\proyecto-1\\proyecto-1\\proyecto\\Resources\\BotsHead\\bot4Head.png") },
                { 10, Image.FromFile("C:\\Users\\User\\desktop\\Datos 1\\proyecto-1\\proyecto-1\\proyecto\\Resources\\BotsBody\\bot1Body.png") },
                { 11, Image.FromFile("C:\\Users\\User\\desktop\\Datos 1\\proyecto-1\\proyecto-1\\proyecto\\Resources\\BotsBody\\bot2Body.png") },
                { 12, Image.FromFile("C:\\Users\\User\\desktop\\Datos 1\\proyecto-1\\proyecto-1\\proyecto\\Resources\\BotsBody\\bot3Body.png") },
                { 13, Image.FromFile("C:\\Users\\User\\desktop\\Datos 1\\proyecto-1\\proyecto-1\\proyecto\\Resources\\BotsBody\\bot4Body.png") },
                { 14, Image.FromFile("C:\\Users\\User\\desktop\\Datos 1\\proyecto-1\\proyecto-1\\proyecto\\Resources\\PlayersHead\\Player1Head.png") },
                { 15, Image.FromFile("C:\\Users\\User\\desktop\\Datos 1\\proyecto-1\\proyecto-1\\proyecto\\Resources\\PlayersBody\\Player1Body.png") },
            };
        }

        private void InitializePlayer1()
        {
            Node? startNode = gameGrid.GetNode(5, 5);
            if (startNode == null)
            {
                Debug.WriteLine("No se pudo iniciar el player 1");
            }
            else
            {
                player1 = new Moto(startNode, 15, 3, 3, 14); // Nivel 3 de velocidad

                // Suscribimos el evento al recoger un poder
                player1.OnPoderCollected += UpdatePoderesUI;
                Debug.WriteLine("Moto inicializada en la posición (5, 5)");
            }
        }

        private void InitializeGrid()
        {
            int cellSize = 15;
            int borderWidth = 1;

            for (int row = 0; row < GridRows; row++)
            {
                for (int col = 0; col < GridColumns; col++)
                {
                    Panel borderPanel = new Panel
                    {
                        Width = cellSize + borderWidth * 2,
                        Height = cellSize + borderWidth * 2,
                        Location = new Point(col * (cellSize + borderWidth * 2), row * (cellSize + borderWidth * 2)),
                        BackColor = Color.MidnightBlue
                    };

                    PictureBox cellBox = new PictureBox
                    {
                        Width = cellSize,
                        Height = cellSize,
                        Location = new Point(borderWidth, borderWidth), // Ubicación dentro del panel
                        BorderStyle = BorderStyle.None
                    };

                    borderPanel.Controls.Add(cellBox);
                    pictureBoxes[row, col] = cellBox;
                    this.Controls.Add(borderPanel);
                }
            }

        }

        private void CenterGrid()
        {
            if (pictureBoxes == null || pictureBoxes.Length == 0)
            {
                return;
            }

            int panelWidth = pictureBoxes[0, 0].Width + 2;
            int panelHeight = pictureBoxes[0, 0].Height + 2;

            int gridWidth = panelWidth * GridColumns;
            int gridHeight = panelHeight * GridRows;

            int startX = (this.ClientSize.Width - gridWidth) / 2;
            int startY = (this.ClientSize.Height - gridHeight) / 2;

            for (int row = 0; row < GridRows; row++)
            {
                for (int col = 0; col < GridColumns; col++)
                {
                    Control borderPanel = pictureBoxes[row, col].Parent;
                    if (borderPanel != null)
                    {
                        borderPanel.Location = new Point(startX + col * panelWidth, startY + row * panelHeight);
                    }
                }
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            CenterGrid();
        }

        private void InitializeBots()
        {
            Node? botStartNode1 = gameGrid.GetNode(20, 10);
            Node? botStartNode2 = gameGrid.GetNode(20, 20);
            Node? botStartNode3 = gameGrid.GetNode(20, 30);
            Node? botStartNode4 = gameGrid.GetNode(20, 5);

            if (botStartNode1 != null)
            {
                BotMoto bot1 = new BotMoto(botStartNode1, 6, gameGrid, 0.1, 3, 3, 10); // Nivel 3 de velocidad
                bot1.ChangeDirection(Direction.Left);
                bots.Add(bot1);
                Debug.WriteLine("Bot 1 inicializado");
            }

            if (botStartNode2 != null)
            {
                BotMoto bot2 = new BotMoto(botStartNode2, 7, gameGrid, 0.1, 4, 3, 11); // Nivel 4 de velocidad
                bot2.ChangeDirection(Direction.Up);
                bots.Add(bot2);
                Debug.WriteLine("Bot 2 inicializado");
            }

            if (botStartNode3 != null)
            {
                BotMoto bot3 = new BotMoto(botStartNode3, 8, gameGrid, 0.1, 2, 3, 12); // Nivel 2 de velocidad
                bot3.ChangeDirection(Direction.Right);
                bots.Add(bot3);
                Debug.WriteLine("Bot 3 inicializado");
            }

            if (botStartNode4 != null)
            {
                BotMoto bot4 = new BotMoto(botStartNode4, 9, gameGrid, 0.1, 5, 3, 13); // Nivel 5 de velocidad
                bot4.ChangeDirection(Direction.Down);
                bots.Add(bot4);
                Debug.WriteLine("Bot 4 inicializado");
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
                        double itemProbability = rand.NextDouble();

                        if (itemProbability < 0.05)
                        {
                            int itemType = rand.Next(1, 6);

                            switch (itemType)
                            {
                                case 1:
                                    currentNode.Value = 1; // Gas
                                    break;
                                case 2:
                                    currentNode.Value = 2; // Incremento de estela
                                    break;
                                case 3:
                                    currentNode.Value = 3; // Bomba
                                    break;
                                case 4:
                                    currentNode.Value = 4; // Hiper velocidad
                                    break;
                                case 5:
                                    currentNode.Value = 5; // Escudo
                                    break;
                            }
                        }
                    }
                }
            }

            RenderGrid();
        }

        private void Form1_KeyDown(object? sender, KeyEventArgs e)
        {
            if (canMove) // Solo permite movimiento si el contador ha terminado
            {
                HandlePlayer1KeyDown(e);
                HandlePowerSelectionKeyDown(e);
            }
        }

        private void HandlePlayer1KeyDown(KeyEventArgs e)
        {
            if (player1 != null)
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

                if (IsValidDirectionChange(player1.CurrentDirection, newDirection))
                {
                    player1.ChangeDirection(newDirection);
                    Debug.WriteLine($"Jugador cambió de dirección a {newDirection}");
                }
            }
        }

        // Implementación de IsValidDirectionChange
        private bool IsValidDirectionChange(Direction currentDirection, Direction newDirection)
        {
            return (currentDirection == Direction.Left && newDirection != Direction.Right) ||
                   (currentDirection == Direction.Right && newDirection != Direction.Left) ||
                   (currentDirection == Direction.Up && newDirection != Direction.Down) ||
                   (currentDirection == Direction.Down && newDirection != Direction.Up);
        }

        // Manejo de las teclas para selección de poderes
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
                default:
                    Debug.WriteLine($"Tecla no reconocida para selección de poder: {e.KeyCode}");
                    break;
            }
        }

        // Mueve el selector hacia arriba
        private void MovePowerSelectorUp()
        {
            if (currentPoderIndex > 0)
            {
                currentPoderIndex--;
                HighlightSelectedPower();
                Debug.WriteLine($"Selector de poder movido hacia arriba. Índice actual: {currentPoderIndex}");
            }
            else
            {
                Debug.WriteLine("No hay más poderes arriba para seleccionar.");
            }
        }

        // Mueve el selector hacia abajo
        private void MovePowerSelectorDown()
        {
            if (currentPoderIndex < player1.PoderCount() - 1)
            {
                currentPoderIndex++;
                HighlightSelectedPower();
                Debug.WriteLine($"Selector de poder movido hacia abajo. Índice actual: {currentPoderIndex}");
            }
            else
            {
                Debug.WriteLine("No hay más poderes abajo para seleccionar.");
            }
        }

        // Aplica el poder seleccionado
        private void ApplySelectedPower()
        {
            if (currentPoderIndex >= 0 && player1.PoderCount() > 0)
            {
                player1.AplicarPoderConDelay(currentPoderIndex); // Pasa el índice seleccionado
                Debug.WriteLine($"Poder aplicado desde índice: {currentPoderIndex}");
            }
            else
            {
                Debug.WriteLine("No hay más poderes para aplicar.");
            }
        }

        // Resalta el poder actualmente seleccionado en el PictureBox
        private void HighlightSelectedPower()
        {
            for (int i = 0; i < poderesPictureBoxes.Count; i++)
            {
                if (i == currentPoderIndex)
                {
                    poderesPictureBoxes[i].BackColor = Color.Cyan; // Resalta el poder seleccionado
                    poderesPictureBoxes[i].BorderStyle = BorderStyle.FixedSingle; // Borde visible para el seleccionado
                }
                else
                {
                    poderesPictureBoxes[i].BackColor = Color.Transparent; // Restaura los no seleccionados
                    poderesPictureBoxes[i].BorderStyle = BorderStyle.None; // Sin borde para no seleccionados
                }
            }
            Debug.WriteLine("Poderes actualizados visualmente en los PictureBoxes");
        }

        public void RenderGrid()
        {
            for (int row = 0; row < GridRows; row++)
            {
                for (int col = 0; col < GridColumns; col++)
                {
                    Node? currentNode = gameGrid.GetNode(row, col);
                    if (currentNode == null) continue;

                    PictureBox cellBox = pictureBoxes[row, col];
                    if (cellBox == null)
                    {
                        throw new InvalidOperationException($"PictureBox at [{row},{col}] is null in RenderGrid.");
                    }

                    if (cellBox.Image == null || cellBox.Image.Tag == null || (int)cellBox.Image.Tag != currentNode.Value)
                    {
                        cellBox.Image = imageCache[currentNode.Value];
                        cellBox.Image.Tag = currentNode.Value;
                    }
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (player1 != null)
            {
                player1.StopTimer();
            }

            foreach (var bot in bots)
            {
                bot.StopTimer();
            }

            base.OnFormClosing(e);
        }
    }
}

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
        private Moto playerMoto1;
        private Moto playerMoto2;
        private List<BotMoto> bots;
        private const int GridRows = 40;
        private const int GridColumns = 40;
        private PictureBox[,] pictureBoxes;
        private Dictionary<int, Image> imageCache;

        private System.Timers.Timer startDelayTimer;
        private bool canMove;

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

            InitializePlayers();
            InitializeBots();
            GenerateItemsAndPowers();

            InitializeStartDelay();

            this.KeyDown += new KeyEventHandler(Form1_KeyDown);
            this.Focus();
        }

        private void InitializeStartDelay()
        {
            canMove = false;

            startDelayTimer = new System.Timers.Timer(5000); // 5 segundos de retraso
            startDelayTimer.Elapsed += OnStartDelayElapsed;
            startDelayTimer.AutoReset = false;
            startDelayTimer.Start();
        }

        private void OnStartDelayElapsed(object? sender, ElapsedEventArgs e)
        {
            canMove = true;

            playerMoto1.StartTimer();  // Inicia el movimiento de la moto 1
            playerMoto2.StartTimer();  // Inicia el movimiento de la moto 2

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
                { 15, Image.FromFile("C:\\Users\\User\\desktop\\Datos 1\\proyecto-1\\proyecto-1\\proyecto\\Resources\\PlayersHead\\Player2Head.png") },
                { 16, Image.FromFile("C:\\Users\\User\\desktop\\Datos 1\\proyecto-1\\proyecto-1\\proyecto\\Resources\\PlayersBody\\Player1Body.png") },
                { 17, Image.FromFile("C:\\Users\\User\\desktop\\Datos 1\\proyecto-1\\proyecto-1\\proyecto\\Resources\\PlayersBody\\Player2Body.png") }
            };
        }

        private void InitializePlayers()
        {
            InitializePlayer1();
            InitializePlayer2();
        }

        private void InitializePlayer1()
        {
            Node? startNode = gameGrid.GetNode(5, 5);
            if (startNode == null)
            {
                Debug.WriteLine("No se pudo iniciar el player 1");
            }
            playerMoto1 = new Moto(startNode, 16, 3, 3, 14); // Nivel 3 de velocidad
        }

        private void InitializePlayer2()
        {
            Node? startNode = gameGrid.GetNode(9, 5);
            if (startNode == null)
            {
                Debug.WriteLine("No se pudo iniciar el player 2");
            }
            playerMoto2 = new Moto(startNode, 17, 5, 3, 15);  // Nivel 5 de velocidad
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
            }

            if (botStartNode2 != null)
            {
                BotMoto bot2 = new BotMoto(botStartNode2, 7, gameGrid, 0.1, 4, 3, 11); // Nivel 4 de velocidad
                bot2.ChangeDirection(Direction.Up);
                bots.Add(bot2);
            }

            if (botStartNode3 != null)
            {
                BotMoto bot3 = new BotMoto(botStartNode3, 8, gameGrid, 0.1, 2, 3, 12); // Nivel 2 de velocidad
                bot3.ChangeDirection(Direction.Right);
                bots.Add(bot3);
            }

            if (botStartNode4 != null)
            {
                BotMoto bot4 = new BotMoto(botStartNode4, 9, gameGrid, 0.1, 5, 3, 13); // Nivel 5 de velocidad
                bot4.ChangeDirection(Direction.Down);
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
                HandlePlayer2KeyDown(e);
            }
        }

        private void HandlePlayer1KeyDown(KeyEventArgs e)
        {
            if (playerMoto1 != null)
            {
                Direction newDirection = playerMoto1.CurrentDirection;

                switch (e.KeyCode)
                {
                    case Keys.Up:
                        newDirection = Direction.Up;
                        break;
                    case Keys.Down:
                        newDirection = Direction.Down;
                        break;
                    case Keys.Left:
                        newDirection = Direction.Left;
                        break;
                    case Keys.Right:
                        newDirection = Direction.Right;
                        break;
                }

                if (IsValidDirectionChange(playerMoto1.CurrentDirection, newDirection))
                {
                    playerMoto1.ChangeDirection(newDirection);
                }
            }
        }

        private void HandlePlayer2KeyDown(KeyEventArgs e)
        {
            if (playerMoto2 != null)
            {
                Direction newDirection = playerMoto2.CurrentDirection;

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

                if (IsValidDirectionChange(playerMoto2.CurrentDirection, newDirection))
                {
                    playerMoto2.ChangeDirection(newDirection);
                }
            }
        }

        private bool IsValidDirectionChange(Direction currentDirection, Direction newDirection)
        {
            return (currentDirection == Direction.Left && newDirection != Direction.Right) ||
                   (currentDirection == Direction.Right && newDirection != Direction.Left) ||
                   (currentDirection == Direction.Up && newDirection != Direction.Down) ||
                   (currentDirection == Direction.Down && newDirection != Direction.Up);
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
            if (playerMoto1 != null)
            {
                playerMoto1.StopTimer();
            }
            if (playerMoto2 != null)
            {
                playerMoto2.StopTimer();
            }

            foreach (var bot in bots)
            {
                bot.StopTimer();
            }

            base.OnFormClosing(e);
        }
    }
}

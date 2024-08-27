using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace proyecto
{
    public partial class Form1 : Form
    {
        private Grid gameGrid;
        private Moto playerMoto;
        private List<BotMoto> bots;
        private const int GridRows = 40;
        private const int GridColumns = 40;
        private PictureBox[,] pictureBoxes;
        private Dictionary<int, Image> imageCache;
        public Label powerMessageLabel;

        public Form1()
        {
            InitializeComponent();

            this.DoubleBuffered = true;

            gameGrid = new Grid(GridRows, GridColumns);
            pictureBoxes = new PictureBox[GridRows, GridColumns];
            bots = new List<BotMoto>();
            imageCache = LoadImageCache();

            InitializeGrid();
            InitializeUI();

            Node? startNode = gameGrid.GetNode(20, 20);
            if (startNode == null)
            {
                throw new InvalidOperationException("No se pudo encontrar un nodo válido en la posición inicial.");
            }
            playerMoto = new Moto(startNode, 1, 150, 3); // Reducción de velocidad para evitar sobrecarga

            InitializeBots();
            GenerateItemsAndPowers();

            this.KeyDown += new KeyEventHandler(Form1_KeyDown);
            this.Focus();

            RenderGrid();
        }

        private Dictionary<int, Image> LoadImageCache()
        {
            return new Dictionary<int, Image>
            {
                { 0, Image.FromFile("C:\\Users\\User\\desktop\\Datos 1\\proyecto-1\\proyecto-1\\proyecto\\Resources\\emptySpace.png") },
                { 1, Image.FromFile("C:\\Users\\User\\desktop\\Datos 1\\proyecto-1\\proyecto-1\\proyecto\\Resources\\principalBody.png") },
                { 2, Image.FromFile("C:\\Users\\User\\desktop\\Datos 1\\proyecto-1\\proyecto-1\\proyecto\\Resources\\botBody.png") },
                { 3, Image.FromFile("C:\\Users\\User\\desktop\\Datos 1\\proyecto-1\\proyecto-1\\proyecto\\Resources\\gas.png") },
                { 4, Image.FromFile("C:\\Users\\User\\desktop\\Datos 1\\proyecto-1\\proyecto-1\\proyecto\\Resources\\shield.png") },
                { 5, Image.FromFile("C:\\Users\\User\\desktop\\Datos 1\\proyecto-1\\proyecto-1\\proyecto\\Resources\\hiperVelocity.png") },
                { 6, Image.FromFile("C:\\Users\\User\\desktop\\Datos 1\\proyecto-1\\proyecto-1\\proyecto\\Resources\\principalHead.png") },
                { 7, Image.FromFile("C:\\Users\\User\\desktop\\Datos 1\\proyecto-1\\proyecto-1\\proyecto\\Resources\\BotHead.png") },
                { 8, Image.FromFile("C:\\Users\\User\\desktop\\Datos 1\\proyecto-1\\proyecto-1\\proyecto\\Resources\\bomb.png") },
                { 9, Image.FromFile("C:\\Users\\User\\desktop\\Datos 1\\proyecto-1\\proyecto-1\\proyecto\\Resources\\trailGrowth.png") }
            };
        }

        private void InitializeGrid()
        {
            int cellSize = 15;

            for (int row = 0; row < GridRows; row++)
            {
                for (int col = 0; col < GridColumns; col++)
                {
                    PictureBox cellBox = new PictureBox
                    {
                        Width = cellSize,
                        Height = cellSize,
                        Location = new Point(col * cellSize, row * cellSize),
                        BorderStyle = BorderStyle.FixedSingle,
                        SizeMode = PictureBoxSizeMode.CenterImage
                    };

                    pictureBoxes[row, col] = cellBox;
                    this.Controls.Add(cellBox);
                }
            }
        }

        private void InitializeUI()
        {
            powerMessageLabel = new Label
            {
                Location = new Point(GridColumns * 15 + 10, GridRows * 15 + 20),
                Size = new Size(200, 30),
                Text = "",
                ForeColor = Color.Black,
                BackColor = Color.White,
                AutoSize = true,
            };

            this.Controls.Add(powerMessageLabel);
        }

        private void InitializeBots()
        {
            Node? botStartNode1 = gameGrid.GetNode(2, 10);
            Node? botStartNode2 = gameGrid.GetNode(1, 20);
            Node? botStartNode3 = gameGrid.GetNode(5, 30);
            Node? botStartNode4 = gameGrid.GetNode(5, 5);

            if (botStartNode1 != null)
            {
                BotMoto bot1 = new BotMoto(botStartNode1, 2, gameGrid, 0.2, 300, 3);
                bot1.ChangeDirection(Direction.Down);
                bots.Add(bot1);
            }

            if (botStartNode2 != null)
            {
                BotMoto bot2 = new BotMoto(botStartNode2, 2, gameGrid, 0.4, 300, 3);
                bot2.ChangeDirection(Direction.Down);
                bots.Add(bot2);
            }

            if (botStartNode3 != null)
            {
                BotMoto bot3 = new BotMoto(botStartNode3, 2, gameGrid, 0.3, 300, 3);
                bot3.ChangeDirection(Direction.Down);
                bots.Add(bot3);
            }

            if (botStartNode4 != null)
            {
                BotMoto bot4 = new BotMoto(botStartNode4, 2, gameGrid, 0.1, 300, 3);
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
                                    currentNode.Value = 3; // Este valor debería ser el correspondiente al gas
                                    break;
                                case 2:
                                    currentNode.Value = 4;
                                    break;
                                case 3:
                                    currentNode.Value = 5;
                                    break;
                                case 4:
                                    currentNode.Value = 8;
                                    break;
                                case 5:
                                    currentNode.Value = 9;
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
            Direction newDirection = playerMoto.CurrentDirection;

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

            if (IsValidDirectionChange(playerMoto.CurrentDirection, newDirection))
            {
                playerMoto.ChangeDirection(newDirection);
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

                    // Verifica si cellBox.Image o cellBox.Image.Tag es null
                    if (cellBox.Image == null || cellBox.Image.Tag == null || (int)cellBox.Image.Tag != currentNode.Value)
                    {
                        Console.WriteLine($"Updating image at [{row},{col}] to value {currentNode.Value}"); // Línea de depuración
                        cellBox.Image = imageCache[currentNode.Value];
                        cellBox.Image.Tag = currentNode.Value;
                    }

                }
            }
        }


        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (playerMoto != null)
            {
                playerMoto.StopTimer();
            }

            foreach (var bot in bots)
            {
                bot.StopTimer();
            }

            base.OnFormClosing(e);
        }
    }
}

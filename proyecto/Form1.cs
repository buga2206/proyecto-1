using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace proyecto
{
    public partial class Form1 : Form
    {
        private Grid gameGrid;
        private Moto playerMoto;
        private List<BotMoto> bots; // Lista para almacenar los bots
        private const int GridRows = 40;
        private const int GridColumns = 40;
        private PictureBox[,] pictureBoxes; // Array para almacenar PictureBox de cada celda
        private System.Windows.Forms.Timer gameTimer; // Temporizador para mover la moto y los bots

        public Form1()
        {
            InitializeComponent();

            // Habilitar doble buffering para evitar parpadeo
            this.DoubleBuffered = true;

            gameGrid = new Grid(GridRows, GridColumns);
            pictureBoxes = new PictureBox[GridRows, GridColumns];
            bots = new List<BotMoto>(); // Inicializar la lista de bots

            // Inicializar la moto en una posición de inicio
            Node? startNode = gameGrid.GetNode(20, 20); // Posición inicial de la moto en el centro
            if (startNode == null)
            {
                throw new InvalidOperationException("No se pudo encontrar un nodo válido en la posición inicial.");
            }
            playerMoto = new Moto(startNode, 1); // La moto deja una estela con el valor 1

            // Inicializar bots en la parte superior del grid
            InitializeBots();

            // Suscribirse al evento KeyDown para manejar el cambio de dirección
            this.KeyDown += new KeyEventHandler(Form1_KeyDown);
            this.Focus(); // Asegurarse de que el formulario tenga el foco para capturar las teclas

            InitializeGrid();
            RenderGrid(); // Renderizar la malla inicial

            // Configurar el temporizador para mover la moto y los bots
            gameTimer = new System.Windows.Forms.Timer();
            gameTimer.Interval = 500; // Intervalo de 500 ms (0.5 segundos)
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();
        }

        private void InitializeBots()
        {
            // Crear bots en la parte superior del grid con separación
            Node? botStartNode1 = gameGrid.GetNode(0, 10);
            Node? botStartNode2 = gameGrid.GetNode(0, 20);
            Node? botStartNode3 = gameGrid.GetNode(0, 30);
            Node? botStartNode4 = gameGrid.GetNode(0, 5);

            if (botStartNode1 != null)
            {
                BotMoto bot1 = new BotMoto(botStartNode1, 2, gameGrid);
                bot1.ChangeDirection(Direction.Down); // Comenzar hacia abajo
                bots.Add(bot1);
            }

            if (botStartNode2 != null)
            {
                BotMoto bot2 = new BotMoto(botStartNode2, 2, gameGrid);
                bot2.ChangeDirection(Direction.Down); // Comenzar hacia abajo
                bots.Add(bot2);
            }

            if (botStartNode3 != null)
            {
                BotMoto bot3 = new BotMoto(botStartNode3, 2, gameGrid);
                bot3.ChangeDirection(Direction.Down); // Comenzar hacia abajo
                bots.Add(bot3);
            }

            if (botStartNode4 != null)
            {
                BotMoto bot4 = new BotMoto(botStartNode4, 2, gameGrid);
                bot4.ChangeDirection(Direction.Down); // Comenzar hacia abajo
                bots.Add(bot4);
            }
        }

        private void GameTimer_Tick(object? sender, EventArgs e)
        {
            // Mover la moto del jugador
            if (!playerMoto.Move())
            {
                MessageBox.Show("¡La moto ha chocado!"); // Manejar colisiones de la moto del jugador
                gameTimer.Stop(); // Detener el juego si el jugador choca
                return;
            }

            // Mover cada bot en cada tick del temporizador
            List<BotMoto> botsToRemove = new List<BotMoto>();
            foreach (BotMoto bot in bots)
            {
                if (!bot.MoveBot())
                {
                    // Si el bot choca, marcarlo para eliminación
                    botsToRemove.Add(bot);
                }
            }

            // Eliminar bots que han chocado
            foreach (BotMoto bot in botsToRemove)
            {
                bots.Remove(bot);
                // Puedes hacer algo adicional aquí, como mostrar un mensaje o animación
            }

            // Actualizar la malla después de mover la moto y los bots
            UpdateGrid();
        }

        private void InitializeGrid()
        {
            int cellSize = 15; // Tamaño de cada celda ajustado a 15x15 píxeles

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
                        SizeMode = PictureBoxSizeMode.CenterImage // Mantiene la imagen en su tamaño original y la centra
                    };
                    pictureBoxes[row, col] = cellBox; // Almacenar el PictureBox en la matriz
                    this.Controls.Add(cellBox);
                }
            }
        }

        private void Form1_KeyDown(object? sender, KeyEventArgs e)
        {
            // Cambiar la dirección de la moto según la tecla presionada
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

            // Solo permitir cambios de dirección válidos (evitar giros de 180 grados)
            if (IsValidDirectionChange(playerMoto.CurrentDirection, newDirection))
            {
                playerMoto.ChangeDirection(newDirection);
            }
        }

        private bool IsValidDirectionChange(Direction currentDirection, Direction newDirection)
        {
            // Evitar giros de 180 grados y mantener movimientos válidos
            return (currentDirection == Direction.Left && newDirection != Direction.Right) ||
                   (currentDirection == Direction.Right && newDirection != Direction.Left) ||
                   (currentDirection == Direction.Up && newDirection != Direction.Down) ||
                   (currentDirection == Direction.Down && newDirection != Direction.Up);
        }

        private void UpdateGrid()
        {
            // Recorrer todas las celdas y actualizar solo las que han cambiado
            for (int row = 0; row < GridRows; row++)
            {
                for (int col = 0; col < GridColumns; col++)
                {
                    Node? currentNode = gameGrid.GetNode(row, col);
                    if (currentNode == null) continue;

                    PictureBox cellBox = pictureBoxes[row, col];
                    string currentImage = cellBox.Image?.Tag?.ToString() ?? "";

                    string newImage = GetImageForValue(currentNode.Value);

                    if (currentImage != newImage)
                    {
                        cellBox.Image = Image.FromFile(newImage);
                        cellBox.Image.Tag = newImage; // Guardar el nombre de la imagen para comparaciones futuras
                    }
                }
            }
        }

        private string GetImageForValue(int value)
        {
            return value switch
            {
                0 => "C:\\Users\\User\\desktop\\Datos 1\\proyecto-1\\proyecto-1\\proyecto\\Resources\\emptySpace.png",
                1 => "C:\\Users\\User\\desktop\\Datos 1\\proyecto-1\\proyecto-1\\proyecto\\Resources\\principalBody.png",
                2 => "C:\\Users\\User\\desktop\\Datos 1\\proyecto-1\\proyecto-1\\proyecto\\Resources\\botBody.png",
                3 => "C:\\Users\\User\\desktop\\Datos 1\\proyecto-1\\proyecto-1\\proyecto\\Resources\\gas.png",
                4 => "C:\\Users\\User\\desktop\\Datos 1\\proyecto-1\\proyecto-1\\proyecto\\Resources\\shield.png",
                5 => "C:\\Users\\User\\desktop\\Datos 1\\proyecto-1\\proyecto-1\\proyecto\\Resources\\hiperVelocity.png",
                6 => "C:\\Users\\User\\desktop\\Datos 1\\proyecto-1\\proyecto-1\\proyecto\\Resources\\principalHead.png",
                _ => "C:\\Users\\User\\desktop\\Datos 1\\proyecto-1\\proyecto-1\\proyecto\\Resources\\emptySpace.png",
            };
        }

        private void RenderGrid()
        {
            for (int row = 0; row < GridRows; row++)
            {
                for (int col = 0; col < GridColumns; col++)
                {
                    Node? currentNode = gameGrid.GetNode(row, col);
                    if (currentNode == null) continue;

                    PictureBox cellBox = pictureBoxes[row, col];
                    string imagePath = GetImageForValue(currentNode.Value);
                    cellBox.Image = Image.FromFile(imagePath);
                    cellBox.Image.Tag = imagePath; // Guardar el nombre de la imagen para comparaciones futuras
                }
            }
        }
    }
}

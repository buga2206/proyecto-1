using System;
using System.Drawing;
using System.Windows.Forms;

namespace proyecto
{
    public partial class Form1 : Form
    {
        private Grid gameGrid;
        private Moto playerMoto;
        private const int GridRows = 10;
        private const int GridColumns = 10;
        private PictureBox[,] pictureBoxes; // Array para almacenar PictureBox de cada celda

        public Form1()
        {
            InitializeComponent();

            // Habilitar doble buffering para evitar parpadeo
            this.DoubleBuffered = true;

            gameGrid = new Grid(GridRows, GridColumns);
            pictureBoxes = new PictureBox[GridRows, GridColumns];

            // Inicializar la moto en una posición de inicio
            Node? startNode = gameGrid.GetNode(5, 5); // Posición inicial de la moto en el centro
            if (startNode == null)
            {
                throw new InvalidOperationException("No se pudo encontrar un nodo válido en la posición inicial.");
            }
            playerMoto = new Moto(startNode, 1); // La moto deja una estela con el valor 1

            // Suscribirse al evento KeyDown para manejar el movimiento
            this.KeyDown += new KeyEventHandler(Form1_KeyDown);
            this.Focus(); // Asegurarse de que el formulario tenga el foco para capturar las teclas

            InitializeGrid();
            RenderGrid(); // Renderizar la malla inicial
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
            switch (e.KeyCode)
            {
                case Keys.Up:
                    playerMoto.ChangeDirection(Direction.Up);
                    break;
                case Keys.Down:
                    playerMoto.ChangeDirection(Direction.Down);
                    break;
                case Keys.Left:
                    playerMoto.ChangeDirection(Direction.Left);
                    break;
                case Keys.Right:
                    playerMoto.ChangeDirection(Direction.Right);
                    break;
            }

            // Mover la moto y actualizar solo las celdas necesarias
            if (playerMoto.Move())
            {
                UpdateGrid(); // Redibujar solo las celdas cambiadas
            }
            else
            {
                MessageBox.Show("¡La moto ha chocado!"); // Manejar colisiones
            }
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

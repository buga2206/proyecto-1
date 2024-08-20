namespace proyecto
{
    public partial class Form1 : Form
    {
        private Grid gameGrid;
        private Moto playerMoto;
        private const int GridRows = 10;
        private const int GridColumns = 10;

        public Form1()
        {
            InitializeComponent();
            gameGrid = new Grid(GridRows, GridColumns);

            // Inicializar la moto en una posición de inicio
            Node startNode = gameGrid.GetNode(5, 5); // Posición inicial de la moto en el centro
            playerMoto = new Moto(startNode, 1); // La moto deja una estela con el valor 1

            // Suscribirse al evento KeyDown para manejar el movimiento
            this.KeyDown += new KeyEventHandler(Form1_KeyDown);
            this.Focus(); // Asegurarse de que el formulario tenga el foco para capturar las teclas

            RenderGrid();
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

            // Mover la moto y actualizar la malla
            if (playerMoto.Move())
            {
                RenderGrid(); // Redibujar el grid con la nueva posición de la moto
            }
            else
            {
                MessageBox.Show("¡La moto ha chocado!"); // Manejar colisiones
            }
        }

        private void RenderGrid()
        {
            this.Controls.Clear(); // Limpiar controles anteriores para redibujar la malla

            int cellSize = 15; // Tamaño de cada celda ajustado a 15x15 píxeles
            for (int row = 0; row < GridRows; row++)
            {
                for (int col = 0; col < GridColumns; col++)
                {
                    Node currentNode = gameGrid.GetNode(row, col);
                    PictureBox cellBox = new PictureBox
                    {
                        Width = cellSize,
                        Height = cellSize,
                        Location = new Point(col * cellSize, row * cellSize),
                        BorderStyle = BorderStyle.FixedSingle,
                        SizeMode = PictureBoxSizeMode.CenterImage // Mantiene la imagen en su tamaño original y la centra
                    };

                    // Asignar imagen según el valor del nodo
                    switch (currentNode.Value)
                    {
                        case 0:
                            cellBox.Image = Image.FromFile("C:\\Users\\User\\desktop\\Datos 1\\proyecto-1\\proyecto-1\\proyecto\\Resources\\emptySpace.png"); // Imagen para celda vacía
                            break;
                        case 1:
                            cellBox.Image = Image.FromFile("C:\\Users\\User\\desktop\\Datos 1\\proyecto-1\\proyecto-1\\proyecto\\Resources\\principalBody.png"); // Imagen para estela de la moto
                            break;
                        case 2:
                            cellBox.Image = Image.FromFile("C:\\Users\\User\\desktop\\Datos 1\\proyecto-1\\proyecto-1\\proyecto\\Resources\\botBody.png"); // Imagen para cuerpo del bot
                            break;
                        case 3:
                            cellBox.Image = Image.FromFile("C:\\Users\\User\\desktop\\Datos 1\\proyecto-1\\proyecto-1\\proyecto\\Resources\\gas.png"); // Imagen para gas
                            break;
                        case 4:
                            cellBox.Image = Image.FromFile("C:\\Users\\User\\desktop\\Datos 1\\proyecto-1\\proyecto-1\\proyecto\\Resources\\shield.png"); // Imagen para escudo
                            break;
                        case 5:
                            cellBox.Image = Image.FromFile("C:\\Users\\User\\desktop\\Datos 1\\proyecto-1\\proyecto-1\\proyecto\\Resources\\hiperVelocity.png"); // Imagen para hiper velocidad
                            break;
                        case 6:
                            cellBox.Image = Image.FromFile("C:\\Users\\User\\desktop\\Datos 1\\proyecto-1\\proyecto-1\\proyecto\\Resources\\principalHead.png"); // Imagen para la cabeza de la moto
                            break;
                    }

                    this.Controls.Add(cellBox);
                }
            }
        }


    }
}

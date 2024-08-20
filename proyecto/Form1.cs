namespace proyecto
{
    public partial class Form1 : Form
    {
        private Grid gameGrid;
        private const int GridRows = 10;
        private const int GridColumns = 10;

        public Form1()
        {
            InitializeComponent();
            gameGrid = new Grid(GridRows, GridColumns);
            RenderGrid();
        }

        private void RenderGrid()
        {
            int cellSize = 50; // Tamaño de cada celda
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
                        SizeMode = PictureBoxSizeMode.StretchImage // Esto asegura que la imagen se ajuste al tamaño de la celda
                    };

                    // Asignar imagen según el valor del nodo
                    switch (currentNode.Value)
                    {
                        case 0:
                            cellBox.Image = Image.FromFile("C:\\Users\\User\\desktop\\Datos 1\\proyecto-1\\proyecto-1\\proyecto\\Resources\\emptySpace.png"); // Cambia esta ruta a la ubicación de tu imagen
                            break;
                        case 1:
                            cellBox.Image = Image.FromFile("C:\\Users\\User\\desktop\\Datos 1\\proyecto-1\\proyecto-1\\proyecto\\Resources\\principalBody.png"); // Cambia esta ruta a la ubicación de tu imagen
                            break;
                        case 2:
                            cellBox.Image = Image.FromFile("C:\\Users\\User\\desktop\\Datos 1\\proyecto-1\\proyecto-1\\proyecto\\Resources\\botBody.png"); // Cambia esta ruta a la ubicación de tu imagen
                            break;
                        case 3:
                            cellBox.Image = Image.FromFile("C:\\Users\\User\\desktop\\Datos 1\\proyecto-1\\proyecto-1\\proyecto\\Resources\\gas.png"); // Cambia esta ruta a la ubicación de tu imagen
                            break;
                        case 41:
                            cellBox.Image = Image.FromFile("C:\\Users\\User\\desktop\\Datos 1\\proyecto-1\\proyecto-1\\proyecto\\Resources\\shield.png"); // Cambia esta ruta a la ubicación de tu imagen
                            break;
                        case 5:
                            cellBox.Image = Image.FromFile("C:\\Users\\User\\desktop\\Datos 1\\proyecto-1\\proyecto-1\\proyecto\\Resources\\hiperVelocity.png"); // Cambia esta ruta a la ubicación de tu imagen
                            break;
                            // Puedes agregar más casos para otros valores si es necesario
                    }

                    this.Controls.Add(cellBox);
                }
            }
        }
    }
}

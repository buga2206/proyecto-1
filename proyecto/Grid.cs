namespace proyecto
{
    public class Node
    {
        public Node? Up { get; set; }
        public Node? Down { get; set; }
        public Node? Left { get; set; }
        public Node? Right { get; set; }

        public int Value { get; set; } // Valores de 0 a 5 que representan el estado del nodo

        public Node(int value)
        {
            Value = value;
        }
    }

    public class Grid
    {
        private Node[,] nodes;
        private int rows;
        private int columns;

        public Grid(int rows, int columns)
        {
            this.rows = rows;
            this.columns = columns;
            nodes = new Node[rows, columns];
            InitializeGrid();
        }

        private void InitializeGrid()
        {
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    nodes[row, col] = new Node(0); // Inicialmente todos vacíos (valor 0)
                }
            }

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    if (row > 0)
                        nodes[row, col].Up = nodes[row - 1, col];
                    if (row < rows - 1)
                        nodes[row, col].Down = nodes[row + 1, col];
                    if (col > 0)
                        nodes[row, col].Left = nodes[row, col - 1];
                    if (col < columns - 1)
                        nodes[row, col].Right = nodes[row, col + 1];
                }
            }
        }

        public Node? GetNode(int row, int col)
        {
            if (row >= 0 && row < rows && col >= 0 && col < columns)
            {
                return nodes[row, col];
            }
            return null; // Devuelve null si las coordenadas están fuera del rango
        }
    }
}

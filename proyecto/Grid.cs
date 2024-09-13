namespace proyecto
{
    public class Node
    {
        public Node? Up;
        public Node? Down;
        public Node? Left;
        public Node? Right;

        public int Value;

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
                    nodes[row, col] = new Node(0);
                }
            }

            SetNodeReferences();
        }

        private void SetNodeReferences()
        {
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

        public void ResetGrid()
        {
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    nodes[row, col].Value = 0;
                }
            }
        }

        public Node? GetNode(int row, int col)
        {
            if (row >= 0 && row < rows && col >= 0 && col < columns)
            {
                return nodes[row, col];
            }
            return null;
        }
    }


}
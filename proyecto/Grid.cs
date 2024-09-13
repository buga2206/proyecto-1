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
            for (int row = 0; row < rows; row++)  // Recorre cada fila
            {
                for (int col = 0; col < columns; col++)  // Recorre cada columna
                {
                    nodes[row, col] = new Node(0);  // Crea un nodo en la posición con valor inicial 0
                }
            }

            SetNodeReferences();  // Establece las referencias entre nodos adyacentes
        }

        private void SetNodeReferences() 
        {
            for (int row = 0; row < rows; row++)  // Recorre cada fila
            {
                for (int col = 0; col < columns; col++)  // Recorre cada columna
                {
                    if (row > 0)  // Si no es la primera fila, asigna el nodo de arriba
                        nodes[row, col].Up = nodes[row - 1, col];
                    if (row < rows - 1)  // Si no es la última fila, asigna el nodo de abajo
                        nodes[row, col].Down = nodes[row + 1, col];
                    if (col > 0)  // Si no es la primera columna, asigna el nodo de la izquierda
                        nodes[row, col].Left = nodes[row, col - 1];
                    if (col < columns - 1)  // Si no es la última columna, asigna el nodo de la derecha
                        nodes[row, col].Right = nodes[row, col + 1];
                }
            }
        }

        public void ResetGrid() 
        {
            for (int row = 0; row < rows; row++)  // Recorre cada fila
            {
                for (int col = 0; col < columns; col++)  // Recorre cada columna
                {
                    nodes[row, col].Value = 0;  // Restablece el valor del nodo a 0
                }
            }
        }

        public Node? GetNode(int row, int col)
        {
            if (row >= 0 && row < rows && col >= 0 && col < columns)  // Verifica si la posición está dentro de los límites de la cuadrícula
            {
                return nodes[row, col];  // Retorna el nodo en la posición especificada
            }
            return null;  // Si la posición no es válida, retorna null
        }
    }
}
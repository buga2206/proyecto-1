using System; 

namespace proyecto 
{
    internal class SNode<T>  
    {
        internal T Value;  
        internal SNode<T>? Next; 

        public SNode(T value) 
        {
            this.Value = value; 
            this.Next = null; 
        }
    }

    public interface IStack<T> 
    {
        void Push(T value); 
        T Pop();
        T Peek();  
        T Peek(int index);  
        int Count();  
        void Clear();  
    }

    public class OwnStackList<T> : IStack<T> 
    {
        private SNode<T>? top;  // Referencia al nodo superior de la pila

        public OwnStackList() 
        {
            this.top = null;  
        }

        public int Count()  
        {
            int count = 0;  
            SNode<T>? current = this.top;  
            while (current != null)  // Recorre la pila hasta que no haya más nodos
            {
                count++; 
                current = current.Next;
            }
            return count;  // Retorna el número total de elementos
        }

        public void Push(T value)  
        {
            SNode<T> newNode = new SNode<T>(value); 
            newNode.Next = this.top;  // El nuevo nodo apunta al nodo superior actual
            this.top = newNode;  // El nuevo nodo se convierte en el nodo superior
        }

        public T Pop() 
        {
            if (this.top == null)  
            {
                throw new InvalidOperationException("Stack is empty.");
            }

            T value = this.top.Value;  // Obtiene el valor del nodo superior
            this.top = this.top.Next;  // Actualiza el nodo superior al siguiente en la pila
            return value;  // Retorna el valor del nodo que se eliminó
        }

        public T Peek() 
        {
            if (this.top == null) 
            {
                throw new InvalidOperationException("Stack is empty."); 
            }

            return this.top.Value;  // Retorna el valor del nodo superior
        }

        public T Peek(int index) 
        {
            if (index < 0 || index >= Count())  // Verifica si el índice está fuera de los límites
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
            }

            SNode<T>? current = this.top; 
            for (int i = 0; i < index; i++)  // Recorre la pila hasta llegar al índice deseado
            {
                if (current == null)  // Verifica si hay menos nodos de los esperados
                {
                    throw new InvalidOperationException("Stack is shorter than expected."); 
                }
                current = current.Next;
            }

            return current!.Value;  // Retorna el valor en la posición especificada
        }

        public void Clear() 
        {
            this.top = null; 
        }
    }
}
using System;

namespace proyecto 
{
    internal class QNode<T> 
    {
        internal T Value; 
        internal QNode<T>? Next; 

        public QNode(T value)  
        {
            this.Value = value;  
            this.Next = null;  
        }
    }

    public interface IQueue<T>  
    {
        void Enqueue(T value); 
        T Dequeue(); 
        T Peek(); 
        int Count();  
        void Clear(); 
    }

    public class OwnQueueList<T> : IQueue<T> 
    {
        private QNode<T>? front;  // Referencia al nodo frontal de la cola
        private QNode<T>? rear;  // Referencia al nodo final de la cola

        public OwnQueueList()  
        {
            this.front = this.rear = null;  // Inicialmente, tanto el frente como el final están vacíos
        }

        public int Count() 
        {
            int count = 0; 
            QNode<T>? current = this.front;
            while (current != null)  // Recorre la cola hasta el final
            {
                count++; 
                current = current.Next;  // Avanza al siguiente nodo
            }
            return count; 
        }

        public void Enqueue(T value) 
        {
            QNode<T> newNode = new QNode<T>(value);

            if (this.rear == null)  // Si la cola está vacía, el nuevo nodo es tanto el frente como el final
            {
                this.front = this.rear = newNode; 
                return;
            }

            this.rear.Next = newNode;  // Enlaza el nuevo nodo al final de la cola
            this.rear = newNode;  // Actualiza la referencia del final al nuevo nodo
        }

        public T Dequeue() 
        {
            if (this.front == null) 
            {
                throw new InvalidOperationException("Queue is empty.");
            }

            QNode<T> temp = this.front;  // Almacena temporalmente el nodo frontal
            this.front = this.front.Next;  // Avanza el frente al siguiente nodo

            if (this.front == null)  // Si la cola está vacía después de quitar el nodo, también actualiza el final a null
            {
                this.rear = null;
            }

            return temp.Value;  // Retorna el valor del nodo que se eliminó
        }

        public T Peek()
        {
            if (this.front == null) 
            {
                throw new InvalidOperationException("Queue is empty.");
            }

            return this.front.Value;  // Retorna el valor del nodo frontal
        }

        public void Clear()
        {
            this.front = this.rear = null;  // Elimina todos los nodos 
        }
    }
}
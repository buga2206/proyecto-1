using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace proyecto
{
    internal class ONode<T>
    {
        internal T Value;
        internal ONode<T>? Next;

        public ONode(T value)
        {
            this.Value = value;
            this.Next = null;
        }
    }

    public interface ILista<T>
    {
        void AddFirst(T value);
        void AddLast(T value);
        void RemoveFirst();
        void RemoveLast();
        void RemoveAt(int index);
        void Clear();
        int Count();
        void Print();
        T this[int index] { get; }
    }

    public class OwnLinkedList<T> : ILista<T>, IEnumerable<T>
    {
        private ONode<T>? first;  // Referencia al primer nodo de la lista

        public OwnLinkedList()
        {
            this.first = null;
        }

        public int Count()
        {
            int count = 0;
            ONode<T>? current = this.first;
            while (current != null)  // Recorre la lista hasta el final
            {
                count++;
                current = current.Next;  // Avanza al siguiente nodo
            }
            return count;
        }

        public void AddFirst(T value)
        {
            ONode<T> newNode = new ONode<T>(value);
            newNode.Next = this.first;  // El nuevo nodo apunta al nodo actualmente primero
            this.first = newNode;  // El nuevo nodo se convierte en el primero
        }

        public void AddLast(T value)
        {
            if (this.first == null)  // Si la lista está vacía, agrega el nodo como el primero
            {
                this.first = new ONode<T>(value);
                return;
            }
            ONode<T> current = this.first;
            while (current.Next != null)  // Recorre la lista hasta el último nodo
            {
                current = current.Next;
            }
            current.Next = new ONode<T>(value);  // Agrega el nuevo nodo al final de la lista
        }

        public void Clear()
        {
            this.first = null;  // Establece la referencia del primer nodo a null, eliminando todos los nodos
        }

        public void RemoveFirst()
        {
            if (this.first == null)
            {
                return;
            }
            this.first = this.first.Next;  // El primer nodo ahora es el siguiente nodo
        }

        public void RemoveLast()
        {
            if (this.first == null)
            {
                return;
            }

            if (this.first.Next == null)  // Si solo hay un nodo, elimina la lista
            {
                this.first = null;
                return;
            }

            ONode<T> current = this.first;
            while (current.Next?.Next != null)  // Recorre hasta el penúltimo nodo
            {
                current = current.Next;
            }
            current.Next = null;  // Elimina la referencia al último nodo, eliminando el nodo
        }

        public void RemoveAt(int index)
        {
            if (index < 0)  // Verifica si el índice es válido
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index out of range.");
            }

            if (index == 0)  // Si el índice es 0, elimina el primer nodo
            {
                RemoveFirst();
                return;
            }

            ONode<T>? current = this.first;
            ONode<T>? previous = null;

            for (int i = 0; i < index; i++)  // Recorre hasta llegar al nodo en la posición deseada
            {
                if (current == null)  // Si el índice está fuera de rango, lanza una excepción
                {
                    throw new ArgumentOutOfRangeException(nameof(index), "Index out of range.");
                }

                previous = current;  // Guarda el nodo anterior
                current = current.Next;  // Avanza al siguiente nodo
            }

            if (previous != null && current != null)  // Si se encontraron los nodos, elimina el nodo en la posición
            {
                previous.Next = current.Next;  // Salta el nodo actual, eliminandolo
            }
        }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= Count())  // Verifica si el índice está dentro de los 
                {
                    throw new ArgumentOutOfRangeException(nameof(index), "Index out of range.");
                }

                ONode<T>? current = first;
                for (int i = 0; i < index; i++)  // Recorre hasta llegar al nodo en el índice deseado
                {
                    if (current == null)  // Si el nodo es null, lanza una excepción
                    {
                        throw new ArgumentOutOfRangeException(nameof(index), "Index out of range.");
                    }
                    current = current.Next;  // Avanza al siguiente nodo
                }

                if (current == null)  // Si no hay nodo en esa posición, lanza una excepción
                {
                    throw new InvalidOperationException("Node is unexpectedly null.");
                }

                return current.Value;  // Retorna el valor del nodo en la posición especificada
            }
        }

        public void Print()
        {
            ONode<T>? current = this.first;
            while (current != null)  // Recorre cada nodo de la lista
            {
                Debug.WriteLine(current.Value);  // Imprime el valor del nodo
                current = current.Next;  // Avanza al siguiente nodo
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            ONode<T>? current = this.first;
            while (current != null)  // Recorre cada nodo
            {
                yield return current.Value;  // Retorna el valor actual
                current = current.Next;  // Avanza al siguiente nodo
            }
        }

        IEnumerator IEnumerable.GetEnumerator()  // Implementación de IEnumerable para compatibilidad
        {
            return GetEnumerator();  // Retorna el enumerador genérico
        }
    }
    
}
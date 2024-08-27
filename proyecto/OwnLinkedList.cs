using System;

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
        T this[int index] { get; } // Agregar definición del indexador en la interfaz
    }

    public class OwnLinkedList<T> : ILista<T>
    {
        private ONode<T>? first;

        public OwnLinkedList()
        {
            this.first = null;
        }

        public int Count()
        {
            int count = 0;
            ONode<T>? current = this.first;
            while (current != null)
            {
                count++;
                current = current.Next;
            }
            return count;
        }

        public void AddFirst(T value)
        {
            ONode<T> newNode = new ONode<T>(value);
            newNode.Next = this.first;
            this.first = newNode;
        }

        public void AddLast(T value)
        {
            if (this.first == null)
            {
                this.first = new ONode<T>(value);
                return;
            }
            ONode<T> current = this.first;
            while (current.Next != null)
            {
                current = current.Next;
            }
            current.Next = new ONode<T>(value);
        }

        public void Clear()
        {
            this.first = null;
        }

        public void RemoveFirst()
        {
            if (this.first == null)
            {
                return;
            }
            this.first = this.first.Next;
        }

        public void RemoveLast()
        {
            if (this.first == null)
            {
                return;
            }

            if (this.first.Next == null)
            {
                this.first = null;
                return;
            }

            ONode<T> current = this.first;
            while (current.Next?.Next != null)
            {
                current = current.Next;
            }
            current.Next = null;
        }

        public void RemoveAt(int index)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index out of range.");
            }

            if (index == 0)
            {
                RemoveFirst();
                return;
            }

            ONode<T>? current = this.first;
            ONode<T>? previous = null;

            for (int i = 0; i < index; i++)
            {
                if (current == null)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), "Index out of range.");
                }

                previous = current;
                current = current.Next;
            }

            if (previous != null && current != null)
            {
                previous.Next = current.Next;
            }
        }

        public T this[int index] // Implementación del indexador
        {
            get
            {
                if (index < 0 || index >= Count())
                {
                    throw new ArgumentOutOfRangeException(nameof(index), "Index out of range.");
                }

                ONode<T>? current = first;
                for (int i = 0; i < index; i++)
                {
                    if (current == null)
                    {
                        throw new ArgumentOutOfRangeException(nameof(index), "Index out of range.");
                    }
                    current = current.Next;
                }

                if (current == null)
                {
                    throw new InvalidOperationException("Node is unexpectedly null.");
                }

                return current.Value;
            }
        }

        public void Print()
        {
            ONode<T>? current = this.first;
            while (current != null)
            {
                Console.WriteLine(current.Value);
                current = current.Next;
            }
        }
    }
}
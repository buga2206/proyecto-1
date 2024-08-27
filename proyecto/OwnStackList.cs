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
        T Peek(int index); // Nuevo método para obtener un elemento en un índice específico
        int Count();
        void Clear();
        void Swap(int index1, int index2);
    }

    public class OwnStackList<T> : IStack<T>
    {
        private SNode<T>? top;

        public OwnStackList()
        {
            this.top = null;
        }

        public int Count()
        {
            int count = 0;
            SNode<T>? current = this.top;
            while (current != null)
            {
                count++;
                current = current.Next;
            }
            return count;
        }

        public void Push(T value)
        {
            SNode<T> newNode = new SNode<T>(value);
            newNode.Next = this.top;
            this.top = newNode;
        }

        public T Pop()
        {
            if (this.top == null)
            {
                throw new InvalidOperationException("Stack is empty.");
            }

            T value = this.top.Value;
            this.top = this.top.Next;
            return value;
        }

        public T Peek()
        {
            if (this.top == null)
            {
                throw new InvalidOperationException("Stack is empty.");
            }

            return this.top.Value;
        }

        public T Peek(int index)
        {
            if (index < 0 || index >= Count())
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
            }

            SNode<T>? current = this.top;
            for (int i = 0; i < index; i++)
            {
                if (current == null)
                {
                    throw new InvalidOperationException("Stack is shorter than expected.");
                }
                current = current.Next;
            }

            return current!.Value; // Aquí se garantiza que current no es null debido a la verificación previa.
        }

        public void Clear()
        {
            this.top = null;
        }

        public void Swap(int index1, int index2)
        {
            if (index1 < 0 || index2 < 0 || index1 >= Count() || index2 >= Count())
            {
                throw new ArgumentOutOfRangeException("Index out of range.");
            }

            if (index1 == index2) return; // No need to swap if indices are the same

            SNode<T>? node1 = top;
            SNode<T>? node2 = top;
            SNode<T>? prevNode1 = null;
            SNode<T>? prevNode2 = null;

            for (int i = 0; i < index1; i++)
            {
                prevNode1 = node1;
                node1 = node1?.Next;
            }

            for (int i = 0; i < index2; i++)
            {
                prevNode2 = node2;
                node2 = node2?.Next;
            }

            if (node1 == null || node2 == null) return;

            if (prevNode1 != null)
            {
                prevNode1.Next = node2;
            }
            else
            {
                top = node2;
            }

            if (prevNode2 != null)
            {
                prevNode2.Next = node1;
            }
            else
            {
                top = node1;
            }

            // Swap the next pointers
            SNode<T>? temp = node1.Next;
            node1.Next = node2.Next;
            node2.Next = temp;
        }
    }
}

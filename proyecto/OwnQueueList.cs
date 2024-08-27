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
        private QNode<T>? front;
        private QNode<T>? rear;

        public OwnQueueList()
        {
            this.front = this.rear = null;
        }

        public int Count()
        {
            int count = 0;
            QNode<T>? current = this.front;
            while (current != null)
            {
                count++;
                current = current.Next;
            }
            return count;
        }

        public void Enqueue(T value)
        {
            QNode<T> newNode = new QNode<T>(value);

            if (this.rear == null)
            {
                this.front = this.rear = newNode;
                return;
            }

            this.rear.Next = newNode;
            this.rear = newNode;
        }

        public T Dequeue()
        {
            if (this.front == null)
            {
                throw new InvalidOperationException("Queue is empty.");
            }

            QNode<T> temp = this.front;
            this.front = this.front.Next;

            if (this.front == null)
            {
                this.rear = null;
            }

            return temp.Value;
        }

        public T Peek()
        {
            if (this.front == null)
            {
                throw new InvalidOperationException("Queue is empty.");
            }

            return this.front.Value;
        }

        public void Clear()
        {
            this.front = this.rear = null;
        }
    }
}

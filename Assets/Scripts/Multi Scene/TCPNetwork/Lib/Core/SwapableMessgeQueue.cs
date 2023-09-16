using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;

namespace MNF
{
    public class SwapableMessgeQueue<T>
    {
        private Queue<T>[] messageQueue;
        private int writableQueueIndex;
        private Queue<T> writableQueue;
        private Queue<T> readableQueue;

        public SwapableMessgeQueue()
        {
            messageQueue = new Queue<T>[2];
            messageQueue[0] = new Queue<T>();
            messageQueue[1] = new Queue<T>();

            writableQueueIndex = 0;
            writableQueue = messageQueue[0];
            readableQueue = messageQueue[1];
        }

        public void swap()
        {
            if (writableQueueIndex == 0)
            {
                writableQueueIndex = 1;
                writableQueue = messageQueue[1];
                readableQueue = messageQueue[0];
            }
            else
            {
                writableQueueIndex = 0;
                writableQueue = messageQueue[0];
                readableQueue = messageQueue[1];
            }
        }

        public Queue<T> getWritableQueue()
        {
            return writableQueue;
        }

        public Queue<T> getReadableQueue()
        {
            return readableQueue;
        }
    }
}

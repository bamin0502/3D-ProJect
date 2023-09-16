using System;
using System.Diagnostics;

namespace MNF
{
    public class CircularBuffer
    {
        private byte[] circularBuffer = null;
        private int head = 0;
        private int tail = 0;
        private int appendSize = 0;

        public int Head { get { return head; } }
        public int Tail { get { return tail; } }
        public int ReadableSize { get { return appendSize; } }

        public CircularBuffer(int circularBufferSize)
        {
            this.circularBuffer = new byte[circularBufferSize];

#if DEBUG
            for (int i = 0; i < this.circularBuffer.Length; ++i)
                this.circularBuffer[i] = 0xFF;
#endif
        }

        public void clear()
        {
            head = 0;
            tail = 0;
            appendSize = 0;
        }

        public int capacity()
        {
            return circularBuffer.Length;
        }

        public bool push(byte[] pushData, int pushSize)
        {
            int pushPossibleSize = capacity() - tail;
            int remainCopySize = pushSize - pushPossibleSize;
            try
            {
                if (pushSize == 0)
                    throw new Exception("Push size is zero");

                if (isFull(pushSize) == true)
                    throw new Exception("Circular buffer is full");

                if (pushPossibleSize >= pushSize)
                {
                    Buffer.BlockCopy(pushData, 0, circularBuffer, tail, pushSize);
                }
                else
                {
                    if (remainCopySize < 0)
                        throw new Exception("remainCopySize is minus");

                    Buffer.BlockCopy(pushData, 0, circularBuffer, tail, pushPossibleSize);
                    Buffer.BlockCopy(pushData, pushPossibleSize, circularBuffer, 0, remainCopySize);
                }
                appendSize += pushSize;
                tail = (tail + pushSize) % circularBuffer.Length;
                return true;
            }
            catch (Exception e)
            {
                LogManager.Instance.WriteException(e, "push data({0}) failed", pushSize);
                return false;
            }
        }

        public bool push(byte[] pushData, int pushDataIndex, int pushSize)
        {
            int pushPossibleSize = capacity() - tail;
            try
            {
                if (pushSize == 0)
                    throw new Exception("Push size is zero");

                if (isFull(pushSize) == true)
                    throw new Exception("Circular buffer is full");

                if (pushPossibleSize >= pushSize)
                {
                    Buffer.BlockCopy(pushData, pushDataIndex, circularBuffer, tail, pushSize);
                }
                else
                {
                    if (pushPossibleSize <= 0)
                        throw new Exception("pushPossibleSize is minus");

                    Buffer.BlockCopy(pushData, pushDataIndex, circularBuffer, tail, pushPossibleSize);
                    Buffer.BlockCopy(pushData, pushDataIndex + pushPossibleSize, circularBuffer, 0, pushSize - pushPossibleSize);
                }
                appendSize += pushSize;
                tail = (tail + pushSize) % circularBuffer.Length;
                return true;
            }
            catch (Exception e)
            {
                LogManager.Instance.WriteException(e, "push data({0}) failed", pushSize);
                return false;
            }
        }

        public bool read(byte[] readData, int readSize)
        {
            try
            {
                if (isEmpty() == true)
                    throw new Exception("Readable buffer is zero");

                if (readSize > ReadableSize)
                    throw new Exception("Request invalid readSize");

                int readPossibleSize = capacity() - head;
                if (readPossibleSize >= readSize)
                {
                    Buffer.BlockCopy(circularBuffer, head, readData, 0, readSize);
                }
                else
                {
                    Buffer.BlockCopy(circularBuffer, head, readData, 0, readPossibleSize);
                    Buffer.BlockCopy(circularBuffer, 0, readData, readPossibleSize, readSize - readPossibleSize);
                }
                return true;
            }
            catch (Exception e)
            {
                LogManager.Instance.WriteException(e, "CircularBuffer readable({0}) request readsize({1})",
                    ReadableSize, readSize);
                return false;
            }
        }

        public bool readToTargetIndex(byte[] readData, int readSize, int targetIndex)
        {
            try
            {
                if (isEmpty() == true)
                    throw new Exception("Readable buffer is zero");

                if (readSize > ReadableSize)
                    throw new Exception("Request invalid readSize");

                int readPossibleSize = capacity() - head;
                if (readPossibleSize >= readSize)
                {
                    Buffer.BlockCopy(circularBuffer, head, readData, targetIndex, readSize);
                }
                else
                {
                    Buffer.BlockCopy(circularBuffer, head, readData, targetIndex, readPossibleSize);
                    Buffer.BlockCopy(circularBuffer, 0, readData, targetIndex + readPossibleSize, readSize - readPossibleSize);
                }
                return true;
            }
            catch (Exception e)
            {
                LogManager.Instance.WriteException(e, "CircularBuffer readable({0}) request readsize({1})",
                    ReadableSize, readSize);
                return false;
            }
        }

        public bool read(byte[] readData, int beginIndex, int readSize)
        {
            try
            {
                if (isEmpty() == true)
                    throw new Exception("Readable buffer is zero");

                if (readSize > ReadableSize)
                    throw new Exception("Request invalid readSize");

                int readIndex = (head + beginIndex) % capacity();
                int readPossibleSize = capacity() - readIndex;
                if (readPossibleSize >= readSize)
                {
                    Buffer.BlockCopy(circularBuffer, readIndex, readData, 0, readSize);
                }
                else
                {
                    Buffer.BlockCopy(circularBuffer, readIndex, readData, 0, readPossibleSize);
                    Buffer.BlockCopy(circularBuffer, 0, readData, readPossibleSize, readSize - readPossibleSize);
                }
                return true;
            }
            catch (Exception e)
            {
                LogManager.Instance.WriteException(e, "CircularBuffer readable({0}) request readsize({1})",
                    ReadableSize, readSize);
                return false;
            }
        }

        public bool pop(int popSize)
        {
            try
            {
                if (popSize == 0)
                    throw new Exception("Request popSize is zero");

                if (isEmpty() == true)
                    throw new Exception("Pop buffer is empty");

                if (ReadableSize < popSize)
                    throw new Exception("Request popSize invalid");

#if DEBUG
                if (head + popSize < circularBuffer.Length)
                {
                    for (int i = head; i < popSize; ++i)
                        this.circularBuffer[i] = 0xFF;
                }
                else
                {
                    for (int i = head; i < circularBuffer.Length; ++i)
                        this.circularBuffer[i] = 0xFF;
                    int remainPopSize = popSize - (circularBuffer.Length - head);
                    for (int i = 0; i < remainPopSize; ++i)
                        this.circularBuffer[i] = 0xFF;
                }
#endif
                appendSize -= popSize;
                head = (head + popSize) % capacity();
                return true;
            }
            catch (Exception e)
            {
                LogManager.Instance.WriteException(e, "CircularBuffer pop able({0}) request popSize({1})",
                    ReadableSize, popSize);
                return false;
            }
        }

        public bool isFull(int pushSize)
        {
            bool checkValue = (appendSize + pushSize) > capacity();
            return checkValue;
        }

        public bool isEmpty()
        {
            return (appendSize == 0);
        }
    }
}
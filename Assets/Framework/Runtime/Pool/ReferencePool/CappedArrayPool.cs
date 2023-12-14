using EGFramework.Runtime.Pool;
using System;

namespace EGFramework.Runtime.Pool
{
    public sealed class CappedArrayPool<T>: IReference
    {
        public static readonly T[] EmptyArray = new T[0];

        private const int InitialBucketSize = 4;
        private T[][][] buckets;
        private object syncRoot = new object();
        private int[] tails;

        public int Length => buckets.Length;

        public CappedArrayPool()
        {
        }

        public CappedArrayPool(int maxLength)
        {
            Init(maxLength);
        }

        public void Init(int maxLength)
        {
            buckets = new T[maxLength][][];
            tails = new int[maxLength];
            for (var i = 0; i < maxLength; i++)
            {
                var arrayLength = i + 1;
                buckets[i] = new T[InitialBucketSize][];
                for (var j = 0; j < InitialBucketSize; j++)
                {
                    buckets[i][j] = new T[arrayLength];
                }
                tails[i] = buckets[i].Length - 1;
            }
        }

        public T[] Rent(int length)
        {
            if (length <= 0)
                return EmptyArray;

            if (length > buckets.Length)
                return new T[length]; // Not supported

            var i = length - 1;

            lock (syncRoot)
            {
                var bucket = buckets[i];
                var tail = tails[i];
                if (tail >= bucket.Length)
                {
                    Array.Resize(ref bucket, bucket.Length * 2);
                    buckets[i] = bucket;
                }

                var result = bucket[tail] ?? new T[length];
                tails[i] += 1;
                return result;
            }
        }

        public void Return(T[] array)
        {
            if (array.Length <= 0 || array.Length > buckets.Length)
                return;

            var i = array.Length - 1;
            lock (syncRoot)
            {
                if (tails[i] > 0)
                    tails[i] -= 1;
            }
        }

        public void Clear()
        {
            buckets = null;
            tails = null;
        }
    }
}
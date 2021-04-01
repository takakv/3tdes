using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace TripleDES.Crypto
{
    [SuppressMessage("ReSharper", "SuggestVarOrType_SimpleTypes")]
    [SuppressMessage("ReSharper", "SuggestVarOrType_BuiltInTypes")]
    public sealed class BitStream
    {
        public BitStream(int length)
        {
            if (length < 0) throw new ArgumentOutOfRangeException(nameof(length));

            Bits = new bool[length];
            Count = length;
        }

        public BitStream(byte[] bytes)
        {
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));

            int byteCount = bytes.Length;
            int bitCount = byteCount * 8;

            BitArray bits = new BitArray(bytes);
            bool[] orderedBits = new bool[bitCount];
            bits.CopyTo(orderedBits, 0);

            Bits = new bool[bitCount];
            Count = bitCount;

            // Order the bits
            for (int byteIndex = 0, bitIndex = 0; byteIndex < byteCount; ++byteIndex)
            for (int i = 7; i >= 0; --i)
                Bits[bitIndex++] = orderedBits[byteIndex * 8 + i];
        }

        public BitStream(bool[] bits)
        {
            if (bits == null) throw new ArgumentNullException(nameof(bits));

            Bits = new bool[bits.Length];
            Array.Copy(bits, Bits, bits.Length);
            Count = bits.Length;
        }

        public bool this[int index]
        {
            get => Bits[index];
            set => Bits[index] = value;
        }

        public int Count { get; }

        public bool[] Bits { get; private set; }

        public BitStream LeftShift(int count)
        {
            if (count <= 0)
            {
                if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
                return this;
            }

            if (count < Count)
                for (int i = 0; i < Count; ++i)
                    Bits[i] = i + count < Count && Bits[i + count];
            else
                Bits = new bool[Count];

            return this;
        }
    }
}
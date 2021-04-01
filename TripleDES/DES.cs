using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

// 3-key triple DES (3TDES).
namespace TripleDES
{
    public static class DES
    {
        // DES uses 64 bit blocks, which is 8 bytes.
        private const int BlockSize = 8;

        private static void Main(string[] args)
        {
            BitArray permutedKeyBits = GetPermutedKey(GetKey());

            // Get the 16 subkeys for each of the DES rounds.
            var subKeys = new BitArray[16];
            for (var i = 0; i < 16; ++i)
            {
                // Each shift is applied to the result of the previous round.
                permutedKeyBits = GetSubKey(permutedKeyBits, i);
                subKeys[i] = permutedKeyBits;
            }

            PermuteSubKeys(ref subKeys);

            string plaintext = GetPlaintext();
            byte[] plaintextBytes = GetPlaintextBytes(plaintext);
            int blockCount = plaintextBytes.Length / BlockSize;
            BitArray[] plaintextBlocks = GetPlaintextBlocks(plaintextBytes, blockCount);
            PermuteAllBlocks(ref plaintextBlocks, true);
        }

        private static BitArray[] GetPlaintextBlocks(IReadOnlyList<byte> plaintextBytes,
            int blockCount)
        {
            var bitBlocks = new BitArray[blockCount];
            for (var i = 0; i < blockCount; ++i)
            {
                // Allocate array for bytes.
                var byteBlocks = new byte[BlockSize];
                // Populate byte block.
                for (var j = 0; j < BlockSize; ++j)
                    byteBlocks[j] = plaintextBytes[j + i * BlockSize];
                // Get bit array from bytes.
                bitBlocks[i] = new BitArray(byteBlocks);
            }

            return bitBlocks;
        }

        private static void PermuteAllBlocks(ref BitArray[] blocks, bool initial = false)
        {
            for (var i = 0; i < blocks.Length; ++i) PermuteBlock(ref blocks[i], initial);
        }

        public static void PermuteBlock(ref BitArray bits, bool initial = false)
        {
            const int bitCount = BlockSize * 8;
            if (bits.Count != bitCount) throw new ArgumentException("Illegal block size");

            var permutedBits = new BitArray(bitCount);
            for (var i = 0; i < bitCount; ++i) permutedBits[i] = bits[Tables.BlockIP[i] - 1];
            bits = permutedBits;
        }

        public static BitArray GetPermutedKey(BitArray bits)
        {
            // There is a parity check bit for each byte which gets ignored.
            var permutedBits = new BitArray(56);
            for (var i = 0; i < 56; ++i) permutedBits[i] = bits[Tables.KeyPermutations[i] - 1];
            return permutedBits;
        }

        // Compression permutation.
        private static void PermuteSubKeys(ref BitArray[] subKeys)
        {
            var temp = new BitArray[16];
            for (var i = 0; i < 16; ++i)
            {
                temp[i] = new BitArray(48);
                for (var j = 0; j < 48; ++j)
                    temp[i][j] = subKeys[i][Tables.KeyCompression[j] - 1];
            }

            subKeys = temp;
        }

        private static BitArray GetSubKey(BitArray key, int iteration)
        {
            // Key schedule calculations table, page 21 of the reference manual.
            int[] leftShiftCount = {1, 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1};

            // Split key into halves.
            var keyBitsL = new BitArray(28);
            var keyBitsR = new BitArray(28);
            for (var i = 0; i < 28; ++i)
            {
                keyBitsL[i] = key[i];
                keyBitsR[i] = key[28 + i];
            }

            // Store the leftmost bits.
            bool[] keyBitsLBuffer = {keyBitsL[0], keyBitsL[1]};
            bool[] keyBitsRBuffer = {keyBitsR[0], keyBitsR[1]};

            // Shift bits to the left, and cycle the leftmost bits
            // to be the rightmost ones.
            keyBitsL.LeftShift(leftShiftCount[iteration]);
            keyBitsR.LeftShift(leftShiftCount[iteration]);
            switch (leftShiftCount[iteration])
            {
                case 1:
                    keyBitsL[^1] = keyBitsLBuffer[0];
                    keyBitsR[^1] = keyBitsRBuffer[0];
                    break;
                case 2:
                    keyBitsL[^2] = keyBitsLBuffer[0];
                    keyBitsL[^1] = keyBitsLBuffer[1];
                    keyBitsR[^2] = keyBitsRBuffer[0];
                    keyBitsR[^1] = keyBitsRBuffer[1];
                    break;
                default:
                    throw new ArgumentException("Illegal key schedule calculation value");
            }

            // Reassemble the key.
            var subKey = new BitArray(56);
            for (var i = 0; i < 28; ++i)
            {
                subKey[i] = keyBitsL[i];
                subKey[28 + 1] = keyBitsR[i];
            }

            return subKey;
        }

        private static BitArray GetKey()
        {
            Console.WriteLine($"Enter your key ({BlockSize} ASCII characters long):");
            string key = Console.ReadLine();
            if (!string.IsNullOrEmpty(key) && key.Length == BlockSize)
                return GetBitsFromString(key);

            Console.WriteLine("You have not provided valid input. Exiting the program...");
            Environment.Exit(0);
            return null;
        }

        public static BitArray GetBitsFromString(string key)
        {
            byte[] keyBytes = Encoding.ASCII.GetBytes(key);
            return new BitArray(keyBytes);
        }

        private static string GetPlaintext()
        {
            Console.WriteLine("Enter your plaintext (ASCII):");
            string plaintext = Console.ReadLine();
            if (!string.IsNullOrEmpty(plaintext)) return plaintext;

            Console.WriteLine("You have not provided valid input. Exiting the program...");
            Environment.Exit(0);
            return null;
        }

        private static byte[] GetPlaintextBytes(string plaintext)
        {
            // Converting the input into an array of bytes, adding padding of zeroes
            // which is valid for ASCII input. I take advantage of the fact that
            // Array.Resize will allocate the additional space, filled with zeroes.
            byte[] inputBytes = Encoding.ASCII.GetBytes(plaintext);
            int paddingLength = inputBytes.Length % BlockSize;
            if (paddingLength != 0)
                Array.Resize(ref inputBytes, inputBytes.Length + paddingLength);

            return inputBytes;
        }
    }
}
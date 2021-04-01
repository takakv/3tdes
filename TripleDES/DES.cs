using System;
using System.Collections.Generic;
using System.Text;

// 3-key triple DES (3TDES).
namespace TripleDES
{
    public static class DES
    {
        // DES uses 64 bit blocks, which is 8 bytes.
        private const int BlockSizeBytes = 8;
        private const int DESRounds = 16;
        private const int SubKeyLength = 48;

        private static void Main(string[] args)
        {
            BitStream permutedKeyBits = GetPermutedKey(GetKey());

            // Get the 16 subkeys for each of the DES rounds.
            var subKeys = new BitStream[DESRounds];
            for (var i = 0; i < DESRounds; ++i)
            {
                // Each shift is applied to the result of the previous round.
                permutedKeyBits = GetSubKey(permutedKeyBits, i);
                subKeys[i] = permutedKeyBits;
            }

            CompressSubKeys(ref subKeys);

            string plaintext = GetPlaintext();
            byte[] plaintextBytes = GetPlaintextBytes(plaintext);
            int blockCount = plaintextBytes.Length / BlockSizeBytes;
            BitStream[] plaintextBlocks = GetPlaintextBlocks(plaintextBytes, blockCount);
            PermuteAllBlocks(ref plaintextBlocks, true);
        }

        private static BitStream[] GetPlaintextBlocks(IReadOnlyList<byte> plaintextBytes,
            int blockCount)
        {
            var bitBlocks = new BitStream[blockCount];
            for (var i = 0; i < blockCount; ++i)
            {
                // Allocate array for bytes.
                var byteBlocks = new byte[BlockSizeBytes];
                // Populate byte block.
                for (var j = 0; j < BlockSizeBytes; ++j)
                    byteBlocks[j] = plaintextBytes[j + i * BlockSizeBytes];
                // Get bit array from bytes.
                bitBlocks[i] = new BitStream(byteBlocks);
            }

            return bitBlocks;
        }

        private static void PermuteAllBlocks(ref BitStream[] blocks, bool initial = false)
        {
            for (var i = 0; i < blocks.Length; ++i) PermuteBlock(ref blocks[i], initial);
        }

        public static void PermuteBlock(ref BitStream bits, bool initial = false)
        {
            const int bitCount = BlockSizeBytes * 8;
            if (bits.Count != bitCount) throw new ArgumentException("Illegal block size");

            var permutedBits = new BitStream(bitCount);
            for (var i = 0; i < bitCount; ++i) permutedBits[i] = bits[Tables.BlockIP[i] - 1];
            bits = permutedBits;
        }

        public static BitStream GetPermutedKey(BitStream bits)
        {
            // There is a parity check bit for each byte which gets ignored.
            var permutedBits = new BitStream(56);
            for (var i = 0; i < 56; ++i)
                permutedBits[i] = bits[Tables.KeyPermutations[i] - 1];
            return permutedBits;
        }

        // Compression permutation.
        public static void CompressSubKeys(ref BitStream[] subKeys)
        {
            if (subKeys.Length != DESRounds)
                throw new ArgumentException("Illegal number of subkeys");

            var temp = new BitStream[DESRounds];
            for (var i = 0; i < DESRounds; ++i)
            {
                temp[i] = new BitStream(SubKeyLength);
                for (var j = 0; j < SubKeyLength; ++j)
                    temp[i][j] = subKeys[i][Tables.KeyCompression[j] - 1];
            }

            subKeys = temp;
        }

        public static BitStream GetSubKey(BitStream key, int iterationIndex)
        {
            // Key schedule calculations table, page 21 of the reference manual.
            int[] leftShiftCount = {1, 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1};

            // Split key into halves.
            var keyBitsL = new BitStream(28);
            var keyBitsR = new BitStream(28);
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
            keyBitsL.LeftShift(leftShiftCount[iterationIndex]);
            keyBitsR.LeftShift(leftShiftCount[iterationIndex]);
            switch (leftShiftCount[iterationIndex])
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
            var subKey = new BitStream(56);
            for (var i = 0; i < 28; ++i)
            {
                subKey[i] = keyBitsL[i];
                subKey[28 + i] = keyBitsR[i];
            }

            return subKey;
        }

        private static BitStream GetKey()
        {
            Console.WriteLine($"Enter your key ({BlockSizeBytes} ASCII characters long):");
            string key = Console.ReadLine();
            if (!string.IsNullOrEmpty(key) && key.Length == BlockSizeBytes)
                return GetBitsFromString(key);

            Console.WriteLine("You have not provided valid input. Exiting the program...");
            Environment.Exit(0);
            return null;
        }

        public static BitStream GetBitsFromString(string key)
        {
            byte[] keyBytes = Encoding.ASCII.GetBytes(key);
            return new BitStream(keyBytes);
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
            int paddingLength = inputBytes.Length % BlockSizeBytes;
            if (paddingLength != 0)
                Array.Resize(ref inputBytes, inputBytes.Length + paddingLength);

            return inputBytes;
        }
    }
}
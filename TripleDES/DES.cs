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

            // Initial permutation table, page 10 of the reference manual.
            int[] ip =
            {
                58, 50, 42, 34, 26, 18, 10, 2,
                60, 52, 44, 36, 28, 20, 12, 4,
                62, 54, 46, 38, 30, 22, 14, 6,
                64, 56, 48, 40, 32, 24, 16, 8,
                57, 49, 41, 33, 25, 17, 09, 1,
                59, 51, 43, 35, 27, 19, 11, 3,
                61, 53, 45, 37, 29, 21, 13, 5,
                63, 55, 47, 39, 31, 23, 15, 7
            };

            var permutedBits = new BitArray(bitCount);
            for (var i = 0; i < bitCount; ++i) permutedBits[i] = bits[ip[i] - 1];
            bits = permutedBits;
        }

        public static BitArray GetPermutedKey(BitArray bits)
        {
            // Key permutations table, page 19 of the reference manual.
            int[] permutations =
            {
                57, 49, 41, 33, 25, 17, 09,
                01, 58, 50, 42, 34, 26, 18,
                10, 02, 59, 51, 43, 35, 27,
                19, 11, 03, 60, 52, 44, 36,
                63, 55, 47, 39, 31, 23, 15,
                07, 62, 54, 46, 38, 30, 22,
                14, 06, 61, 53, 45, 37, 29,
                21, 13, 05, 28, 20, 12, 04
            };

            // There is a parity check bit for each byte which gets ignored.
            var permutedBits = new BitArray(56);
            for (var i = 0; i < 56; ++i) permutedBits[i] = bits[permutations[i] - 1];
            return permutedBits;
        }

        // Compression permutation.
        private static void PermuteSubKeys(ref BitArray[] subKeys)
        {
            // Subkeys permutations table, page 21 of the reference manual.
            int[] permutations =
            {
                14, 17, 11, 24, 01, 05,
                03, 28, 15, 06, 21, 10,
                23, 19, 12, 04, 26, 08,
                16, 07, 27, 20, 13, 02,
                41, 52, 31, 37, 47, 55,
                30, 40, 51, 45, 33, 48,
                44, 49, 39, 56, 34, 53,
                46, 42, 50, 36, 29, 32
            };

            var temp = new BitArray[16];
            for (var i = 0; i < 16; ++i)
            {
                temp[i] = new BitArray(48);
                for (var j = 0; j < 48; ++j) temp[i][j] = subKeys[i][permutations[j] - 1];
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
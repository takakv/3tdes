using System;
using System.Collections;
using System.Text;

// 3-key triple DES (3TDES).
namespace TDES
{
    internal static class Program
    {
        // DES uses 64 bit blocks, which is 8 bytes.
        private const int BlockSize = 8;

        private static void Main(string[] args)
        {
            BitArray permutedKeyBits = GetPermutedKey(GetKey());

            // Get the 16 subkeys for each of the DES rounds.
            var subKeys = new BitArray[16];
            for (var i = 0; i < 16; ++i) subKeys[i] = GetSubKey(permutedKeyBits, i);

            string plaintext = GetPlaintext();
            byte[] inputBytes = GetPlaintextBytes(plaintext);

            // Splitting the input array into blocks.
            int numberOfBlocks = inputBytes.Length / BlockSize;
            var inputBlocks = new int[numberOfBlocks, BlockSize];
            for (var i = 0; i < numberOfBlocks; ++i)
            for (var j = 0; j < BlockSize; ++j)
                inputBlocks[i, j] = inputBytes[j + i * BlockSize];
        }

        private static BitArray GetPermutedKey(BitArray bits)
        {
            // Key permutation table:
            // https://csrc.nist.gov/CSRC/media/Publications/fips/46/3/archive/1999-10-25/documents/fips46-3.pdf#page=24
            int[] permutations =
            {
                57, 49, 41, 33, 25, 17, 9, 1, 58, 50, 42, 34, 26, 18, 10, 2, 59, 51, 43, 35, 27, 19,
                11, 3, 60, 52, 44, 36, 63, 55, 47, 39, 31, 23, 15, 7, 62, 54, 46, 38, 30, 22, 14, 6,
                61, 53, 45, 37, 29, 21, 13, 5, 28, 20, 12, 4
            };

            // There is a parity check bit for each byte, which gets ignored.
            var permutedBits = new BitArray(56);
            for (var i = 0; i < 56; ++i) permutedBits[i] = bits[permutations[i] - 1];
            return permutedBits;
        }

        private static BitArray GetSubKey(BitArray key, int iteration)
        {
            var keyBitsL = new BitArray(28);
            var keyBitsR = new BitArray(28);
            for (var i = 0; i < 28; ++i) keyBitsL[i] = key[i];
            for (var i = 0; i < 28; ++i) keyBitsR[i] = key[28 + i];
            var subKey = new BitArray(56);
            return subKey;
        }

        private static BitArray GetKey()
        {
            Console.WriteLine($"Enter your key ({BlockSize} ASCII characters long):");
            string key = Console.ReadLine();
            if (!string.IsNullOrEmpty(key) && key.Length == BlockSize)
                return new BitArray(Encoding.ASCII.GetBytes(key));

            Console.WriteLine("You have not provided valid input. Exiting the program...");
            Environment.Exit(0);
            return null;
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
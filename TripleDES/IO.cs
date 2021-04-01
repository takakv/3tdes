using System;
using System.Collections.Generic;
using System.Text;
using TripleDES.Crypto;

namespace TripleDES
{
    internal static class IO
    {
        internal static BitStream[] GetPlaintextBlocks(IReadOnlyList<byte> plaintextBytes,
            int blockCount)
        {
            var bitBlocks = new BitStream[blockCount];
            for (var i = 0; i < blockCount; ++i)
            {
                // Allocate array for bytes.
                var byteBlocks = new byte[DES.BlockSizeBytes];
                // Populate byte block.
                for (var j = 0; j < DES.BlockSizeBytes; ++j)
                    byteBlocks[j] = plaintextBytes[j + i * DES.BlockSizeBytes];
                // Get bit array from bytes.
                bitBlocks[i] = new BitStream(byteBlocks);
            }

            return bitBlocks;
        }

        internal static BitStream GetKey()
        {
            Console.WriteLine($"Enter your key ({DES.BlockSizeBytes} ASCII characters long):");
            string key = Console.ReadLine();
            if (!string.IsNullOrEmpty(key) && key.Length == DES.BlockSizeBytes)
                return DES.GetBitsFromString(key);

            Console.WriteLine("You have not provided valid input. Exiting the program...");
            Environment.Exit(0);
            return null;
        }

        internal static string GetPlaintext()
        {
            Console.WriteLine("Enter your plaintext (ASCII):");
            string plaintext = Console.ReadLine();
            if (!string.IsNullOrEmpty(plaintext)) return plaintext;

            Console.WriteLine("You have not provided valid input. Exiting the program...");
            Environment.Exit(0);
            return null;
        }

        internal static byte[] GetPlaintextBytes(string plaintext)
        {
            // Converting the input into an array of bytes, adding padding of zeroes
            // which is valid for ASCII input. I take advantage of the fact that
            // Array.Resize will allocate the additional space, filled with zeroes.
            byte[] inputBytes = Encoding.ASCII.GetBytes(plaintext);
            int paddingLength = inputBytes.Length % DES.BlockSizeBytes;
            if (paddingLength != 0)
                Array.Resize(ref inputBytes, inputBytes.Length + paddingLength);

            return inputBytes;
        }
    }
}
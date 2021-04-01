using System;
using System.Text;

namespace TripleDES.Crypto
{
    public static class DES
    {
        // DES uses 64 bit blocks, which is 8 bytes.
        public const int BlockSizeBytes = 8;
        public const int RoundCount = 16;
        private const int SubKeyLength = 48;

        public static BitStream GetBitsFromString(string key)
        {
            byte[] keyBytes = Encoding.ASCII.GetBytes(key);
            return new BitStream(keyBytes);
        }

        /*====================================================================*/
        // DES Key and sub keys
        /*====================================================================*/

        // Permute key and strip parity check bits. 
        public static BitStream GetPermutedKey(BitStream bits)
        {
            var permutedBits = new BitStream(56);
            for (var i = 0; i < 56; ++i)
                permutedBits[i] = bits[Tables.KeyPermutations[i] - 1];
            return permutedBits;
        }

        // Get subkeys according to the DES round.
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

        // Permute and compress all 16 subkeys.
        public static void CompressSubKeys(ref BitStream[] subKeys)
        {
            if (subKeys.Length != RoundCount)
                throw new ArgumentException("Illegal number of subkeys");

            var temp = new BitStream[RoundCount];
            for (var i = 0; i < RoundCount; ++i)
            {
                temp[i] = new BitStream(SubKeyLength);
                for (var j = 0; j < SubKeyLength; ++j)
                    temp[i][j] = subKeys[i][Tables.KeyCompression[j] - 1];
            }

            subKeys = temp;
        }


        /*====================================================================*/
        // Plaintext blocks
        /*====================================================================*/

        // Permute a 64 bit block according to the initial permutation matrix.
        public static void InitialPermuteBlock(ref BitStream bits, bool initial = false)
        {
            const int bitCount = BlockSizeBytes * 8;
            if (bits.Count != bitCount) throw new ArgumentException("Illegal block size");

            var permutedBits = new BitStream(bitCount);
            for (var i = 0; i < bitCount; ++i) permutedBits[i] = bits[Tables.BlockIP[i] - 1];
            bits = permutedBits;
        }

        public static void PermuteAllBlocks(ref BitStream[] blocks, bool initial = false)
        {
            for (var i = 0; i < blocks.Length; ++i) InitialPermuteBlock(ref blocks[i], initial);
        }
    }
}
using System;
using System.Collections.Generic;
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

        // Permute and resize bitstream according to matrix.
        private static void PermuteBits(ref BitStream bits, IReadOnlyList<int> permMatrix)
        {
            int length = permMatrix.Count;
            var permutedBits = new BitStream(length);
            for (var i = 0; i < length; ++i)
                permutedBits[i] = bits[permMatrix[i] - 1];
            bits = permutedBits;
        }

        // Split bitstream in half.
        private static void SplitBits(out BitStream leftHalf, out BitStream rightHalf,
            BitStream bits)
        {
            int halfLength = bits.Count / 2;
            leftHalf = new BitStream(halfLength);
            rightHalf = new BitStream(halfLength);
            for (var i = 0; i < halfLength; ++i)
            {
                leftHalf[i] = bits[i];
                rightHalf[i] = bits[halfLength + i];
            }
        }

        // Assemble bitstream from two halves.
        private static void AssembleBits(out BitStream bits,
            BitStream leftHalf, BitStream rightHalf)
        {
            int halfLength = leftHalf.Count;
            bits = new BitStream(halfLength * 2);
            for (var i = 0; i < halfLength; ++i)
            {
                bits[i] = leftHalf[i];
                bits[28 + i] = rightHalf[i];
            }
        }

        /*====================================================================*/
        // DES Key and sub keys
        /*====================================================================*/

        // Permute key and strip parity check bits. 
        public static void PermuteKey(ref BitStream bits)
        {
            PermuteBits(ref bits, Tables.KeyPermutations);
        }

        // Get subkeys according to the DES round.
        public static BitStream GetSubKey(BitStream key, int iterationIndex)
        {
            // Key schedule calculations table, page 21 of the reference manual.
            int[] leftShiftCount = {1, 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1};

            SplitBits(out BitStream keyBitsL, out BitStream keyBitsR, key);

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

            AssembleBits(out BitStream subKey, keyBitsL, keyBitsR);

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

            PermuteBits(ref bits, Tables.BlockIP);
        }

        public static void PermuteAllBlocks(ref BitStream[] blocks, bool initial = false)
        {
            for (var i = 0; i < blocks.Length; ++i) InitialPermuteBlock(ref blocks[i], initial);
        }


        /*====================================================================*/
        // Feistel function
        /*====================================================================*/

        public static void ExpansionPermute(ref BitStream bits)
        {
            PermuteBits(ref bits, Tables.EPerm);
        }

        public static void FFunction(BitStream block)
        {
            SplitBits(out BitStream lHalf, out BitStream rHalf, block);

            ExpansionPermute(ref rHalf);
            for (var i = 0; i < RoundCount; ++i)
            {
            }
        }
    }
}
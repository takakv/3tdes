using System.Collections;
using FluentAssertions;
using TripleDES;
using Xunit;

// ReSharper disable SuggestVarOrType_SimpleTypes
// ReSharper disable SuggestVarOrType_Elsewhere
// ReSharper disable SuggestVarOrType_BuiltInTypes

// ASCII: key bits
// Bit string: 01101011 01100101 01111001 00100000 01100010 01101001 01110100 01110011
// Index-form bit string: 11010110 10100110 10011110 00000100 01000110 10010110 00101110 11001110
// Decimal: 107 101 121 32 98 105 116 115

// NOTE: The Should().BeEquivalentTo() method does not care if the comparing bit array
// uses bit string order or index-form order. This is NOT A FEATURE, it IS A FLAW,
// as it merely compares if all values are the same, order independently. This means
// that extra care needs to be put into providing check bit arrays in index-form
// and the assertions aren't foolproof!

namespace TripleDESTests
{
    public class TripleDESShould
    {
        private const bool F = false;
        private const bool T = true;

        [Fact]
        public void GetKeyBits()
        {
            const string key = "key bits";

            bool[] bitBoolsCheck =
            {
                T, T, F, T, F, T, T, F,
                T, F, T, F, F, T, T, F,
                T, F, F, T, T, T, T, F,
                F, F, F, F, F, T, F, F,
                F, T, F, F, F, T, T, F,
                T, F, F, T, F, T, T, F,
                F, F, T, F, T, T, T, F,
                T, T, F, F, T, T, T, F
            };

            BitArray bits = DES.GetBitsFromString(key);
            bool[] bitBools = new bool[bits.Count];
            bits.CopyTo(bitBools, 0);

            bitBools.Should().BeEquivalentTo(bitBoolsCheck);
        }

        [Fact]
        public void PermuteKeyBits()
        {
            byte[] bytes = {107, 101, 121, 32, 98, 105, 116, 115};

            bool[] bitBoolsCheck =
            {
                T, F, T, F, T, F, T,
                T, T, F, F, T, F, F,
                F, T, F, T, F, F, F,
                F, T, F, F, F, T, F,
                T, T, T, T, F, T, T,
                T, T, T, T, T, T, T,
                T, T, T, T, F, F, F,
                T, F, F, F, T, F, T
            };

            BitArray bits = DES.GetPermutedKey(new BitArray(bytes));
            bool[] bitBools = new bool[bits.Count];
            bits.CopyTo(bitBools, 0);

            bitBools.Should().BeEquivalentTo(bitBoolsCheck);
        }
    }
}
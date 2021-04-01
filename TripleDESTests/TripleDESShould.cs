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

// NOTE: The Should().BeEquivalentTo() method does not care about the order of elements
// by default. While this means that one could theoretically use bit-string form in the tests,
// this also means that the test is not rigorous and may pass for an erroneous bit string.
// Therefore, the strict ordering option should be used at all times.

namespace TripleDESTests
{
    public class TripleDESShould
    {
        private const bool F = false;
        private const bool T = true;

        private readonly byte[] _bytes = {107, 101, 121, 32, 98, 105, 116, 115};

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

            bitBools.Should()
                .BeEquivalentTo(bitBoolsCheck, options => options.WithStrictOrdering());
        }

        [Fact]
        public void PermuteKeyBits()
        {
            bool[] bitBoolsCheck =
            {
                T, F, T, F, F, T, T,
                T, T, F, F, T, F, F,
                F, T, F, T, F, F, F,
                F, T, F, F, F, T, F,
                T, T, T, T, F, T, T,
                T, T, T, T, T, T, T,
                T, T, T, T, F, F, F,
                T, F, F, F, T, F, T
            };

            BitArray bits = DES.GetPermutedKey(new BitArray(_bytes));

            bool[] bitBools = new bool[bits.Count];
            bits.CopyTo(bitBools, 0);

            bitBools.Should()
                .BeEquivalentTo(bitBoolsCheck, options => options.WithStrictOrdering());
        }

        [Fact]
        public void PermuteBlockInitially()
        {
            bool[] bitBoolsCheck =
            {
                T, F, F, T, F, F, F, T,
                F, F, T, F, F, T, F, T,
                T, T, T, T, T, T, T, T,
                F, F, F, F, F, F, F, F,
                T, F, T, F, F, T, T, T,
                F, T, F, F, F, F, T, F,
                T, T, F, F, F, T, F, F,
                T, T, T, T, F, T, T, T
            };

            BitArray bits = new BitArray(_bytes);
            DES.PermuteBlock(ref bits, true);

            bool[] bitBools = new bool[bits.Count];
            bits.CopyTo(bitBools, 0);

            bitBools.Should()
                .BeEquivalentTo(bitBoolsCheck, options => options.WithStrictOrdering());
        }
    }
}
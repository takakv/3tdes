using System.Collections;
using FluentAssertions;
using TripleDES;
using Xunit;

// ReSharper disable SuggestVarOrType_SimpleTypes
// ReSharper disable SuggestVarOrType_Elsewhere
// ReSharper disable SuggestVarOrType_BuiltInTypes

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
                F, T, T, F, T, F, T, T,
                F, T, T, F, F, T, F, T,
                F, T, T, T, T, F, F, T,
                F, F, T, F, F, F, F, F,
                F, T, T, F, F, F, T, F,
                F, T, T, F, T, F, F, T,
                F, T, T, T, F, T, F, F,
                F, T, T, T, F, F, T, T
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
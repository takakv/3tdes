using FluentAssertions;
using TripleDES.Crypto;
using Xunit;

// ReSharper disable SuggestVarOrType_SimpleTypes
// ReSharper disable SuggestVarOrType_Elsewhere
// ReSharper disable SuggestVarOrType_BuiltInTypes

// ASCII: key bits
// Bit string: 01101011 01100101 01111001 00100000 01100010 01101001 01110100 01110011
// Decimal: 107 101 121 32 98 105 116 115

// NOTE: The Should().BeEquivalentTo() method does not care about the order of elements
// by default. The strict ordering option should thus be used at all times.

namespace TripleDES.Tests
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

            bool[] checkBits =
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

            BitStream bits = DES.GetBitsFromString(key);

            bits.Bits.Should()
                .BeEquivalentTo(checkBits, options => options.WithStrictOrdering());
        }

        [Fact]
        public void PermuteKeyBits()
        {
            bool[] checkBits =
            {
                F, F, F, F, F, F, F,
                F, T, T, T, T, F, T,
                T, T, T, T, T, T, T,
                T, T, T, T, T, F, F,
                T, F, F, T, F, F, F,
                T, F, T, F, F, F, F,
                T, F, F, F, T, F, F,
                T, F, T, F, T, F, F
            };

            BitStream bits = new BitStream(_bytes);
            DES.PermuteKey(ref bits);

            bits.Bits.Should()
                .BeEquivalentTo(checkBits, options => options.WithStrictOrdering());
        }

        [Fact]
        public void PermuteBlockInitially()
        {
            bool[] checkBits =
            {
                T, T, T, T, F, T, T, T,
                T, T, F, F, F, T, F, F,
                F, T, F, F, F, F, T, F,
                T, F, T, F, F, T, T, T,
                F, F, F, F, F, F, F, F,
                T, T, T, T, T, T, T, T,
                F, F, T, F, F, T, F, T,
                T, F, F, T, F, F, F, T
            };

            BitStream bits = new BitStream(_bytes);
            DES.InitialPermuteBlock(ref bits, true);

            bits.Bits.Should()
                .BeEquivalentTo(checkBits, options => options.WithStrictOrdering());
        }

        [Fact]
        public void GetSubKey()
        {
            bool[] bitInitialiser =
            {
                F, F, F, F, F, F, F,
                F, T, T, T, T, F, T,
                T, T, T, T, T, T, T,
                T, T, T, T, T, F, F,
                T, F, F, T, F, F, F,
                T, F, T, F, F, F, F,
                T, F, F, F, T, F, F,
                T, F, T, F, T, F, F
            };

            bool[] checkBits =
            {
                F, F, F, F, F, F, F,
                T, T, T, T, F, T, T,
                T, T, T, T, T, T, T,
                T, T, T, T, F, F, F,
                F, F, T, F, F, F, T,
                F, T, F, F, F, F, T,
                F, F, F, T, F, F, T,
                F, T, F, T, F, F, T
            };

            // First iteration.
            BitStream bits = DES.GetSubKey(new BitStream(bitInitialiser), 0);

            bits.Bits.Should()
                .BeEquivalentTo(checkBits, options => options.WithStrictOrdering());
        }

        [Fact]
        public void CompressSubKeys()
        {
            bool[] bitInitialiser =
            {
                F, F, F, F, F, F, F,
                F, T, T, T, T, F, T,
                T, T, T, T, T, T, T,
                T, T, T, T, T, F, F,
                T, F, F, T, F, F, F,
                T, F, T, F, F, F, F,
                T, F, F, F, T, F, F,
                T, F, T, F, T, F, F
            };

            bool[] checkBits =
            {
                T, T, T, T, F, F,
                F, F, T, F, T, T,
                T, T, T, F, T, F,
                T, F, F, T, F, F,
                F, T, F, F, T, F,
                F, F, F, F, F, F,
                F, F, F, F, F, F,
                F, F, T, T, T, T
            };

            BitStream bits = new BitStream(bitInitialiser);
            BitStream[] subKeys = new BitStream[16];
            for (int i = 0; i < 16; ++i)
                subKeys[i] = bits;

            DES.CompressSubKeys(ref subKeys);

            subKeys[0].Bits.Should()
                .BeEquivalentTo(checkBits, options => options.WithStrictOrdering());
        }
    }
}
using System.Collections;
using System.Collections.Generic;
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
        [Fact]
        public void GetKeyBits()
        {
            const string key = "I am groot";
            string[] keyBitStrings =
            {
                "01001001", "00100000", "01100001", "01101101", "00100000", "01100111", "01110010",
                "01101111", "01101111", "01110100"
            };

            List<bool> tmp = new List<bool>();

            // The least significant bit represents the lowest index value.
            // On bits are represented by truth values.
            foreach (string bitString in keyBitStrings)
                for (int j = 7; j >= 0; --j)
                    tmp.Add(bitString[j] == '1');
            bool[] keyBitBoolsCheck = tmp.ToArray();

            BitArray keyBits = DES.GetBitsFromString(key);
            bool[] keyBitBools = new bool[keyBits.Count];
            keyBits.CopyTo(keyBitBools, 0);

            keyBitBools.Should().BeEquivalentTo(keyBitBoolsCheck);
        }

        [Fact]
        public void PermuteKeyBits()
        {
            byte[] bytes = {107, 101, 121, 32, 98, 105, 116, 115};

            bool[] bitBoolsCheck =
            {
                true, false, true, false, false, true, true, true, true, false, false, true, false,
                false, false, true, false, true, false, false, false, false, true, false, false,
                false, true, false, true, true, true, true, false, true, true, true, true, true,
                true, true, true, true, true, true, true, true, false, false, false, true, false,
                false, false, true, false, true
            };

            BitArray bits = DES.GetPermutedKey(new BitArray(bytes));
            bool[] bitBools = new bool[bits.Count];
            bits.CopyTo(bitBools, 0);

            bitBools.Should().BeEquivalentTo(bitBoolsCheck);
        }
    }
}
using TripleDES.Crypto;

// 3-key triple DES (3TDES).
namespace TripleDES
{
    public static class Program
    {
        private static void Main()
        {
            BitStream key = IO.GetKey();
            DES.PermuteKey(ref key);

            // Get the 16 subkeys for each of the DES rounds.
            var subKeys = new BitStream[DES.RoundCount];
            for (var i = 0; i < DES.RoundCount; ++i)
            {
                // Each shift is applied to the result of the previous round.
                key = DES.GetSubKey(key, i);
                subKeys[i] = key;
            }

            DES.CompressSubKeys(ref subKeys);

            string plaintext = IO.GetPlaintext();
            byte[] plaintextBytes = IO.GetPlaintextBytes(plaintext);
            int blockCount = plaintextBytes.Length / DES.BlockSizeBytes;
            BitStream[] plaintextBlocks = IO.GetPlaintextBlocks(plaintextBytes, blockCount);
            DES.PermuteAllBlocks(ref plaintextBlocks, true);

            DES.FFunction(plaintextBlocks[0]);
        }
    }
}
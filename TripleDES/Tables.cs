namespace TripleDES
{
    internal struct Tables
    {
        // Key permutations table with parity bit drop, ref. manual page 19.
        internal static readonly int[] KeyPermutations =
        {
            57, 49, 41, 33, 25, 17, 09,
            01, 58, 50, 42, 34, 26, 18,
            10, 02, 59, 51, 43, 35, 27,
            19, 11, 03, 60, 52, 44, 36,
            63, 55, 47, 39, 31, 23, 15,
            07, 62, 54, 46, 38, 30, 22,
            14, 06, 61, 53, 45, 37, 29,
            21, 13, 05, 28, 20, 12, 04
        };

        // Subkeys compression permutation table, ref. manual page 21.
        internal static readonly int[] KeyCompression =
        {
            14, 17, 11, 24, 01, 05,
            03, 28, 15, 06, 21, 10,
            23, 19, 12, 04, 26, 08,
            16, 07, 27, 20, 13, 02,
            41, 52, 31, 37, 47, 55,
            30, 40, 51, 45, 33, 48,
            44, 49, 39, 56, 34, 53,
            46, 42, 50, 36, 29, 32
        };

        // Expansion permutation table, ref. manual page 13.
        internal static readonly int[] EPerm =
        {
            32, 01, 02, 03, 04, 05,
            04, 05, 06, 07, 08, 09,
            08, 09, 10, 11, 12, 13,
            12, 13, 14, 15, 16, 17,
            16, 17, 18, 19, 20, 21,
            20, 21, 22, 23, 24, 25,
            24, 25, 26, 27, 28, 29,
            28, 29, 30, 31, 32, 01
        };

        // Feistel primitive function table, ref. manual page 18.
        internal static readonly int[] FPerm =
        {
            16, 07, 20, 21,
            29, 12, 28, 17,
            01, 15, 23, 26,
            05, 18, 31, 10,
            02, 08, 24, 14,
            32, 27, 03, 09,
            19, 13, 30, 06,
            22, 11, 04, 25
        };

        // Initial Permutation table, ref.manual page 10.
        internal static readonly int[] BlockIP =
        {
            58, 50, 42, 34, 26, 18, 10, 2,
            60, 52, 44, 36, 28, 20, 12, 4,
            62, 54, 46, 38, 30, 22, 14, 6,
            64, 56, 48, 40, 32, 24, 16, 8,
            57, 49, 41, 33, 25, 17, 09, 1,
            59, 51, 43, 35, 27, 19, 11, 3,
            61, 53, 45, 37, 29, 21, 13, 5,
            63, 55, 47, 39, 31, 23, 15, 7
        };

        // Final Permutation table, ref. manual page 10.
        internal static readonly int[] BlockFP =
        {
            40, 8, 48, 16, 56, 24, 64, 32,
            39, 7, 47, 15, 55, 23, 63, 31,
            38, 6, 46, 14, 54, 22, 62, 30,
            37, 5, 45, 13, 53, 21, 61, 29,
            36, 4, 44, 12, 52, 20, 60, 28,
            35, 3, 43, 11, 51, 19, 59, 27,
            34, 2, 42, 10, 50, 18, 58, 26,
            33, 1, 41, 09, 49, 17, 57, 25
        };
    }
}
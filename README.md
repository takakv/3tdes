# 3-key Triple DES

A from the ground up implementation of 3-key Triple DES I wrote in C# to familiarise myself with the standard. It is not a "by the letter" implementation, as I brushed over some parts such as checking the parity bits, which I did not find critical for understanding the algorithm itself.

The general validity of the results of the program are dubious however, since manipulating bits with the BitArray method, working with byte arrays etc makes it quite probable for an endianness error or the likes to slip in.

[NIST DES specification (archived)](https://csrc.nist.gov/CSRC/media/Publications/fips/46/3/archive/1999-10-25/documents/fips46-3.pdf)


// attrib : https://www.developpez.net/forums/d863959/dotnet/general-dotnet/contribuez/algorithm-rabin-fingerprint/
namespace Toucan.Common
{
    using System;
    using System.Text;
    
    /// <summary>
    /// Génère des empreintes de fichiers
    /// </summary>
    internal static class RabinFingerPrint
    {
        /// <summary>
        /// Bit 64 of the polynomial P is always 1 and not treated directly. This is the polynomial
        /// with the leading coefficient removed (lcr).
        /// Represents t^64 + t^4 + t^3 + t + 1.
        /// </summary>
        private static readonly UInt64 p_lcr = 0x000000000000001BL;
        /// <summary>
        /// It's not necessary to provide definitions for such integral constant variables as long as their
        /// addresses are not taken.
        /// Degree of the polynomial P.
        /// </summary>
        private static readonly int K = 64;
        /// <summary>
        /// Represents t^(K-1)
        /// </summary>
        private static readonly UInt64 T_K_minus_1 = (UInt64)1L << (K - 1);
        /// <summary>
        /// Broder's paper presents four pre-computed tables because words are considered to be 32-bit.
        /// In this implementation W is a 64-bit integral type. Eight tables are used.
        /// Table A is i1^127 + i2^126 + ... + i8^120.
        /// Table B is i1^119 + i2^118 + ... + i8^112.
        /// Table C, D, ..
        /// Table E is i1^95  + i2^94  + ... + i8^88. (This is table A in the paper.)
        /// Table F, G, H.
        /// </summary>
        private static UInt64[] tableA_ = new UInt64[256]; //Assuming byte size 8.
        private static UInt64[] tableB_ = new UInt64[256];
        private static UInt64[] tableC_ = new UInt64[256];
        private static UInt64[] tableD_ = new UInt64[256];
        private static UInt64[] tableE_ = new UInt64[256];
        private static UInt64[] tableF_ = new UInt64[256];
        private static UInt64[] tableG_ = new UInt64[256];
        private static UInt64[] tableH_ = new UInt64[256];
        /// <summary>
        /// Constructor
        /// </summary>
        static RabinFingerPrint()
        {
            InitTables();
        }
        /// <summary>
        /// Initialize tables
        /// </summary>
        private static void InitTables()
        {
            //This represents t^(k + i) mod P, where i is the index of the array.
            //It will be used to compute the tables.
            UInt64[] mods = new UInt64[K];
            //Remember t^k mod P is equivalent to p_lcr.
            mods[0] = p_lcr;
            for (int i = 1; i < K; ++i)
            {
                //By property: t^i mod P = t(t^(i - 1)) mod P.
                mods[i] = mods[i - 1] << 1;
                //If mods[i - 1] had a term at k-1, mods[i] would have had the term k, which is not represented.
                //The term k would account for exactly one more division by P. Then, the effect is the same
                //as adding p_lcr to the mod.
                if ((mods[i - 1] & T_K_minus_1) != 0)
                    mods[i] ^= p_lcr;
            }
            //Compute tables. A control variable is used to indicate whether the current bit should be
            //considered.
            for (int i = 0; i < 256; ++i)
            {
                int control = i;
                for (int j = 0; j < 8 && control > 0; ++j)
                {
                    if ((control & 1) == 1) //Ok, consider bit. ((byte))
                    {
                        tableA_[i] ^= mods[j + 56];
                        tableB_[i] ^= mods[j + 48];
                        tableC_[i] ^= mods[j + 40];
                        tableD_[i] ^= mods[j + 32];
                        tableE_[i] ^= mods[j + 24];
                        tableF_[i] ^= mods[j + 16];
                        tableG_[i] ^= mods[j + 8];
                        tableH_[i] ^= mods[j];
                    }
                    control >>= 1;
                }
            }
        }

        /// <summary>
        /// Compute hash key
        /// </summary>
        /// <param name="value">Value to hash</param>
        /// <returns>Value</returns>
        private static UInt64 ComputeTablesSum(ref UInt64 value)
        {
            value = tableH_[((value) & 0xFF)] ^
                    tableG_[((value >> 8) & 0xFF)] ^
                    tableF_[((value >> 16) & 0xFF)] ^
                    tableE_[((value >> 24) & 0xFF)] ^
                    tableD_[((value >> 32) & 0xFF)] ^
                    tableC_[((value >> 40) & 0xFF)] ^
                    tableB_[((value >> 48) & 0xFF)] ^
                    tableA_[((value >> 56) & 0xFF)];
            return value; //Pass by reference to return the same w. (Convenience and efficiency.)
        }
        /// <summary>
        /// Compute hask hey
        /// </summary>
        /// <param name="HashArray">Array of Ulong bytes to ahsh</param>
        /// <returns>Hash key</returns>
        private static UInt64 Compute(UInt64[] HashArray)
        {
            UInt64 w = 0L;
            for (int s = 0; s < HashArray.Length; ++s)
                w = ComputeTablesSum(ref w) ^ HashArray[s];
            return w;
        }
        /// <summary>
        /// Compute the fingerprint
        /// </summary>
        /// <param name="source">String to compute</param>
        /// <returns>Hash key</returns>
        internal static UInt64 ComputeFingerPrint(string source)
        {
            byte[] table = Encoding.Unicode.GetBytes(source);
            UInt64[] values = new UInt64[table.LongLength];
            ConvertBytes(ref table, ref values);
            return Compute(values);
        }
        /// <summary>
        /// Compute the fingerprint, you must use this method for very large text
        /// </summary>
        /// <param name="source">String to compute</param>
        /// <returns>Hash key</returns>
        internal static UInt64 ComputeFingerPrint(ref string source)
        {
            return ComputeFingerPrint(source);
        }
        /// <summary>
        /// Compute the fingerprint, you must use this method for very large binary data
        /// </summary>
        /// <param name="source">Data to compute</param>
        /// <returns>Hash key</returns>
        internal static UInt64 ComputeFingerPrint(ref byte[] source)
        {
            UInt64[] values = new UInt64[source.LongLength];
            ConvertBytes(ref source, ref values);
            return Compute(values);
        }
        /// <summary>
        /// Compute the fingerprint, you must use this method for very large binary data
        /// </summary>
        /// <param name="source">Data to compute</param>
        /// <returns>Hash key</returns>
        internal static UInt64 ComputeFingerPrint(byte[] source)
        {
            return ComputeFingerPrint(ref source);
        }
        /// <summary>
        /// Compute byte array to Uint64 array
        /// </summary>
        /// <param name="source">Table de byte source</param>
        /// <param name="destination">Tableau de Uin64</param>
        private static void ConvertBytes(ref byte[] source, ref UInt64[] destination)
        {
            for (long i = 0; i < source.LongLength; i++)
                destination[i] = Convert.ToUInt64(source[i]);
        }
    }
}
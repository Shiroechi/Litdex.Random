using System;

namespace Litdex.Random.PRNG
{
	/// <summary>
	/// PCG helper method
	/// </summary>
	public class PcgStatic
	{
		public UInt128 multiplier = (UInt128)2549297995355413924 << 64 | 4865540595714422341;
		public UInt128 increment = (UInt128)6364136223846793005 << 64 | 1442695040888963407;


		/// <summary>
		/// Advance the state.
		/// </summary>
		/// <param name="state">
		/// State to advance.
		/// </param>
		/// <param name="increment">
		/// Increment to add into state.
		/// </param>
		/// <returns>
		///	Advanced state.
		/// </returns>
		public static ulong PcgSetseq64StepR(ulong state, ulong increment)
		{
			return state * 6364136223846793005 + increment;
		}

		/// <summary>
		/// Multi-step advance functions (jump-ahead, jump-back) 
		/// <para></para>
		/// The method used here is based on Brown, "Random Number Generation
		/// with Arbitrary Stride,", Transactions of the American Nuclear
		/// Society (Nov. 1994).  The algorithm is very similar to fast exponentiation.
		/// <para></para>
		/// Even though delta is an unsigned integer, we can pass a
		/// signed integer to go backwards, it just goes "the long way round".
		/// </summary>
		/// <param name="state">
		/// State to advance.
		/// </param>
		/// <param name="delta">
		/// How many step to advance.
		/// </param>
		/// <param name="curMult">
		/// Random multiplier.
		/// </param>
		/// <param name="curPlus">
		/// Random odd increment.
		/// </param>
		/// <returns>
		///	New state that already advaced by few step.
		/// </returns>
		public static ulong PcgAdvanceLcg64(ulong state, int delta, ulong curMult = 6364136223846793005, ulong curPlus = 1442695040888963407)
		{
			ulong accMult = 1;
			ulong accPlus = 0;

			while (delta > 0)
			{
				if ((delta & 1) == 1)
				{
					accMult *= curMult;
					accPlus = accPlus * curMult + curPlus;
				}
				curPlus = (curMult + 1) * curPlus;
				curMult *= curMult;
				delta >>= 1;
			}
			return accMult * state + accPlus;
		}

#if NET6_0_OR_GREATER
		/// <summary>
		/// Advance the state.
		/// </summary>
		/// <param name="state">
		/// State to advance.
		/// </param>
		/// <param name="increment">
		/// Increment to add into state.
		/// </param>
		/// <returns>
		///	Advanced state.
		/// </returns>
		public static UInt128 PcgSetseq128StepR(UInt128 state, UInt128 increment)
		{
			var multiplier = (UInt128)2549297995355413924 << 64 | 4865540595714422341;
			return state * multiplier + increment;
		}

		/// <summary>
		/// Multi-step advance functions (jump-ahead, jump-back) 
		/// <para></para>
		/// The method used here is based on Brown, "Random Number Generation
		/// with Arbitrary Stride,", Transactions of the American Nuclear
		/// Society (Nov. 1994).  The algorithm is very similar to fast exponentiation.
		/// <para></para>
		/// Even though delta is an unsigned integer, we can pass a
		/// signed integer to go backwards, it just goes "the long way round".
		/// </summary>
		/// <param name="state">
		/// State to advance.
		/// </param>
		/// <param name="delta">
		/// How many step to advance.
		/// </param>
		/// <param name="curMult">
		/// Random multiplier.
		/// </param>
		/// <param name="curPlus">
		/// Random odd increment.
		/// </param>
		/// <returns>
		///	New state that already advaced by few step.
		/// </returns>
		public static UInt128 PcgAdvanceLcg128(ulong state, int delta, UInt128 curMult, UInt128 curPlus)
		{
			UInt128 accMult = 1;
			UInt128 accPlus = 0;

			while (delta > 0)
			{
				if ((delta & 1) == 1)
				{
					accMult *= curMult;
					accPlus = accPlus * curMult + curPlus;
				}
				curPlus = (curMult + 1) * curPlus;
				curMult *= curMult;
				delta >>= 1;
			}
			return accMult * state + accPlus;
		}
#endif

	}
}

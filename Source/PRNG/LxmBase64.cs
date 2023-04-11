namespace Litdex.Random.PRNG
{
	public abstract class LxmBase64 : Random64
	{
		#region Member

		/// <summary>
		/// 64-bit LCG multiplier.
		/// </summary>
		protected const ulong M64 = 0xd1342543de82ef95;

		/// <summary>
		/// Jump constant for an advance of the 64-bit LCG by 2^32.
		/// </summary>
		protected const ulong M64P = 0x8d23804c00000001;

		/// <summary>
		/// Jump constant precursor for an advance of the 64-bit LCG by 2^32.
		/// </summary>7
		protected const ulong C64P = 0x16691c9700000000;

		/// <summary>
		/// Low half of 128-bit LCG multiplier. The upper half is 1.
		/// </summary>
		protected const ulong M128L = 0xd605bbb58c8abbfd;

		/// <summary>
		/// High half of the jump constant for an advance of the 128-bit LCG by 2^64. The low half is 1.
		/// </summary>
		protected const ulong M128PH = 0x31f179f5224754f4;

		/// <summary>
		/// High half of the jump constant for an advance of the 128-bit LCG by 2^64.
		/// The low half is zero.
		/// </summary>
		protected const long C128PH = 0x61139b28883277c3;

		/// <summary>
		/// The fractional part of the golden ratio, phi, scaled to 64-bits and rounded to odd.
		/// </summary>
		protected const ulong GOLDEN_RATIO_64 = 0x9e3779b97f4a7c15;

		/// <summary>
		/// A mask to convert an <see cref="int"/> to an unsigned integer stored as a <see cref="ulong"/>.
		/// </summary>
		protected const ulong INT_TO_UNSIGNED_BYTE_MASK = 0xffff_ffff;

		protected const byte SEED_SIZE = 4;

		#endregion Member

		#region Protected Method

		/// <summary>
		/// Perform a 64-bit mixing function using Doug Lea's 64-bit mix constants and shifts.
		/// </summary>
		/// <param name="x">
		/// Value to be mixed.
		/// </param>
		/// <returns>
		/// The output value.
		/// </returns>
		protected ulong Lea64(ulong x)
		{
			x = (x ^ (x >> 32)) * 0xdaba0b6eb09322e3;
			x = (x ^ (x >> 32)) * 0xdaba0b6eb09322e3;
			return x ^ (x >> 32);
		}

		/// <summary>
		/// Multiply the two values as if unsigned 64-bit longs to produce the high 64-bits
		/// of the 128-bit unsigned result.
		/// </summary>
		/// <returns>
		/// The high 64-bits of the 128-bit unsigned result.
		/// </returns>
		protected ulong UnsignedMultiplyHigh(ulong value1, ulong value2)
		{
			// Computation is based on the following observation about the upper (a and x)
			// and lower (b and y) bits of unsigned big-endian integers:
			//   ab * xy
			// =  b *  y
			// +  b * x0
			// + a0 *  y
			// + a0 * x0
			// = b * y
			// + b * x * 2^32
			// + a * y * 2^32
			// + a * x * 2^64
			//
			// Summation using a character for each byte:
			//
			//             byby byby
			// +      bxbx bxbx 0000
			// +      ayay ayay 0000
			// + axax axax 0000 0000
			//
			// The summation can be rearranged to ensure no overflow given
			// that the result of two unsigned 32-bit integers multiplied together
			// plus two full 32-bit integers cannot overflow 64 bits:
			// > long x = (1L << 32) - 1
			// > x * x + x + x == -1 (all bits set, no overflow)
			//
			// The carry is a composed intermediate which will never overflow:
			//
			//             byby byby
			// +           bxbx 0000
			// +      ayay ayay 0000
			//
			// +      bxbx 0000 0000
			// + axax axax 0000 0000

			ulong a = value1 >> 32;
			ulong b = value1 & INT_TO_UNSIGNED_BYTE_MASK;
			ulong x = value2 >> 32;
			ulong y = value2 & INT_TO_UNSIGNED_BYTE_MASK;

			ulong by = b * y;
			ulong bx = b * x;
			ulong ay = a * y;
			ulong ax = a * x;

			// Cannot overflow
			ulong carry = (by >> 32) + (bx & INT_TO_UNSIGNED_BYTE_MASK) + ay;

			// Note:
			// low = (carry << 32) | (by & INT_TO_UNSIGNED_BYTE_MASK)
			// Benchmarking shows outputting low to a long[] output argument
			// has no benefit over computing 'low = value1 * value2' separately.

			return (bx >> 32) + (carry >> 32) + ax;
		}

		/// <summary>
		/// Add the two values as if unsigned 64-bit longs to produce the high 64-bits
		/// of the 128-bit unsigned result.
		/// </summary>
		/// <returns>
		/// The high 64-bits of the 128-bit unsigned result.
		/// </returns>
		protected ulong UnsignedAddHigh(ulong left, ulong right)
		{
			// Method compiles to 13 bytes as Java byte code.
			// This is below the default of 35 for code inlining.
			//
			// The unsigned add of left + right may have a 65-bit result.
			// If both values are shifted right by 1 then the sum will be
			// within a 64-bit long. The right is assumed to have a low
			// bit of 1 which has been lost in the shift. The method must
			// compute if a 1 was shifted off the left which would have
			// triggered a carry when adding to the right's assumed 1.
			// The intermediate 64-bit result is shifted
			// 63 bits to obtain the most significant bit of the 65-bit result.
			// Using -1 is the same as a shift of (64 - 1) as only the last 6 bits
			// are used by the shift but requires 1 less byte in java byte code.
			//
			//    01100001      left
			// +  10011111      right always has low bit set to 1
			//
			//    0110000   1   carry last bit of left
			// +  1001111   |
			// +        1 <-+
			// = 10000000       carry bit generated
			return ((left >> 1) + (right >> 1) + (left & 1)) >> -1;
		}

		#endregion Protected Method

		#region Public Method

		/// <summary>
		/// The jump is performed by advancing the state of the LCG sub-generator by 1 cycle.
		/// The XBG state is unchanged.
		/// </summary>
		public virtual void NextJump()
		{
			this._State[1] = (M64 * this._State[1]) + this._State[0];
		}

		/// <summary>
		/// The jump is performed by advancing the state of the LCG sub-generator by 2^32 cycle.
		/// The XBG state is unchanged.
		/// </summary>
		public virtual void NextLongJump()
		{
			this._State[1] = (M64P * this._State[1]) + (C64P * this._State[0]);
		}

		#endregion Public Method

	}
}

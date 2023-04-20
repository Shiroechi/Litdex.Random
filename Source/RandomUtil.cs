#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
using System.Numerics;
#endif

using System.Runtime.CompilerServices;
using System;

namespace Litdex.Random
{
	/// <summary>
	/// Static class for helping <see cref="Random"/> class.
	/// </summary>
	public static class RandomUtil
	{
		/// <summary>
		///	Amount of roll after the state is initialized or seeded.
		/// </summary>
		public const byte InitialRoll = 20;

		/// <summary>
		///	Returns the integer (ceiling) log of the specified value, base 2.
		/// </summary>
		/// <param name="value">
		///	The value to ceil.
		///	</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static int Log2Ceiling(ulong value)
		{
#if NET5_0_OR_GREATER
			int result = BitOperations.Log2(value);
			if (BitOperations.PopCount(value) != 1)
			{
				result++;
			}
			return result;
#else
			int result = (int)Math.Log(value, 2);
			if (PopCount(value) != 1)
			{
				result++;
			}
			return result;
#endif
		}

		/// <summary>
		///	Returns the population count (number of bits set) of a mask.
		///	Similar in behavior to the x86 instruction POPCNT.
		/// </summary>
		/// <param name="value">
		///	The value.
		///	</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static int PopCount(ulong value)
		{
			const ulong c1 = 0x_55555555_55555555ul;
			const ulong c2 = 0x_33333333_33333333ul;
			const ulong c3 = 0x_0F0F0F0F_0F0F0F0Ful;
			const ulong c4 = 0x_01010101_01010101ul;

			value -= (value >> 1) & c1;
			value = (value & c2) + ((value >> 2) & c2);
			value = (((value + (value >> 4)) & c3) * c4) >> 56;

			return (int)value;
		}

		/// <summary>
		/// Convert 2 unsigned 32-bit integer into unsigned 64-bit integer.
		/// </summary>
		/// <param name="high">
		///	The high bit order of unsigned 64-bit integer.
		/// </param>
		/// <param name="low">
		/// The high bit order of unsigned 64-bit integer.
		/// </param>
		/// <returns>
		/// Converted unsigned 64-bit integer.
		/// </returns>
		internal static ulong ToUint64(uint high, uint low)
		{
			return (((ulong)high) << 32) | low;
		}

		/// <summary>
		/// Convert unsigned 64-bit integer into 2 unsigned 32-bit integer.
		/// </summary>
		/// <param name="number">
		///	Unsigned 64-bit integer to convert.
		/// </param>
		/// <returns></returns>
		internal static (uint, uint) ToUint(ulong number)
		{
			uint high = (uint)(number >> 32);
			uint low = (uint)number;
			return (high, low);
		}

	}
}

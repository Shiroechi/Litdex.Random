#if NET5_0_OR_GREATER
using System.Numerics;
#endif

using System;
using System.Runtime.CompilerServices;

using Litdex.Utilities;

namespace Litdex.Random
{
	/// <summary>
	///	Represents a pseudo-random number generator, which is an algorithm that produces a sequence of numbers
	/// that meet certain statistical requirements for randomness.
	/// </summary>
	public abstract partial class Random
	{
		#region Member

		/// <summary>
		///	Amount of roll after the state is initialized or seeded.
		/// </summary>
		protected const byte _InitialRoll = 20;

		#endregion Member

		#region Public Method

		/// <summary>
		///	The name of the algorithm this generator implements.
		/// </summary>
		/// <returns>
		///	The name of RNG.
		/// </returns>
		public virtual string AlgorithmName()
		{
			return "Random";
		}

		/// <summary>
		///	Seed with <see cref="System.Security.Cryptography.RandomNumberGenerator"/>.
		///	</summary>
		public abstract void Reseed();

		/// <inheritdoc/>
		public override string ToString()
		{
			return this.AlgorithmName();
		}

		#endregion Public Method

		#region Basic Method

		/// <summary>
		///	Generate <see cref="bool"/> value.
		/// </summary>
		/// <returns>
		///	<see langword="true"/> or <see langword="false"/>.
		/// </returns>
		public abstract bool NextBoolean();

		/// <summary>
		///	Generate a non-negative random integer.
		/// </summary>
		/// <returns>
		///	A 8-bit unsigned integer that is greater than or equal to 0.
		/// </returns>
		public abstract byte NextByte();

		/// <summary>
		///	Generate <see cref="byte"/> that is within a specified range.
		/// </summary>
		/// <param name="minValue">
		///	The inclusive lower bound of the random number returned.
		/// </param>
		/// <param name="maxValue">
		///	The exclusive upper bound of the random number returned. <paramref name="maxValue"/> must be greater than or equal to <paramref name="minValue"/>.
		/// </param>
		/// <returns>
		///	A 8-bit unsigned integer greater than or equal to <paramref name="minValue"/> and less than <paramref name="maxValue"/>; that is, the range of return values includes <paramref name="minValue"/>
		///	but not <paramref name="maxValue"/>. If minValue equals <paramref name="maxValue"/>, <paramref name="minValue"/> is returned.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		///	<paramref name="minValue"/> is greater than <paramref name="maxValue"/>.
		/// </exception>
		public virtual byte NextByte(byte minValue, byte maxValue)
		{
			if (minValue > maxValue)
			{
				ThrowMinMaxValueSwapped();
			}
			else if (minValue == maxValue)
			{
				return minValue;
			}

			return (byte)this.NextUInt(minValue, maxValue);
		}

		/// <summary>
		///	Generate array of random bytes from generator.
		/// </summary>
		/// <param name="length">
		///	Requested output length.
		/// </param>
		/// <returns>
		///	Array of bytes.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		///	The requested output size can't lower than 1.
		/// </exception>
		public abstract byte[] NextBytes(int length);

		/// <summary>
		///	Fills the elements of a specified array of bytes with random numbers.
		/// </summary>
		/// <param name="buffer">
		///	The array to be filled with random numbers.
		///	</param>
		/// <exception cref="ArgumentNullException">
		///	Array length can't be lower than 1 or null.
		/// </exception>
		public abstract void Fill(byte[] buffer);

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER

		/// <summary>
		///	Fills the elements of a specified array of bytes with random numbers.
		/// </summary>
		/// <param name="buffer">
		///	The array to be filled with random numbers.
		///	</param>
		/// <exception cref="ArgumentNullException">
		///	Array length can't be lower than 1 or null.
		/// </exception>
		public abstract void Fill(Span<byte> buffer);

#endif

		/// <summary>
		///	Generate <see cref="int"/> value from generator.
		/// </summary>
		/// <returns>
		///	A 32-bit signed integer.
		/// </returns>
		public abstract int NextInt();

		/// <summary>
		///	Generate <see cref="int"/> that is within a specified range.
		/// </summary>
		/// <param name="minValue">
		///	The inclusive lower bound of the random number returned.
		/// </param>
		/// <param name="maxValue">
		///	The exclusive upper bound of the random number returned. <paramref name="maxValue"/> must be greater than or equal to <paramref name="minValue"/>.
		/// </param>
		/// <returns>
		///	A 32-bit signed integer greater than or equal to <paramref name="minValue"/> and less than <paramref name="maxValue"/>; that is, the range of return values includes <paramref name="minValue"/>
		///	but not <paramref name="maxValue"/>. If minValue equals <paramref name="maxValue"/>, <paramref name="minValue"/> is returned.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		///	<paramref name="minValue"/> is greater than <paramref name="maxValue"/>.
		/// </exception>
		/// <exception cref="ArgumentException">
		///	<paramref name="minValue"/> or <paramref name="maxValue"/> is negative.
		/// </exception>
		public virtual int NextInt(int minValue, int maxValue)
		{
			if (minValue < 0 || maxValue < 0)
			{
				ThrowMinMaxValueNegative();
			}

			var result = this.NextUInt((uint)minValue, (uint)maxValue);

			if (result <= int.MaxValue)
			{
				return (int)result;
			}

			return (int)(result >> 1);
		}

		/// <summary>
		///	Generate <see cref="uint"/> value from generator.
		/// </summary>
		/// <returns>
		///	A 32-bit unsigned integer.
		/// </returns>
		public abstract uint NextUInt();

		/// <summary>
		///	Generate <see cref="uint"/> that is within a specified range.
		/// </summary>
		/// <param name="minValue">
		///	The inclusive lower bound of the random number returned.
		/// </param>
		/// <param name="maxValue">
		///	The exclusive upper bound of the random number returned. <paramref name="maxValue"/> must be greater than or equal to <paramref name="minValue"/>.
		/// </param>
		/// <returns>
		///	A 32-bit unsigned integer greater than or equal to <paramref name="minValue"/> and less than <paramref name="maxValue"/>; that is, the range of return values includes <paramref name="minValue"/>
		///	but not <paramref name="maxValue"/>. If minValue equals <paramref name="maxValue"/>, <paramref name="minValue"/> is returned.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		///	<paramref name="minValue"/> is greater than <paramref name="maxValue"/>.
		/// </exception>
		public virtual uint NextUInt(uint minValue, uint maxValue)
		{
			if (minValue > maxValue)
			{
				ThrowMinMaxValueSwapped();
			}
			else if (minValue == maxValue)
			{
				return minValue;
			}

			ulong exclusiveRange = maxValue - minValue;

			if (exclusiveRange > 1)
			{
				int bits = Log2Ceiling(exclusiveRange);
				while (true)
				{
					ulong result = this.NextUInt64() >> (64 - (bits));

					if (result < exclusiveRange)
					{
						return (uint)result + minValue;
					}
				}
			}

			return minValue;
		}

		/// <summary>
		///	Generate <see cref="long"/> value from generator. 
		/// </summary>
		/// <returns>
		///	A 64-bit signed integer.
		/// </returns>
		public abstract long NextInt64();

		/// <summary>
		///	Generate <see cref="long"/> that is within a specified range.
		/// </summary>
		/// <param name="minValue">
		///	The inclusive lower bound of the random number returned.
		/// </param>
		/// <param name="maxValue">
		///	The exclusive upper bound of the random number returned. <paramref name="maxValue"/> must be greater than or equal to <paramref name="minValue"/>.
		/// </param>
		/// <returns>
		///	A 64-bit signed integer greater than or equal to <paramref name="minValue"/> and less than <paramref name="maxValue"/>; that is, the range of return values includes <paramref name="minValue"/>
		///	but not <paramref name="maxValue"/>. If minValue equals <paramref name="maxValue"/>, <paramref name="minValue"/> is returned.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		///	<paramref name="minValue"/> is greater than <paramref name="maxValue"/>.
		/// </exception>
		/// <exception cref="ArgumentException">
		///	<paramref name="minValue"/> or <paramref name="maxValue"/> is negative.
		/// </exception>
		public virtual long NextInt64(long minValue, long maxValue)
		{
			if (minValue < 0 || maxValue < 0)
			{
				ThrowMinMaxValueNegative();
			}

			var result = this.NextUInt64((ulong)minValue, (ulong)maxValue);

			if (result <= long.MaxValue)
			{
				return (long)result;
			}

			return (long)(result >> 1);
		}

		/// <summary>
		///	Generate <see cref="ulong"/> value from generator. 
		/// </summary>
		/// <returns>
		///	A 64-bit unsigned integer.
		/// </returns>
		public abstract ulong NextUInt64();

		/// <summary>
		///	Generate <see cref="ulong"/> that is within a specified range.
		/// </summary>
		/// <param name="minValue">
		///	The inclusive lower bound of the random number returned.
		/// </param>
		/// <param name="maxValue">
		///	The exclusive upper bound of the random number returned. <paramref name="maxValue"/> must be greater than or equal to <paramref name="minValue"/>.
		/// </param>
		/// <returns>
		///	A 64-bit unsigned integer greater than or equal to <paramref name="minValue"/> and less than <paramref name="maxValue"/>; that is, the range of return values includes <paramref name="minValue"/>
		///	but not <paramref name="maxValue"/>. If minValue equals <paramref name="maxValue"/>, <paramref name="minValue"/> is returned.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		///	<paramref name="minValue"/> is greater than <paramref name="maxValue"/>.
		/// </exception>
		public virtual ulong NextUInt64(ulong minValue, ulong maxValue)
		{
			if (minValue > maxValue)
			{
				ThrowMinMaxValueSwapped();
			}
			else if (minValue == maxValue)
			{
				return minValue;
			}

			// using unbiased lemire method
			// from https://www.pcg-random.org/posts/bounded-rands.html

			var range = maxValue - minValue;
			ulong x = this.NextUInt64();
			var (m, l) = Math128.Multiply(x, range);
			if (l < range)
			{
				ulong t = 0 - range;
				if (t >= range)
				{
					t -= range;
					if (t >= range)
					{
						t %= range;
					}
				}
				while (l < t)
				{
					x = this.NextUInt64();
					(m, l) = Math128.Multiply(x, range);
				}
			}
			return m;
		}

		/// <summary>
		///	Generate <see cref="double"/> value from generator.
		/// </summary>
		/// <returns>
		///	A double-precision floating point number that is greater than or equal to 0.0, and less than 1.0
		/// </returns>
		public virtual double NextDouble()
		{
			return (this.NextUInt64() >> 11) * (1.0 / (1UL << 53));
		}

		/// <summary>
		///	Generate <see cref="double"/> that is within a specified range.
		/// </summary>
		/// <param name="minValue">
		///	The inclusive lower bound of the random number returned.
		/// </param>
		/// <param name="maxValue">
		///	The exclusive upper bound of the random number returned. <paramref name="maxValue"/> must be greater than or equal to <paramref name="minValue"/>.
		/// </param>
		/// <returns>
		///	A double-precision floating point number greater than or equal to <paramref name="minValue"/> and less than <paramref name="maxValue"/>; that is, the range of return values includes <paramref name="minValue"/>
		///	but not <paramref name="maxValue"/>. If minValue equals <paramref name="maxValue"/>, <paramref name="minValue"/> is returned.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		///	<paramref name="minValue"/> is greater than <paramref name="maxValue"/>.
		/// </exception>
		public virtual double NextDouble(double minValue, double maxValue)
		{
			if (minValue > maxValue)
			{
				ThrowMinMaxValueSwapped();
			}

			var diff = maxValue - minValue + 1;
			return minValue + (this.NextDouble() % diff);
		}

		#endregion Basic Method

		private static void ThrowMinMaxValueSwapped()
		{
			throw new ArgumentOutOfRangeException("minValue", "minValue is greater than maxValue");
		}

		private static void ThrowMinMaxValueNegative()
		{
			throw new ArgumentException("minValue or maxValue is negative.");
		}

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
	}
}

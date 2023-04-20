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
	public partial class Random
	{
		#region Member

		private IRandomEngine _Engine;

		#endregion Member

		#region Constructor & Destructor

		/// <summary>
		/// Create a new instance of the <see cref="Litdex.Random.Random"/> class.
		/// </summary>
		/// <param name="engine">
		/// Random algorithm to utilizes.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Engine can't be null.
		/// </exception>
		public Random(IRandomEngine engine)
		{
			if (engine == null)
			{
				throw new ArgumentNullException("Engine can't be null.");
			}

			this._Engine = engine;
		}

		~Random()
		{
			this._Engine = null;
		}

		#endregion Constructor & Destructor

		#region Private Method

		private static void ThrowMinMaxValueSwapped()
		{
			throw new ArgumentOutOfRangeException("minValue", "minValue is greater than maxValue");
		}

		private static void ThrowMinMaxValueNegative()
		{
			throw new ArgumentException("minValue or maxValue is negative.");
		}

		#endregion Private Method

		#region Public Method

		/// <summary>
		///	The name of the algorithm this generator implements.
		/// </summary>
		/// <returns>
		///	The name of RNG.
		/// </returns>
		public virtual string AlgorithmName()
		{
			return this._Engine.AlgorithmName();
		}

		/// <summary>
		///	Seed with <see cref="System.Security.Cryptography.RandomNumberGenerator"/>.
		///	</summary>
		public virtual void Reseed()
		{
			this._Engine.Reseed();
		}

		/// <inheritdoc/>
		public override string ToString()
		{
			return this.AlgorithmName();
		}

		/// <summary>
		/// Gets and sets <see cref="Random"/> engine algorithm.
		/// </summary>
		public IRandomEngine Engine
		{
			get
			{
				return this._Engine;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Engine can't be null.");
				}

				this._Engine = value;
			}
		}

		#endregion Public Method

		#region Basic Method

		/// <summary>
		///	Generate <see cref="bool"/> value.
		/// </summary>
		/// <returns>
		///	<see langword="true"/> or <see langword="false"/>.
		/// </returns>
		public virtual bool NextBoolean()
		{
			return this._Engine.NextBoolean();
		}

		/// <summary>
		///	Generate a non-negative random integer.
		/// </summary>
		/// <returns>
		///	A 8-bit unsigned integer that is greater than or equal to 0.
		/// </returns>
		public virtual byte NextByte()
		{
			return this._Engine.NextByte();
		}

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
		public virtual byte[] NextBytes(int length)
		{
			return this._Engine.NextBytes(length);
		}

		/// <summary>
		///	Fills the elements of a specified array of bytes with random numbers.
		/// </summary>
		/// <param name="buffer">
		///	The array to be filled with random numbers.
		///	</param>
		/// <exception cref="ArgumentNullException">
		///	Array length can't be lower than 1 or null.
		/// </exception>
		public virtual void Fill(byte[] buffer)
		{
			this._Engine.Fill(buffer);
		}

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
		public virtual void Fill(Span<byte> buffer)
		{
			this._Engine.Fill(buffer);
		}

#endif

		/// <summary>
		///	Generate <see cref="int"/> value from generator.
		/// </summary>
		/// <returns>
		///	A 32-bit signed integer.
		/// </returns>
		public virtual int NextInt()
		{
			return this._Engine.NextInt();
		}

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

			return (int)result;
		}

		/// <summary>
		///	Generate <see cref="uint"/> value from generator.
		/// </summary>
		/// <returns>
		///	A 32-bit unsigned integer.
		/// </returns>
		public virtual uint NextUInt()
		{
			return this._Engine.NextUInt();
		}

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
				int bits = RandomUtil.Log2Ceiling(exclusiveRange);
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
		public virtual long NextInt64()
		{
			return this._Engine.NextInt64();
		}

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

			return (long)result;
		}

		/// <summary>
		///	Generate <see cref="ulong"/> value from generator. 
		/// </summary>
		/// <returns>
		///	A 64-bit unsigned integer.
		/// </returns>
		public virtual ulong NextUInt64()
		{
			return this._Engine.NextUInt64();
		}

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
	}
}

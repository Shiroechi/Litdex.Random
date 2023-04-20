#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
using System.Buffers.Binary;
# endif
using System;
using System.Security.Cryptography;

using Litdex.Utilities;
using Litdex.Utilities.Extension;

namespace Litdex.Random.PRNG
{
	/// <summary>
	///	Romu random variations, the fastest generator using 64-bit arith., but not suited for huge jobs.
	///	Est. capacity = 2^51 bytes. Register pressure = 4. State size = 128 bits.
	/// </summary>
	/// <remarks>
	///	Source: https://www.romu-random.org/
	/// </remarks>
	public class RomuDuoJr : Random64
	{
		#region Member

		/// <summary>
		///	The internal state of RNG.
		/// </summary>
		protected ulong[] _State;

		#endregion Member

		#region Constructor & Destructor

		/// <summary>
		///	Create an instance of <see cref="RomuDuoJr"/> object.
		/// </summary>
		/// <param name="seed1">
		///	First RNG seed.
		/// </param>
		/// <param name="seed2">
		///	Second RNG seed.
		/// </param>
		public RomuDuoJr(ulong seed1 = 0, ulong seed2 = 0)
		{
			this._State = new ulong[2];
			this.SetSeed(seed1, seed2);
		}

		/// <summary>
		///	Create an instance of <see cref="RomuDuoJr"/> object.
		/// </summary>
		/// <param name="seed">
		///	RNG seed numbers.
		/// </param>
		/// <exception cref="ArgumentOutOfRangeException">
		///	Seed need 2 numbers.
		/// </exception>
		public RomuDuoJr(ulong[] seed)
		{
			this._State = new ulong[2];
			this.SetSeed(seed);
		}

		~RomuDuoJr()
		{
			Array.Clear(this._State, 0, this._State.Length);
		}

		#endregion Constructor & Destructor

		#region Protected Method

		/// <inheritdoc/>
		protected override ulong Next()
		{
			ulong xp = this._State[0];
			this._State[0] = 15241094284759029579u * this._State[1];
			this._State[1] -= xp;
			this._State[1] = this._State[1].RotateLeft(27);
			return xp;
		}

		#endregion Protected Method

		#region Public Method

		/// <inheritdoc/>
		public override string AlgorithmName()
		{
			return "Romu Duo Jr 64-bit";
		}

		/// <inheritdoc/>
		public override void Reseed()
		{
			using (var rng = RandomNumberGenerator.Create())
			{
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
				Span<byte> span = new byte[16];
				rng.GetNonZeroBytes(span);
				this.SetSeed(
					seed1: BinaryPrimitives.ReadUInt64LittleEndian(span),
					seed2: BinaryPrimitives.ReadUInt64LittleEndian(span.Slice(8)));
#else
				var bytes = new byte[16];
				rng.GetNonZeroBytes(bytes);
				this.SetSeed(
					seed1: BinaryConverter.ToUInt64(bytes, 0),
					seed2: BinaryConverter.ToUInt64(bytes, 8));
#endif
			}
		}

		/// <summary>
		///	Set RNG seed manually.
		/// </summary>
		/// <param name="seed1">
		///	First RNG seed.
		/// </param>
		/// <param name="seed2">
		///	Second RNG seed.
		/// </param>
		public void SetSeed(ulong seed1, ulong seed2)
		{
			this._State[0] = seed1;
			this._State[1] = seed2;
		}

		/// <summary>
		///	Set RNG internal state manually.
		/// </summary>
		/// <param name="seed">
		///	Number to generate the random numbers.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///	Array of seed is null or empty.
		/// </exception>
		/// <exception cref="ArgumentException">
		///	Seed amount must same as the internal state amount.
		/// </exception>
		public virtual void SetSeed(params ulong[] seed)
		{
			if (seed == null || seed.Length == 0)
			{
				throw new ArgumentNullException(nameof(seed), "Seed can't null or empty.");
			}

			if (seed.Length < this._State.Length)
			{
				throw new ArgumentException($"Seed need at least {this._State.Length} numbers.", nameof(seed));
			}

			var length = seed.Length > this._State.Length ? this._State.Length : seed.Length;
			Array.Copy(seed, 0, this._State, 0, length);
		}

		#endregion Public Method
	}
}
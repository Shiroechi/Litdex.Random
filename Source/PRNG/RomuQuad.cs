#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
using System.Buffers.Binary;
#endif
using System;
using System.Security.Cryptography;

using Litdex.Utilities;
using Litdex.Utilities.Extension;

namespace Litdex.Random.PRNG
{
	/// <summary>
	///	Romu random variations, more robust than anyone could need, but uses more registers than RomuTrio.
	///	Est. capacity >= 2^90 bytes. Register pressure = 8 (high). State size = 256 bits.
	/// </summary>
	/// <remarks>
	///	Source: https://www.romu-random.org/
	/// </remarks>
	public class RomuQuad : Random64
	{
		#region Member

		/// <summary>
		///	The internal state of RNG.
		/// </summary>
		protected ulong[] _State;

		#endregion Member

		#region Constructor & Destructor

		/// <summary>
		///	Create an instance of <see cref="RomuQuad"/> object.
		/// </summary>
		/// <param name="seed1">
		///	W state.
		/// </param>
		/// <param name="seed2">
		///	X state.
		/// </param>
		/// <param name="seed3">
		///	Y state.
		/// </param>
		/// <param name="seed4">
		///	Z state.
		/// </param>
		public RomuQuad(ulong seed1 = 0, ulong seed2 = 0, ulong seed3 = 0, ulong seed4 = 0)
		{
			this._State = new ulong[4];
			this.SetSeed(seed1, seed2, seed3, seed4);
		}

		/// <summary>
		///	Create an instance of <see cref="RomuQuad"/> object.
		/// </summary>
		/// <param name="seed">
		///	A array of seed numbers.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///	Array of <paramref name="seed"/> is null or empty.
		/// </exception>
		/// <exception cref="ArgumentException">
		///	Seed need 4 numbers.
		/// </exception>
		public RomuQuad(ulong[] seed)
		{
			this._State = new ulong[4];
			this.SetSeed(seed);
		}

		~RomuQuad()
		{
			Array.Clear(this._State, 0, this._State.Length);
		}

		#endregion Constructor & Destructor

		#region Protected Method

		/// <inheritdoc/>
		protected override ulong Next()
		{
			ulong wp = this._State[0];
			ulong xp = this._State[1];
			ulong yp = this._State[2];
			ulong zp = this._State[3];

			this._State[0] = 15241094284759029579u * zp; // a-mult
			this._State[1] = zp + wp.RotateLeft(52); // b-rotl, c-add
			this._State[2] = yp - xp; // d-sub
			this._State[3] = yp + wp; // e-add
			this._State[3] = this._State[3].RotateLeft(19); // f-rotl
			return xp;
		}

		#endregion Protected Method

		#region Public Method

		/// <inheritdoc/>
		public override string AlgorithmName()
		{
			return "Romu Quad 64-bit";
		}

		/// <inheritdoc/>
		public override void Reseed()
		{
			using (var rng = RandomNumberGenerator.Create())
			{
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
				Span<byte> span = new byte[32];
				rng.GetNonZeroBytes(span);
				this.SetSeed(
					seed1: BinaryPrimitives.ReadUInt64LittleEndian(span),
					seed2: BinaryPrimitives.ReadUInt64LittleEndian(span.Slice(8)),
					seed3: BinaryPrimitives.ReadUInt64LittleEndian(span.Slice(16)),
					seed4: BinaryPrimitives.ReadUInt64LittleEndian(span.Slice(24)));
#else
				var bytes = new byte[32];
				rng.GetNonZeroBytes(bytes);
				this.SetSeed(
					seed1: BinaryConverter.ToUInt64(bytes, 0),
					seed2: BinaryConverter.ToUInt64(bytes, 8),
					seed3: BinaryConverter.ToUInt64(bytes, 16),
					seed4: BinaryConverter.ToUInt64(bytes, 24));
#endif
			}
		}

		/// <summary>
		///	Set RNG seed manually.
		/// </summary>
		/// <param name="seed1">
		///	W state.
		/// </param>
		/// <param name="seed2">
		///	X state.
		/// </param>
		/// <param name="seed3">
		///	Y state.
		/// </param>
		/// <param name="seed4">
		///	Z state.
		/// </param>
		public void SetSeed(ulong seed1, ulong seed2, ulong seed3, ulong seed4)
		{
			this._State[0] = seed1;
			this._State[1] = seed2;
			this._State[2] = seed3;
			this._State[3] = seed4;
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
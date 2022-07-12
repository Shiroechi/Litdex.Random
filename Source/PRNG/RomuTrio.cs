#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
using System.Buffers.Binary;
#endif
using System;
using System.Security.Cryptography;

using Litdex.Utilities.Extension;

namespace Litdex.Random.PRNG
{
	/// <summary>
	///	Romu random variations, great for general purpose work, including huge jobs.
	///	Est. capacity = 2^75 bytes. Register pressure = 6. State size = 192 bits.
	/// </summary>
	/// <remarks>
	///	Source: https://www.romu-random.org/
	/// </remarks>
	public class RomuTrio : Random64
	{
		#region Constructor & Destructor

		/// <summary>
		///	Create an instance of <see cref="RomuTrio"/> object.
		/// </summary>
		/// <param name="seed1">
		///	X state.
		/// </param>
		/// <param name="seed2">
		///	Y state.
		/// </param>
		/// <param name="seed3">
		///	Z state.
		/// </param>
		public RomuTrio(ulong seed1 = 0, ulong seed2 = 0, ulong seed3 = 0)
		{
			this._State = new ulong[3];
			this.SetSeed(seed1, seed2, seed3);
		}

		/// <summary>
		///	Create an instance of <see cref="RomuTrio"/> object.
		/// </summary>
		/// <param name="seed">
		///	A array of seed numbers.
		/// </param>
		/// <exception cref="ArgumentException">
		///	Seed need 3 numbers.
		/// </exception>
		public RomuTrio(ulong[] seed)
		{
			this._State = new ulong[3];
			this.SetSeed(seed);
		}

		~RomuTrio()
		{
			Array.Clear(this._State, 0, this._State.Length);
		}

		#endregion Constructor & Destructor

		#region Protected Method

		/// <inheritdoc/>
		protected override ulong Next()
		{
			ulong xp = this._State[0];
			ulong yp = this._State[1];
			ulong zp = this._State[2];

			this._State[0] = 15241094284759029579u * zp; // a-mult
			this._State[1] = yp - xp; // d-sub
			this._State[1] = this._State[1].RotateLeft(12);
			this._State[2] = zp - yp; // e-add
			this._State[2] = this._State[2].RotateLeft(44); // f-rotl
			return xp;
		}

		#endregion Protected Method

		#region Public Method

		/// <inheritdoc/>
		public override string AlgorithmName()
		{
			return "Romu Trio 64-bit";
		}

		/// <inheritdoc/>
		public override void Reseed()
		{
			using (var rng = RandomNumberGenerator.Create())
			{
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
				Span<byte> span = new byte[24];
				rng.GetNonZeroBytes(span);
				this.SetSeed(
					seed1: BinaryPrimitives.ReadUInt64LittleEndian(span),
					seed2: BinaryPrimitives.ReadUInt64LittleEndian(span.Slice(8)),
					seed3: BinaryPrimitives.ReadUInt64LittleEndian(span.Slice(16)));	
#else
				var bytes = new byte[24];
				rng.GetNonZeroBytes(bytes);
				this.SetSeed(
					seed1: BitConverter.ToUInt64(bytes, 0),
					seed2: BitConverter.ToUInt64(bytes, 8),
					seed3: BitConverter.ToUInt64(bytes, 16));
#endif
			}
		}

		/// <summary>
		///	Set RNG seed manually.
		/// </summary>
		/// <param name="seed1">
		///	X state.
		/// </param>
		/// <param name="seed2">
		///	Y state.
		/// </param>
		/// <param name="seed3">
		///	Z state.
		/// </param>
		public void SetSeed(ulong seed1, ulong seed2, ulong seed3)
		{
			this._State[0] = seed1;
			this._State[1] = seed2;
			this._State[2] = seed3;
		}

		#endregion Public Method
	}
}
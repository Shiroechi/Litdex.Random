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
	///	Romu random variations, might be faster than <see cref="RomuTrio"/> due to using fewer registers, but might struggle with massive jobs.
	///	Est. capacity = 2^61 bytes. Register pressure = 5. State size = 128 bits.
	/// </summary>
	/// <remarks>
	///	Source: https://www.romu-random.org/
	/// </remarks>
	public class RomuDuo : Random64
	{
		#region Constructor & Destructor

		/// <summary>
		///	Create an instance of <see cref="RomuDuo"/> object.
		/// </summary>
		/// <param name="seed1">
		///	First RNG seed.
		/// </param>
		/// <param name="seed2">
		///	Second RNG seed.
		/// </param>
		public RomuDuo(ulong seed1 = 0, ulong seed2 = 0)
		{
			this._State = new ulong[2];
			this.SetSeed(seed1, seed2);
		}

		/// <summary>
		///	Create an instance of <see cref="RomuDuo"/> object.
		/// </summary>
		/// <param name="seed">
		///	RNG seed numbers.
		/// </param>
		/// <exception cref="ArgumentOutOfRangeException">
		///	Seed need 2 numbers.
		/// </exception>
		public RomuDuo(ulong[] seed)
		{
			this._State = new ulong[2];
			this.SetSeed(seed);
		}

		~RomuDuo()
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
			this._State[1] = this._State[1].RotateLeft(27) + this._State[1].RotateLeft(15) - xp;
			return xp;
		}

		#endregion Protected Method

		#region Public Method

		/// <inheritdoc/>
		public override string AlgorithmName()
		{
			return "Romu Duo 64-bit";
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

		#endregion Public Method
	}
}
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
	///	Romu random variations, 32-bit arithmetic: Good for general purpose use, except for huge jobs.
	///	Est. capacity >= 2^53 bytes. Register pressure = 5. State size = 96 bits.
	/// </summary>
	/// <remarks>
	///	Source: https://www.romu-random.org/
	/// </remarks>
	public class RomuTrio32 : Random32
	{
		#region Member

		/// <summary>
		///	The internal state of RNG.
		/// </summary>
		protected uint[] _State;

		#endregion Member

		#region Constructor & Destructor

		/// <summary>
		///	Create an instance of <see cref="RomuTrio32"/> object.
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
		public RomuTrio32(uint seed1 = 0, uint seed2 = 0, uint seed3 = 0)
		{
			this._State = new uint[3];
			this.SetSeed(seed1, seed2, seed3);
		}

		/// <summary>
		///	Create an instance of <see cref="RomuTrio32"/> object.
		/// </summary>
		/// <param name="seed">
		///	A array of seed numbers.
		/// </param>
		/// <exception cref="ArgumentException">
		///	Seed need 3 numbers.
		/// </exception>
		public RomuTrio32(uint[] seed)
		{
			this._State = new uint[3];
			this.SetSeed(seed);
		}

		#endregion Constructor & Destructor

		#region Protected Method

		/// <inheritdoc/>
		protected override uint Next()
		{
			uint xp = this._State[0];
			uint yp = this._State[1];
			uint zp = this._State[2];
			this._State[0] = 3323815723u * zp;
			this._State[1] = yp - xp;
			this._State[1] = this._State[1].RotateLeft(6);
			this._State[2] = zp - yp;
			this._State[2] = this._State[2].RotateLeft(22);
			return xp;
		}

		#endregion Protected Method

		#region Public Method

		/// <inheritdoc/>
		public override string AlgorithmName()
		{
			return "Romu Trio 32-bit";
		}

		/// <inheritdoc/>
		public override void Reseed()
		{
			using (var rng = RandomNumberGenerator.Create())
			{
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
				Span<byte> span = new byte[12];
				rng.GetNonZeroBytes(span);
				this.SetSeed(
					seed1: BinaryPrimitives.ReadUInt32LittleEndian(span),
					seed2: BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(4)),
					seed3: BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(8)));
#else
				var bytes = new byte[12];
				rng.GetNonZeroBytes(bytes);
				this.SetSeed(
					seed1: BinaryConverter.ToUInt32(bytes, 0),
					seed2: BinaryConverter.ToUInt32(bytes, 4),
					seed3: BinaryConverter.ToUInt32(bytes, 8));
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
		public void SetSeed(uint seed1, uint seed2, uint seed3)
		{
			this._State[0] = seed1;
			this._State[1] = seed2;
			this._State[2] = seed3;
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
		public virtual void SetSeed(params uint[] seed)
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
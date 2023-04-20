#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
using System.Buffers.Binary;
#endif
using System;
using System.Security.Cryptography;

using Litdex.Utilities.Extension;
using Litdex.Utilities;

namespace Litdex.Random.PRNG
{
	/// <summary>
	///	Implementation of David Blackman's GJrand PRNG(s)
	/// </summary>
	public class GJrand64 : Random64
	{
		#region Member

		/// <summary>
		///	The internal state of RNG.
		/// </summary>
		protected ulong[] _State;

		#endregion Member

		#region Constructor & Destructor

		/// <summary>
		///	Create an instance of <see cref="GJrand64"/> object.
		/// </summary>
		/// <param name="seed1">
		///	First seed.
		/// </param>
		/// <param name="seed2">
		///	Second seed.
		/// </param>
		/// <param name="seed3">
		///	Third seed.
		/// </param>
		/// <param name="seed4">
		///	Fourth seed.
		/// </param>
		public GJrand64(ulong seed1 = 0xCAFEF00DBEEF5EED, ulong seed2 = 0, ulong seed3 = 0, ulong seed4 = 0)
		{
			this._State = new ulong[4];
			this.SetSeed(seed1, seed2, seed3, seed4);
		}

		/// <summary>
		///	Create an instance of <see cref="GJrand64"/> object.
		/// </summary>
		/// <param name="seed">
		///	Array of 64-bit unsigned integer with minimum length of 4. 
		/// </param>
		public GJrand64(ulong[] seed)
		{
			this._State = new ulong[4];
			this.SetSeed(seed);
		}

		~GJrand64()
		{
			Array.Clear(this._State, 0, this._State.Length);
		}

		#endregion Constructor & Destructor

		#region Protected Method

		/// <inheritdoc/>
		protected override ulong Next()
		{
			this.Advance();
			return this._State[0];
		}

		protected void Advance()
		{
			this._State[1] += this._State[2];
			this._State[0] = this._State[0].RotateLeft(32);
			this._State[2] ^= this._State[1];
			this._State[3] += 0x55AA96A5;
			this._State[0] += this._State[1];
			this._State[2] = this._State[2].RotateLeft(23);
			this._State[1] ^= this._State[0];
			this._State[0] += this._State[2];
			this._State[1] = this._State[1].RotateLeft(19);
			this._State[2] += this._State[0];
			this._State[1] += this._State[3];
		}

		#endregion Protected Method

		#region Public Method

		/// <inheritdoc/>
		public override string AlgorithmName()
		{
			return "Gjrand";
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
		public void SetSeed(ulong seed1, ulong seed2, ulong seed3, ulong seed4)
		{
			this._State[0] = seed1;
			this._State[1] = seed2;
			this._State[2] = seed3;
			this._State[3] = seed4;

			for (var i = 0; i < RandomUtil.InitialRoll; i++)
			{
				this.Advance();
			}
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
		public void SetSeed(params ulong[] seed)
		{
			if (seed == null || seed.Length == 0)
			{
				throw new ArgumentNullException(nameof(seed), "Seed can't null or empty.");
			}

			if (seed.Length < this._State.Length)
			{
				throw new ArgumentException($"Seed need at least {this._State.Length} numbers.", nameof(seed));
			}

			this.SetSeed(seed[0], seed[1], seed[2], seed[3]);
		}

		#endregion Public Method
	}
}
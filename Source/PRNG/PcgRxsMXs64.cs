#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
using System.Buffers.Binary;
#endif
using System;
using System.Security.Cryptography;

using Litdex.Utilities;

namespace Litdex.Random.PRNG
{
	/// <summary>
	///	A Permuted Congruential Generator (PCG) that is composed of a 64-bit 
	///	Linear Congruential Generator(LCG) combined with the RXS-M-XS (random xorshift; multiply; xorshift) output
	///	transformation to create 64-bit output.
	/// </summary>
	/// <remarks>
	///	Source: https://www.pcg-random.org/
	/// </remarks>
	public class PcgRxsMXs64 : Random64
	{
		#region Member

		protected ulong _State;
		protected ulong _Increment;
		protected const ulong _Multiplier = 6364136223846793005u;

		#endregion Member

		#region Constructor & Destructor

		/// <summary>
		///	Create an instance of <see cref="PcgRxsMXs32"/> object.
		/// </summary>
		/// <param name="seed">
		///	RNG seed.
		///	</param>
		/// <param name="increment">
		///	Increment step.
		///	</param>
		public PcgRxsMXs64(ulong seed = 0, ulong increment = 0)
		{
			this.SetSeed(seed, increment);
		}

		/// <summary>
		///	Destructor.
		/// </summary>
		~PcgRxsMXs64()
		{
			this._State = 0;
			this._Increment = 0;
		}

		#endregion Constructor & Destructor

		#region Protected Method

		/// <inheritdoc/>
		protected override ulong Next()
		{
			var oldState = this._State;
			this._State = this._State * _Multiplier + this._Increment;
			
			ulong word = ((oldState >> ((int)(oldState >> 59) + 5)) ^ oldState) * 12605985483714917081;
			return (word >> 43) ^ word;
		}

		#endregion Protected Method

		#region Public Method

		/// <inheritdoc/>
		public override string AlgorithmName()
		{
			return "PCG RXS-M-XS 64-bit";
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
					seed: BinaryPrimitives.ReadUInt64LittleEndian(span),
					increment: BinaryPrimitives.ReadUInt64LittleEndian(span.Slice(8)));
#else
				var bytes = new byte[16];
				rng.GetNonZeroBytes(bytes);
				this.SetSeed(
					seed: BinaryConverter.ToUInt64(bytes, 0),
					increment: BinaryConverter.ToUInt64(bytes, 8));
#endif

			}
		}

		/// <summary>
		///	Set RNG seed manually.
		/// </summary>
		/// <param name="seed">
		///	RNG seed.
		/// </param>
		/// <param name="increment">
		///	Increment step.
		/// </param>
		public void SetSeed(ulong seed, ulong increment)
		{
			this._State = 0;
			this._Increment = increment << 1 | 1;
			this._State = this._State * _Multiplier + this._Increment;
			this._State += seed;
			this._State = this._State * _Multiplier + this._Increment;
		}

		/// <inheritdoc/>
		public void SetSeed(params ulong[] seed)
		{
			if (seed == null || seed.Length == 0)
			{
				throw new ArgumentNullException(nameof(seed), "Seed can't null or empty.");
			}

			this.SetSeed(seed[0], seed[1]);
		}

		#endregion Public Method
	}
}
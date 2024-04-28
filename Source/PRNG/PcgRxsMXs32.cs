#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
using System.Buffers.Binary;
#endif
using System;
using System.Security.Cryptography;

using Litdex.Utilities;

namespace Litdex.Random.PRNG
{
	/// <summary>
	///	A Permuted Congruential Generator (PCG) that is composed of a 32-bit 
	///	Linear Congruential Generator(LCG) combined with the RXS-M-XS (random xorshift; multiply; xorshift) output
	///	transformation to create 64-bit output.
	/// </summary>
	/// <remarks>
	///	Source: https://www.pcg-random.org/
	/// </remarks>
	public class PcgRxsMXs32 : Random64
	{
		#region Member

		protected ulong _State;
		protected ulong _Increment;
		protected const ulong _Multiplier = 747796405;

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
		public PcgRxsMXs32(uint seed = 0, uint increment = 0)
		{
			this.SetSeed(seed, increment);
		}

		/// <summary>
		///	Destructor.
		/// </summary>
		~PcgRxsMXs32()
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
			
			ulong word = ((oldState >> ((int)(oldState >> 28) + 4)) ^ oldState) * 277803737;
			return (word >> 22) ^ word;
		}

		#endregion Protected Method

		#region Public Method

		/// <inheritdoc/>
		public override string AlgorithmName()
		{
			return "PCG RXS-M-XS 32-bit";
		}

		/// <inheritdoc/>
		public override void Reseed()
		{
			using (var rng = RandomNumberGenerator.Create())
			{
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
				Span<byte> span = new byte[8];
				rng.GetNonZeroBytes(span);
				this.SetSeed(
					seed: BinaryPrimitives.ReadUInt32LittleEndian(span),
					increment: BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(4)));
#else
				var bytes = new byte[8];
				rng.GetNonZeroBytes(bytes);
				this.SetSeed(
					seed: BinaryConverter.ToUInt32(bytes, 0),
					increment: BinaryConverter.ToUInt32(bytes, 4));
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
		public void SetSeed(uint seed, uint increment)
		{
			this._State = 0;
			this._Increment = increment << 1 | 1;
			this._State = this._State * _Multiplier + this._Increment;
			this._State += seed;
			this._State = this._State * _Multiplier + this._Increment;
		}

		/// <inheritdoc/>
		public void SetSeed(params uint[] seed)
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
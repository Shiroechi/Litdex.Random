#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
using System.Buffers.Binary;
# endif
using System;
using System.Security.Cryptography;

namespace Litdex.Random.PRNG
{
	/// <summary>
	/// This abstract class is a base for algorithms from the 
	/// Permuted Congruential Generator(PCG) family that use 
	/// an internal 64-bit Linear Congruential Generator(LCG)
	/// and output 32-bits per cycle.
	/// </summary>
	/// <remarks>
	/// Due to the use of an underlying linear congruential generator (LCG) 
	/// alterations to the 128 bit seed have the following effect: 
	/// <list type="bullet">
	/// The first 64-bits alter the generator state.
	/// </list>
	/// <list type="bullet">
	/// The second 64 bits, with the exception of the most significant bit,
	/// which is discarded, choose between one of two alternative LCGs
	/// where the output of the chosen LCG is the same sequence except for
	/// an additive constant determined by the seed bits.The result is that
	/// seeds that differ only in the last 64-bits will have a 50% chance
	/// of producing highly correlated output sequences.
	/// </list>
	/// </remarks>
	public abstract class Pcg32Base : Random32
	{
		#region Member

		/// <summary>
		/// Default multiplier for PCG with 64-bit state
		/// </summary>
		protected const ulong _PCG_Multiplier_64 = 6364136223846793005u;

		/// <summary>
		/// Default increment for PCG with 64-bit state
		/// </summary>
		protected const ulong _PCG_Increment_64 = 1442695040888963407u;
		
		protected ulong _State0;
		protected ulong _Increment;

		#endregion Member

		#region Protected Method

		#endregion Protected Method

		#region Public Method

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
					seed: BitConverter.ToUInt64(bytes, 0),
					increment: BitConverter.ToUInt64(bytes, 8));
#endif
			}
		}

		/// <inheritdoc/>
		public override void SetSeed(params uint[] seed)
		{
			if (seed == null || seed.Length == 0)
			{
				throw new ArgumentNullException(nameof(seed), "Seed can't null or empty.");
			}

			if (seed.Length < 4)
			{
				throw new ArgumentException($"Seed need at least 4 numbers.", nameof(seed));
			}

			this.SetSeed(ToUint64(seed[0], seed[1]), ToUint64(seed[2], seed[3]));
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
		public virtual void SetSeed(ulong seed, ulong increment)
		{
			if (increment == 0)
			{
				increment = _PCG_Increment_64;
			}

			this._State0 = 0;
			this._Increment = increment | 1;
			this._State0 = this._State0 * _PCG_Multiplier_64 + _PCG_Increment_64;
			this._State0 += seed;
			this._State0 = this._State0 * _PCG_Multiplier_64 + _PCG_Increment_64;
		}

		#endregion Public Method
	}
}

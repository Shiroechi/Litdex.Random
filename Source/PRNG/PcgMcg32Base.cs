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
	/// an internal 64-bit Multiplicative Congruential Generator (MCG)
	/// and output 32-bits per cycle.
	/// </summary>
	public abstract class PcgMcg32Base : Random32
	{
		#region Member

		protected const ulong _PCG_Multiplier_64 = 6364136223846793005u;
		protected ulong _State0;

		#endregion Member

		#region Public Method

		/// <inheritdoc/>
		public override void Reseed()
		{
			using (var rng = RandomNumberGenerator.Create())
			{
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
				Span<byte> span = new byte[8];
				rng.GetNonZeroBytes(span);
				this.SetSeed(
					seed: BinaryPrimitives.ReadUInt64LittleEndian(span));
#else
				var bytes = new byte[8];
				rng.GetNonZeroBytes(bytes);
				this.SetSeed(
					seed: BitConverter.ToUInt64(bytes, 0));
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

			if (seed.Length < 2)
			{
				throw new ArgumentException($"Seed need at least 2 numbers.", nameof(seed));
			}

			this.SetSeed(ToUint64(seed[0], seed[1]));
		}

		/// <summary>
		///	Set RNG seed manually.
		/// </summary>
		/// <param name="seed">
		///	RNG seed.
		/// </param>
		public virtual void SetSeed(ulong seed)
		{
			this._State0 = seed | 1;
		}

		#endregion Public Method
	}
}

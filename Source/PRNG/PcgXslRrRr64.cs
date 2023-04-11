#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
using System.Buffers.Binary;
#endif
using System.Security.Cryptography;
using System;
using Litdex.Utilities.Extension;

namespace Litdex.Random.PRNG
{
	/// <summary>
	/// 
	/// </summary>
	public class PcgXslRrRr64 : Random64
	{
		#region Member

		/// <summary>
		/// Default multiplier for PCG with 64-bit state
		/// </summary>
		protected const ulong _PCG_Multiplier_64 = 6364136223846793005u;

		#endregion Member

		#region Protected Method

		/// <inheritdoc/>
		protected override ulong Next()
		{
			var oldState = this._State[0];
			this._State[0] = this._State[0] * _PCG_Multiplier_64 + this._State[1];

			var rot1 = (int)(oldState >> 59);
			var high = (uint)(oldState >> 32);
			var low = (uint)oldState;
			var xored = high ^ low;
			var newlow = xored.RotateRight(rot1);
			var newhigh = high.RotateRight((int)(newlow & 31));
			return (((ulong)newhigh) << 32) | newlow;
		}

		#endregion Protected Method

		#region Public Method

		/// <inheritdoc/>
		public override string AlgorithmName()
		{
			return "PCG XSL-RR-RR 64-bit";
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
					seed: BitConverter.ToUInt64(bytes, 0),
					increment: BitConverter.ToUInt64(bytes, 8));
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
			this._State[0] = 0;
			this._State[1] = increment | 1;
			this._State[0] = this._State[0] * _PCG_Multiplier_64 + this._State[1];
			this._State[0] += seed;
			this._State[0] = this._State[0] * _PCG_Multiplier_64 + this._State[1];
		}

		/// <inheritdoc/>
		public override void SetSeed(params ulong[] seed)
		{
			base.SetSeed(seed);
			this.SetSeed(seed[0], seed[1]);
		}

		#endregion Public Method
	}
}

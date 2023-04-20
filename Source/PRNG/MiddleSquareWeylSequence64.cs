#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
using System.Buffers.Binary;
#endif
using System;
using System.Security.Cryptography;

using Litdex.Utilities;

namespace Litdex.Random.PRNG
{
	/// <summary>
	///	Improved version from Middle Square Method, invented by John Von Neumann.
	/// </summary>
	/// <remarks>
	///	Source: https://arxiv.org/abs/1704.00358
	/// </remarks>
	public class MiddleSquareWeylSequence64 : Random64
	{
		#region Member

		ulong _X1 = 0, _W1 = 0, _S1 = 0xb5ad4eceda1ce2a9;
		ulong _X2 = 0, _W2 = 0, _S2 = 0x278c5a4d8419fe6b;

		#endregion Member

		#region Constructor & Destructor

		/// <summary>
		///	Create an instance of <see cref="MiddleSquareWeylSequence64"/> object.
		/// </summary>
		/// <param name="seed1">
		///	RNG seed.
		///	</param>
		/// <param name="seed2">
		///	RNG seed.
		///	</param>
		public MiddleSquareWeylSequence64(ulong seed1 = 0, ulong seed2 = 1)
		{
			this.SetSeed(seed1, seed2);
		}

		/// <summary>
		///	Destructor.
		/// </summary>
		~MiddleSquareWeylSequence64()
		{
			this._X1 = 0;
			this._W1 = 0;
			this._S1 = 0;
			this._X2 = 0;
			this._W2 = 0;
			this._S2 = 0;
		}

		#endregion Constructor & Destructor

		#region Protected Method

		/// <inheritdoc/>
		protected override ulong Next()
		{
			ulong xx;

			this._X1 *= this._X1;
			xx = this._X1 += (this._W1 += this._S1);
			this._X1 = (this._X1 >> 32) | (this._X1 << 32);
			this._X2 *= this._X2;
			this._X2 += (this._W2 += this._S2);
			this._X2 = (this._X2 >> 32) | (this._X2 << 32);

			return xx ^ this._X2;

		}

		#endregion Protected Method

		#region Public Method

		/// <inheritdoc/>
		public override string AlgorithmName()
		{
			return "Middle Square Weyl Sequence 64-bit";
		}

		/// <inheritdoc/>
		public override void Reseed()
		{
			using (var rng = RandomNumberGenerator.Create())
			{
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
				Span<byte> span = new byte[32];
				rng.GetNonZeroBytes(span);
				this._X1 = BinaryPrimitives.ReadUInt64LittleEndian(span);
				this._X2 = BinaryPrimitives.ReadUInt64LittleEndian(span.Slice(8));
				this._W1 = BinaryPrimitives.ReadUInt64LittleEndian(span.Slice(16));
				this._W2 = BinaryPrimitives.ReadUInt64LittleEndian(span.Slice(24));
#else
				var bytes = new byte[32];
				rng.GetNonZeroBytes(bytes);
				this._X1 = BinaryConverter.ToUInt64(bytes, 0);
				this._X2 = BinaryConverter.ToUInt64(bytes, 8);
				this._W1 = BinaryConverter.ToUInt64(bytes, 16);
				this._W2 = BinaryConverter.ToUInt64(bytes, 24);
#endif
			}
		}

		/// <summary>
		///	Set RNG seed manually.
		/// </summary>
		/// <param name="seed1">
		///	RNG seed.
		/// </param>
		/// <param name="seed2">
		///	RNG seed.
		/// </param>
		public void SetSeed(ulong seed1, ulong seed2)
		{
			this._X1 = seed1;
			this._W1 = seed1;
			this._X2 = seed2;
			this._W2 = seed2;
		}

		public void Jump()
		{

		}

		#endregion Public Method
	}
}
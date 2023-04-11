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
	///	(XOR/shift/rotate) all-purpose generators.
	/// </summary>
	/// <remarks>
	///	Source: https://prng.di.unimi.it/xoroshiro1024star.c
	/// </remarks>
	public class Xoroshiro1024Star : Random64
	{
		#region Member

		protected int _P;

		#endregion Member

		#region Constructor & Destructor

		/// <summary>
		///	Create an instance of <see cref="Xoroshiro1024Star"/> object.
		/// </summary>
		/// <param name="seeds">
		/// RNG seeds.
		/// </param>
		public Xoroshiro1024Star(params ulong[] seeds)
		{
			this._State = new ulong[16];
			this.SetSeed(seeds);
		}

		~Xoroshiro1024Star()
		{
			Array.Clear(this._State, 0, this._State.Length);
			this._P = 0;
		}

		#endregion Constructor & Destructor

		#region Protected Method

		/// <inheritdoc/>
		protected override ulong Next()
		{
			int q = this._P;
			ulong s0 = this._State[this._P = (this._P + 1) & 15];
			ulong s15 = this._State[q];
			ulong result = s0 * 0x9E3779B97F4A7C13;

			s15 ^= s0;
			this._State[q] = s0.RotateLeft(25) ^ s15 ^ (s15 << 27);
			this._State[this._P] = s15.RotateLeft(36);

			return result;
		}

		#endregion Protected Method

		#region Public Method

		/// <inheritdoc/>
		public override string AlgorithmName()
		{
			return "Xoroshiro 1024*";
		}

		/// <inheritdoc/>
		public override void Reseed()
		{
			using (var rng = RandomNumberGenerator.Create())
			{
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
				Span<byte> span = new byte[128];
				rng.GetNonZeroBytes(span);
				this.SetSeed(
					BinaryPrimitives.ReadUInt64LittleEndian(span),
					BinaryPrimitives.ReadUInt64LittleEndian(span.Slice(8)),
					BinaryPrimitives.ReadUInt64LittleEndian(span.Slice(16)),
					BinaryPrimitives.ReadUInt64LittleEndian(span.Slice(24)),
					BinaryPrimitives.ReadUInt64LittleEndian(span.Slice(32)),
					BinaryPrimitives.ReadUInt64LittleEndian(span.Slice(40)),
					BinaryPrimitives.ReadUInt64LittleEndian(span.Slice(48)),
					BinaryPrimitives.ReadUInt64LittleEndian(span.Slice(56)),
					BinaryPrimitives.ReadUInt64LittleEndian(span.Slice(64)),
					BinaryPrimitives.ReadUInt64LittleEndian(span.Slice(72)),
					BinaryPrimitives.ReadUInt64LittleEndian(span.Slice(80)),
					BinaryPrimitives.ReadUInt64LittleEndian(span.Slice(88)),
					BinaryPrimitives.ReadUInt64LittleEndian(span.Slice(96)),
					BinaryPrimitives.ReadUInt64LittleEndian(span.Slice(104)),
					BinaryPrimitives.ReadUInt64LittleEndian(span.Slice(112)),
					BinaryPrimitives.ReadUInt64LittleEndian(span.Slice(120)));
#else
				var bytes = new byte[128];
				rng.GetNonZeroBytes(bytes);
				this.SetSeed(
					BinaryConverter.ToUInt64(bytes, 0),
					BinaryConverter.ToUInt64(bytes, 8),
					BinaryConverter.ToUInt64(bytes, 16),
					BinaryConverter.ToUInt64(bytes, 24),
					BinaryConverter.ToUInt64(bytes, 32),
					BinaryConverter.ToUInt64(bytes, 40),
					BinaryConverter.ToUInt64(bytes, 48),
					BinaryConverter.ToUInt64(bytes, 56), 
					BinaryConverter.ToUInt64(bytes, 64),
					BinaryConverter.ToUInt64(bytes, 72),
					BinaryConverter.ToUInt64(bytes, 80),
					BinaryConverter.ToUInt64(bytes, 88),
					BinaryConverter.ToUInt64(bytes, 96),
					BinaryConverter.ToUInt64(bytes, 104),
					BinaryConverter.ToUInt64(bytes, 112),
					BinaryConverter.ToUInt64(bytes, 120)
					);
#endif
			}
		}

		#endregion Public Method
	}
}

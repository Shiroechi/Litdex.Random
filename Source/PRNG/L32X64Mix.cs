#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
using System.Buffers.Binary;
#endif
using System;
using System.Security.Cryptography;

using Litdex.Utilities.Extension;

namespace Litdex.Random.PRNG
{
	/// <summary>
	/// This is a member of the LXM family of generators: L=Linear congruential generator; 
	/// X = Xor based generator; and M = Mix.This member uses a 32-bit LCG and 64-bit Xor-based generator.
	/// </summary>
	public class L32X64Mix : LxmBase32
	{
		#region Member

		/// <summary>
		/// Size of the state vector.
		/// </summary>
		//private const int _SeedSize = 4;

		/// <summary>
		/// Per-instance LCG additive parameter (must be odd).
		/// </summary>
		//private uint la;

		/// <summary>
		/// State of the LCG generator.
		/// </summary>
		//private uint ls;

		/// <summary>
		/// State 0 of the XBG generator.
		/// </summary>
		//private uint x0;

		/// <summary>
		/// State 1 of the XBG generator.
		/// </summary>
		//private uint x1;

		#endregion Member

		#region Constructor & Destructor

		/// <summary>
		/// Creates a new instance of <see cref="L32X64Mix"/> object using a 4 element seed.
		/// A seed containing all zeros in the last two elements
		/// will create a non-functional XBG sub-generator and a low
		/// quality output with a period of 2^32.
		/// </summary>
		/// <remarks>
		/// The 1st element is used to set the LCG increment; the least significant bit
		/// is set to odd to ensure a full period LCG.The 2nd element is used
		/// to set the LCG state.
		/// </remarks>
		/// <param name="seed1">Initial seed element 1</param>
		/// <param name="seed2">Initial seed element 2</param>
		/// <param name="seed3">Initial seed element 3</param>
		/// <param name="seed4">Initial seed element 4</param>
		public L32X64Mix(uint seed1 = 0, uint seed2 = 0, uint seed3 = 1, uint seed4 = 1)
		{
			this._State = new uint[_Size];
			this._State[0] = seed1; // la
			this._State[1] = seed2; // ls
			this._State[2] = seed3; // x0
			this._State[3] = seed4; // x1
		}

		/// <summary>
		/// Create an instance of <see cref="L32X64Mix"/> object.
		/// </summary>
		/// <param name="seeds">Initial seed.</param>
		public L32X64Mix(params uint[] seeds)
		{
			if (seeds.Length < SEED_SIZE)
			{
				throw new ArgumentException($"Seeds size is lower than { SEED_SIZE }.", nameof(seeds));
			}

			this._State = new uint[_Size];
			this._State[0] = seeds[0] | 1; // la
			this._State[1] = seeds[1]; // ls
			this._State[2] = seeds[2]; // x0
			this._State[3] = seeds[3]; // x1
		}

		#endregion Constructor & Destructor

		#region Protected Method

		/// <inheritdoc/>
		protected override uint Next()
		{
			// LXM generate.
			// Old state is used for the output allowing parallel pipelining
			// on processors that support multiple concurrent instructions.

			uint s0 = this._State[2];
			uint s = this._State[1];

			// Mix
			uint z = this.Lea32(s + s0);

			// LCG update
			this._State[1] = M32 * s + this._State[0];

			// XBG update
			uint s1 = this._State[3];

			s1 ^= s0;
			this._State[2] = s0.RotateLeft(26); // a
			this._State[2] = this._State[2] ^ s1 ^ (s1 << 9); // b
			this._State[3] = s1.RotateLeft(13); // c

			return z;
		}

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
				this.SetSeed(BinaryPrimitives.ReadUInt32LittleEndian(span),
							 BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(4)),
							 BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(8)),
							 BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(12)));
#else
				var bytes = new byte[16];
				rng.GetNonZeroBytes(bytes);
				this.SetSeed(BitConverter.ToUInt32(bytes, 0),
							 BitConverter.ToUInt32(bytes, 4),
							 BitConverter.ToUInt32(bytes, 8),
							 BitConverter.ToUInt32(bytes, 12));
#endif
			}
		}

		#endregion Public Method
	}
}

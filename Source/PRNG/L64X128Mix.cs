using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;

using Litdex.Random.PRNG;
using Litdex.Utilities.Extension;

namespace Litdex.Random.PRNG
{
	/// <summary>
	/// This is a member of the LXM family of generators: L=Linear congruential generator;
	/// X = Xor based generator; and M = Mix.This member uses a 64-bit LCG and 128-bit Xor-based generator.
	/// </summary>
	public class L64X128Mix : LxmBase64
	{
		#region Member

		#endregion Member

		#region Constructor & Destructor

		/// <summary>
		/// Create an instance of <see cref="L64X128Mix"/> object.
		/// </summary>
		/// <param name="seed1">The first seed.</param>
		/// <param name="seed2">The second seed.</param>
		/// <param name="seed3">The third seed.</param>
		/// <param name="seed4">The fourth seed.</param>
		public L64X128Mix(ulong seed1 = 0, ulong seed2 = 0, ulong seed3 = 1, ulong seed4 = 1)
		{
			this._State= new ulong[4];
			this._State[0] = seed1 | 1;
			this._State[1] = seed2;
			this._State[2] = seed3;
			this._State[3] = seed4;
		}

		/// <summary>
		/// Create an instance of <see cref="L64X128Mix"/> object.
		/// </summary>
		/// <param name="seeds">Initial seed.</param>
		public L64X128Mix(params ulong[] seeds)
		{
			if (seeds.Length < SEED_SIZE)
			{
				throw new ArgumentException($"Seeds size is lower than { SEED_SIZE }.", nameof(seeds));
			}

			this._State = new ulong[4];
			this._State[0] = seeds[0] | 1;
			this._State[1] = seeds[1];
			this._State[2] = seeds[2];
			this._State[3] = seeds[3];
		}

		#endregion Constructor & Destructor

		#region Protected Method

		/// <inheritdoc/>
		protected override ulong Next()
		{
			// LXM generate.
			// Old state is used for the output allowing parallel pipelining
			// on processors that support multiple concurrent instructions.

			ulong s0 = this._State[2];
			ulong s = this._State[1];

			// Mix
			ulong z = this.Lea64(s + s0);

			// LCG update
			this._State[1] = M64 * s + this._State[0];

			// XBG update
			ulong s1 = this._State[3];

			s1 ^= s0;
			this._State[2] = s0.RotateLeft(24) ^ s1 ^ (s1 << 16); // a, b
			this._State[2] = s1.RotateLeft(37); // c

			return z;
		}

		#endregion Protected Method

		#region Public Method

		/// <inheritdoc/>
		public override void Reseed()
		{
			throw new NotImplementedException();
		}

		#endregion Public Method

	}
}

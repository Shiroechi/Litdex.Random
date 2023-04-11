using System;

using Litdex.Utilities.Extension;

namespace Litdex.Random.PRNG
{
	/// <summary>
	///	Vartions of <see cref="Xoroshiro1024Star"/>.
	/// </summary>
	/// <remarks>
	///	Source: https://prng.di.unimi.it/xoroshiro1024plusplus.c
	/// </remarks>
	public class Xoroshiro1024PlusPlus : Xoroshiro1024Star
	{
		#region Constructor & Destructor

		/// <summary>
		///	Create an instance of <see cref="Xoroshiro1024PlusPlus"/> object.
		/// </summary>
		/// <param name="seeds">
		/// RNG seeds.
		/// </param>
		public Xoroshiro1024PlusPlus(params ulong[] seeds)
		{
			this._State = new ulong[16];
			this.SetSeed(seeds);
		}

		~Xoroshiro1024PlusPlus()
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
			ulong result = (s0 + s15).RotateLeft(23) + s15;

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
			return "Xoroshiro 1024++";
		}

		#endregion Public Method
	}
}

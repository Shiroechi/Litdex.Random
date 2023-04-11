﻿using Litdex.Utilities.Extension;

namespace Litdex.Random.PRNG
{
	/// <summary>
	///	A Permuted Congruential Generator (PCG) that is composed of 
	///	a 64-bit Linear Congruential Generator (LCG) combined with 
	///	the XSH-RR (xorshift; random rotate) output transformation 
	///	to create 32-bit output.
	/// </summary>
	/// <remarks>
	///	Source: https://www.pcg-random.org/
	/// </remarks>
	public class PcgXshRr32 : Pcg32Base
	{
		#region Constructor & Destructor

		/// <summary>
		///	Create an instance of <see cref="PcgXshRr32"/> object.
		/// </summary>
		/// <param name="seed">
		///	RNG seed.
		///	</param>
		/// <param name="increment">
		///	Increment step.
		///	</param>
		public PcgXshRr32(ulong seed = 0, ulong increment = 0)
		{
			this._State = new uint[1]; // not used, but initilized
			this.SetSeed(seed, increment);
		}

		/// <summary>
		///	Destructor.
		/// </summary>
		~PcgXshRr32()
		{
			this._State0 = 0;
			this._Increment = 0;
		}

		#endregion Constructor & Destructor

		#region Protected Method

		/// <inheritdoc/>
		protected override uint Next()
		{
			var oldState = this._State0;
			this._State0 = (oldState * _PCG_Multiplier_64) + (this._Increment | 1);
			var xorshifted = (uint)(((oldState >> 18) ^ oldState) >> 27);
			var rot = (int)(oldState >> 59);
			return xorshifted.RotateRight(rot);
		}

		#endregion Protected Method

		#region Public Method

		/// <inheritdoc/>
		public override string AlgorithmName()
		{
			return "PCG XSH-RR 32-bit";
		}

		#endregion Public Method
	}
}
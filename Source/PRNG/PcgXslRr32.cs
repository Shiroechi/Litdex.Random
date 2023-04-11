using Litdex.Utilities.Extension;

namespace Litdex.Random.PRNG
{
	/// <summary>
	/// A Permuted Congruential Generator (PCG) that is composed of
	/// a 64-bit Linear Congruential Generator (LCG) combined with 
	/// the XSL-RR(xorshift low bits; random rotate) output transformation
	/// to create 32-bit output.
	/// </summary>
	/// <remarks>
	///	Source: https://www.pcg-random.org/
	/// </remarks>
	public class PcgXslRr32 : Pcg32Base
	{
		#region Constructor & Destructor

		/// <summary>
		///	Create an instance of <see cref="PcgXslRr32"/> object.
		/// </summary>
		/// <param name="seed">
		///	RNG seed.
		///	</param>
		/// <param name="increment">
		///	Increment step.
		///	</param>
		public PcgXslRr32(ulong seed = 0, ulong increment = 0)
		{
			this._State = new uint[1];
			this.SetSeed(seed, increment);
		}

		~PcgXslRr32()
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
			this._State0 = this._State0 * _PCG_Multiplier_64 + this._Increment;

			var xorshift = ((uint)(oldState >> 32)) ^ (uint)oldState;
			int rotate = (int)(oldState >> 59);
			return xorshift.RotateRight(rotate);
		}

		#endregion Protected Method
		
		#region Public Method
		
		/// <inheritdoc/>
		public override string AlgorithmName()
		{
			return "PCG XSL RR 32-bit";
		}

		#endregion Public Method
	}
}

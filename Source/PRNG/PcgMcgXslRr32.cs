using Litdex.Utilities.Extension;

namespace Litdex.Random.PRNG
{
	/// <summary>
	/// A Permuted Congruential Generator (PCG) that is composed of
	/// a 64-bit Multiplicative Congruential Generator(MCG) combined
	/// with the XSL-RR(xorshift low bits; random rotate) output transformation
	/// to create 32-bit output.
	/// </summary>
	public class PcgMcgXslRr32 : PcgMcg32Base
	{
		#region Constructor & Destructor

		/// <summary>
		/// Create an instance of <see cref="PcgMcgXslRr32"/> object.
		/// </summary>
		/// <param name="seed">
		/// RNG seed.
		/// </param>
		public PcgMcgXslRr32(ulong seed = 0)
		{
			this._State = new uint[1];
			this.SetSeed(seed);
		}

		~PcgMcgXslRr32()
		{
			this._State0 = 0;
		}

		#endregion Constructor & Destructor

		#region Protected Method

		/// <inheritdoc/>
		protected override uint Next()
		{
			ulong oldState = this._State0;
			this._State0 *= _PCG_Multiplier_64;
			int count = (int)(oldState >> 59);
			return ((uint)((uint)oldState ^ (oldState >> 32))).RotateRight(count);
		}

		#endregion Protected Method

		#region Public Method

		/// <inheritdoc/>
		public override string AlgorithmName()
		{
			return "PCG MCG-XSL-RR 32-bit";
		}

		#endregion Public Method
	}
}

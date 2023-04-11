using Litdex.Utilities.Extension;

namespace Litdex.Random.PRNG
{
	/// <summary>
	/// A Permuted Congruential Generator (PCG) that is composed of
	/// a 64-bit Multiplicative Congruential Generator(MCG) combined
	/// with the XSH-RR(xorshift; random rotate) output transformation
	/// to create 32-bit output.
	/// </summary>
	public class PcgMcgXshRr32 : PcgMcg32Base
	{
		#region Constructor & Destructor

		/// <summary>
		/// Create an instance of <see cref="PcgMcgXshRr32"/> object.
		/// </summary>
		/// <param name="seed">
		/// RNG seed.
		/// </param>
		public PcgMcgXshRr32(ulong seed)
		{
			this._State = new uint[1];
			this.SetSeed(seed);
		}

		~PcgMcgXshRr32()
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
			return ((uint)((oldState ^ (oldState >> 18)) >> 27)).RotateRight(count);
		}

		#endregion Protected Method

		#region Public Method

		/// <inheritdoc/>
		public override string AlgorithmName()
		{
			return "PCG MCG-XSH-RR 32-bit";
		}

		#endregion Public Method
	}
}

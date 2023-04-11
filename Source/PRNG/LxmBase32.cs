namespace Litdex.Random.PRNG
{	
	public abstract class LxmBase32 : Random32
	{
		#region Member

		/// <summary>
		/// 32-bit LCG multiplier.
		/// </summary>
		protected const uint M32 = 0xadb4a92d;

		/// <summary>
		/// Jump constant for an advance of the 32-bit LCG by 2^16.
		/// </summary>
		protected const int M32P = 0x65640001;
		
		/// <summary>
		/// Jump constant precursor for an advance of the 32-bit LCG by 2^16.
		/// </summary>
		protected const int C32P = 0x046b0000;

		/// <summary>
		/// The fractional part of the golden ratio, phi, scaled to 32-bits and rounded to odd.
		/// </summary>
		/// <remarks>
		/// phi = (sqrt(5) - 1) / 2) * 2^32
		/// </remarks>
		protected const uint GOLDEN_RATIO_32 = 0x9e3779b9;

		protected const byte SEED_SIZE = 4;

		#endregion Member

		#region Protected Method

		/// <summary>
		/// Perform a 32-bit mixing function using Doug Lea's 32-bit mix constants and shifts.
		/// </summary>
		/// <param name="x">
		/// Value to be mixed.
		/// </param>
		/// <returns>
		/// The output value.
		/// </returns>
		protected uint Lea32(uint x)
		{
			x = (x ^ (x >> 16)) * 0xd36d884b;
			x = (x ^ (x >> 16)) * 0xd36d884b;
			return x ^ (x >> 16);
		}

#endregion Protected Method

#region Public Mrthod

		/// <summary>
		/// The jump is performed by advancing the state of the LCG sub-generator by
		/// 1 cycles. The XBG state is unchanged.
		/// </summary>
		public virtual void NextJump()
		{
			this._State[1] = M32 * this._State[1] + this._State[0];
		}

		/// <summary>
		/// The jump is performed by advancing the state of the LCG sub-generator by
		/// 2^16 cycles. The XBG state is unchanged.
		/// </summary>
		public virtual void NextLongJump()
		{
			this._State[1] = M32P * this._State[1] + C32P * this._State[0];
		}

#endregion Public Method

	}
}

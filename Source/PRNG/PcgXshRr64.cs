#if NET7_0_OR_GREATER
using System;
using System.Buffers.Binary;
using System.Security.Cryptography;

using Litdex.Utilities.Extension;

namespace Litdex.Random.PRNG
{
	/// <summary>
	///	A Permuted Congruential Generator (PCG) that is composed of 
	///	a 128-bit Linear Congruential Generator (LCG) combined with 
	///	the XSH-RR (xorshift; random rotate) output transformation 
	///	to create 64-bit output.
	/// </summary>
	/// <remarks>
	///	Source: https://www.pcg-random.org/
	/// </remarks>
	public class PcgXshRr64 : Random64
	{
		#region Member

		protected UInt128 _State;
		protected UInt128 _Increment = (UInt128)6364136223846793005 << 64 | 1442695040888963407;
		protected UInt128 _Multiplier = (UInt128)2549297995355413924 << 64 | 4865540595714422341;

		#endregion Member

		#region Constructor & Destructor

		/// <summary>
		///	Create an instance of <see cref="PcgXshRr64"/> object.
		/// </summary>
		public PcgXshRr64() : this(UInt128.Zero, UInt128.Zero)
		{

		}

		/// <summary>
		///	Create an instance of <see cref="PcgXshRr64"/> object.
		/// </summary>
		/// <param name="seed">
		///	RNG seed.
		///	</param>
		/// <param name="increment">
		///	Increment step.
		///	</param>
		public PcgXshRr64(UInt128 seed, UInt128 increment)
		{
			this._State = seed;
			this._Increment = increment;
		}

		/// <summary>
		///	Destructor.
		/// </summary>
		~PcgXshRr64()
		{
			this._State = 0;
			this._Increment = 0;
			this._Multiplier = 0;
		}

		#endregion Constructor & Destructor

		#region Protected Method

		/// <inheritdoc/>
		protected override ulong Next()
		{
			var oldState = this._State;
			this._State = (oldState * this._Multiplier) + (this._Increment);
			var xorshifted = (((oldState >> 35) ^ oldState) >> 58);
			var rot = (int)(oldState >> 122);
			return (ulong)xorshifted.RotateRight(rot);
		}

		#endregion Protected Method

		#region Public Method

		/// <inheritdoc/>
		public override string AlgorithmName()
		{
			return "PCG XSH-RR 64-bit";
		}

		/// <inheritdoc/>
		public override void Reseed()
		{
			using var rng = RandomNumberGenerator.Create();
			Span<byte> span = new byte[32];
			rng.GetNonZeroBytes(span);
			this.SetSeed(
				seed: BinaryPrimitives.ReadUInt128LittleEndian(span),
				increment: BinaryPrimitives.ReadUInt128LittleEndian(span.Slice(16)));
		}

		/// <summary>
		///	Set RNG seed manually.
		/// </summary>
		/// <param name="seed">
		///	RNG seed.
		/// </param>
		/// <param name="increment">
		/// RNG increment.
		/// </param>
		public void SetSeed(UInt128 seed, UInt128 increment)
		{
			this._State = 0;
			this._Increment = (increment << 1) | 1;
			this._State = PcgStatic.PcgSetseq128StepR(this._State, this._Increment);
			this._State += seed;
			this._State = PcgStatic.PcgSetseq128StepR(this._State, this._Increment);
		}

		/// <summary>
		///	Set RNG internal state manually.
		/// </summary>
		/// <param name="seed">
		///	Number to generate the random numbers.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///	Array of seed is null or empty.
		/// </exception>
		public void SetSeed(params UInt128[] seed)
		{
			if (seed == null || seed.Length == 0)
			{
				throw new ArgumentNullException(nameof(seed), "Seed can't null or empty.");
			}

			this.SetSeed(seed[0], seed[1]);
		}

		/// <summary>
		/// Advanced the state.
		/// </summary>
		/// <param name="delta">
		/// Number of steps that the generator should advance forward.
		/// </param>
		public void Jump(int delta)
		{
			this._State = PcgStatic.PcgAdvanceLcg128(this._State, delta, this._Multiplier, this._Increment);
		}

		#endregion Public Method
	}
}

#endif
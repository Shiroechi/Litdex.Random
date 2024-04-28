#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
using System.Buffers.Binary;
# endif
using System.Security.Cryptography;
using System;

using Litdex.Utilities.Extension;
using Litdex.Utilities;

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
	public class PcgXshRr32 : Random32
	{
		#region Member

		protected ulong _State;
		protected ulong _Increment = 1442695040888963407;
		protected const ulong _Multiplier = 6364136223846793005;

		#endregion Member

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
		public PcgXshRr32(ulong seed = 0, ulong increment = 1442695040888963407)
		{
			this.SetSeed(seed, increment);
		}

		/// <summary>
		///	Destructor.
		/// </summary>
		~PcgXshRr32()
		{
			this._State = 0;
			this._Increment = 0;
		}

		#endregion Constructor & Destructor

		#region Protected Method

		/// <inheritdoc/>
		protected override uint Next()
		{
			var oldState = this._State;
			this._State = (oldState * _Multiplier) + (this._Increment);
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

		/// <inheritdoc/>
		public override void Reseed()
		{
			using (var rng = RandomNumberGenerator.Create())
			{
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
				Span<byte> span = stackalloc byte[16];
				rng.GetNonZeroBytes(span);
				this.SetSeed(
					seed: BinaryPrimitives.ReadUInt64LittleEndian(span), 
					increment: BinaryPrimitives.ReadUInt64LittleEndian(span.Slice(8)));
#else
				var bytes = new byte[16];
				rng.GetNonZeroBytes(bytes);
				this.SetSeed(
					seed: BinaryConverter.ToUInt64(bytes, 0),
					increment: BinaryConverter.ToUInt64(bytes, 8));
#endif
			}
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
		public void SetSeed(ulong seed, ulong increment)
		{
			this._State = 0;
			this._Increment = (increment << 1) | 1;
			this._State = PcgStatic.PcgSetseq64StepR(this._State, this._Increment);
			this._State += seed;
			this._State = PcgStatic.PcgSetseq64StepR(this._State, this._Increment);
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
		public void SetSeed(params ulong[] seed)
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
			this._State = PcgStatic.PcgAdvanceLcg64(this._State, delta);
		}

		#endregion Public Method
	}
}
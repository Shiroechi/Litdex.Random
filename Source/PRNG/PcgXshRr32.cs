﻿#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
using System.Buffers.Binary;
#endif
using System;
using System.Security.Cryptography;

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

		protected ulong _State0;
		protected ulong _Increment;
		protected const ulong _Multiplier = 6364136223846793005u;

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
		public PcgXshRr32(ulong seed = 0, ulong increment = 0)
		{
			// 2 for seed, 2 for increment 
			this._State = new uint[4];
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
			var oldseed = this._State0;
			this._State0 = (oldseed * _Multiplier) + (this._Increment | 1);
			var xorshifted = (uint)(((oldseed >> 18) ^ oldseed) >> 27);
			var rot = (uint)(oldseed >> 59);
			return (xorshifted >> (int)rot) | (xorshifted << (int)((-rot) & 31));
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
				Span<byte> span = new byte[16];
				rng.GetNonZeroBytes(span);
				this.SetSeed(
					seed: BinaryPrimitives.ReadUInt64LittleEndian(span),
					increment: BinaryPrimitives.ReadUInt64LittleEndian(span.Slice(8)));
#else
				var bytes = new byte[16];
				rng.GetNonZeroBytes(bytes);
				this.SetSeed(
					seed: BitConverter.ToUInt64(bytes, 0),
					increment: BitConverter.ToUInt64(bytes, 8));
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
		///	Increment step.
		/// </param>
		public void SetSeed(ulong seed, ulong increment)
		{
			this._State0 = (seed + increment) * _Multiplier + increment;
			this._Increment = increment;
		}

		/// <inheritdoc/>
		public override void SetSeed(params uint[] seed)
		{
			if (seed == null || seed.Length == 0)
			{
				throw new ArgumentNullException(nameof(seed), "Seed can't null or empty.");
			}

			if (seed.Length < this._State.Length)
			{
				throw new ArgumentException($"Seed need at least {this._State.Length} numbers.", nameof(seed));
			}

			ulong a, b, c, d;
			a = seed[0];
			b = seed[1];
			c = seed[2];
			d = seed[3];

			this.SetSeed((a << 32) | b, (c << 32) | d);
		}

		#endregion Public Method
	}
}
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
using System.Buffers.Binary;
#endif
using System;
using System.Security.Cryptography;

using Litdex.Utilities;
using Litdex.Utilities.Extension;

namespace Litdex.Random.PRNG
{
	/// <summary>
	///	LFSR-based pseudorandom number generators.
	/// </summary>
	/// <remarks>
	///	Source: https://github.com/andanteyk/prng-seiran
	/// </remarks>
	public class Seiran : Random64
	{
		#region Member

		/// <summary>
		///	The internal state of RNG.
		/// </summary>
		protected ulong[] _State;

		#endregion Member

		#region Constructor & Destructor

		/// <summary>
		///	Create an instance of <see cref="Seiran"/> object.
		/// </summary>
		/// <param name="seed1">
		///	First seed.
		/// </param>
		/// <param name="seed2">
		///	Second seed.
		/// </param>
		public Seiran(ulong seed1 = 0, ulong seed2 = 0)
		{
			this._State = new ulong[2];
			this.SetSeed(seed1, seed2);
		}

		/// <summary>
		///	Create an instance of <see cref="Seiran"/> object.
		/// </summary>
		/// <param name="seed">
		///	RNG seed numbers.
		/// </param>
		/// <exception cref="ArgumentOutOfRangeException">
		///	Seed need 2 numbers.
		/// </exception>
		public Seiran(ulong[] seed)
		{
			this.SetSeed(seed);
		}

		~Seiran()
		{
			Array.Clear(this._State, 0, this._State.Length);
		}

		#endregion Constructor & Destructor

		#region Protected Method

		/// <inheritdoc/>
		protected override ulong Next()
		{
			var s0 = this._State[0];
			var s1 = this._State[1];

			ulong result = ((s0 + s1) * 9).RotateLeft(29) + s0;

			this._State[0] = s0 ^ s1.RotateLeft(29);
			this._State[1] = s0 ^ s1 << 9;

			return result;
		}

		#endregion Protected Method

		#region Public Method

		/// <inheritdoc/>
		public override string AlgorithmName()
		{
			return "Seiran";
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
					seed1: BinaryPrimitives.ReadUInt64LittleEndian(span),
					seed2: BinaryPrimitives.ReadUInt64LittleEndian(span.Slice(8)));
#else
				var bytes = new byte[16];
				rng.GetNonZeroBytes(bytes);
				this.SetSeed(
					seed1: BinaryConverter.ToUInt64(bytes, 0),
					seed2: BinaryConverter.ToUInt64(bytes, 8));
#endif
			}
		}

		/// <summary>
		///	Set RNG seed manually.
		/// </summary>
		/// <param name="seed1">
		///	First RNG seed.
		/// </param>
		/// <param name="seed2">
		///	Second RNG seed.
		/// </param>
		public void SetSeed(ulong seed1, ulong seed2)
		{
			this._State[0] = seed1;
			this._State[1] = seed2;
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
		/// <exception cref="ArgumentException">
		///	Seed amount must same as the internal state amount.
		/// </exception>
		public virtual void SetSeed(params ulong[] seed)
		{
			if (seed == null || seed.Length == 0)
			{
				throw new ArgumentNullException(nameof(seed), "Seed can't null or empty.");
			}

			if (seed.Length < this._State.Length)
			{
				throw new ArgumentException($"Seed need at least {this._State.Length} numbers.", nameof(seed));
			}

			var length = seed.Length > this._State.Length ? this._State.Length : seed.Length;
			Array.Copy(seed, 0, this._State, 0, length);
		}

		private void Jump(ulong[] jumpPolynomial)
		{
			ulong t0 = 0, t1 = 0;

			for (var i = 0; i < 2; i++)
			{
				for (var b = 0; b < 64; b++)
				{
					if (((jumpPolynomial[i] >> b) & 1) != 0)
					{
						t0 ^= this._State[0];
						t1 ^= this._State[1];
					}
					this.Next();
				}
			}
			this._State[0] = t0;
			this._State[1] = t1;
		}

		/// <summary>
		/// Advance the internal state by 2^32 steps.
		/// </summary>
		/// <remarks>
		/// This method is equivalent to 2^32 <see cref="Next"/> calls.
		/// It can be executed in the same amount of time as 128 <see cref="Next"/> calls.
		/// </remarks>
		public void Jump32()
		{
			this.Jump(new ulong[] { 0x40165CBAE9CA6DEBu, 0x688E6BFC19485AB1u });
		}

		/// <summary>
		/// Advance the internal state by 2^64 steps.
		/// </summary>
		/// <remarks>
		/// This method is equivalent to 2^64 <see cref="Next"/> calls.
		/// It can be executed in the same amount of time as 128 <see cref="Next"/> calls.
		/// </remarks>
		public void Jump64()
		{
			this.Jump(new ulong[] { 0xF4DF34E424CA5C56u, 0x2FE2DE5C2E12F601u });
		}

		/// <summary>
		/// Advance the internal state by 2^96 steps.
		/// </summary>
		/// <remarks>
		/// This method is equivalent to 2^96 <see cref="Next"/> calls.
		/// It can be executed in the same amount of time as 128 <see cref="Next"/> calls.
		/// </remarks>
		public void Jump96()
		{
			this.Jump(new ulong[] { 0x185F4DF8B7634607u, 0x95A98C7025F908B2u });
		}

		/// <summary>
		///	Rewinds the internal state to the previous state.
		/// </summary>
		public void Previous()
		{
			ulong t1 = (this._State[0] ^ this._State[1]).RotateLeft(64 - 29);
			t1 ^= t1 << 44 ^ (t1 & ~0xFfffful) << 24;
			t1 ^= (t1 & (0x7FFFul << 40)) << 4;
			t1 ^= (t1 & (0x7FFul << 40)) << 8;
			t1 ^= (t1 & (0x7ul << 40)) << 16;
			t1 ^= (t1 & (0xFFFFFul << 35)) >> 20;
			t1 ^= (t1 & (0x7FFFul << 20)) >> 20;

			this._State[0] ^= t1.RotateLeft(29);
			this._State[1] = t1;
		}

		#endregion Public Method
	}
}
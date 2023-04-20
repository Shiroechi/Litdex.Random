#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
using System.Buffers.Binary;
#endif
using System;
using System.Security.Cryptography;

using Litdex.Utilities;

namespace Litdex.Random.PRNG
{
	/// <summary>
	///	<see cref="Tyche"/> is based on ChaCha's quarter-round.
	/// </summary>
	/// <remarks>
	///	Source: http://citeseerx.ist.psu.edu/viewdoc/download?doi=10.1.1.714.1893&rep=rep1&type=pdf
	/// </remarks>
	public class Tyche : Random32
	{
		#region Member

		/// <summary>
		///	The internal state of RNG.
		/// </summary>
        protected uint[] _State;

		#endregion Member

		#region Constructor & Destructor

		/// <summary>
		///	Create an instance of <see cref="Tyche"/> object.
		/// </summary>
		/// <param name="seed">
		///	RNG seed.
		/// </param>
		/// <param name="idx">
		///	Unique key.
		/// </param>
		public Tyche(ulong seed = 0, uint idx = 0)
		{
			this._State = new uint[4];
			this.SetSeed(seed, idx);
		}

		~Tyche()
		{
			Array.Clear(this._State, 0, this._State.Length);
		}

		#endregion Constructor & Destructor

		#region Protected Method

		/// <inheritdoc/>
		protected override uint Next()
		{
			this.Mix();
			return this._State[1];
		}

		/// <summary>
		///	Initialzied internal state.
		/// </summary>
		/// <param name="seed">
		///	RNG seed.
		/// </param>
		/// <param name="idx">
		///	Unique key.
		/// </param>
		protected void Init(ulong seed, uint idx)
		{
			this._State[0] = (uint)(seed / uint.MaxValue);
			this._State[1] = (uint)(seed % uint.MaxValue);
			this._State[2] = 2654435769;
			this._State[3] = idx ^ 1367130551;

			for (var i = 0; i < RandomUtil.InitialRoll; i++)
			{
				this.Mix();
			}
		}

		/// <summary>
		///	Update internal state based on quater round function of ChaCha stream chiper.
		/// </summary>
		protected virtual void Mix()
		{
			this._State[0] += this._State[1];
			this._State[3] ^= this._State[0];
			this._State[3] = this._State[3] << 16 | this._State[3] >> 16;

			this._State[2] += this._State[3];
			this._State[1] ^= this._State[2];
			this._State[1] = this._State[1] << 12 | this._State[1] >> 20;

			this._State[0] += this._State[1];
			this._State[3] ^= this._State[0];
			this._State[3] = this._State[3] << 8 | this._State[3] >> 24;

			this._State[2] += this._State[3];
			this._State[1] ^= this._State[2];
			this._State[1] = this._State[1] << 7 | this._State[1] >> 25;
		}

		#endregion Protected Method

		#region Public Method

		/// <inheritdoc/>
		public override string AlgorithmName()
		{
			return "Tyche";
		}

		/// <inheritdoc/>
		public override void Reseed()
		{
			using (var rng = RandomNumberGenerator.Create())
			{
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
				Span<byte> span = new byte[12];
				rng.GetNonZeroBytes(span);
				this.Init(
					seed: BinaryPrimitives.ReadUInt64LittleEndian(span),
					idx: BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(8)));
#else
				var bytes = new byte[12];
				rng.GetNonZeroBytes(bytes);
				this.Init(
					seed: BinaryConverter.ToUInt64(bytes, 0),
					idx: BinaryConverter.ToUInt32(bytes, 8));
#endif
			}
		}

		/// <summary>
		///	Set RNG seed manually.
		/// </summary>
		/// <param name="seed">
		///	RNG seed.
		/// </param>
		/// <param name="idx">
		///	Unique key.
		/// </param>
		public virtual void SetSeed(ulong seed, uint idx)
		{
			this.Init(seed, idx);
		}

		///// <inheritdoc/>
		//public override void SetSeed(params uint[] seed)
		//{
		//	if (seed == null || seed.Length == 0)
		//	{
		//		throw new ArgumentNullException(nameof(seed), "Seed can't null or empty.");
		//	}

		//	if (seed.Length < this._State.Length)
		//	{
		//		throw new ArgumentException($"Seed need at least {this._State.Length} numbers.", nameof(seed));
		//	}

		//	ulong a, b;
		//	a = seed[0];
		//	b = seed[1];

		//	this.Init(a << 32 | b, seed[2]);
		//}

		#endregion Public Method
	}
}
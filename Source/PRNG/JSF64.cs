#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
using System.Buffers.Binary;
#endif
using System;
using System.Security.Cryptography;

using Litdex.Utilities.Extension;

namespace Litdex.Random.PRNG
{
	/// <summary>
	///	Implementation of a Bob Jenkins Small Fast (Noncryptographic) PRNGs.
	/// </summary>
	/// <remarks>
	///	Source: http://burtleburtle.net/bob/rand/smallprng.html
	/// </remarks>
	public class JSF64 : Random64
	{
		#region Constructor & Destructor

		/// <summary>
		///	Create an instance of <see cref="JSF64"/> object.
		/// </summary>
		/// <param name="seed">
		///	RNG seed.
		///	</param>
		public JSF64(ulong seed = 0)
		{
			this._State = new ulong[4];
			this.SetSeed(seed);
		}

		/// <summary>
		///	Destructor.
		/// </summary>
		~JSF64()
		{
			Array.Clear(this._State, 0, this._State.Length);
		}

		#endregion Constructor & Destructor

		#region	Protected Method

		/// <inheritdoc/>
		protected override ulong Next()
		{
			var e = this._State[0] - this._State[1].RotateLeft(7);
			this._State[0] = this._State[1] ^ this._State[2].RotateLeft(13);
			this._State[1] = this._State[2] + this._State[3].RotateLeft(37);
			this._State[2] = this._State[3] + e;
			this._State[3] = e + this._State[0];
			return this._State[3];
		}

		#endregion Protected Method

		#region	Public Method

		/// <inheritdoc/>
		public override string AlgorithmName()
		{
			return "JSF 64-bit";
		}

		/// <inheritdoc/>
		public override void Reseed()
		{
			using (var rng = RandomNumberGenerator.Create())
			{
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
				Span<byte> span = new byte[8];
				rng.GetNonZeroBytes(span);
				this.SetSeed(BinaryPrimitives.ReadUInt64LittleEndian(span));
#else
				var bytes = new byte[8];
				rng.GetNonZeroBytes(bytes);
				this.SetSeed(BitConverter.ToUInt64(bytes, 0));
#endif
			}
		}

		/// <summary>
		///	Set RNG seed manually.
		/// </summary>
		/// <param name="seed">
		///	RNG seed.
		/// </param>
		public void SetSeed(ulong seed)
		{
			this._State[0] = 0xF1EA5EED;
			this._State[1] = this._State[2] = this._State[3] = seed;

			for (var i = 0; i < _InitialRoll; i++)
			{
				this.Next();
			}
		}

		/// <inheritdoc/>
		public override void SetSeed(params ulong[] seed)
		{
			if (seed == null || seed.Length == 0)
			{
				throw new ArgumentNullException(nameof(seed), "Seed can't null or empty.");
			}

			this.SetSeed(seed[0]);
		}

		#endregion	Public
	}
}
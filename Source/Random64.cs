using System;
using System.Runtime.CompilerServices;

namespace Litdex.Random
{
	/// <summary>
	///	Base class for Random Number Generator that the internal state produces 64-bit output.
	/// </summary>
	public abstract class Random64 : Random
	{
		#region Member

		/// <summary>
		///	The internal state of RNG.
		/// </summary>
		protected ulong[] _State;

		/// <summary>
		///	<see cref="long"/> and <see cref="ulong"/> is 8 bytes.
		/// </summary>
		protected const byte _Size = 8;

		#endregion Member

		#region Protected Method

		/// <summary>
		///	Generate next random number.
		/// </summary>
		/// <returns>
		///	A 64-bit unsigned integer.
		///	</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected abstract ulong Next();

		#endregion Protected Method

		#region Public Method

		/// <inheritdoc/>
		public override string AlgorithmName()
		{
			return "Random64";
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

		/// <inheritdoc/>
		public override bool NextBoolean()
		{
			return this.Next() >> 63 == 0;
		}

		/// <inheritdoc/>
		public override byte NextByte()
		{
			return (byte)(this.Next() >> 56);
		}

		/// <inheritdoc/>
		public override byte[] NextBytes(int length)
		{
			if (length <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(length), "The requested output size can't lower than 1.");
			}

			var bytes = new byte[length];
			this.Fill(bytes);
			return bytes;
		}

		/// <inheritdoc/>
		public override void Fill(byte[] buffer)
		{
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
			var span = new Span<byte>(buffer);
			this.Fill(span);
#else
			if (buffer.Length <= 0 || buffer == null)
			{
				throw new ArgumentNullException(nameof(buffer), "Array length can't be lower than 1 or null.");
			}

			ulong sample = 0;
			var idx = 0;
			var length = buffer.Length;

			while (length >= _Size)
			{
				sample = this.Next();

				buffer[idx] = (byte)sample;
				buffer[idx + 1] = (byte)(sample >> 8);
				buffer[idx + 2] = (byte)(sample >> 16);
				buffer[idx + 3] = (byte)(sample >> 24);
				buffer[idx + 4] = (byte)(sample >> 32);
				buffer[idx + 5] = (byte)(sample >> 40);
				buffer[idx + 6] = (byte)(sample >> 48);
				buffer[idx + 7] = (byte)(sample >> 56);

				length -= _Size;
				idx += _Size;
			}

			if (length != 0)
			{
				sample = this.Next();

				for (var i = 0; i < length; i++)
				{
					buffer[idx] = (byte)sample;
					sample >>= 8;
					idx++;
				}
			}
#endif
		}

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER

		/// <inheritdoc/>
		public override void Fill(Span<byte> buffer)
		{
			if (buffer.Length <= 0 || buffer == null)
			{
				throw new ArgumentNullException(nameof(buffer), "Array length can't be lower than 1 or null.");
			}

			while (buffer.Length >= _Size)
			{
				System.Buffers.Binary.BinaryPrimitives.WriteUInt64LittleEndian(buffer, this.Next());
				buffer = buffer.Slice(_Size);
			}

			if (buffer.Length != 0)
			{
				var chunk = new byte[_Size];
				System.Buffers.Binary.BinaryPrimitives.WriteUInt64LittleEndian(chunk, this.Next());

				for (var i = 0; i < buffer.Length; i++)
				{
					buffer[i] = chunk[i];
				}
			}
		}
		
#endif

		/// <inheritdoc/>
		public override int NextInt()
		{
			return (int)(this.Next() >> 33);
		}

		/// <inheritdoc/>
		public override uint NextUInt()
		{
			return (uint)(this.Next() >> 32);
		}

		/// <inheritdoc/>
		public override long NextInt64()
		{
			return (long)(this.Next() >> 1);
		}

		/// <inheritdoc/>
		public override ulong NextUInt64()
		{
			return this.Next();
		}

		#endregion Public Method
	}
}
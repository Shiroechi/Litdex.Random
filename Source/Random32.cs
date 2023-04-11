using System;
using System.Runtime.CompilerServices;

namespace Litdex.Random
{
	/// <summary>
	///	Base class for Random Number Generator that the internal state produces 32-bit output.
	/// </summary>
	public abstract class Random32 : IRandomEngine
	{
		#region Member

		/// <summary>
		///	The internal state of RNG.
		/// </summary>
		protected uint[] _State;

		/// <summary>
		///	<see cref="int"/> and <see cref="uint"/> is 4 bytes.
		/// </summary>
		protected const byte _Size = 4;

		#endregion Member

		#region Protected Method

		/// <summary>
		///	Generate next random number.
		/// </summary>
		/// <returns>
		///	A 32-bit unsigned integer.
		///	</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected abstract uint Next();

		#endregion Protected Method

		#region Public Method

		/// <inheritdoc/>
		public virtual string AlgorithmName()
		{
			return "Random32";
		}

		/// <inheritdoc/>
		public abstract void Reseed();

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
		public virtual void SetSeed(params uint[] seed)
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
		public bool NextBoolean()
		{
			return this.NextUInt() >> 31 == 0;
		}

		/// <inheritdoc/>
		public byte NextByte()
		{
			return (byte)(this.Next() >> 24);
		}

		/// <inheritdoc/>
		public byte[] NextBytes(int length)
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
		public void Fill(byte[] buffer)
		{
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
			var span = new Span<byte>(buffer);
			this.Fill(span);
#else
			if (buffer.Length <= 0 || buffer == null)
			{
				throw new ArgumentNullException(nameof(buffer), "Array length can't be lower than 1 or null.");
			}

			uint sample = 0;
			var idx = 0;
			var length = buffer.Length;

			while (length >= _Size)
			{
				sample = this.Next();

				buffer[idx] = (byte)sample;
				buffer[idx + 1] = (byte)(sample >> 8);
				buffer[idx + 2] = (byte)(sample >> 16);
				buffer[idx + 3] = (byte)(sample >> 24);

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
				}
			}
#endif
		}

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER

		/// <inheritdoc/>
		public void Fill(Span<byte> buffer)
		{
			if (buffer.Length <= 0 || buffer == null)
			{
				throw new ArgumentNullException(nameof(buffer), "Array length can't be lower than 1 or null.");
			}

			while (buffer.Length >= _Size)
			{
				System.Buffers.Binary.BinaryPrimitives.WriteUInt32LittleEndian(buffer, this.Next());
				buffer = buffer.Slice(_Size);
			}

			if (buffer.Length != 0)
			{
				var chunk = new byte[_Size];
				System.Buffers.Binary.BinaryPrimitives.WriteUInt32LittleEndian(chunk, this.Next());

				for (var i = 0; i < buffer.Length; i++)
				{
					buffer[i] = chunk[i];
				}
			}
		}

#endif

		/// <inheritdoc/>
		public int NextInt()
		{
			return (int)(this.Next());
		}

		/// <inheritdoc/>
		public uint NextUInt()
		{
			return this.Next();
		}

		/// <inheritdoc/>
		public long NextInt64()
		{
			return (long)(this.NextUInt64());
		}

		/// <inheritdoc/>
		public ulong NextUInt64()
		{
			ulong high = this.NextUInt();
			ulong low = this.NextUInt();
			return high << 32 | low;
		}

		#endregion Public Method
	}
}
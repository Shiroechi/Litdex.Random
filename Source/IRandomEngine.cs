using System;
using System.Security.Cryptography;

namespace Litdex.Random
{
	/// <summary>
	/// Interface for random engine.
	/// </summary>
	public interface IRandomEngine
	{

		#region Public Method

		/// <summary>
		///	The name of the algorithm this generator implements.
		/// </summary>
		/// <returns>
		///	The name of RNG.
		/// </returns>
		string AlgorithmName();

		/// <summary>
		///	Seed with <see cref="RandomNumberGenerator"/>.
		///	</summary>
		void Reseed();

		#endregion Public Method

		#region Basic Method

		/// <summary>
		///	Generate <see cref="bool"/> value.
		/// </summary>
		/// <returns>
		///	<see langword="true"/> or <see langword="false"/>.
		/// </returns>
		bool NextBoolean();

		/// <summary>
		///	Generate a non-negative random integer.
		/// </summary>
		/// <returns>
		///	A 8-bit unsigned integer that is greater than or equal to 0.
		/// </returns>
		byte NextByte();

		/// <summary>
		///	Generate array of random bytes from generator.
		/// </summary>
		/// <param name="length">
		///	Requested output length.
		/// </param>
		/// <returns>
		///	Array of bytes.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		///	The requested output size can't lower than 1.
		/// </exception>
		byte[] NextBytes(int length);

		/// <summary>
		///	Fills the elements of a specified array of bytes with random numbers.
		/// </summary>
		/// <param name="buffer">
		///	The array to be filled with random numbers.
		///	</param>
		/// <exception cref="ArgumentNullException">
		///	Array length can't be lower than 1 or null.
		/// </exception>
		void Fill(byte[] buffer);

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER

		/// <summary>
		///	Fills the elements of a specified array of bytes with random numbers.
		/// </summary>
		/// <param name="buffer">
		///	The array to be filled with random numbers.
		///	</param>
		/// <exception cref="ArgumentNullException">
		///	Array length can't be lower than 1 or null.
		/// </exception>
		void Fill(Span<byte> buffer);

#endif

		/// <summary>
		///	Generate <see cref="int"/> value from generator.
		/// </summary>
		/// <returns>
		///	A 32-bit signed integer.
		/// </returns>
		int NextInt();

		/// <summary>
		///	Generate <see cref="uint"/> value from generator.
		/// </summary>
		/// <returns>
		///	A 32-bit unsigned integer.
		/// </returns>
		uint NextUInt();

		/// <summary>
		///	Generate <see cref="long"/> value from generator. 
		/// </summary>
		/// <returns>
		///	A 64-bit signed integer.
		/// </returns>
		long NextInt64();

		/// <summary>
		///	Generate <see cref="ulong"/> value from generator. 
		/// </summary>
		/// <returns>
		///	A 64-bit unsigned integer.
		/// </returns>
		ulong NextUInt64();

		#endregion Basic Method
	}
}

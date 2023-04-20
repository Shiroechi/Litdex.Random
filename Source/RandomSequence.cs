using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Litdex.Random
{
	public partial class Random
	{
		#region Choice

		/// <summary>
		/// Select one element randomly from the given set.
		/// </summary>
		/// <typeparam name="T">
		///	The type of objects in array.
		/// </typeparam>
		/// <param name="items">
		///	A set of items to select.
		/// </param>
		/// <returns>
		///	Random element from the given set.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		///	The items is null, empty or not initialized. 
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		///	The items length or size can't be greater than int.MaxValue.
		/// </exception>
		public virtual T Choice<T>(T[] items)
		{
			if (items == null || items.Length <= 0)
			{
				ThrowNullArray();
			}

			var index = this.NextInt(0, items.Length - 1);
			return items[index];
		}

		/// <summary>
		///	Select arbitrary element randomly.
		/// </summary>
		/// <typeparam name="T">
		///	The type of objects in array.
		/// </typeparam>
		/// <param name="items">
		///	A set of items to select.
		/// </param>
		/// <param name="select">
		///	The desired amount to select.
		/// </param>
		/// <returns>
		///	Multiple random elements from the given set.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		///	The items is null, empty or not initialized. 
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		///	- The number of elements to be retrieved is negative or less than 1.
		///	- The number of elements to be retrieved exceeds the items size.
		/// </exception>
		public virtual T[] Choice<T>(T[] items, int select)
		{
			if (items.Length <= 0 || items == null)
			{
				ThrowNullArray();
			}
			var length = items.Length;

			if (select < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(select), "The number of elements to be retrieved is negative or less than 1.");
			}
			else if (select > length)
			{
				throw new ArgumentOutOfRangeException(nameof(select), "The number of elements to be retrieved exceeds the items size.");
			}
			else if (select == length)
			{
				return items;
			}

			var selected = new T[select];
			uint index;

			for (var i = 0; i < select; i++)
			{
				index = this.NextUInt(0, (uint)length - 1);
				selected[i] = items[index];
			}

			return selected;
		}

		/// <summary>
		///	Select arbitrary element randomly.
		/// </summary>
		/// <typeparam name="T">
		///	The type of objects in array.
		/// </typeparam>
		/// <param name="items">
		///	A set of items to select.
		/// </param>
		/// <param name="select">
		///	The desired amount to select.
		/// </param>
		/// <returns>
		///	Multiple random elements from the given set.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		///	The items is null, empty or not initialized. 
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		///	- The number of elements to be retrieved is negative or less than 1.
		///	- The number of elements to be retrieved exceeds the items size.
		/// </exception>
		public virtual Task<T[]> ChoiceAsync<T>(T[] items, int select)
		{
			return this.ChoiceAsync(items, select, CancellationToken.None);
		}

		/// <summary>
		///	Select arbitrary element randomly.
		/// </summary>
		/// <typeparam name="T">
		///	The type of objects in array.
		/// </typeparam>
		/// <param name="items">
		///	A set of items to select.
		/// </param>
		/// <param name="select">
		///	The desired amount to select.
		/// </param>
		/// <param name="cancellationToken">
		///	Token to cancel the operations.
		/// </param>
		/// <returns>
		///	Multiple random elements from the given set.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		///	The items is null, empty or not initialized. 
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		///	- The number of elements to be retrieved is negative or less than 1.
		///	- The number of elements to be retrieved exceeds the items size.
		/// </exception>
		public virtual Task<T[]> ChoiceAsync<T>(T[] items, int select, CancellationToken cancellationToken)
		{
			return Task.Run(() =>
			{
				return this.Choice(items, select);
			}, cancellationToken);
		}

		/// <summary>
		///	Select one element randomly.
		/// </summary>
		/// <typeparam name="T">
		///	The type of objects in array.
		/// </typeparam>
		/// <param name="items">
		///	A set of items to select.
		/// </param>
		/// <returns>
		///	Random element from the given set.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		///	The items is null, empty or not initialized. 
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		///	The items length or size can't be greater than int.MaxValue.
		/// </exception>
		public virtual T Choice<T>(IEnumerable<T> items)
		{
			if (!items.Any() || items == null)
			{
				ThrowNullArray();
			}

			return items.ElementAt((int)this.NextUInt(0, (uint)(items.Count() - 1)));
		}

		/// <summary>
		///	Select arbitrary element randomly.
		/// </summary>
		/// <typeparam name="T">
		///	The type of objects in array.
		/// </typeparam>
		/// <param name="items">
		///	A set of items to select.
		/// </param>
		/// <param name="select">
		///	The desired amount to select.
		/// </param>
		/// <returns>
		///	Multiple random elements from the given set.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		///	The items is null, empty or not initialized. 
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		///	- The number of elements to be retrieved is negative or less than 1.
		///	- The number of elements to be retrieved exceeds the items size.
		/// </exception>
		public virtual T[] Choice<T>(IEnumerable<T> items, int select)
		{
			if (items == null || !items.Any())
			{
				ThrowNullArray();
			}

			var length = items.Count();

			if (select < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(select), "The number of elements to be retrieved is negative or less than 1.");
			}
			else if (select > length)
			{
				throw new ArgumentOutOfRangeException(nameof(select), "The number of elements to be retrieved exceeds the items size.");
			}
			else if (select == length)
			{
				return items.ToArray();
			}

			var selected = new T[select];
			uint index;

			for (var i = 0; i < select; i++)
			{
				index = this.NextUInt(0, (uint)length - 1);
				selected[i] = items.ElementAt((int)index);
			}

			return selected;
		}

		/// <summary>
		///	Select arbitrary element randomly.
		/// </summary>
		/// <typeparam name="T">
		///	The type of objects in array.
		/// </typeparam>
		/// <param name="items">
		///	A set of items to select.
		/// </param>
		/// <param name="select">
		///	The desired amount to select.
		/// </param>
		/// <returns>
		///	Multiple random elements from the given set.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		///	The items is null, empty or not initialized. 
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		///	- The number of elements to be retrieved is negative or less than 1.
		///	- The number of elements to be retrieved exceeds the items size.
		/// </exception>
		public virtual Task<T[]> ChoiceAsync<T>(IEnumerable<T> items, int select)
		{
			return this.ChoiceAsync(items, select, CancellationToken.None);
		}

		/// <summary>
		///	Select arbitrary element randomly.
		/// </summary>
		/// <typeparam name="T">
		///	The type of objects in array.
		/// </typeparam>
		/// <param name="items">
		///	A set of items to select.
		/// </param>
		/// <param name="select">
		///	The desired amount to select.
		/// </param>
		/// <param name="cancellationToken">
		///	Token to cancel the operations.
		/// </param>
		/// <returns>
		///	Multiple random elements from the given set.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		///	The items is null, empty or not initialized. 
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		///	- The number of elements to be retrieved is negative or less than 1.
		///	- The number of elements to be retrieved exceeds the items size.
		/// </exception>
		public virtual Task<T[]> ChoiceAsync<T>(IEnumerable<T> items, int select, CancellationToken cancellationToken)
		{
			return Task.Run(() =>
			{
				return this.Choice(items, select);
			}, cancellationToken);
		}

		#endregion Choice

		#region Sample

		/// <summary>
		///	Select arbitrary distinct element randomly.
		/// </summary>
		/// <typeparam name="T">
		///	The type of objects in array.
		/// </typeparam>
		/// <param name="items">
		/// A set of items to select.
		/// </param>
		/// <param name="select">
		///	The desired amount to select.
		/// </param>
		/// <returns>
		///	Multiple random elements from the given set.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		///	The items is null, empty or not initialized. 
		/// </exception>
		/// <exception cref="ArgumentException">
		///	The number of elements to be retrieved is negative or less than 1.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		///	The number of elements to be retrieved exceeds the items size.
		/// </exception>
		public virtual T[] Sample<T>(T[] items, int select)
		{
			if (items == null || items.Length <= 0)
			{
				ThrowNullArray();
			}

			if (select <= 0)
			{
				throw new ArgumentException("The number of elements to be retrieved is negative or less than 1.", nameof(select));
			}
			//else if (select > items.Length)
			//{
			//	throw new ArgumentOutOfRangeException(nameof(select), "The number of elements to be retrieved exceeds the items size.");
			//}
			else if (select >= items.Length)
			{
				return items;
			}

			var reservoir = new T[select];
			int index;

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
			items.AsSpan(0, select).CopyTo(reservoir);
#else
			Array.Copy(items, 0, reservoir, 0, reservoir.Length);
#endif
			for (var i = select; i < items.Length; i++)
			{
				index = (int)this.NextUInt(0, (uint)i);

				if (index < select)
				{
					reservoir[index] = items[i];
				}
			}

			return reservoir;
		}

		/// <summary>
		///	Select arbitrary distinct element randomly.
		/// </summary>
		/// <remarks>
		///	Used for large data, objects or arrays.
		/// </remarks>
		/// <typeparam name="T">
		///	The type of objects in array.
		/// </typeparam>
		/// <param name="items">
		/// A set of items to select.
		/// </param>
		/// <param name="select">
		///	The desired amount to select.
		/// </param>
		/// <returns>
		///	Multiple random elements from the given set.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		///	The items is null, empty or not initialized. 
		/// </exception>
		/// <exception cref="ArgumentException">
		///	The number of elements to be retrieved is negative or less than 1.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		///	The number of elements to be retrieved exceeds the items size.
		/// </exception>
		public virtual Task<T[]> SampleAsync<T>(T[] items, int select)
		{
			return this.SampleAsync(items, select, CancellationToken.None);
		}

		/// <summary>
		///	Select arbitrary distinct element randomly.
		/// </summary>
		/// <remarks>
		///	Used for large data, objects or arrays.
		/// </remarks>
		/// <typeparam name="T">
		///	The type of objects in array.
		/// </typeparam>
		/// <param name="items">
		/// A set of items to select.
		/// </param>
		/// <param name="select">
		///	The desired amount to select.
		/// </param>
		/// <param name="cancellationToken">
		///	Token to cancel the operations.
		/// </param>
		/// <returns>
		///	Multiple random elements from the given set.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		///	The items is null, empty or not initialized. 
		/// </exception>
		/// <exception cref="ArgumentException">
		///	The number of elements to be retrieved is negative or less than 1.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		///	The number of elements to be retrieved exceeds the items size.
		/// </exception>
		public virtual Task<T[]> SampleAsync<T>(T[] items, int select, CancellationToken cancellationToken)
		{
			return Task.Run(() =>
			{
				return this.Sample(items, select);
			}, cancellationToken);
		}

		/// <summary>
		///	Select arbitrary distinct element randomly.
		/// </summary>
		/// <typeparam name="T">
		///	The type of objects in array.
		/// </typeparam>
		/// <param name="items">
		///	A set of items to select.
		/// </param>
		/// <param name="select">
		///	The desired amount to select.
		/// </param>
		/// <returns>
		///	Multiple random elements from the given set.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		///	The items is null, empty or not initialized. 
		/// </exception>
		/// <exception cref="ArgumentException">
		///	The number of elements to be retrieved is negative or less than 1.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		///	The number of elements to be retrieved exceeds the items size.
		/// </exception>
		public virtual T[] Sample<T>(IEnumerable<T> items, int select)
		{
			if (items == null || items.Any())
			{
				ThrowNullArray();
			}

			var length = items.Count();

			if (select <= 0)
			{
				throw new ArgumentException("The number of elements to be retrieved is negative or less than 1.", nameof(select));
			}
			//else if (select > length)
			//{
			//	throw new ArgumentOutOfRangeException(nameof(select), "The number of elements to be retrieved exceeds the items size.");
			//}
			else if (select >= length)
			{
				return items.ToArray();
			}

			var reservoir = new T[select];
			int index;

			for (var i = 0; i < items.Count(); i++)
			{
				reservoir[i] = items.ElementAt(i);
			}

			for (var i = select; i < length; i++)
			{
				index = (int)this.NextUInt(0, (uint)i);

				if (index < select)
				{
					reservoir[index] = items.ElementAt(i);
				}
			}

			return reservoir;
		}

		/// <summary>
		///	Select arbitrary distinct element randomly.
		/// </summary>
		/// <remarks>
		///	Used for large data, objects or arrays.
		/// </remarks>
		/// <typeparam name="T">
		///	The type of objects in array.
		/// </typeparam>
		/// <param name="items">
		/// A set of items to select.
		/// </param>
		/// <param name="select">
		///	The desired amount to select.
		/// </param>
		/// <returns>
		///	Multiple random elements from the given set.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		///	The items is null, empty or not initialized. 
		/// </exception>
		/// <exception cref="ArgumentException">
		///	The number of elements to be retrieved is negative or less than 1.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		///	The number of elements to be retrieved exceeds the items size.
		/// </exception>
		public virtual Task<T[]> SampleAsync<T>(IEnumerable<T> items, int select)
		{
			return this.SampleAsync(items, select, CancellationToken.None);
		}

		/// <summary>
		///	Select arbitrary distinct element randomly.
		/// </summary>
		/// <remarks>
		///	Used for large data, objects or arrays.
		/// </remarks>
		/// <typeparam name="T">
		///	The type of objects in array.
		/// </typeparam>
		/// <param name="items">
		/// A set of items to select.
		/// </param>
		/// <param name="select">
		///	The desired amount to select.
		/// </param>
		/// <param name="cancellationToken">
		///	Token to cancel the operations.
		/// </param>
		/// <returns>
		///	Multiple random elements from the given set.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		///	The items is null, empty or not initialized. 
		/// </exception>
		/// <exception cref="ArgumentException">
		///	The number of elements to be retrieved is negative or less than 1.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		///	The number of elements to be retrieved exceeds the items size.
		/// </exception>
		public virtual Task<T[]> SampleAsync<T>(IEnumerable<T> items, int select, CancellationToken cancellationToken)
		{
			return Task.Run(() =>
			{
				return this.Sample(items, select);
			}, cancellationToken);
		}

		#endregion Sample

		#region Shuffle

		/// <summary>
		///	Shuffle items with Fisher-Yates shuffle then return the shuffled item in new array.
		/// </summary>
		/// <typeparam name="T">
		///	The type of objects in array.
		/// </typeparam>
		/// <param name="items">
		///	A set of items to shuffle.
		/// </param>
		/// <returns>
		///	Array of shuffled items.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		///	The items is null, empty or not initialized. 
		/// </exception>
		public virtual T[] Shuffle<T>(T[] items)
		{
			if (items == null || items.Length <= 0)
			{
				ThrowNullArray();
			}

			var newArray = new T[items.Length];

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
			items.AsSpan(0, items.Length).CopyTo(newArray);
#else
			Array.Copy(items, newArray, newArray.Length);
#endif

			T temp;
			for (var i = newArray.Length - 1; i > 1; i--)
			{
				var index = this.NextUInt(0, (uint)i);
				temp = newArray[i];
				newArray[i] = newArray[index];
				newArray[index] = temp;
			}

			return newArray;
		}

		/// <summary>
		///	Shuffle items with Fisher-Yates shuffle then return the shuffled item in new array.
		/// </summary>
		/// <remarks>
		///	Used for large data, objects or arrays.
		/// </remarks>
		/// <typeparam name="T">
		///	The type of objects in array.
		/// </typeparam>
		/// <param name="items">
		///	A set of items to shuffle.
		/// </param>
		/// <returns>
		///	Array of shuffled items.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		///	The items is null, empty or not initialized. 
		/// </exception>
		public virtual Task<T[]> ShuffleAsync<T>(T[] items)
		{
			return this.ShuffleAsync(items, CancellationToken.None);
		}

		/// <summary>
		///	Shuffle items with Fisher-Yates shuffle then return the shuffled item in new array.
		/// </summary>
		/// <remarks>
		///	Used for large data, objects or arrays.
		/// </remarks>
		/// <typeparam name="T">
		///	The type of objects in array.
		/// </typeparam>
		/// <param name="items">
		///	A set of items to shuffle.
		/// </param>
		/// <param name="cancellationToken">
		///	Token to cancel the operations.
		/// </param>
		/// <returns>
		///	Array of shuffled items.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		///	The items is null, empty or not initialized. 
		/// </exception>
		public virtual Task<T[]> ShuffleAsync<T>(T[] items, CancellationToken cancellationToken)
		{
			return Task.Run(() =>
			{
				return this.Shuffle(items);
			}, cancellationToken);
		}

		/// <summary>
		///	Shuffle items with Fisher-Yates shuffle then return the shuffled item in new array.
		/// </summary>
		/// <typeparam name="T">
		///	The type of objects in array.
		/// </typeparam>
		/// <param name="items">
		///	A set of items to shuffle.
		/// </param>
		/// <returns>
		///	Array of shuffled items.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		///	The items is null, empty or not initialized. 
		/// </exception>
		public virtual T[] Shuffle<T>(IEnumerable<T> items)
		{
			return this.Shuffle(items.ToArray());
		}

		/// <summary>
		///	Shuffle items with Fisher-Yates shuffle then return the shuffled item in new array.
		/// </summary>
		/// <remarks>
		///	Used for large data, objects or arrays.
		/// </remarks>
		/// <typeparam name="T">
		///	The type of objects in array.
		/// </typeparam>
		/// <param name="items">
		///	A set of items to shuffle.
		/// </param>
		/// <returns>
		///	Array of shuffled items.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		///	The items is null, empty or not initialized. 
		/// </exception>
		public virtual Task<T[]> ShuffleAsync<T>(IEnumerable<T> items)
		{
			return this.ShuffleAsync(items.ToArray(), CancellationToken.None);
		}

		/// <summary>
		///	Shuffle items with Fisher-Yates shuffle then return the shuffled item in new array.
		/// </summary>
		/// <remarks>
		///	Used for large data, objects or arrays.
		/// </remarks>
		/// <typeparam name="T">
		///	The type of objects in array.
		/// </typeparam>
		/// <param name="items">
		///	A set of items to shuffle.
		/// </param>
		/// <param name="cancellationToken">
		///	Token to cancel the operations.
		/// </param>
		/// <returns>
		///	Array of shuffled items.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		///	The items is null, empty or not initialized. 
		/// </exception>
		public virtual Task<T[]> ShuffleAsync<T>(IEnumerable<T> items, CancellationToken cancellationToken)
		{
			return this.ShuffleAsync(items.ToArray(), cancellationToken);
		}

		/// <summary>
		///	Shuffle items in place with Fisher-Yates shuffle.
		/// </summary>
		/// <typeparam name="T">
		///	The type of objects in array.
		/// </typeparam>
		/// <param name="items">
		///	A set of items to shuffle.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///	The items is null, empty or not initialized. 
		/// </exception>
		public virtual void ShuffleInPlace<T>(T[] items)
		{
			if (items == null || items.Length <= 0)
			{
				ThrowNullArray();
			}

			T temp;

			for (var i = items.Length - 1; i > 1; i--)
			{
				var index = this.NextUInt(0, (uint)i);
				temp = items[i];
				items[i] = items[index];
				items[index] = temp;
			}
		}

		/// <summary>
		///	Shuffle items in place with Fisher-Yates shuffle.
		/// </summary>
		/// <remarks>
		///	Used for large data, objects or arrays.
		/// </remarks>
		/// <typeparam name="T">
		///	The type of objects in array.
		/// </typeparam>
		/// <param name="items">
		///	A set of items to shuffle.
		/// </param>
		/// <returns>
		///	Task based operations.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		///	The items is null, empty or not initialized. 
		/// </exception>
		public virtual Task ShuffleInPlaceAsync<T>(T[] items)
		{
			return this.ShuffleInPlaceAsync(items, CancellationToken.None);
		}

		/// <summary>
		///	Shuffle items in place with Fisher-Yates shuffle.
		/// </summary>
		/// <remarks>
		///	Used for large data, objects or arrays.
		/// </remarks>
		/// <typeparam name="T">
		///	The type of objects in array.
		/// </typeparam>
		/// <param name="items">
		///	A set of items to shuffle.
		/// </param>
		/// <param name="cancellationToken">
		///	Token to cancel the operations.
		/// </param>
		/// <returns>
		///	Task based operations.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		///	The items is null, empty or not initialized. 
		/// </exception>
		public virtual Task ShuffleInPlaceAsync<T>(T[] items, CancellationToken cancellationToken)
		{
			return Task.Run(() =>
			{
				this.ShuffleInPlace(items);
			}, cancellationToken);
		}

		/// <summary>
		///	Shuffle items in place with Fisher-Yates shuffle.
		/// </summary>
		/// <typeparam name="T">
		///	The type of objects in array.
		/// </typeparam>
		/// <param name="items">
		///	A set of items to shuffle.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///	The items is null, empty or not initialized. 
		/// </exception>
		public virtual void ShuffleInPlace<T>(IList<T> items)
		{
			if (items == null || items.Count <= 0)
			{
				ThrowNullArray();
			}

			T temp;

			for (var i = items.Count - 1; i > 1; i--)
			{
				var index = (int)this.NextUInt(0, (uint)i);
				temp = items[i];
				items[i] = items[index];
				items[index] = temp;
			}
		}

		/// <summary>
		///	Shuffle items in place with Fisher-Yates shuffle.
		/// </summary>
		/// <remarks>
		///	Used for large data, objects or arrays.
		/// </remarks>
		/// <typeparam name="T">
		///	The type of objects in array.
		/// </typeparam>
		/// <param name="items">
		///	A set of items to shuffle.
		/// </param>
		/// <returns>
		///	Task based operations.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		///	The items is null, empty or not initialized. 
		/// </exception>
		public virtual Task ShuffleInPlaceAsync<T>(IList<T> items)
		{
			return this.ShuffleInPlaceAsync(items, CancellationToken.None);
		}

		/// <summary>
		///	Shuffle items in place with Fisher-Yates shuffle.
		/// </summary>
		/// <remarks>
		///	Used for large data, objects or arrays.
		/// </remarks>
		/// <typeparam name="T">
		///	The type of objects in array.
		/// </typeparam>
		/// <param name="items">
		///	A set of items to shuffle.
		/// </param>
		/// <param name="cancellationToken">
		///	Token to cancel the operations.
		/// </param>
		/// <returns>
		///	Task based operations.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		///	The items is null, empty or not initialized. 
		/// </exception>
		public virtual Task ShuffleInPlaceAsync<T>(IList<T> items, CancellationToken cancellationToken)
		{
			return Task.Run(() =>
			{
				this.ShuffleInPlace(items);
			}, cancellationToken);
		}

		#endregion Shuffle

		private static void ThrowNullArray()
		{
			throw new ArgumentNullException("items", "The items is empty or null.");
		}
	}
}
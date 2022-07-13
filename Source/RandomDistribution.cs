using System;

namespace Litdex.Random
{
	public abstract partial class Random
	{
		/// <summary>
		///	Hold a copy of gaussian number.
		/// </summary>
		protected double _NextGaussian = 0.0;
		protected bool _HaveNextGaussian = false;

		/// <summary>
		///	Generate gaussian distribution.
		/// </summary>
		/// <returns>
		///	A 64-bit floating point number normal distribution.
		/// </returns>
		public virtual double NextGaussian()
		{
			// See Knuth, ACP, Section 3.4.1 Algorithm C.
			if (this._HaveNextGaussian == true)
			{
				this._HaveNextGaussian = false;
				return this._NextGaussian;
			}
			else
			{
				double v1, v2, s;

				do
				{
					v1 = 2 * this.NextDouble() - 1; // between -1 and 1
					v2 = 2 * this.NextDouble() - 1; // between -1 and 1
					s = v1 * v1 + v2 * v2;
				} while (s >= 1 || s == 0);

				double multiplier = Math.Sqrt(-2 * Math.Log(s) / s);

				this._NextGaussian = v2 * multiplier;
				this._HaveNextGaussian = true;

				return v1 * multiplier;
			}
		}

		/// <summary>
		///	Generate gaussian distribution.
		/// </summary>
		/// <param name="mean">
		/// Mean or average value.
		/// </param>
		/// <param name="stdDev">
		///	Standard daviation value.
		/// </param>
		/// <returns>
		///	A 64-bit floating point number normal distribution.
		/// </returns>
		public virtual double NextGaussian(double mean = 0, double stdDev = 1)
		{
			if (this._HaveNextGaussian == true)
			{
				this._HaveNextGaussian = false;
				return this._NextGaussian;
			}
			else
			{
				double v1, v2, s;

				do
				{
					v1 = 2 * this.NextDouble() - 1; // between -1 and 1
					v2 = 2 * this.NextDouble() - 1; // between -1 and 1
					s = v1 * v1 + v2 * v2;
				} while (s >= 1 || s == 0);

				double multiplier = Math.Sqrt(-2 * Math.Log(s) / s);

				this._NextGaussian = mean + ((v2 * multiplier) * stdDev);
				this._HaveNextGaussian = true;

				return mean + ((v1 * multiplier) * stdDev);
			}
		}

		/// <summary>
		///	Generate gamma distribution from 2 numbers.
		/// </summary>
		/// <param name="alpha">
		///	Alpha uniform number.
		/// </param>
		/// <param name="beta">
		///	Beta uniform number.
		/// </param>
		/// <returns>
		///	Gamma distribution.
		/// </returns>
		protected virtual double NextGamma(double alpha, double beta)
		{
			if (alpha < 0.0)
			{
				throw new ArgumentOutOfRangeException(nameof(alpha), "Alpha must > 0.0");
			}

			if (beta < 0.0)
			{
				throw new ArgumentOutOfRangeException(nameof(beta), "Beta must > 0.0");
			}

			if (alpha > 1.0)
			{
				// Uses R.C.H. Cheng, "The generation of Gamma
				// variables with non-integral shape parameters",
				// Applied Statistics, (1977), 26, No. 1, p71-74

				var ainv = Math.Sqrt(2.0 * alpha - 1.0);

				var bbb = alpha - Math.Log(4.0);
				var ccc = alpha + ainv;

				while (true)
				{
					var u1 = this.NextDouble();

					if (!(0.0000001 < u1 && u1 < 0.9999999))
					{
						continue;
					}

					var u2 = 1.0 - this.NextDouble();
					var v = Math.Log(u1 / (1.0 - u1)) / ainv;
					var x = alpha * Math.Exp(v);
					var z = u1 * u1 * u2;
					var r = bbb + ccc * v - x;

					if ((r + (1.0 + Math.Log(4.5)) - 4.5 * z >= 0.0) || (r >= Math.Log(z)))
					{
						return x * beta;
					}
				}
			}
			else if (alpha == 1.0)
			{
				return -Math.Log(1.0 - this.NextDouble()) * beta;
			}
			else
			{
				// alpha is between 0 and 1 (exclusive)
				// Uses ALGORITHM GS of Statistical Computing - Kennedy & Gentle
				double x;

				while (true)
				{
					var u = this.NextDouble();
					var b = (Math.E + alpha) / Math.E;
					var p = b * u;

					if (p < 1.0)
					{
						x = Math.Pow(p, (1.0 / alpha));
					}
					else
					{
						x = -Math.Log((b - p) / alpha);
					}

					var u1 = this.NextDouble();

					if (p > 1.0)
					{
						if (u1 <= Math.Pow(x, (alpha - 1.0)))
						{
							break;
						}
					}
					else if (u1 <= Math.Exp(-x))
					{
						break;
					}
				}
				return x * beta;
			}
		}
	}
}
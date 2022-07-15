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
		/// Generate beta distribution.
		/// </summary>
		/// <param name="alpha">
		/// The α shape parameter of the Beta distribution. Range: α ≥ 0.
		/// </param>
		/// <param name="beta">
		/// The β shape parameter of the Beta distribution. Range: β ≥ 0.
		/// </param>
		/// <returns>
		///	A 64-bit floating point number beta distribution.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public virtual double BetaDistribution(double alpha, double beta)
		{ 
			if (alpha < 0.0 || beta < 0.0)
			{
				throw new ArgumentOutOfRangeException("Alpha or Beta", "Alpha or beta can't be negative or lower than 0.");
			}

			double x, y;

			if (alpha == beta)
			{
				x = this.GaussianDistribution(alpha, 1);
				y = this.GaussianDistribution(beta, 1);
			}
			else
			{
				do
				{
					x = this.GaussianDistribution(alpha, 1);
					y = this.GaussianDistribution(beta, 1);
				} while (x == 0 && y == 0);
			}

			return x / (x + y);
		}

		/// <summary>
		///	Generate gaussian distribution.
		/// </summary>
		/// <returns>
		///	A 64-bit floating point number normal distribution.
		/// </returns>
		public virtual double GaussianDistribution()
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
		public virtual double GaussianDistribution(double mean = 0, double stdDev = 1)
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
		/// <param name="shape">
		///	The shape (k, α) of the Gamma distribution. Range: α ≥ 0.
		/// </param>
		/// <param name="rate">
		///	The rate or inverse scale (β) of the Gamma distribution. Range: β ≥ 0.
		/// </param>
		/// <returns>
		///	A 64-bit floating point number gamma distribution.
		/// </returns>
		public virtual double GammaDistribution(double shape, double rate)
		{
			if (shape < 0 || rate < 0)
			{
				throw new ArgumentOutOfRangeException("shape or rate", "The shape or rate can't lower than 0.");
			}

			if (double.IsPositiveInfinity(rate))
			{
				return shape;
			}

			var a = shape;
			var alphafix = 1.0;

			// Fix when alpha is less than one.
			if (shape < 1.0)
			{
				a = shape + 1.0;
				alphafix = Math.Pow(this.NextDouble(), 1.0 / shape);
			}

			var d = a - (1.0 / 3.0);
			var c = 1.0 / Math.Sqrt(9.0 * d);
			while (true)
			{
				var x = this.GaussianDistribution();
				var v = 1.0 + (c * x);
				while (v <= 0.0)
				{
					x = this.GaussianDistribution();
					v = 1.0 + (c * x);
				}

				v = v * v * v;
				var u = this.NextDouble();
				x = x * x;
				if (u < 1.0 - (0.0331 * x * x))
				{
					return alphafix * d * v / rate;
				}

				if (Math.Log(u) < (0.5 * x) + (d * (1.0 - v + Math.Log(v))))
				{
					return alphafix * d * v / rate;
				}
			}

		}

		/// <summary>
		/// Generate laplace distribution.
		/// </summary>
		/// <param name="mean">
		///	The mean (μ) of the distribution.
		/// </param>
		/// <param name="scale">
		/// The scale (b) of the distribution. Range: b > 0.
		/// </param>
		/// <returns>
		/// A 64-bit floating point number laplace distribution.
		/// </returns>
		public virtual double LaplaceDistribution(double mean, double scale)
		{
			var u = this.NextDouble() - 0.5;
			return mean - (scale * Math.Sign(u) * Math.Log(1.0 - (2.0 * Math.Abs(u))));
		}
	}
}
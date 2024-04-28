using System;

namespace Litdex.Random
{
	public partial class Random
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
		/// <exception cref="ArgumentOutOfRangeException">
		/// Alpha or Beta", "Alpha or beta can't be negative or lower than 0.
		/// </exception>
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
		/// Generate chi square distribution.
		/// </summary>
		/// <param name="k">
		/// The degrees of freedom (k) of the distribution. Range: k > 0.
		/// </param>
		/// <returns>
		/// A 64-bit floating point number chi square distribution.
		/// </returns>
		public virtual double ChiSquareDistribution(double k)
		{
			// Use the simple method if the degrees of freedom is an integer anyway
			if (Math.Floor(k) == k && k < int.MaxValue)
			{
				double sum = 0;
				var n = (int)k;
				for (var i = 0; i < n; i++)
				{
					sum += Math.Pow(this.GaussianDistribution(0.0, 1.0), 2);
				}

				return sum;
			}

			return this.GammaDistribution(k / 2.0, 0.5);
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
				return mean + stdDev * this._NextGaussian;
			}
			else
			{
				double u1 = this.NextDouble();
				double u2 = this.NextDouble();
				double z0 = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);
				double z1 = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
				this._NextGaussian = z1;
				return mean + stdDev * z0;
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
		/// <exception cref="ArgumentException">Scale must be positive.</exception>
		public virtual double LaplaceDistribution(double mean, double scale)
		{
			if (scale <= 0)
			{
				throw new ArgumentException("Scale must be positive.");
			}

			var u = this.NextDouble() - 0.5;
			return mean - (scale * Math.Sign(u) * Math.Log(1.0 - (2.0 * Math.Abs(u))));
		}

		/// <summary>
		/// Generate triangular distribution.
		/// </summary>
		/// <param name="lower">
		/// Lower bound. Range: lower ≤ mode ≤ upper.
		/// </param>
		/// <param name="upper">
		/// Upper bound. Range: lower ≤ mode ≤ upper.
		/// </param>
		/// <param name="mode">
		/// Mode (most frequent value). Range: lower ≤ mode ≤ upper.
		/// </param>
		/// <exception cref="ArgumentException">
		/// If the upper bound is smaller than the mode or if the mode is smaller than the lower bound.</exception>
		/// <returns>
		/// A 64-bit floating point number laplace distribution.
		/// </returns>
		/// <exception cref="ArgumentException">Lower bound must be less than upper bound.</exception>
		/// <exception cref="ArgumentException">Mode must be between lower and upper bounds.</exception>
		public virtual double TriangularDistribution(double lower, double upper, double mode)
		{
			if (lower >= upper)
			{
				throw new ArgumentException("Lower bound must be less than upper bound.");
			}

			if (mode < lower || mode > upper)
			{
				throw new ArgumentException("Mode must be between lower and upper bounds.");
			}

			var u = this.NextDouble();
			if (u < (mode - lower) / (upper - lower))
			{
				return lower + Math.Sqrt(u * (upper - lower) * (mode - lower));
			}
			else
			{
				return upper - Math.Sqrt((1 - u) * (upper - lower) * (upper - mode));
			}
		}
	}
}
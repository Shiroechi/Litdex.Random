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
		/// <param name="shape">
		///	The shape (k, α) of the Gamma distribution. Range: α ≥ 0.
		/// </param>
		/// <param name="rate">
		///	The rate or inverse scale (β) of the Gamma distribution. Range: β ≥ 0.
		/// </param>
		/// <returns>
		///	Gamma distribution.
		/// </returns>
		public virtual double NextGamma(double shape, double rate)
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
				var x = this.NextGaussian();
				var v = 1.0 + (c * x);
				while (v <= 0.0)
				{
					x = this.NextGaussian();
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
		/// Generates laplace distribution.
		/// </summary>
		/// <param name="mean">
		///	The mean (μ) of the distribution.
		/// </param>
		/// <param name="scale">
		/// The scale (b) of the distribution. Range: b > 0.
		/// </param>
		/// <returns></returns>
		public virtual double NextLaplace(double mean, double scale)
		{
			var u = this.NextDouble() - 0.5;
			return mean - (scale * Math.Sign(u) * Math.Log(1.0 - (2.0 * Math.Abs(u))));
		}
	}
}
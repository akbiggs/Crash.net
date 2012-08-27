using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace CrashNet.GRNG
{
    public class GRandom
    {
        /// <summary>
        /// The random generator behind this random generator.
        /// </summary>
        Random random;

        /// <summary>
        /// Make a new random generator.
        /// </summary>
        public GRandom()
        {
            random = new Random();
        }

        public int GetUniformInt(int min, int max)
        {
            return random.Next(min, max);
        }

        /// <summary>
        /// Return a random double from the specified distribution.
        /// </summary>
        /// <param name="type">The type of distribution.</param>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <returns>A random double.</returns>
        public double GetRandomDouble(Distribution type, double alpha, double beta)
        {
            if (type == Distribution.Uniform)
            {
                return GetUniformDouble(alpha, beta);
            }

            if (type == Distribution.Normal)
            {
                return GetNormalDouble(alpha, beta);
            }

            if (type == Distribution.Exponential)
            {
                return GetExpDouble(alpha);
            }

            if (type == Distribution.Fixed)
            {
                return alpha;
            }

            return 0.0;
        }


        /**
         * Returns a random Vector3D from specified
         * random distribution.
         **/
        public Vector3D GetRandomVector3D(Distribution type, Vector3D alpha, Vector3D beta)
        {
            if (type == Distribution.Uniform)
            {
                return GetUniformVector3D(alpha, beta);
            }

            if (type == Distribution.Normal)
            {
                return GetNormalVector3D(alpha, beta);
            }

            if (type == Distribution.Exponential)
            {
                return GetExpVector3D(alpha);
            }


            if (type == Distribution.Fixed)
            {
                return alpha;
            }

            return new Vector3D(0, 0, 0);
        }

        /**
         * Returns a random Vector3D with each component
         * drawn from a uniform distribution of specified
         * min and max.
         * **/
        public Vector3D GetUniformVector3D(Vector3D min, Vector3D max)
        {
            Vector3D newVector = new Vector3D();

            newVector.X = GetUniformDouble(min.X, max.X);
            newVector.Y = GetUniformDouble(min.Y, max.Y);
            newVector.Z = GetUniformDouble(min.Z, max.Z);

            return newVector;
        }

        /**
         * Returns a random double from a uniform dist.
         * Wrapper for Random class's NextDouble().
         **/
        public double GetUniformDouble(double min = 0.0, double max = 1.0)
        {
            return (random.NextDouble() * Math.Abs(max - min)) + min;

        }
        /**
         * Returns a randomly drawn double from a gaussian distribution
         * of specified mean and variance. Uses the Box-Muller transformation.
         **/
        public double GetNormalDouble(double mean = 0.0, double variance = 1.0)
        {
            double x1, x2, y1;

            x1 = random.NextDouble();
            x2 = random.NextDouble();

            y1 = Math.Sqrt(-2.0 * Math.Log(x1));
            return (mean + Math.Sqrt(variance) * y1 * Math.Sin(2.0 * Math.PI * x2));
        }

        /**
         * Returns a random vector3d from the normal distribution
         * with Vector3d mean and Vector3d variance.
         **/
        public Vector3D GetNormalVector3D(Vector3D mean, Vector3D variance)
        {
            Vector3D vector = new Vector3D();
            vector.X = GetNormalDouble(mean.X, variance.X);
            vector.Y = GetNormalDouble(mean.Y, variance.Y);
            vector.Z = GetNormalDouble(mean.Z, variance.Z);


            return vector;

        }

        /** 
         * Returns a double drawn from an exponential distribution
         * with parameter alpha.
         * TODO: implement.
         **/
        public double GetExpDouble(double alpha)
        {
            double u = random.NextDouble();

            return Math.Log(1-u) / (-1.0 * alpha);
        }

        /** 
         * Returns a Vector3D with each component drawn from
         * an exponential distribution of given parameters in alpha.
         **/
        public Vector3D GetExpVector3D(Vector3D alpha)
        {
            return new Vector3D(GetExpDouble(alpha.X), GetExpDouble(alpha.X), GetExpDouble(alpha.X));
        }

        /**
         * Finds the mean of given data.
         **/
        static public double Mean(double[] data, int n)
        {
            double sx = 0;

            for (int i = 0; i < n; i++)
            {
                sx += data[i];
            }

            return (sx / (double)n);
        }

        /**
         * Finds the variance of given data.
         **/
        static public double variance(double[] data, int n)
        {
            double sxx = 0;
            double mean = Mean(data, n);

            for (int i = 0; i < n; i++)
            {
                sxx += Math.Pow((data[i] - mean), 2);
            }
            return (sxx / (n - 1));


        }


    }
}

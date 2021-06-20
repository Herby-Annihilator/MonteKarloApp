using System;
using System.Collections.Generic;
using System.Text;

namespace MonteKarloApp.Models.Data
{
	public class Point
	{
        private const double EPS = 0.000000001;
		public double X { get; set; }
		public double Y { get; set; }

		public Point(double x, double y)
		{
			X = x;
			Y = y;
		}

        private static double Det(double a, double b, double c, double d)
        {
            return a * d - b * c;
        }

        private static bool Between(double a, double b, double c)
        {
            return Math.Min(a, b) <= c + EPS && c <= Math.Max(a, b) + EPS;
        }

        private static bool Intersect1(double a, double b, double c, double d)
        {
            if (a > b) Swap(ref a, ref b);
            if (c > d) Swap(ref c, ref d);
            return Math.Max(a, c) <= Math.Min(b, d);
        }

        public static bool Intersect(Point a, Point b, Point c, Point d)
        {
            double A1 = a.Y - b.Y, B1 = b.X - a.X, C1 = -A1 * a.X - B1 * a.Y;
            double A2 = c.Y - d.Y, B2 = d.X - c.X, C2 = -A2 * c.X - B2 * c.Y;
            double zn = Det(A1, B1, A2, B2);
            if (zn != 0)
            {
                double x = -Det(C1, B1, C2, B2) / zn;
                double y = -Det(A1, C1, A2, C2) / zn;
                return Between(a.X, b.X, x) && Between(a.Y, b.Y, y)
                    && Between(c.X, d.X, x) && Between(c.Y, d.Y, y);
            }
            else
                return Det(A1, C1, A2, C2) == 0 && Det(B1, C1, B2, C2) == 0
                && Intersect1(a.X, b.X, c.X, d.X)
                && Intersect1(a.Y, b.Y, c.Y, d.Y);
        }
        private static void Swap(ref double a, ref double b)
		{
            double tmp = a;
            a = b;
            b = tmp;
		}
    }
}

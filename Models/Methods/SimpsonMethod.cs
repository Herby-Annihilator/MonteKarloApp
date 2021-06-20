using System;
using System.Collections.Generic;
using System.Text;

namespace MonteKarloApp.Models.Methods
{
	public class SimpsonMethod
	{
        public double GetSolutionWithAutoStep(Func<double, double> function, double left, double right, double precision, double segmentNumber)
        {
            double resultValue = 0;
            double intervalSize = Math.Abs(right - left) / segmentNumber;
            double currentLeft = left, currentRight = left + intervalSize;
            for (int i = 0; i < segmentNumber; i++)
            {
                resultValue += Recalculate(function, currentLeft, currentRight, precision);
                currentLeft = currentRight;
                currentRight += intervalSize;
            }            
            return resultValue;
        }

        double Recalculate(Func<double, double> function, double left, double right, double precision)
        {
            double step = Math.Abs(left - right);
            double result = -1, previousResult = 0;
            while (Math.Abs(result - previousResult) > precision)
            {
                previousResult = result;
                result = 0;
                double odd = 0, even = 0;
                int i = 1;
                double x = left + step;
                while (x < right)
                {
                    if (i % 2 == 1) odd += function(x);
                    else even += function(x);
                    x += step;
                    i++;
                }
                result += 4 * odd + 2 * even + function(left) + function(right);
                result *= (step / 3);
                step /= 2;
            }
            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace MonteKarloApp.Models.Data
{
	public class ParamAndPoint
	{
		public double ParamT { get; set; }
		public double X { get; set; }
		public double Y { get; set; }

		public ParamAndPoint(double paramT, double x, double y)
		{
			ParamT = paramT;
			X = x;
			Y = y;
		}
	}
}

using MonteKarloApp.Infrastructure.Converters.Base;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace MonteKarloApp.Infrastructure.Converters
{
	public class DoubleToStringConverter : Converter
	{
		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value.ToString();
		}
		public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string str = (string)value;
			str = str.Replace('.', ',');
			if (str.IndexOf(',') == str.LastIndexOf(','))
			{
				if (str.IndexOf(',') == str.Length - 1)
				{
					str += "1";
				}
				if (double.TryParse(str, out double number))
				{
					return number;
				}
				else
				{
					return 0;
				}
			}
			else
			{
				return 0;
			}
		}
	}
}

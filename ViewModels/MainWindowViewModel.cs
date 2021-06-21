using MonteKarloApp.Models.Data;
using MonteKarloApp.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Markup;
using MonteKarloApp.Infrastructure.Commands;
using System.Windows.Input;
using org.mariuszgromada.math.mxparser;
using MonteKarloApp.Models.Methods;

namespace MonteKarloApp.ViewModels
{
	[MarkupExtensionReturnType(typeof(MainWindowViewModel))]
	public class MainWindowViewModel : ViewModel
	{
		private const double PRECISION = 0.00001;
		public MainWindowViewModel()
		{
			ShowrBorderTableCommand = new LambdaCommand(OnShowrBorderTableCommandExecuted, CanShowrBorderTableCommandExecute);
			ShowBorderAreaCommand = new LambdaCommand(OnShowBorderAreaCommandExecuted, CanShowBorderAreaCommandExecute);
			ClearBorderAreaCommand = new LambdaCommand(OnClearBorderAreaCommandExecuted, CanClearBorderAreaCommandExecute);
			ClearBorderAreaTableCommand = new LambdaCommand(OnClearBorderAreaTableCommandExecuted, CanClearBorderAreaTableCommandExecute);
			ShowOrClearBoundingSquareCommand = new LambdaCommand(OnShowOrClearBoundingSquareCommandExecuted, CanShowOrClearBoundingSquareCommandExecute);
			CalculateSquareCommand = new LambdaCommand(OnCalculateSquareCommandExecuted, CanCalculateSquareCommandExecute);
			ShowMonteKarloCommand = new LambdaCommand(OnShowMonteKarloCommandExecuted, CanShowMonteKarloCommandExecute);
			ClearMonteKarloCommand = new LambdaCommand(OnClearMonteKarloCommandExecuted, CanClearMonteKarloCommandExecute);
			ShowOrClearCenterCommand = new LambdaCommand(OnShowOrClearCenterCommandExecuted, CanShowOrClearCenterCommandExecute);
		}

		#region Properties
		private string _title = "Title";
		public string Title { get => _title; set => Set(ref _title, value); }

		private string _status = "Status";
		public string Status { get => _status; set => Set(ref _status, value); }

		private string _simpsonSquare = "";
		public string SimpsonSquare { get => _simpsonSquare; set => Set(ref _simpsonSquare, value); }

		private string _monteKarloSquare = "";
		public string MonteKarloSquare { get => _monteKarloSquare; set => Set(ref _monteKarloSquare, value); }

		private string _difference = "";
		public string Difference { get => _difference; set => Set(ref _difference, value); }

		private string _centerX = "";
		public string CenterX { get => _centerX; set => Set(ref _centerX, value); }

		private string _centerY = "";
		public string CenterY { get => _centerY; set => Set(ref _centerY, value); }

		private string _xtExpression = "";
		public string XTExpression { get => _xtExpression; set => Set(ref _xtExpression, value); }

		private string _ytExpression = "";
		public string YTExpression { get => _ytExpression; set => Set(ref _ytExpression, value); }

		private double _left;
		public double Left { get => _left; set => Set(ref _left, value); }

		private double _right;
		public double Right { get => _right; set => Set(ref _right, value); }

		private int _stepsCount;
		public int StepsCount { get => _stepsCount; set => Set(ref _stepsCount, value); }

		private int _pointsNumber;
		public int PointsNumber { get => _pointsNumber; set => Set(ref _pointsNumber, value); }

		public ObservableCollection<Point> BorderAreaGraph { get; private set; } = new ObservableCollection<Point>();
		public ObservableCollection<ParamAndPoint> BorderAreaTable { get; private set; } = new ObservableCollection<ParamAndPoint>();

		public ObservableCollection<Point> BoundingSquare { get; private set; } = new ObservableCollection<Point>();

		public ObservableCollection<Point> GoodPoints { get; private set; } = new ObservableCollection<Point>();
		private List<Point> _goodPoints = new List<Point>();
		public ObservableCollection<Point> BadPoints { get; private set; } = new ObservableCollection<Point>();
		private List<Point> _badPoints = new List<Point>();

		public ObservableCollection<Point> Center { get; private set; } = new ObservableCollection<Point>();

		private string _xtdExpression = "";
		public string XTDExpression 
		{ 
			get => _xtdExpression;
			set
			{
				if (value == "")
					_ytdFunction = null;
				Set(ref _xtdExpression, value);
				_xtdFunction = new Function($"f(t) = {_xtdExpression.Replace(',', '.')}");
				_ytdExpression = "";
			}
		}
		private Function _xtdFunction;

		private string _ytdExpression = "";
		public string YTDExpression 
		{ 
			get => _ytdExpression;
			set
			{
				if (value == "")
					_ytdFunction = null;
				Set(ref _ytdExpression, value);
				_ytdFunction = new Function($"f(t) = {_ytdExpression.Replace(',', '.')}");
				XTDExpression = "";
			}
		}
		private Function _ytdFunction;
		#endregion

		#region Commands
		public ICommand ShowrBorderTableCommand { get; }
		private void OnShowrBorderTableCommandExecuted(object p)
		{
			try
			{
				BorderAreaTable.Clear();
				double step = (Right - Left) / StepsCount;
				double currentParamValue = Left;
				Function xt = new Function($"x(t) = {XTExpression.Replace(',', '.')}");
				Function yt = new Function($"y(t) = {YTExpression.Replace(',', '.')}");
				for (int i = 0; i < StepsCount; i++)
				{
					BorderAreaTable.Add(new ParamAndPoint(currentParamValue, xt.calculate(currentParamValue), yt.calculate(currentParamValue)));
					currentParamValue += step;
					if (currentParamValue > Right)
						currentParamValue = Right;
				}
				Status = "Таблица отображена";
			}
			catch (Exception e)
			{
				Status = $"Неудача, причина - {e.Message}";
			}
		}
		private bool CanShowrBorderTableCommandExecute(object p) => Left < Right && StepsCount > 0 && !string.IsNullOrWhiteSpace(XTExpression) && !string.IsNullOrWhiteSpace(YTExpression);

		public ICommand ShowBorderAreaCommand { get; }
		private void OnShowBorderAreaCommandExecuted(object p)
		{
			try
			{
				BorderAreaGraph.Clear();
				for (int i = 0; i < BorderAreaTable.Count; i++)
				{
					BorderAreaGraph.Add(new Point(BorderAreaTable[i].X, BorderAreaTable[i].Y));
				}
				Status = "Область отображена";
			}
			catch (Exception e)
			{
				Status = $"Неудача, причина - {e.Message}";
			}			
		}
		private bool CanShowBorderAreaCommandExecute(object p) => BorderAreaTable.Count > 0;

		public ICommand ClearBorderAreaCommand { get; }
		private void OnClearBorderAreaCommandExecuted(object p)
		{
			BorderAreaGraph.Clear();
			Status = "Область стерта";
		}
		private bool CanClearBorderAreaCommandExecute(object p) => BorderAreaGraph.Count > 0;

		public ICommand ClearBorderAreaTableCommand { get; }
		private void OnClearBorderAreaTableCommandExecuted(object p)
		{
			BorderAreaTable.Clear();
			Status = "Таблица очищена";
		}
		private bool CanClearBorderAreaTableCommandExecute(object p) => BorderAreaTable.Count > 0;


		#region BoundingSquareCommand
		public ICommand ShowOrClearBoundingSquareCommand { get; }
		private void OnShowOrClearBoundingSquareCommandExecuted(object p)
		{
			try
			{
				if (BoundingSquare.Count > 0)
				{
					BoundingSquare.Clear();
					Status = "Описывающий прямоугольник стерт";
				}
				else
				{
					double maxX, maxY;
					double minX, minY;
					GetExtreemPoints(out minX, out maxX, out minY, out maxY);					
					BoundingSquare.Add(new Point(minX, maxY));
					BoundingSquare.Add(new Point(maxX, maxY));
					BoundingSquare.Add(new Point(maxX, minY));
					BoundingSquare.Add(new Point(minX, minY));

					BoundingSquare.Add(new Point(minX, maxY));

					Status = "Описывающий прямоугольник нарисован";
				}
			}
			catch (Exception e)
			{
				Status = $"Неудача, причина - {e.Message}";
			}
		}
		private bool CanShowOrClearBoundingSquareCommandExecute(object p) => BorderAreaTable.Count > 0;
		#endregion

		#region CalculateSquareCommand
		public ICommand CalculateSquareCommand { get; }
		private void OnCalculateSquareCommandExecuted(object p)
		{
			try
			{
				SimpsonMethod simpsonMethod = new SimpsonMethod();
				GetExtreemPoints(out double minX, out double maxX, out double minY, out double maxY);
				double simpsonSquare = MonteKarlo(PointsNumber * 10);
				SimpsonSquare = simpsonSquare.ToString();

				double x, y;
				Point rndPoint = new Point(0, 0);
				Point firstSegmentPoint = new Point(0, 0);
				Point secondSegmentPoint = new Point(0, 0);
				Point outsidePoint = new Point(maxX * 1.1, maxY * 1.1);
				Random random = new Random();
				int intersections_count = 0;
				double rectangleSquare = (maxX - minX) * (maxY - minY);
				_badPoints.Clear();
				_goodPoints.Clear();
				for (int i = 0; i < PointsNumber; i++)
				{
					x = minX + (maxX - minX) * random.NextDouble();
					y = minY + (maxY - minY) * random.NextDouble();
					rndPoint.X = x;
					rndPoint.Y = y;
					for (int j = 0; j < BorderAreaTable.Count - 1; j++)
					{
						firstSegmentPoint.X = BorderAreaTable[j].X;
						firstSegmentPoint.Y = BorderAreaTable[j].Y;

						secondSegmentPoint.X = BorderAreaTable[j + 1].X;
						secondSegmentPoint.Y = BorderAreaTable[j + 1].Y;
						if (Point.Intersect(outsidePoint, rndPoint, firstSegmentPoint, secondSegmentPoint))
						{
							intersections_count++;
						}
					}
					if ((intersections_count % 2 == 1) && rndPoint.X >= minX && rndPoint.Y >= minY && rndPoint.X <= maxX && rndPoint.Y <= maxY)
					{
						_goodPoints.Add(rndPoint.Clone());
					}
					else
					{
						_badPoints.Add(rndPoint.Clone());
					}
				}
				double monteKarloSquare = _goodPoints.Count / (double)PointsNumber * rectangleSquare;
				MonteKarloSquare = monteKarloSquare.ToString();
				Difference = (Math.Abs(simpsonSquare - monteKarloSquare)).ToString();
				double xCenter = 0, yCenter = 0;
				for (int i = 0; i < _goodPoints.Count; i++)
				{
					xCenter += _goodPoints[i].X;
					yCenter += _goodPoints[i].Y;
				}
				xCenter /= _goodPoints.Count;
				yCenter /= _goodPoints.Count;
				CenterX = xCenter.ToString();
				CenterY = yCenter.ToString();
				Status = "Площадь рассчитана успешно";
			}
			catch (Exception e)
			{
				Status = $"Неудача, причина - {e.Message}";
			}
		}
		private bool CanCalculateSquareCommandExecute(object p) => BorderAreaTable.Count > 0 && (_xtdFunction != null || _ytdFunction != null);
		#endregion

		#region ShowMonteKarloCommand
		public ICommand ShowMonteKarloCommand { get; }
		private void OnShowMonteKarloCommandExecuted(object p)
		{
			try
			{
				int minGoodpointsAmount = _goodPoints.Count > 5000 ? 5000 : _goodPoints.Count;
				int minBadpointsAmount = _badPoints.Count > 5000 ? 5000 : _badPoints.Count;
				GoodPoints.Clear();
				BadPoints.Clear();
				for (int i = 0; i < minGoodpointsAmount; i++)
				{
					GoodPoints.Add(_goodPoints[i]);
				}
				for (int i = 0; i < minBadpointsAmount; i++)
				{
					BadPoints.Add(_badPoints[i]);
				}
				Status = "Точки отображены";
			}
			catch(Exception e)
			{
				Status = $"Неудача, причина - {e.Message}";
			}
		}
		private bool CanShowMonteKarloCommandExecute(object p) => _badPoints.Count > 0 && _goodPoints.Count > 0;
		#endregion

		#region ClearMonteKarloCommand
		public ICommand ClearMonteKarloCommand { get; }
		private void OnClearMonteKarloCommandExecuted(object p)
		{
			GoodPoints.Clear();
			BadPoints.Clear();
			Status = "Точки стерты";
		}
		private bool CanClearMonteKarloCommandExecute(object p) => BadPoints.Count > 0 && GoodPoints.Count > 0;

		#endregion

		#region ShowOrClearCenterCommand
		public ICommand ShowOrClearCenterCommand { get; }
		private void OnShowOrClearCenterCommandExecuted(object p)
		{
			try
			{
				if (Center.Count > 0)
				{
					Center.Clear();
					Status = "Центр стерт";
				}					
				else
				{
					double x = Convert.ToDouble(CenterX.Replace('.', ','));
					double y = Convert.ToDouble(CenterY.Replace('.', ','));
					Center.Add(new Point(x, y));
					Status = "Центр отображен";
				}				
			}
			catch(Exception e)
			{
				Status = $"Неудача, причина - {e.Message}";
			}
		}
		private bool CanShowOrClearCenterCommandExecute(object p) => !(string.IsNullOrWhiteSpace(CenterX) || string.IsNullOrWhiteSpace(CenterY));
		#endregion

		#endregion

		private void GetExtreemPoints(out double minX, out double maxX, out double minY, out double maxY)
		{
			maxX = BorderAreaTable[0].X;
			maxY = BorderAreaTable[0].Y;
			minX = BorderAreaTable[0].X;
			minY = BorderAreaTable[0].Y;
			for (int i = 1; i < BorderAreaTable.Count; i++)
			{
				if (BorderAreaTable[i].X > maxX)
					maxX = BorderAreaTable[i].X;
				else if (BorderAreaTable[i].X < minX)
					minX = BorderAreaTable[i].X;
				if (BorderAreaTable[i].Y > maxY)
					maxY = BorderAreaTable[i].Y;
				else if (BorderAreaTable[i].Y < minY)
					minY = BorderAreaTable[i].Y;
			}
		}

		private double GetFunctionValue(double arg)
		{
			double result = 0;
			if (_xtdFunction != null)
			{
				for (int i = 0; i < BorderAreaTable.Count; i++)
				{
					if (arg == BorderAreaTable[i].ParamT)
					{
						result = BorderAreaTable[i].Y * _xtdFunction.calculate(arg);
						break;
					}
				}
			}
			else
			{
				for (int i = 0; i < BorderAreaTable.Count; i++)
				{
					if (arg == BorderAreaTable[i].ParamT)
					{
						result = BorderAreaTable[i].X * _ytdFunction.calculate(arg);
						break;
					}
				}
			}
			return result;
		}

		private double MonteKarlo(int numberOfPoints)
		{
			GetExtreemPoints(out double minX, out double maxX, out double minY, out double maxY);
			double x, y;
			Point rndPoint = new Point(0, 0);
			Point firstSegmentPoint = new Point(0, 0);
			Point secondSegmentPoint = new Point(0, 0);
			Point outsidePoint = new Point(maxX * 1.1, maxY * 1.1);
			Random random = new Random();
			int intersections_count = 0;
			double rectangleSquare = (maxX - minX) * (maxY - minY);
			List<Point> goodPoints = new List<Point>();
			for (int i = 0; i < numberOfPoints; i++)
			{
				x = minX + (maxX - minX) * random.NextDouble();
				y = minY + (maxY - minY) * random.NextDouble();
				rndPoint.X = x;
				rndPoint.Y = y;
				for (int j = 0; j < BorderAreaTable.Count - 1; j++)
				{
					firstSegmentPoint.X = BorderAreaTable[j].X;
					firstSegmentPoint.Y = BorderAreaTable[j].Y;

					secondSegmentPoint.X = BorderAreaTable[j + 1].X;
					secondSegmentPoint.Y = BorderAreaTable[j + 1].Y;
					if (Point.Intersect(outsidePoint, rndPoint, firstSegmentPoint, secondSegmentPoint))
					{
						intersections_count++;
					}
				}
				if ((intersections_count % 2 == 1) && rndPoint.X >= minX && rndPoint.Y >= minY && rndPoint.X <= maxX && rndPoint.Y <= maxY)
				{
					goodPoints.Add(rndPoint.Clone());
				}
			}
			double monteKarloSquare = goodPoints.Count / (double)numberOfPoints * rectangleSquare;
			return monteKarloSquare;
		}
	}
}

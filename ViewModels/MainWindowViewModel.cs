using MonteKarloApp.Models.Data;
using MonteKarloApp.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Markup;
using MonteKarloApp.Infrastructure.Commands;
using MonteKarloApp.Infrastructure.Converters;
using System.Windows.Input;
using org.mariuszgromada.math.mxparser;

namespace MonteKarloApp.ViewModels
{
	[MarkupExtensionReturnType(typeof(MainWindowViewModel))]
	public class MainWindowViewModel : ViewModel
	{
		public MainWindowViewModel()
		{
			ShowrBorderTableCommand = new LambdaCommand(OnShowrBorderTableCommandExecuted, CanShowrBorderTableCommandExecute);
			ShowBorderAreaCommand = new LambdaCommand(OnShowBorderAreaCommandExecuted, CanShowBorderAreaCommandExecute);
			ClearBorderAreaCommand = new LambdaCommand(OnClearBorderAreaCommandExecuted, CanClearBorderAreaCommandExecute);
			ClearBorderAreaTableCommand = new LambdaCommand(OnClearBorderAreaTableCommandExecuted, CanClearBorderAreaTableCommandExecute);
			ShowOrClearBoundingSquareCommand = new LambdaCommand(OnShowOrClearBoundingSquareCommandExecuted, CanShowOrClearBoundingSquareCommandExecute);
		}

		#region Properties
		private string _title = "Title";
		public string Title { get => _title; set => Set(ref _title, value); }

		private string _status = "Status";
		public string Status { get => _status; set => Set(ref _status, value); }

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

		public ObservableCollection<Point> BorderAreaGraph { get; private set; } = new ObservableCollection<Point>();
		public ObservableCollection<ParamAndPoint> BorderAreaTable { get; private set; } = new ObservableCollection<ParamAndPoint>();

		public ObservableCollection<Point> BoundingSquare { get; private set; } = new ObservableCollection<Point>();
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


		#region BoundingSquare
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
					double maxX = BorderAreaTable[0].X, maxY = BorderAreaTable[0].Y;
					double minX = BorderAreaTable[0].X, minY = BorderAreaTable[0].Y;
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
		#endregion
	}
}

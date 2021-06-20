using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonteKarloApp.ViewModels
{
	public class ViewModelLocator
	{
		public MainWindowViewModel MainWindowModel => App.Services.GetRequiredService<MainWindowViewModel>();
	}
}

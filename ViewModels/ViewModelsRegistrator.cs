using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonteKarloApp.ViewModels
{
	public static class ViewModelsRegistrator
	{
		public static IServiceCollection AddViewModels(this IServiceCollection services) => services
		   .AddSingleton<MainWindowViewModel>()
		;
	}
}

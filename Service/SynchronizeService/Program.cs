using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace SynchronizeService
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
#if DEBUG
Windows.MainWindow mainWindow = new Windows.MainWindow();

			mainWindow.ShowDialog();
#else
			ServiceBase[] ServicesToRun;

			ServicesToRun = new ServiceBase[] 
			{ 
				new Services.SynchronizeService() 
			};
			ServiceBase.Run(ServicesToRun);
#endif
		}
	}
}

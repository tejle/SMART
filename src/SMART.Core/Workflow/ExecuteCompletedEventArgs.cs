using System;
using System.ComponentModel;
using SMART.Core.Interfaces.Reporting;

namespace SMART.Core.Workflow
{
	public class ExecuteCompletedEventArgs : AsyncCompletedEventArgs
	{
	    public IReport Report { get; set; }

	    public ExecuteCompletedEventArgs(UnitOfWork work, Exception error, IReport report, bool cancelled, object userState) : base(error, cancelled, userState)
		{
		    Report = report;
		}
	}
}
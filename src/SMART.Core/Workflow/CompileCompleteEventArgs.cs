using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace SMART.Core.Workflow
{
	public class CompileCompletedEventArgs : AsyncCompletedEventArgs
	{
		private UnitOfWork unitOfWork;

		public UnitOfWork UnitOfWork
		{
			get { return unitOfWork; }
		}

		public CompileCompletedEventArgs(UnitOfWork unitOfWork, Exception error, bool cancelled, object userState) : base(error, cancelled, userState)
		{
			this.unitOfWork = unitOfWork;
		}
	}
}

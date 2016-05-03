using System;
using System.ComponentModel;

namespace SMART.Core.Workflow
{
	public class GenerateCompletedEventArgs : AsyncCompletedEventArgs
	{
		private readonly UnitOfWork work;
		

		public GenerateCompletedEventArgs(UnitOfWork work, Exception error, bool cancelled, object userState)
			: base(error, cancelled, userState)
		{
			this.work = work;
		}

		public UnitOfWork UnitOfWork
		{
			get { return work; }
		}
	}
}
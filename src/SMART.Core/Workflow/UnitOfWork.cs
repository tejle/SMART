using System.Collections.Generic;
using SMART.Core.Interfaces;
using System;

namespace SMART.Core.Workflow
{
	public class UnitOfWork
	{
	    private IProject project;
		private ITestcase testcase;
		private IModel model;
		private List<Queue<IStep>> steps;
        public TimeSpan ElapsedTime { get; set; }
        public IEnumerable<Queue<IStep>> DefectFlows { get; set; }

		internal UnitOfWork(IProject project, ITestcase testcase, IModel model, List<Queue<IStep>> steps)
		{
		    this.project = project;
			this.testcase = testcase;
			this.model = model;
			this.steps = steps;
		}

		public List<Queue<IStep>> Steps
		{
			get { return steps; }
		}

	    public int TotalSteps
	    {
	        get
	        {
	            var totalSteps = 0;
	            foreach (var queue in steps)
	            {
	                totalSteps += queue.Count;
	            }
	            return totalSteps;
	        }
	    }

		public ITestcase Testcase
		{
			get
			{
				return testcase;
			}
		}

		public IModel Model
		{
			get
			{
				return model;
			}
		}

        public IProject Project
        {
            get
            {
                return project;
            }
        }
	}
}
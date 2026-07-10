namespace SMART.Gui.ViewModel.TestcaseConfiguration
{
    using System;

    using Core.Metadata;

    public class AlgorithmTypeViewModel: ViewModelBase
    {
        public readonly ClassDescription Algorithm;
        

        public override string Icon
        {
            get { return Constants.MISSING_ICON_URL; }
        }

        public override string Name
        {
            get
            {
                return this.Algorithm.Name;
            }
        }

        public override Guid Id
        {
            get;
            set;
        }

        public string Description
        {
            get
            {
                return this.Algorithm.Description;
            }
        }

        public AlgorithmTypeViewModel(ClassDescription algorithm)
        {
            this.Algorithm = algorithm;            
        }
    }
}
namespace SMART.Gui.ViewModel.TestcaseConfiguration
{
    using System;

    using Core.Metadata;

    public class GenerationStopCriteriaTypeViewModel: ViewModelBase
    {
        public readonly ClassDescription StopCriteria;        

        public override string Icon
        {
            get { return Constants.MISSING_ICON_URL; }
        }

        public override string Name
        {
            get
            {
                return this.StopCriteria.Name;
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
                return this.StopCriteria.Description;
            }
        }

        public GenerationStopCriteriaTypeViewModel(ClassDescription stopCriteria)
        {
            this.StopCriteria = stopCriteria;            
        }
    }
}
namespace SMART.Gui.ViewModel.TestcaseConfiguration
{
    using System;

    using Core.Metadata;

    public class AdapterTypeViewModel: ViewModelBase
    {
        public readonly ClassDescription Adapter;        

        public override string Icon
        {
            get { return Constants.MISSING_ICON_URL; }
        }

        public override string Name
        {
            get
            {
                return this.Adapter.Name;
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
                return this.Adapter.Description;
            }
        }

        public AdapterTypeViewModel(ClassDescription adapter)
        {
            this.Adapter = adapter;            
        }
    }
}
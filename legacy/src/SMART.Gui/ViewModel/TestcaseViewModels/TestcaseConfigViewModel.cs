using SMART.Core.Interfaces;

namespace SMART.Gui.ViewModel.TestcaseViewModels
{
    using System.Collections.ObjectModel;

    public class TestcaseConfigViewModel:ViewModelBase, ITestcaseViewModel
    {
        //public ObservableCollection<AdapterViewModel> Adapters
        //{
        //    get;
        //    private set;
        //}

        private readonly ITestcase testcase;

        public TestcaseConfigViewModel(ITestcase testcase) : base(testcase.Name)
        {
            this.testcase = testcase;
            CreateCollections();
        }

        private void CreateCollections()
        {
            //Adapters = new ObservableCollection<AdapterViewModel>();
            //testcase.Adapters.ForEach(a => Adapters.Add(new AdapterViewModel(testcase) ););
        }

        public override string Icon
        {
            get { return Constants.CONFIGSETTINGS_ICON_URL; }
        }
    }
}

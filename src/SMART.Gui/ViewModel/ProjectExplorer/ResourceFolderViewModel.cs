namespace SMART.Gui.ViewModel.ProjectExplorer
{
    using System;

    public class ResourceFolderViewModel : ViewModelBase
    {
        public ResourceFolderViewModel() : base()
        {
        }

        public override string Icon
        {
            get { return ""; }
        }

        public override Guid Id { get; set; }
    }
}
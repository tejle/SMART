namespace SMART.Gui.ViewModel.ProjectExplorer
{
    using System;
    using System.Collections.ObjectModel;

    public class RootResourceFolderViewModel : ViewModelBase, IFolderViewModel
    {

        public ObservableCollection<ResourceFolderViewModel> FolderViewModels
        {
            get;
            set;
        }
        
        public RootResourceFolderViewModel() : base("Resources")
        {
            
            this.createResourceFolders();
        }

        private void createResourceFolders()
        {
            this.FolderViewModels = new ObservableCollection<ResourceFolderViewModel>();
        }

        public override string Icon
        {
            get { return "/Resources/Images/OpenFolder.ico"; }
        }

        public override Guid Id
        {
            get;
            set;
        }
    }
}
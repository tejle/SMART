using SMART.Core.DomainModel;
using SMART.Core.Events;
using SMART.Core.Interfaces;
using SMART.Core.Interfaces.Services;

namespace SMART.Gui.ViewModel.ProjectExplorer
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Commands;

    using IOC;

    public class RootModelFolderViewModel : ViewModelBase, IFolderViewModel
    {
        private readonly IProject project;
        private readonly IModelService modelService;
        

        public ObservableCollection<ModelFolderViewModel> FolderViewModels
        {
            get;
            set;
        }

        public RoutedActionCommand AddModel
        {
            get;
            private set;
        }
        public RootModelFolderViewModel(IProject project):this(project, Resolver.Resolve<IModelService>()){}
        public RootModelFolderViewModel(IProject project, IModelService modelService) : base("Models")
        {
            this.project = project;
            this.modelService = modelService;
            
            project.PropertyChanged += this.project_PropertyChanged;
            this.CreateModelFolders();
            this.CreateCommands();
        }

        private void CreateCommands()
        {
            this.AddModel = new RoutedActionCommand("AddModel", typeof(RootModelFolderViewModel))
                           {
                                   Description = "Add a new model", 
                                   OnCanExecute = (o) => true,
                                   OnExecute = this.OnAddModel,
                                   Text = "Add Model",
                                   Icon = Constants.MODEL_ADD_ICON_URL
                           };
        }

        protected virtual void OnAddModel(object obj)
        {
            var model = this.modelService.CreateModel("new model");

            this.project.AddModel(model);
        }

        void project_PropertyChanged(object sender, SmartPropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Models")
            {
                var model = e.Item as Model;
                if(model == null)
                    return;

                switch (e.Action)
                {
                    case SmartPropertyChangedAction.Add:
                        var g = new ModelFolderViewModel(model, this.project);
                      
                        this.FolderViewModels.Add(g);
                        break;
                    case SmartPropertyChangedAction.Remove:
                        var fvm = this.FolderViewModels.Where(gvm => gvm.ModelId == model.Id); 
                        if(fvm.Count() == 0) return;
                        this.FolderViewModels.Remove(fvm.First());
                        break;
                    default:break;
                }
            }
        }

        private void CreateModelFolders()
        {

            var list = from g in this.project.Models select new ModelFolderViewModel(g, this.project);
            this.FolderViewModels = new ObservableCollection<ModelFolderViewModel>();
            foreach( var g in list)
            {
                this.FolderViewModels.Add(g);
                
            }
        }

        public override string Icon
        {
            get { return Constants.OPEN_ICON_URL; }
        }

        public override Guid Id
        {
            get;
            set;
        }
    }
}
using SMART.Core.Events;
using SMART.Core.Interfaces;

namespace SMART.Gui.ViewModel.ProjectExplorer
{
    using System;

    using Commands;

    using Core.Interfaces.Services;

    using Events;

    using IOC;

    public class ModelFolderViewModel : ViewModelBase
    {
        private readonly IModel model;
        private readonly IProject project;

        public RoutedActionCommand Open {get;private set;}
        public RoutedActionCommand Remove {get;private set;}
        public RoutedActionCommand Rename{get;private set;}
        public RoutedActionCommand AddToTestcase{get;private set;}
        public override string Name
        {
            get
            {
                return base.Name;
            }
            set
            {
                if(!base.Name.Equals(value))
                {
                    base.Name = value;
                    if(!this.model.Name.Equals(value))
                        this.model.Name = value;
                }
                
            }
        }

        public override Guid Id
        {
            get { return model.Id; }
            set { model.Id = value; }
        }

        public ModelFolderViewModel(IModel model, IProject project) : base(model.Name)
        {
            this.model = model;
            model.PropertyChanged += this.model_PropertyChanged;
            this.project = project;
            this.CreateCommand();
        }

        void model_PropertyChanged(object sender, SmartPropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Name"))
                this.Name = this.model.Name;
        }

        private void CreateCommand()
        {
            this.Open = new RoutedActionCommand("Open", typeof(ModelFolderViewModel))
                       {Description = "Open model", OnCanExecute = (o) => true, OnExecute = this.OnOpen, Text = "Open", Icon = Constants.OPEN_ICON_URL};
            this.Remove = new RoutedActionCommand("Remove", typeof(ModelFolderViewModel))
                         {Description = "Remove model", OnCanExecute = (o) => true, OnExecute = this.OnRemove, Text = "Remove", Icon = Constants.DELETE_ICON_URL};
            this.Rename = new RoutedActionCommand("Rename", typeof(ModelFolderViewModel))
                         {Description = "Rename model", OnCanExecute = (o) => true, OnExecute = this.OnRename, Text = "Rename",  Icon = Constants.RENAME_ICON_URL};
            this.AddToTestcase = new RoutedActionCommand("AddToTestcase", typeof(ModelFolderViewModel))
                                {Description = "Add model to testcase", OnCanExecute = (o) => true, OnExecute = this.OnAddToTestcase, Text = "Add to testcase"};
        }

        protected virtual void OnAddToTestcase(object obj)
        {
            
        }

        protected virtual void OnRename(object obj)
        {
            string name = "new name";
            //model.Name = name;
            this.Name = name;
        }

        protected virtual void OnRemove(object obj)
        {
            var success = this.project.RemoveModel(this.model);

        }

        protected virtual void OnOpen(object obj)
        {
            var eventService = Resolver.Resolve<IEventService>();
            eventService.GetEvent<OpenModelViewEvent>().Publish(this.model);
        }

        public override string Icon
        {
            get { return Constants.GRAPH_ICON_URL; }
        }

        public Guid ModelId
        {
            get { return this.model.Id; }
        }
    }
}
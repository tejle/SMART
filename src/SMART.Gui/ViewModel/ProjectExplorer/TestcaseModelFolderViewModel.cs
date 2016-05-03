using SMART.Core.Events;
using SMART.Core.Interfaces;

namespace SMART.Gui.ViewModel.ProjectExplorer
{
    using System;

    using Commands;

    using Core.Interfaces.Services;

    using Events;

    using IOC;

    public class TestcaseModelFolderViewModel : ViewModelBase
    {
        private readonly IModel model;
        public IModel Model { get{ return model;} }
        private readonly ITestcase testcase;

        public RoutedActionCommand Open { get;private set;}
        public RoutedActionCommand Remove{get;private set;}
        public RoutedActionCommand Rename{get;private set;}
		
        public TestcaseModelFolderViewModel(IModel model, ITestcase testcase): base(model.Name)
        {
            this.model = model;
            model.PropertyChanged += this.model_PropertyChanged;
            this.testcase = testcase;
            
            this.CreateCommand();
        }

        void model_PropertyChanged(object sender, SmartPropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Name"))
                this.Name = this.model.Name;
        }

        private void CreateCommand()
        {
            this.Open = new RoutedActionCommand("Open", typeof(TestcaseModelFolderViewModel))
                       {Description = "Open model", OnCanExecute = (o) => true, OnExecute = this.OnOpen, Text = "Open", Icon = Constants.OPEN_ICON_URL};
            this.Remove = new RoutedActionCommand("Remove", typeof(TestcaseModelFolderViewModel))
                         {Description = "Remove model", OnCanExecute = (o) => true, OnExecute = this.OnRemove, Text = "Remove", Icon = Constants.MODEL_REMOVE_ICON_URL};
            this.Rename = new RoutedActionCommand("Rename", typeof(TestcaseModelFolderViewModel))
                         {Description = "Rename model", OnCanExecute = (o) => true, OnExecute = this.OnRename, Text = "Rename", Icon = Constants.RENAME_ICON_URL};
        }
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
            get;
            set;
        }

        protected virtual void OnRename(object obj)
        {
            string name = string.Format("{0}-{1}", Name, DateTime.Now.Second);
            this.Name = name;
        }
        
        protected virtual void OnRemove(object obj)
        {
            testcase.Remove(model);
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
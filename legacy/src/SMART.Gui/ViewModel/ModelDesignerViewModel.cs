namespace SMART.Gui.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using Commands;
    using Core;
    using Core.Interfaces;
    using Core.Interfaces.Services;
    using Events;
    using IOC;

    public class ModelDesignerViewModel : ViewModelBase
    {
        // Paging
        private const int PAGE_SIZE = 10;
        private int startIndex;
        private int endIndex;

        private List<IModel> models;
        private readonly IEventService eventService;

        public RoutedActionCommand PreviousCommand { get; set; }
        public RoutedActionCommand NextCommand { get; set; }

        public override string Icon { get { return Constants.GRAPH_ICON_URL; } }

        public override Guid Id { get; set; }

        public InvokeOC<ModelViewModel> Models { get; set; }
        private List<ModelViewModel> internalModels;

        private ModelViewModel currentModel;
        public ModelViewModel CurrentModel
        {
            get { return currentModel; }
            set
            {
                if (currentModel != null)
                {
                    eventService.GetEvent<DeactivateModelEvent>().Publish(currentModel.Model);
                    currentModel.IsSelected = false;
                }
                currentModel = value;
                if (currentModel != null)
                {
                    currentModel.IsSelected = true;
                }
                SendPropertyChanged("CurrentModel");
                if (currentModel != null)
                {
                    eventService.GetEvent<ActivateModelEvent>().Publish(currentModel.Model);
                    eventService.GetEvent<MenuBarEvent>().Publish(currentModel);
                }
            }
        }

        public ModelDesignerViewModel(List<IModel> models, IModel initialModel)
        {
            eventService = Resolver.Resolve<IEventService>();

            PreviousCommand = new RoutedActionCommand("PreviousCommand", typeof(ModelDesignerViewModel))
                                  {
                                      Text = "Previous page",
                                      OnCanExecute = OnCanPreviousPage,
                                      OnExecute = OnPreviousPage
                                  };

            NextCommand = new RoutedActionCommand("NextCommand", typeof(ModelDesignerViewModel))
                                {
                                    Text = "Next page",
                                    OnCanExecute = OnCanNextPage,
                                    OnExecute = OnNextPage
                                };

            Models = new InvokeOC<ModelViewModel>(Application.Current.Dispatcher);
            internalModels = new List<ModelViewModel>();

            Init(models, initialModel);
        }

        private void OnNextPage(object obj)
        {
            startIndex = endIndex;
            endIndex += PAGE_SIZE -1;
            if (endIndex > models.Count -1)
            {
                startIndex -= endIndex - models.Count;
                endIndex = models.Count - 1;
            }
            AddModelsPage();
        }

        private bool OnCanNextPage(object obj)
        {            
            return endIndex < models.Count -1;
        }

        private void OnPreviousPage(object obj)
        {
            startIndex -= PAGE_SIZE;
            endIndex -= PAGE_SIZE;
            if (startIndex < 0)
            {
                startIndex = 0;
                endIndex = PAGE_SIZE - 1;
            }
            AddModelsPage();
        }

        private bool OnCanPreviousPage(object obj)
        {
            return startIndex > 0;
        }

        public void Init(List<IModel> m, IModel current)
        {
            models = m;
            internalModels = new List<ModelViewModel>();
            models.ForEach(model => internalModels.Add(new ModelViewModel(model)));

            LoadPage(current);

            CurrentModel = internalModels.Where(mo => mo.Id.Equals(current.Id)).FirstOrDefault();
            if (CurrentModel != null)
            {
                CurrentModel.IsFirstRun = false;
            }
            
        }

        private void LoadPage(IModel selected)
        {
            if (models.Count <= PAGE_SIZE)
            {
                Models.Clear();
                internalModels.ForEach(m => Models.Add(m));
                //models.ForEach(m => Models.Add(new ModelViewModel(m)));
                startIndex = 0;
                endIndex = models.Count;
            }
            else
            {
                var selectedIndex = models.IndexOf(selected);
                endIndex = selectedIndex + PAGE_SIZE / 2;                
                if (endIndex > models.Count - 1) endIndex = models.Count - 1;
                startIndex = endIndex - PAGE_SIZE + 1;
                if (startIndex < 0)
                {
                    endIndex += Math.Abs(startIndex);
                    startIndex = 0;
                }
                AddModelsPage();
            }
        }

        private void AddModelsPage()
        {
            Models.Clear();
            for (var i = startIndex; i < endIndex + 1; i++)
            {
                Models.Add(internalModels[i]);
            }
            //if (Models.Contains(CurrentModel))
            //{
            //    CurrentModel.IsSelected = true;
            //}
        }

        public void LastModel()
        {
            if (Models.Count == internalModels.Count)
            {
                CurrentModel = Models.Last();
            }
            else
            {
                endIndex = internalModels.Count - 1;
                startIndex = endIndex - PAGE_SIZE + 1;
                AddModelsPage();
                CurrentModel = internalModels.Last();
            }
        }

        public void PreviousModel()
        {
            if (Models.IndexOf(CurrentModel) == 0) // First?
            {
                if (internalModels.IndexOf(CurrentModel) > 0)
                {
                    OnPreviousPage(null);
                    CurrentModel = internalModels[internalModels.IndexOf(CurrentModel) - 1];
                }
                else
                {
                    LastModel();
                }
            }
            else
                CurrentModel = Models[Models.IndexOf(CurrentModel) - 1];
        }

        public void FirstModel()
        {
            if (Models.Count == internalModels.Count)
            {
                CurrentModel = Models.First();
            }
            else
            {
                startIndex = 0;
                endIndex = PAGE_SIZE - 1;
                AddModelsPage();
                CurrentModel = internalModels.First();
            }
        }

        public void NextModel()
        {
            if (Models.IndexOf(CurrentModel) == Models.Count - 1) // Last?
            {
                if (internalModels.IndexOf(CurrentModel) < internalModels.Count - 1)
                {                    
                    OnNextPage(null);
                    CurrentModel = internalModels[internalModels.IndexOf(CurrentModel) + 1];
                }
                else
                {
                    FirstModel();   
                }                
            }
            else
                CurrentModel = Models[Models.IndexOf(CurrentModel) + 1];
        }

        public override void ViewLoaded()
        {
            base.ViewLoaded();

            CurrentModel = currentModel;
        }
    }
}

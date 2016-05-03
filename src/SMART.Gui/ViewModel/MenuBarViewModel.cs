using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using SMART.Core.Interfaces.Services;
using SMART.Gui.Commands;
using SMART.Gui.Events;

namespace SMART.Gui.ViewModel
{
  public class MenuBarViewModel : ViewModelBase
  {
    private IViewModel project;
    private IViewModel scenario;
    private IViewModel model;

    public RoutedActionCommand MenuItemCommand { get; set; }

    private readonly ApplicationViewModel applicationViewModel;

    public MenuBarViewModel(ApplicationViewModel applicationViewModel, IEventService eventService)
      : base(String.Empty)
    {
      this.applicationViewModel = applicationViewModel;
      MenuItems = new ObservableCollection<IViewModel>();
      CreateCommands();
      eventService.GetEvent<MenuBarEvent>().Subscribe(OnMenuBarEvent);
    }

    private void CreateCommands()
    {
      MenuItemCommand = new RoutedActionCommand("MenuItemCommand", typeof(MenuBarViewModel))
      {       
        OnCanExecute = e => true,
        OnExecute = OnMenuItemExecute
      };
    }

    private void OnMenuItemExecute(object menuItem)
    {
      if (menuItem is HomeMenuViewModel)
      {
        applicationViewModel.ShowHomeScreen();
      }
      else if (menuItem is ProjectViewModel)
      {
        applicationViewModel.SetMainContent(Project);
          ((ProjectViewModel) Project).CurrentProject = ((ProjectViewModel) Project).Projects[0];
      }
      else if (menuItem is ProjectScenarioViewModel)
      {          
          applicationViewModel.SetMainContent(Project);
          ((ProjectViewModel)Project).CurrentScenario = menuItem as ProjectScenarioViewModel;
      }
      else if (menuItem is ProjectModelViewModel)
      {
        
      }
    }

    public IViewModel Project
    {
      get { return project; }
      set
      {
        project = value; SendPropertyChanged("Project");
      }
    }

    public IViewModel Scenario
    {
      get { return scenario; }
      set { scenario = value; SendPropertyChanged("Scenario"); }
    }

    public IViewModel Model
    {
      get { return model; }
      set { model = value; SendPropertyChanged("Model"); }
    }

    public ObservableCollection<IViewModel> MenuItems { get; set; }

    private void OnMenuBarEvent(IViewModel viewModel)
    {
      if (viewModel is ProjectViewModel)
      {
        Project = viewModel;
        Model = null;
        Scenario = null;
      }
      else if (viewModel is ProjectScenarioViewModel)
      {
        Scenario = viewModel;
        Model = null;
      }
      else if (viewModel is ModelViewModel)
      {
        Model = viewModel;
      }

      UpdateMenuItems();
    }

    private void UpdateMenuItems()
    {
      MenuItems.Clear();
      MenuItems.Add(new HomeMenuViewModel());
      MenuItems.Add(new SeparatorMenuViewModel());
      MenuItems.Add(Project);
      if (scenario != null)
      {
        MenuItems.Add(new SeparatorMenuViewModel());
        MenuItems.Add(Scenario);
      }
      if (model != null)
      {
        MenuItems.Add(new SeparatorMenuViewModel());
        MenuItems.Add(Model);
      }
    }

    public override string Icon
    {
      get { return Constants.MISSING_ICON_URL; }

    }

    public override Guid Id
    {
      get;
      set;

    }
  }

  public class MenuBarEvent : SmartUIEvent<IViewModel>
  {
  }

  public class HomeMenuViewModel : ViewModelBase
  {
    public override string Icon
    {
      get { return Constants.MISSING_ICON_URL; }
    }

    public override Guid Id
    {
      get;
      set;
    }

    public HomeMenuViewModel()
      : base("Home")
    {

    }
  }

  public class SeparatorMenuViewModel : ViewModelBase
  {
    public override string Icon
    {
      get { return Constants.MISSING_ICON_URL; }
    }

    public override Guid Id
    {
      get;
      set;
    }
  }

  public class SeparatorStyleSelector : StyleSelector
  {
    public override Style SelectStyle(object item, DependencyObject container)
    {
      if (item is SeparatorMenuViewModel)
      {
        return (Style)((FrameworkElement)container).FindResource("separatorStyle");
      }
      return null;
    }
  }

}
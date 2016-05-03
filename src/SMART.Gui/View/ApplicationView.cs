using SMART.Core.DomainModel;
using SMART.Core.Interfaces;

namespace SMART.Gui.View
{
    using System;
    using System.Collections.Generic;
    using System.Windows;

    using Commands;

    using ProjectExplorer;

    using Syncfusion.Windows.Tools.Controls;

    using ViewModel;
    using IOC;

    using Core.Interfaces.Services;
    using Events;
    using System.Windows.Controls;
    

    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class ApplicationView
    {
        private const string SmartProjectFileDefaultExt = ".smart";
        private readonly string SmartProjectFileFilter = string.Format("SMART Project Files ({0})|*{0}", SmartProjectFileDefaultExt);

        public ApplicationViewModel ViewModel { get; private set; }

        public ApplicationView()
        {
            
        }

        public ApplicationView(ApplicationViewModel viewModel, IEventService eventService)
        {
            InitializeComponent();

            DataContext = ViewModel = viewModel;
            eventService.GetEvent<OpenModelViewEvent>().Subscribe(handleOpenModelViewEvent);
            eventService.GetEvent<OpenTestcaseViewEvent>().Subscribe(handleOpenTestcaseViewEvent);
            eventService.GetEvent<OpenProjectEvent>().Subscribe(handleOpenProjectEvent);
            eventService.GetEvent<SaveProjectEvent>().Subscribe(handleSaveProjectEvent);

            RoutedActionCommand.Bind(this);

            var projectExplorerView = new ProjectExplorerView() { DataContext = viewModel.ProjectExplorerViewModel };

            appDock.Children.Add(projectExplorerView);

            //listening to ReturnValueChanged event and setting the toggle button state correspondingly
            SmartCommands.ZoomRect.ReturnValueChanged += (sender, e) =>
            {
                zoomRectButton.IsChecked = (bool)SmartCommands.ZoomRect.ReturnValue;
            };
            
        }

        private void handleSaveProjectEvent(string file)
        {
            RecentFiles.InsertFile(file);
        }

        private void handleOpenProjectEvent(string file)
        {
            RecentFiles.InsertFile(file);
            var childrenToRemove = new List<FrameworkElement>();

            foreach (FrameworkElement child in appDock.Children)
            {
                var vmModel = child.DataContext as ModelViewModel;
                if (vmModel != null)
                {
                    childrenToRemove.Add(child);                    
                }
                //var vmTestcase = child.DataContext as TestcaseViewModel;
                //if (vmTestcase != null)
                //{
                //    childrenToRemove.Add(child);
                //}
            }
            foreach (var childToRemove in childrenToRemove)
            {
                childToRemove.DataContext = null;                  
                appDock.Children.Remove(childToRemove);                
            }
        }

        private void handleOpenTestcaseViewEvent(ITestcase testcase)
        {
            //appDock.Children.Add(new TestcaseView(new TestcaseViewModel(testcase)));
        }

        private void handleOpenModelViewEvent(IModel model)
        {
            FrameworkElement childWindow = null;

            foreach (FrameworkElement child in appDock.Children)
            {
                var vm = child.DataContext as ModelViewModel;

                if (vm != null && vm.Id == model.Id)
                {
                    childWindow = child;
                    break;
                }
            }

            if (childWindow == null)
            {
                appDock.Children.Add(new ModelView(new ModelViewModel(model as Model, Resolver.Resolve<IModelService>())));
            }
            else
            {
                DockingManager.SetState(childWindow, DockState.Document);
                appDock.ActiveWindow = childWindow;
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

    }

}

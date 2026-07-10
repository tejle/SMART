using System.Collections.Generic;
using System.Linq;
using SMART.Core.Interfaces;
using SMART.Gui.ViewModel.TestcaseCodeGeneration;
using SMART.Gui.ViewModel.TestcaseConfiguration;
using SMART.Gui.ViewModel.TestcaseExecution;
using SMART.IOC;

namespace SMART.Gui.ViewModel
{
    public interface IViewModelFactory
    {
        IViewModel CreateTitleBar();
        IViewModel CreateStartViewModel();
        IViewModel CreateProjectViewModel(IProject project);
        IViewModel CreateMenuBar();
        IViewModel CreateTestcaseConfiguration(ITestcase testcase);
        IViewModel CreateCodeGeneration(IProject project, ITestcase testcase);
        IViewModel CreateGenerateAndExecute(IProject project, ITestcase testcase);
        IViewModel CreateModelDesigner(List<IModel> models, IModel currentModel);
        void ClearProjectViewModels();
    }

    public class ViewModelFactory : IViewModelFactory
    {
        private readonly List<IViewModel> viewModels;
        private ViewModelFactory()
        {
            viewModels = new List<IViewModel>();
        }

        public static ViewModelFactory Create()
        {
            return new ViewModelFactory();    
        }

        public IViewModel CreateTitleBar()
        {
            var tmp = viewModels.Find(v=> v is TitleBarViewModel);
            if(tmp ==null)
            {
                tmp = new TitleBarViewModel();
                viewModels.Add(tmp);
            }
            return tmp;
        }

        public IViewModel CreateStartViewModel()
        {
            var tmp = viewModels.Find(v => v is StartViewModel);
            if(tmp == null)
            {
                tmp = new StartViewModel();
                viewModels.Add(tmp);
            }

            return tmp;
        }

        public IViewModel CreateProjectViewModel(IProject project)
        {
            var tmp = viewModels.Find(v => v is ProjectViewModel && v.Id == project.Id) as ProjectViewModel;
            
            if (tmp == null)
            {

                tmp = new ProjectViewModel(project);
                viewModels.Add(tmp);
            }

            return tmp;
        }

        public IViewModel CreateMenuBar()
        {
            var tmp = viewModels.Find(v => v is MenuBarViewModel);
            if (tmp == null)
            {
                tmp = Resolver.Resolve<MenuBarViewModel>();
                viewModels.Add(tmp);
            }
            return tmp;
        }

        public IViewModel CreateTestcaseConfiguration(ITestcase testcase)
        {
            var tmp = viewModels.Find(v => v is TestcaseConfigurationCompositeViewModel && v.Id == testcase.Id);
            if(tmp == null)
            {
                tmp = new TestcaseConfigurationCompositeViewModel(testcase);
                viewModels.Add(tmp);
            }
            return tmp;
        }

        public IViewModel CreateCodeGeneration(IProject project, ITestcase testcase)
        {
            var tmp = viewModels.Find(v => v is TestcaseCodeGenerationViewModel && v.Id == testcase.Id);
            if (tmp == null)
            {
                tmp = new TestcaseCodeGenerationViewModel(project, testcase);
                viewModels.Add(tmp);
            }
            return tmp;
        }

        public IViewModel CreateGenerateAndExecute(IProject project, ITestcase testcase)
        {
            var tmp = viewModels.OfType<TestcaseExecutionCompositeViewModel>().FirstOrDefault(); //.Find(v => v is TestcaseExecutionCompositeViewModel && v.Id == testcase.Id);
            if (tmp == null)
            {
                tmp = new TestcaseExecutionCompositeViewModel(project, testcase);
                viewModels.Add(tmp);
            }
            else
            {
                tmp.Init(project, testcase);
            }
            return tmp;
        }

        public IViewModel CreateModelDesigner(List<IModel> models, IModel currentModel)
        {
            //var tmp = viewModels.OfType<ModelDesignerViewModel>().ToList().Find(v => v.Models.Equals(models) && v.CurrentModel.Equals(currentModel));
            var tmp = viewModels.OfType<ModelDesignerViewModel>().FirstOrDefault();
            if (tmp == null)
            {
                tmp = new ModelDesignerViewModel(models, currentModel);
                viewModels.Add(tmp);
            }
            else
            {
                tmp.Init(models, currentModel);
            }
            return tmp;
        }

        public void ClearProjectViewModels()
        {
            viewModels.RemoveAll(v => v is ProjectViewModel);
        }
    }
}
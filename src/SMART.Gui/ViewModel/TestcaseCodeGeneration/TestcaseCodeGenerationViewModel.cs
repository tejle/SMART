namespace SMART.Gui.ViewModel.TestcaseCodeGeneration
{
    using System;
    using Commands;
    using Core.Interfaces;
    using Core.Interfaces.Services;

    using Events;
    using SMART.IOC;
    using Microsoft.Win32;
    using System.Windows;
    using SMART.Core.Services;

    public class TestcaseCodeGenerationViewModel : ViewModelBase
    {
        private readonly IProject project;
        private readonly ITestcase Testcase;

        private readonly IEventService EventService;        

        public override string Icon
        {
            get { return Constants.MISSING_ICON_URL; }
        }

        public override Guid Id
        {
            get { return Testcase.Id; }
            set { Testcase.Id = value; }
        }

        private TestcaseCommonCommandsViewModel commonCommands;

        public TestcaseCommonCommandsViewModel CommonCommands
        {
            get { return this.commonCommands; }
            set { this.commonCommands = value; this.SendPropertyChanged("CommonCommands"); }
        }

        //public RoutedActionCommand Close { get; set; }
        public RoutedActionCommand SaveCode { get; set; }
        public RoutedActionCommand CopyCode { get; set; }

        private string code;

        public string Code
        {
            get { return this.code; }
            set { this.code = value; this.SendPropertyChanged("Code"); }
        }

        public TestcaseCodeGenerationViewModel(IProject project, ITestcase testcase)
            : this(project, testcase, Resolver.Resolve<IEventService>())
        {
        }
        public TestcaseCodeGenerationViewModel(IProject project, ITestcase testcase, IEventService eventService) : base(testcase.Name)
        {
            this.project = project;
            Testcase = testcase;
            EventService = eventService;

            CommonCommands = new TestcaseCommonCommandsViewModel(this, Testcase)
                                 {
                                         //CodeButtonChecked = true
                                 };

            this.SaveCode = new RoutedActionCommand("SaveCode", typeof(TestcaseCodeGenerationViewModel))
            {
                Description = "Save the code to file",
                OnCanExecute = (o) => true,
                OnExecute = this.OnSaveCode
            };

            this.CopyCode = new RoutedActionCommand("CopyCode", typeof(TestcaseCodeGenerationViewModel))
            {
                Description = "Copy the code to the clipboard",
                OnCanExecute = (o) => true,
                OnExecute = this.OnCopyCode
            };

            var codeService = Resolver.Resolve<ICodeGenerationService>();

            Code = codeService.GetCodeAsString(project, testcase);
                   //"using System.Linq;" + Environment.NewLine + Environment.NewLine + 
                   //"namespace SMART.Base" +
                   //Environment.NewLine + 
                   //"{" + Environment.NewLine + 
                   //"public class TestCaseCode" + Environment.NewLine+
                   //"{" + Environment.NewLine + 
                   //"// This is a comment" + Environment.NewLine +
                   //"public int theNumber = 1;" + Environment.NewLine + 
                   //"internal double anotherNumber = 325;" +
                   //Environment.NewLine + Environment.NewLine + 
                   //"private string theString = \"this is a string\";" + Environment.NewLine +
                   //"}" + Environment.NewLine +                    
                   //"}" ;
        }

        private void OnCopyCode(object obj)
        {
            Clipboard.Clear();
            Clipboard.SetText(code);
        }

        private void OnSaveCode(object obj)
        {
            var sfd = new SaveFileDialog();
            sfd.ShowDialog();
        }

        public override void ViewLoaded()
        {
            commonCommands.ConfigButtonChecked = false;
            commonCommands.ExecuteButtonChecked = false;
            commonCommands.CodeButtonChecked = true;

        }
    }
}

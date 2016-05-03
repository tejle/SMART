using System;
using SMART.Gui.Commands;
using SMART.IOC;

namespace SMART.Gui.ViewModel
{
    public class TitleBarViewModel : ViewModelBase
    {
        public override string Icon { get { return Constants.MISSING_ICON_URL; } }

        public override Guid Id { get; set; }

        public RoutedActionCommand SaveProject { get; set; }
        public RoutedActionCommand SaveProjectAs { get; set; }

        public TitleBarViewModel()
        {
            SaveProject = Resolver.Resolve<ApplicationViewModel>().SaveProject;
            SaveProjectAs = Resolver.Resolve<ApplicationViewModel>().SaveAsProject;
        }
    }
}
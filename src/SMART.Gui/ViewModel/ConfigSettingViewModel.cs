
namespace SMART.Gui.ViewModel
{
    using System;

    using Commands;
    using Core;
    using Core.DataLayer.Interfaces;
    using Core.Metadata;

    public class ConfigSettingViewModel : ViewModelBase
    {
        private readonly IConfigSetting ConfigSetting;

        private readonly object Target;

        public override string Icon { get { return Constants.MISSING_ICON_URL; } }

        public override string Name
        {
            get
            {
                return ConfigSetting.Name;
            }
            set
            {
                ConfigSetting.Name = value;
            }
        }

        public override Guid Id
        {
            get;
            set;
        }

        public RoutedActionCommand EditSetting { get; set; }
        public RoutedActionCommand CloseEditor { get; set; }

        private bool showConfigSettingEditor;

        public bool ShowConfigSettingEditor
        {
            get { return this.showConfigSettingEditor; }
            set { this.showConfigSettingEditor = value; this.SendPropertyChanged("ShowConfigSettingEditor"); }
        }

        public string Description { get { return ConfigSetting.Config.Description; } }

        public object Default { get { return ConfigSetting.Config.Default; } }

        public Type Type { get { return ConfigSetting.Type; } }

        public ConfigEditor Editor { get { return ConfigSetting.Config.Editor; } }

        public object Value { 
            get { return ConfigSetting.Value; } 
            set
            {
                if (ConfigSetting.Value != null && ConfigSetting.Value.Equals(value)) return;
                ConfigSetting.Value = value;
                Target.SetConfig(ConfigSetting);
                SendPropertyChanged("Value");
            } 
        }

        public ConfigSettingViewModel(IConfigSetting configSetting, object target)
        {
            ConfigSetting = configSetting;
            Target = target;

            EditSetting = new RoutedActionCommand("EditSetting", typeof(ConfigSettingViewModel))
                              {
                                      OnCanExecute = OnCanEditSetting, 
                                      OnExecute = OnEditSetting
                              };
            CloseEditor = new RoutedActionCommand("CloseEditor", typeof(ConfigSettingViewModel))
            {
                OnCanExecute = OnCanCloseEditor,
                OnExecute = OnCloseEditor
            };
        }

        private void OnCloseEditor(object obj)
        {
            ShowConfigSettingEditor = false;
        }

        private bool OnCanCloseEditor(object obj)
        {
            return true;
        }

        private void OnEditSetting(object obj)
        {
            ShowConfigSettingEditor = true;
        }

        private bool OnCanEditSetting(object obj)
        {
            return true;
        }
    }
}
using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SMART.Core.DomainModel;
using SMART.Core.Interfaces;
using SMART.Gui.Commands;
using SMART.IOC;

namespace SMART.Gui.ViewModel
{
    public class ProjectModelViewModel : ViewModelBase, IEditableViewModel, IDisposable
    {
        private readonly IModel model;
        private readonly IProject project;
        private readonly ITestcase scenario;
        private bool isEditMode = false;

        public RoutedActionCommand Open { get; private set; }
        public RoutedActionCommand Remove { get; private set; }
        public RoutedActionCommand Rename { get; private set; }


        public override string Icon
        {
            get { return Constants.GRAPH_ICON_URL; }
        }

        public override Guid Id
        {
            get { return model.Id; }
            set { model.Id = value; SendPropertyChanged("Id"); }
        }

        public bool IsEditMode
        {
            get { return isEditMode; }
            set { isEditMode = value;
                StartInEditMode = false; SendPropertyChanged("IsEditMode"); }
        }

        public bool StartInEditMode
        {
            get; set;
        }

        public IModel Model
        {
            get
            {
                return model;
            }
        }

        public override string Name
        {
            get
            {
                return base.Name;
            }
            set
            {
                if (base.Name.Equals(value)) return;

                base.Name = value;
                if (!model.Name.Equals(value))
                    model.Name = value;
            }
        }

        public Image Thumbnail
        {
          get {
            return LoadImage();}
        }

        private Image LoadImage()
        {
          if (model.Thumbnail != null && model.Thumbnail.Image != null)
          {
            var stream = new MemoryStream(model.Thumbnail.Image);
            var decoder = new PngBitmapDecoder(stream, BitmapCreateOptions.None, BitmapCacheOption.Default);
            var img = new Image {Source = decoder.Frames[0], SnapsToDevicePixels = true };
            return img;
          }
          return null;
        }


        public ProjectModelViewModel(IModel model, IProject project, ITestcase scenario)
            : base(model.Name)
        {
            this.model = model;
            this.project = project;
            this.scenario = scenario;

            model.PropertyChanged += model_PropertyChanged;
            
            CreateCommands();
        }
        

        public void Dispose()
        {
            model.PropertyChanged -= model_PropertyChanged;
        }

        private void CreateCommands()
        {
            Open = new RoutedActionCommand("Open", typeof(ProjectModelViewModel))
                            {
                                Description = "Open model",
                                OnCanExecute = (o) => true,
                                OnExecute = OnOpen,
                                Text = "Open",
                                Icon = Constants.OPEN_ICON_URL
                            };
            Remove = new RoutedActionCommand("Remove", typeof(ProjectModelViewModel))
                              {
                                  Description = "Remove model",
                                  OnCanExecute = (o) => true,
                                  OnExecute = OnRemove,
                                  Text = "Remove",
                                  Icon = Constants.MODEL_REMOVE_ICON_URL
                              };
            Rename = new RoutedActionCommand("Rename", typeof(ProjectModelViewModel))
                              {
                                  Description = "Rename model",
                                  OnCanExecute = (o) => true,
                                  OnExecute = OnRename,
                                  Text = "Rename",
                                  Icon = Constants.RENAME_ICON_URL
                              };

        }

        private void OnRename(object obj)
        {
            IsEditMode = true;
        }

        private void OnRemove(object obj)
        {
            if (scenario != null)
                scenario.Remove(model);
            else
                project.RemoveModel(model);

        }

        private void OnOpen(object obj)
        {

        }

        void model_PropertyChanged(object sender, Core.Events.SmartPropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Name"))
                Name = model.Name;
        }
    }
}
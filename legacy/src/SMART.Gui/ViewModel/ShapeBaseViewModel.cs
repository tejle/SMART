namespace SMART.Gui.ViewModel
{
    using System;
    using System.Windows.Controls;

    using Commands;

    using Controls.DiagramControl;
    using Controls.DiagramControl.Shapes;
    using System.Diagnostics;

    public class ShapeBaseViewModel : ViewModelBase, IDiagramItem, ISelectable, IEditable
    {
        private bool selected;
        private bool isDimmed;
        private bool isDefect;
        private bool isCurrent;
        private int visitCount;
        private bool isInEditMode;
        private bool showVisitCounter;

        public override string Icon { get { return Constants.MISSING_ICON_URL; } }

        public override Guid Id
        {
            get;
            set;
        }

        public override ContentControl View { get; set; }

        public RoutedActionCommand Rename { get; set; }
        public RoutedActionCommand Delete { get; set; }

        public bool StartInEditMode { get; set; }

        public bool ShowVisitCounter
        {
            get { return this.showVisitCounter; }
            set { this.showVisitCounter = value; this.SendPropertyChanged("ShowVisitCounter"); }
        }

        public bool Selected
        {
            get { return this.selected; }
            set { this.selected = value; this.SendPropertyChanged("Selected"); }
        }


        public bool IsDimmed
        {
            get { return this.isDimmed; }
            set { this.isDimmed = value; SendPropertyChanged("IsDimmed"); }
        }

        public bool IsCurrent
        {
            get { return this.isCurrent; }
            set { isCurrent = value; this.SendPropertyChanged("IsCurrent"); }
        }


        public bool IsDefect
        {
            get { return this.isDefect; }
            set { isDefect = value; this.SendPropertyChanged("IsDefect"); }
        }

        public bool IsVisited
        {
            get { return visitCount > 0; }
        }

        public int VisitCount
        {
            get { return this.visitCount; }
            set
            {
                if (visitCount == -1 && value != 0)
                {
                    visitCount = 1;
                }
                else
                {
                    this.visitCount = value;

                }
                
                this.SendPropertyChanged("VisitCount");
                this.SendPropertyChanged("IsVisited");
            }
        }

        public string OldText { get; set; }

        public bool IsInEditMode
        {
            get { return isInEditMode; }
            set
            {
                isInEditMode = value; if (value == true)
                {
                    OldText = this.Name; // Save the text when entering editmode
                }
                SendPropertyChanged("IsInEditMode");
            }
        }

        public ShapeBaseViewModel() : this("")
        {            
        }

        public ShapeBaseViewModel(string name)
            : base(name)
        {
            VisitCount = -1;

            Rename = new RoutedActionCommand("Rename", typeof(StateViewModel))
            {
                Description = "Rename the item",
                OnCanExecute = OnCanRename,
                OnExecute = OnRename,
                Text = "Rename",
                Icon = Constants.RENAME_ICON_URL
            };

            Delete = new RoutedActionCommand("Delete", typeof(ModelViewModel))
            {
                Description = "Delete the item",
                OnCanExecute = OnCanDelete,
                OnExecute = OnDelete,
                Text = "Delete",
                Icon = Constants.DELETE_ICON_URL
            }; 
        }

        public virtual bool OnCanDelete(object obj)
        {
            return true;
        }

        public virtual bool OnCanRename(object obj)
        {
            return true;
        }

        public virtual void OnDelete(object obj)
        {            
        }

        public virtual void OnRename(object obj)
        {
            if (obj is IEditable)
            {
                (obj as IEditable).IsInEditMode = true;
            }
        }

        public void Refresh()
        {

        }

        public void Select()
        {
            if (this.Selected) return;
            Selected = true;
        }

        public void Unselect()
        {
            if (this.Selected)
            {
                this.Selected = false;
            }
        }

    }
}

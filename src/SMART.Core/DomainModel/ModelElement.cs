using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMART.Core.Interfaces;
using SMART.Core.Metadata;

namespace SMART.Core.DomainModel
{
    public abstract class ModelElement : SmartEntityBase, IModelElement, ITaggable
    {
        private bool isCurrent;
        private bool isDefect;
        private string label;
        private int internalVisitCounter;
        private Guid id;

        public int VisitCount
        {
            get { return internalVisitCounter; }
            set
            {
                internalVisitCounter = value;
                OnPropertyChanged("VisitCount");
            }
        }

        public bool IsCurrent
        {
            get { return isCurrent; }
            set { isCurrent = value; OnPropertyChanged("IsCurrent"); }
        }

        public bool IsDefect
        {
            get { return isDefect; }
            set { isDefect = value; OnPropertyChanged("IsDefect"); }
        }

        [Config]
        public string Label
        {
            get { return label; }
            set { label = value; OnPropertyChanged("Label"); }
        }

        public Guid Id
        {
            get { return id; }
            set { id = value; OnPropertyChanged("Id"); }
        }

        protected ModelElement(string label, Guid id)
        {
            this.label = label;
            this.id = id;
            internalVisitCounter = 0;
        }

        public virtual void Visit()
        {
            VisitCount++;
        }

        public void Reset()
        {
            VisitCount = 0;
            

            IsDefect = false;
            IsCurrent = false;
        }

        private Dictionary<string, object> tags = new Dictionary<string, object>();

        public Dictionary<string, object> Tags
        {
            get { return tags; }
            set { tags = value; }
        }
    }
}

using System;
using SMART.Core.Interfaces;

namespace SMART.Gui.ViewModel
{
  public class ModelInfoViewModel : ViewModelBase
  {
    private readonly IModel model;

    public override string Icon
    {
      get { return Constants.MISSING_ICON_URL; }
    }

    public override Guid Id
    {
      get { return model.Id; }
      set { model.Id = value; }
    }

    public int TransitionCount
    {
      get { return model.Transitions.Count; }
    }

    public int StateCount
    {
      get { return model.States.Count; }
    }

    public DateTime Created
    {
        get { return DateTime.Now.AddDays(-1).AddHours(-2); }
    }

    public DateTime Modified
    {
      get { return DateTime.Now; }
    }

    public ModelInfoViewModel(IModel model) : base(model.Name)
    {
      this.model = model;
    }
  }
}

namespace SMART.Core.Interfaces.Services
{
    using SMART.Core.DomainModel;

    public interface IModelService
    {
        Model CreateModel(string name);

        State AddState(Model model);

        Transition AddTransition(Model model);

        bool AddExistingState(Model model, State state);
        bool AddExistingTransition(Model model, Transition transition);

        bool RemoveTransition(Model model, Transition transition);
        bool RemoveState(Model model, State state);
    }
}
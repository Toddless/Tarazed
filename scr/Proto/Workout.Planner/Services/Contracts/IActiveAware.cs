namespace Workout.Planner.Services.Contracts
{
    public interface IActiveAware
    {
        void Activated();

        void Deactivated();
    }
}

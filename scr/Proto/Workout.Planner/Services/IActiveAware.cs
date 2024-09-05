namespace Workout.Planner.Services
{
    public interface IActiveAware
    {
        void Activated();

        void Deactivated();
    }
}

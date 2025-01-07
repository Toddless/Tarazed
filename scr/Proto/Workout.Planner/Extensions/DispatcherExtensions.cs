namespace Workout.Planner.Extensions
{
    public static class DispatcherExtensions
    {
        public static Task DispatchToUIAsync(this IDispatcher dispatcher, Func<Task> action)
        {
            if (action == null || dispatcher == null)
            {
                throw new ArgumentNullException();
            }

            if (dispatcher.IsDispatchRequired)
            {
                return dispatcher.DispatchAsync(action);
            }

            return action.Invoke();
        }
    }
}

namespace Workout.Planner.Models
{
    using System.Windows.Input;

    public class NavigationEntry : BindableObject
    {
        public string? Title { get; set; }

        public string? Icon { get; set; }

        public ICommand? Command { get; set; }

        public string? CommandParameter { get; set; }
    }
}

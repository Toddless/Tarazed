namespace Workout.Planner.Models
{
    using DataModel;

    public class UnitsModel : ObservableObject
    {
        private Unit _unit;

        private string _unitName;

        private UnitsModel()
        {
        }

        public string UnitName
        {
            get => _unitName;
            set => SetProperty(ref _unitName, value);
        }

        public static UnitsModel Import(Unit unit)
        {
            return new UnitsModel() { _unit = unit, UnitName = unit.Name };
        }
    }
}

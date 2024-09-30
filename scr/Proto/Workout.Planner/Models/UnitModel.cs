namespace Workout.Planner.Models
{
    using DataModel;

    public class UnitModel : ObservableObject
    {
        private Unit _unit;
        private long _id;
        private string _unitName;

        private UnitModel()
        {
        }

        public Unit Unit
        {
            get => _unit;
            private set => SetProperty(ref _unit, value);
        }

        public long Id
        {
            get => _id;
            private set => SetProperty(ref _id, value);
        }

        public string UnitName
        {
            get => _unitName;
            set => SetProperty(ref _unitName, value);
        }

        public static UnitModel Import(Unit unit)
        {
            return new UnitModel() { Id = unit.Id, UnitName = unit.Name, Unit = unit };
        }

        public static IEnumerable<UnitModel> Import(IEnumerable<Unit>? units)
        {
            if (units != null)
            {
                foreach (var item in units)
                {
                    yield return new UnitModel() { Id = item.Id, UnitName = item.Name, Unit = item };
                }
            }
        }
    }
}

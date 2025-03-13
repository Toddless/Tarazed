namespace Workout.Planner.Services
{
    using DataModel;
    using Workout.Planner.Services.Contracts;

    public class UnitService : UserDataBaseService<Unit>, IUnitService
    {
        public UnitService([FromKeyedServices("AuthRestAPI")] IRestApiService restApiService)
            : base(restApiService)
        {
        }
    }
}

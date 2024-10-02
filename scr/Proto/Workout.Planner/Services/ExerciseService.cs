namespace Workout.Planner.Services
{
    using DataModel;
    using Workout.Planner.Services.Contracts;

    public class ExerciseService : UserDataBaseService<Exercise>, IExerciseService
    {
        public ExerciseService([FromKeyedServices("AuthRestAPI")] IRestApiService restApiService)
            : base(restApiService)
        {
        }
    }
}

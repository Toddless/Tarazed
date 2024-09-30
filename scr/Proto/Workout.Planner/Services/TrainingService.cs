namespace Workout.Planner.Services
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using DataModel;
    using Microsoft.Extensions.DependencyInjection;
    using Workout.Planner.Services.Contracts;

    public class TrainingService : UserDataBaseService<TrainingPlan>, ITrainingService
    {
        public TrainingService([FromKeyedServices("AuthRestAPI")] IRestApiService restApiService)
            : base(restApiService)
        {
        }

        public override Task<IEnumerable<TrainingPlan>> GetDataAsync(bool additionalData, CancellationToken token, IEnumerable<long>? ids = null) => base.GetDataAsync(additionalData, token, ids);

        public override Task<TrainingPlan> CreateDataAsync(TrainingPlan item, CancellationToken token) => base.CreateDataAsync(item, token);

        public override Task<bool> DeleteDataAsync(IEnumerable<long>? ids, CancellationToken token) => base.DeleteDataAsync(ids, token);

        public override Task<TrainingPlan> UpdataDataAsync(TrainingPlan item, CancellationToken token) => base.UpdataDataAsync(item, token);
    }
}

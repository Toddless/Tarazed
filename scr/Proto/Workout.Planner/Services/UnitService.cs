namespace Workout.Planner.Services
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using DataModel;
    using Workout.Planner.Services.Contracts;

    public class UnitService : UserDataBaseService<Unit>, IUnitService
    {
        public UnitService([FromKeyedServices("AuthRestAPI")] IRestApiService restApiService)
            : base(restApiService)
        {
        }

        public override Task<IEnumerable<Unit>> GetDataAsync(bool additionalData, CancellationToken token, IEnumerable<long>? ids = null) => base.GetDataAsync(additionalData, token, ids);

        public override Task<Unit> CreateDataAsync(Unit item, CancellationToken token) => base.CreateDataAsync(item, token);

        public override Task<Unit> UpdataDataAsync(Unit item, CancellationToken token) => base.UpdataDataAsync(item, token);

        public override Task<bool> DeleteDataAsync(IEnumerable<long>? ids, CancellationToken token) => base.DeleteDataAsync(ids, token);
    }
}

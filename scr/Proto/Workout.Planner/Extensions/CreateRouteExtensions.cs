namespace Workout.Planner.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using DataModel;

    public static class CreateRouteExtensions
    {
        public static string CreateStringRoute(IEnumerable<long>? ids, string route, bool loadAdditionalData)
        {
            if (string.IsNullOrWhiteSpace(route))
            {
                throw new Exception();
            }

            var builder = new StringBuilder(route);
            builder.Append('?');
            if (ids != null)
            {
                builder.Append(string.Join('&', ids!.Select(x => $"{nameof(ids)}={x}")));
            }

            builder.Append($"{RouteParameter.LoadAdditionalData}" + '=' + loadAdditionalData);
            return builder.ToString();
        }
    }
}

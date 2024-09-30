namespace Workout.Planner.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Web;
    using DataModel;

    public static class CreateRouteExtensions
    {
        public static string CreateGetStringRoute(IEnumerable<long>? ids, string route, bool loadAdditionalData)
        {
            if (string.IsNullOrWhiteSpace(route))
            {
                throw new Exception();
            }

            var builder = new StringBuilder(route);
            if (ids != null)
            {
                builder.Append(string.Join('&', ids!.Select(x => $"{nameof(ids)}={x}")));
                builder.Append('&');
            }

            builder.Append($"{RouteParameter.LoadAdditionalData}" + '=' + loadAdditionalData);
            return builder.ToString();
        }

        public static string CreateDeleteStringRoute(IEnumerable<long>? id, string route)
        {
            if (string.IsNullOrWhiteSpace(route))
            {
                throw new Exception();
            }

            var builder = new StringBuilder(route);

            // obwohl der name "id" ist, könnte sein dass man mehrere elemente gelöscht werden muss,
            // die methode kann passende route dafür generieren, aber die name muss "id" stehen bleiben.
            if (id != null)
            {
                builder.Append(string.Join('&', id!.Select(x => $"{nameof(id)}={x}")));
            }

            return builder.ToString();
        }

        public static string ObjToQuery(object obj, string route)
        {
            // die methode kriegt alle property des objectes und generiert daraus route für den server.
            var properties = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            var queryString = HttpUtility.ParseQueryString(string.Empty);
            var builder = new StringBuilder(route);
            foreach (var property in properties)
            {
                var value = property.GetValue(obj);
                if (value == null)
                {
                    continue;
                }

                queryString.Add(property.Name, value.ToString());
            }

            builder.Append(queryString.ToString());
            return builder.ToString();
        }
    }
}

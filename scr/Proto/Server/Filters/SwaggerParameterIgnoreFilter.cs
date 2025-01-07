namespace Server.Filters
{
    using System.Reflection;
    using DataModel.Attributes;

    public class SwaggerParameterIgnoreFilter : Swashbuckle.AspNetCore.SwaggerGen.IOperationFilter
    {
        public void Apply(Microsoft.OpenApi.Models.OpenApiOperation operation, Swashbuckle.AspNetCore.SwaggerGen.OperationFilterContext context)
        {
            if (operation == null || context == null || context.ApiDescription?.ParameterDescriptions == null)
            {
                return;
            }

            var parameters = context.ApiDescription.ActionDescriptor.Parameters;

            if (parameters.Count != 1)
            {
                // zur Implementierungzeit haben die Controller nur ein parameter zum übergeben,
                // behandlung des Controllers mit mehreren parametern sind nicht vorgesehen.
                return;
            }

            var properties = parameters.Single().ParameterType.GetProperties();

            var propertiesToRemove = new List<string>();

            foreach (var property in properties)
            {
                if (property.GetCustomAttribute<SwaggerParameterIgnoreAttribute>() != null)
                {
                    propertiesToRemove.Add(property.Name);
                }
            }

            if (!propertiesToRemove.Any())
            {
                return;
            }

            var parametersToHide = context.ApiDescription.ParameterDescriptions
                .Where(parameterDescription => ParameterHasIgnoreAttribute(parameterDescription, propertiesToRemove))
                .ToList();

            if (parametersToHide.Count == 0)
            {
                return;
            }

            foreach (var parameterToHide in parametersToHide)
            {
                var parameter = operation.Parameters.FirstOrDefault(parameter => string.Equals(parameter.Name, parameterToHide.Name, System.StringComparison.Ordinal));
                if (parameter != null)
                {
                    operation.Parameters.Remove(parameter);
                }
            }
        }

        private static bool ParameterHasIgnoreAttribute(
            Microsoft.AspNetCore.Mvc.ApiExplorer.ApiParameterDescription parameterDescription,
            IEnumerable<string> propertiesToRemove)
        {
            var result = propertiesToRemove.Where(x => parameterDescription.Name.StartsWith($"{x}.")).Any();
            if (result)
            {
                return true;
            }

            if (parameterDescription.ModelMetadata is Microsoft.AspNetCore.Mvc.ModelBinding.Metadata.DefaultModelMetadata metadata)
            {
                return metadata.Attributes.Attributes.Any(attribute => attribute.GetType() == typeof(SwaggerParameterIgnoreAttribute));
            }

            return false;
        }
    }
}

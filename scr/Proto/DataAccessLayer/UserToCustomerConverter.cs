namespace DataAccessLayer
{
    using System.Linq.Expressions;
    using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

    public class UserToCustomerConverter : ValueConverter
    {
        public UserToCustomerConverter(LambdaExpression convertToProviderExpression,
            LambdaExpression convertFromProviderExpression,
            ConverterMappingHints? mappingHints = null)
            : base(convertToProviderExpression, convertFromProviderExpression, mappingHints)
        {
        }

        public override Func<object?, object?> ConvertToProvider => throw new NotImplementedException();

        public override Func<object?, object?> ConvertFromProvider => throw new NotImplementedException();

        public override Type ModelClrType => throw new NotImplementedException();

        public override Type ProviderClrType => throw new NotImplementedException();
    }
}

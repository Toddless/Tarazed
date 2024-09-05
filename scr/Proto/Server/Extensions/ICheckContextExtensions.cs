namespace Server.Extensions
{
    using DataAccessLayer;
    using Server.Filters;

    public static class ICheckContextExtensions
    {
        public static IDatabaseContext CheckContext(this IDatabaseContext? context)
        {
            if (context == null)
            {
                throw new InternalServerException(DataModel.Resources.Errors.NotFound);
            }

            return context;
        }
    }
}

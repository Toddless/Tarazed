namespace Server.Extensions.Tests
{
    using DataAccessLayer;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Server.Filters;

    [TestClass]
    public class ICheckContextExtensionsTests
    {
        [TestMethod]
        public void CheckContextFromNullTest()
        {
            var context = (IDatabaseContext?)null;
            try
            {
                var result = context.CheckContext();
                Assert.Fail("Did not throw expected exception.");
            }
            catch (InternalServerException ex)
            {
                Assert.AreEqual(DataModel.Resources.Errors.NotFound, ex.Message);
            }
        }

        [TestMethod]
        public void CheckContextTest()
        {
            var context = new Mock<IDatabaseContext>();
            var result = context.Object.CheckContext();
            Assert.IsNotNull(result);
            Assert.ReferenceEquals(context.Object, result);
        }
    }
}

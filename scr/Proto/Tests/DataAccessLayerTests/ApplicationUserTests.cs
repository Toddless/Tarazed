namespace DataAccessLayerTests
{
    using DataAccessLayer;

    [TestClass]
    public class ApplicationUserTests
    {
        [TestInitialize]
        public void SetupDependencies()
        {
        }

        [TestCleanup]
        public void CleanupDependencies()
        {
        }

        [TestMethod]
        public void TestCreateInstance()
        {
            var user = new ApplicationUser();
            Assert.IsNotNull(user);
            Assert.AreEqual(default(string), user.Email, "Email is not null");
        }
    }
}

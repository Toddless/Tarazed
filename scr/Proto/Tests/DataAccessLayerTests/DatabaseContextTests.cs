namespace DataAccessLayerTests
{
    using DataAccessLayer;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Moq;

    [TestClass]
    public class DatabaseContextTests
    {
        private DbContextOptions<DatabaseContext>? _options;
        private DbContextOptions? _dbContextOptions;

        [TestInitialize]
        public void SetupDependencies()
        {
            _dbContextOptions = new Mock<DbContextOptions>().Object;
            var options = new Mock<DbContextOptions<DatabaseContext>>();
            options.Setup(x => x.WithExtension<IDbContextOptionsExtension>(It.IsAny<IDbContextOptionsExtension>())).Returns(_dbContextOptions);
            options.Setup(x => x.ContextType).Returns(typeof(DatabaseContext));
            _options = options.Object;
        }

        [TestCleanup]
        public void CleanupDependencies()
        {
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestCreateInstanceWithNull()
        {
            var context = new DatabaseContext(null);
            Assert.Fail();
        }

        [TestMethod]
        public void TestCreateInstanceWithOptionsNull()
        {
            var options = new Mock<DbContextOptions<DatabaseContext>>();
            options.Setup(x => x.WithExtension<IDbContextOptionsExtension>(It.IsAny<IDbContextOptionsExtension>())).Returns((DbContextOptions)null);
            options.Setup(x => x.ContextType).Returns(typeof(DatabaseContext));
            var context = new DatabaseContext(options.Object);
            Assert.IsNotNull(context);
        }

        [TestMethod]
        public void TestCreateInstance()
        {
            Assert.IsNotNull(_options);
            Assert.ReferenceEquals(_dbContextOptions, _options.WithExtension<IDbContextOptionsExtension>((IDbContextOptionsExtension)null));
            var context = new DatabaseContext(_options);
            Assert.IsNotNull(context);
        }
    }
}

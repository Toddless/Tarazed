namespace Server.Controllers.Tests
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Security.Claims;
    using DataAccessLayer;
    using DataModel;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Server.Extensions;
    using Server.Filters;
    using ServerTests;

    [TestClass]
    public class ExerciseControllerTests
    {
        private Mock<IDatabaseContext> _context;
        private Mock<ILogger<ExerciseController>> _logger;
        private Mock<UserManager<ApplicationUser>> _userManager;
        private MockAbstractBaseController _classUnderTest;

        [TestInitialize]
        public void Setup()
        {
            _context = new Mock<IDatabaseContext>(MockBehavior.Strict);
            _logger = new Mock<ILogger<ExerciseController>>(MockBehavior.Strict);
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _userManager = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            _classUnderTest = new MockAbstractBaseController(_userManager.Object, _context.Object, _logger.Object);

        }

        [TestMethod]
        public void AbstractBaseControllerConstruktorTest()
        {
        }

        [DataTestMethod]
        [DynamicData(nameof(GetConstructorArguments), DynamicDataSourceType.Method)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AbstractBaseControllerConstruktorWithNullTest(UserManager<ApplicationUser> manager, IDatabaseContext context, ILogger logger)
        {
            var controller = new MockAbstractBaseController(
                manager,
                context,
                logger);
        }

        [TestMethod]
        public async Task GetAsyncTest()
        {
            var data = new List<Exercise>
            {
                new Exercise { PrimaryId = 1, CustomerId = "1" },
                new Exercise { PrimaryId = 2, CustomerId = "1" },
                new Exercise { PrimaryId = 3, CustomerId = "1" },
                new Exercise { PrimaryId = 4, CustomerId = "1" },
            }.AsQueryable();

            var setMock = new Mock<DbSet<Exercise>>(MockBehavior.Strict);
            setMock.As<IAsyncEnumerable<Exercise>>()
               .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
               .Returns(new AsyncEnumerator<Exercise>(data.GetEnumerator()));
            setMock.As<IQueryable<Exercise>>().Setup(m => m.Provider).Returns(new AsyncQueryProvider<Exercise>(data.Provider));
            setMock.As<IQueryable<Exercise>>().Setup(m => m.Expression).Returns(data.Expression);
            setMock.As<IQueryable<Exercise>>().Setup(m => m.ElementType).Returns(data.ElementType);
            setMock.As<IQueryable<Exercise>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            _context.Setup(o => o.Set<Exercise>()).Returns(setMock.Object);

            _userManager.Setup(o => o.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new ApplicationUser() { Id = "1" });
            var controller = new ExerciseController(_context.Object, _userManager.Object, _logger.Object);
            var result = await _classUnderTest.GetAsync(null);
            Assert.IsTrue(result.Any());
        }

        [DataTestMethod()]
        [DynamicData(nameof(CreateExerciseData), DynamicDataSourceType.Method)]
        [ExpectedException(typeof(InternalServerException))]
        public async Task CreateAsyncTest(Exercise exercise)
        {
            var setMock = new Mock<DbSet<Exercise>>(MockBehavior.Strict);
            setMock.As<IAsyncEnumerable<Exercise>>()
             .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()));

            var entityEntryMock = new Mock<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Exercise>>();

            setMock.Setup(o => o.Add(It.IsAny<Exercise>())).Returns(entityEntryMock.Object);
            setMock.As<IAsyncEnumerable<Exercise>>()
               .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()));
            _context.Setup(o => o.Set<Exercise>()).Returns(setMock.Object);
            _userManager.Setup(o => o.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new ApplicationUser() { Id = "1" });
            var controller = new ExerciseController(_context.Object, _userManager.Object, _logger.Object);
            var result = await _classUnderTest.CreateAsync(exercise);
        }

        [TestMethod()]
        public async Task UpdateAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public async Task DeleteAsyncTest()
        {
            Assert.Fail();
        }

        private static IEnumerable<object[]> CreateExerciseData()
        {
            yield return new object[] { null };
            yield return new object[]
            {
                new Exercise
            {
                Reps = 1,
                CustomerId = "3",
                Description = "asdadas",
                PrimaryId = 1,
                Name = "name",
                ExerciseSetId = 2,
                Set = 1,
                Weight = 10,
            },
            };
            yield return new object[]
            {
                new Exercise
            {
                Reps = 1,
                CustomerId = "3",
                Description = "asdadas",
                PrimaryId = 0,
                Name = "name",
                ExerciseSetId = 2,
                Set = 1,
                Weight = 300,
            },
            };
            yield return new object[]
            {
                new Exercise
            {
                Reps = 1,
                CustomerId = "3",
                Description = "asdadas",
                PrimaryId = 0,
                Name = "name",
                ExerciseSetId = 2,
                Set = 1,
                Weight = 1,
            },
            };
            yield return new object[]
             {
                new Exercise
            {
                Reps = 1,
                CustomerId = "3",
                Description = "asdadas",
                PrimaryId = 0,
                Name = "name",
                ExerciseSetId = 2,
                Set = 1,
                Weight = 1,
            },
             };
            yield return new object[]
             {
                new Exercise
            {
                Reps = 1,
                CustomerId = " ",
                Description = "asdadas",
                PrimaryId = 0,
                Name = "name",
                ExerciseSetId = 2,
                Set = 1,
                Weight = 1,
            },
             };
            yield return new object[]
            {
                new Exercise
            {
                Reps = 1,
                CustomerId = string.Empty,
                Description = "asdadas",
                PrimaryId = 0,
                Name = "name",
                ExerciseSetId = 2,
                Set = 1,
                Weight = 1,
            },
            };
        }

        private static IEnumerable<object[]> GetConstructorArguments()
        {
            var mock = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
            yield return new object[] { null, Mock.Of<IDatabaseContext>(), Mock.Of<ILogger>() };
            yield return new object[] { mock, null, Mock.Of<ILogger>() };
            yield return new object[] { mock, Mock.Of<IDatabaseContext>(), null };
            yield return new object[] { null, Mock.Of<IDatabaseContext>(), null };
            yield return new object[] { null, null, Mock.Of<ILogger>() };
            yield return new object[] { mock, null, null };
            yield return new object[] { null, null, null };
        }

        private class MockAbstractBaseController : AbstractBaseController<Exercise>
        {
            public MockAbstractBaseController(UserManager<ApplicationUser> manager, IDatabaseContext context, ILogger logger)
                : base(manager, context, logger)
            {
            }
        }
    }
}

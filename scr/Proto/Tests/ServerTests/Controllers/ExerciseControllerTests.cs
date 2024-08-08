namespace Server.Controllers.Tests
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using DataAccessLayer;
    using DataModel;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ChangeTracking;
    using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
    using Microsoft.EntityFrameworkCore.Metadata;
    using Microsoft.EntityFrameworkCore.Metadata.Internal;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Moq.EntityFrameworkCore;
    using Server.Filters;
    using ServerTests;

    [TestClass]
    public class ExerciseControllerTests
    {
        private Mock<IDatabaseContext> _context;
        private ILogger<ExerciseController> _logger;
        private Mock<UserManager<ApplicationUser>> _userManager;
        private ExerciseController _classUnderTest;

        [TestInitialize]
        public void Setup()
        {
            _context = new Mock<IDatabaseContext>(MockBehavior.Strict);
            _logger = this.SetupLogger<ExerciseController>();
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _userManager = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            _classUnderTest = new ExerciseController(_userManager.Object, _context.Object, _logger);
        }

        [DataTestMethod]
        [DynamicData(nameof(ConstructorTestData), DynamicDataSourceType.Method)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateInstanceTest(UserManager<ApplicationUser> manager, IDatabaseContext context, ILogger<ExerciseController> logger)
        {
            var controller = new ExerciseController(
                manager,
                context,
                logger);
        }

        [TestMethod]
        public async Task GetAsyncTest()
        {
            IEnumerable<long> longs = new List<long>
            {
                1,
                2,
                6,
                3,
            };

            var data = new List<Exercise>
            {
                new Exercise { PrimaryId = 1, CustomerId = "1" },
                new Exercise { PrimaryId = 2, CustomerId = "1" },
                new Exercise { PrimaryId = 2, CustomerId = "2" },
                new Exercise { PrimaryId = 3, CustomerId = "3" },
                new Exercise { PrimaryId = 3, CustomerId = "2" },
                new Exercise { PrimaryId = 3, CustomerId = "5" },
                new Exercise { PrimaryId = 4, CustomerId = "1" },
                new Exercise { PrimaryId = 6, CustomerId = "1" },
                new Exercise { PrimaryId = 8, CustomerId = "1" },
            }.AsQueryable();

            var setMock = new Mock<DbSet<Exercise>>(MockBehavior.Strict);
            setMock.As<IAsyncEnumerable<Exercise>>()
               .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
               .Returns(new AsyncEnumerator<Exercise>(data.GetEnumerator()));

            setMock.As<IQueryable<Exercise>>().Setup(m => m.Provider).Returns(new AsyncQueryProvider<Exercise>(data.Provider));
            setMock.As<IQueryable<Exercise>>().Setup(m => m.Expression).Returns(data.Expression);
            setMock.As<IQueryable<Exercise>>().Setup(m => m.ElementType).Returns(data.ElementType);
            setMock.As<IQueryable<Exercise>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            _context.Setup(o => o.Set<Exercise>()).Returns(setMock.Object);

            _userManager.Setup(o => o.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new ApplicationUser() { Id = "1" });
            var controller = new ExerciseController(_userManager.Object, _context.Object, _logger);
            var resultNull = await _classUnderTest.GetAsync(null);
            var result = await _classUnderTest.GetAsync(longs);
            Assert.IsTrue(resultNull.Count() >= 5);
            Assert.IsTrue(result.Count() >= 3);
        }

        [DataTestMethod]
        [DynamicData(nameof(CreateExerciseData), DynamicDataSourceType.Method)]
        public async Task CreateAsyncTest(Exercise exercise, bool raisesException, string? expectedMessage)
        {
            var mockDbSet = new Mock<DbSet<Exercise>>(MockBehavior.Strict);
            mockDbSet.As<IAsyncEnumerable<Exercise>>()
             .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()));
            var mockIStateManager = new Mock<IStateManager>();
            var mockIEntityType = new Mock<IEntityType>();
            var mockISnapshot = new Mock<ISnapshot>().Object;

            mockIEntityType.As<IRuntimeEntityType>().Setup(x => x.GetFlattenedProperties()).Returns(() => new List<IProperty>());
            mockIEntityType.As<IRuntimeEntityType>().Setup(x => x.EmptyShadowValuesFactory).Returns(() => mockISnapshot);

            var internalEntityEntry = new InternalEntityEntry(mockIStateManager.Object, mockIEntityType.Object, exercise);
            var entityEntry = new EntityEntry<Exercise>(internalEntityEntry);

            mockDbSet.Setup(o => o.Add(It.IsAny<Exercise>())).Returns(entityEntry);
            mockDbSet.As<IAsyncEnumerable<Exercise>>()
               .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()));
            _context.Setup(o => o.Set<Exercise>()).Returns(mockDbSet.Object);
            _context.Setup(o => o.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            _userManager.Setup(o => o.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new ApplicationUser() { Id = "1" });
            var controller = new ExerciseController(_userManager.Object, _context.Object, _logger);

            if (raisesException)
            {
                Assert.IsNotNull(expectedMessage);
                try
                {
                    await _classUnderTest.CreateAsync(exercise);
                    Assert.Fail("Did not throw specified Exception");
                }
                catch (ServerException ex)
                {
                    Assert.AreEqual(expectedMessage, ex.Message);
                }
                catch (InternalServerException ex)
                {
                    Assert.AreEqual(expectedMessage, ex.Message);
                }

                return;
            }

            var result = await _classUnderTest.CreateAsync(exercise);
            Assert.IsNotNull(result);
            Assert.AreEqual(result, exercise);
            Assert.IsFalse(result.PrimaryId > 0);
        }

        [TestMethod]
        [DynamicData(nameof(UpdateExercise), DynamicDataSourceType.Method)]
        public async Task UpdateAsyncTest(Exercise exercise, bool exceptionsThrown, string expectedResult)
        {
            var dbSetMock = new Mock<DbSet<Exercise>>(MockBehavior.Strict);

            var mockIStateManager = new Mock<IStateManager>();
            var mockIEntityType = new Mock<IEntityType>();
            var mockISnapshot = new Mock<ISnapshot>().Object;

            mockIEntityType.As<IRuntimeEntityType>().Setup(x => x.GetFlattenedProperties()).Returns(() => new List<IProperty>());
            mockIEntityType.As<IRuntimeEntityType>().Setup(x => x.EmptyShadowValuesFactory).Returns(() => mockISnapshot);

            _userManager.Setup(o => o.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new ApplicationUser() { Id = "1" });
            var exercises = new List<Exercise>() { exercise };
            _context.Setup(o => o.Set<Exercise>()).ReturnsDbSet(exercises);
            _context.Setup(o => o.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var internalEntityEntry = new InternalEntityEntry(mockIStateManager.Object, mockIEntityType.Object, exercise);
            var entityEntry = new EntityEntry<Exercise>(internalEntityEntry);

            dbSetMock.Setup(m => m.Update(It.IsAny<Exercise>())).Returns(entityEntry);
            var controller = new ExerciseController(_userManager.Object, _context.Object, _logger);

            if (exceptionsThrown)
            {
                Assert.IsNotNull(expectedResult);
                try
                {
                    await _classUnderTest.UpdateAsync(exercise);
                    Assert.Fail("Did not throw specified Exception");
                }
                catch (ServerException ex)
                {
                    Assert.AreEqual(expectedResult, ex.Message);
                }
                catch (InternalServerException ex)
                {
                    Assert.AreEqual(expectedResult, ex.Message);
                }

                return;
            }

            var result = await _classUnderTest.UpdateAsync(exercise);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task DeleteAsyncTest()
        {
            Assert.Fail();
        }

        private static IEnumerable<object[]> UpdateExercise()
        {
            yield return new object[] { null, true,DataModel.Resources.Errors.NullObject };

            yield return new object[]
            {
                new Exercise
            {
                CustomerId = "2",
                Description = "Wrong",
                ExerciseSetId = 2,
                Name = "Names",
                PrimaryId = 2,
                Reps = 1,
                Set = 2,
                Weight = 10,
            },
                true,
                DataModel.Resources.Errors.ElementNotExists,
            };
            yield return new object[]
            {
                new Exercise
            {
                CustomerId = "1",
                Description = "Wrong",
                ExerciseSetId = 2,
                Name = "Names",
                PrimaryId = 1,
                Reps = 1,
                Set = 2,
                Weight = 10,
            },
                false,
                null,
            };
            yield return new object[]
            {
                new Exercise
            {
                CustomerId = "1",
                Description = "Wrong",
                ExerciseSetId = 2,
                Name = "Names",
                PrimaryId = 0,
                Reps = 1,
                Set = 2,
                Weight = 10,
            },
                true,
                DataModel.Resources.Errors.InvalidRequest_PrimaryKeyNotSet,
            };
            yield return new object[]
            {
                new Exercise
            {
                CustomerId = "3",
                Description = "Wrong",
                ExerciseSetId = 2,
                Name = "Names",
                PrimaryId = 5,
                Reps = 1,
                Set = 2,
                Weight = 10,
            },
                true,
                DataModel.Resources.Errors.ElementNotExists,
            };
        }

        private static IEnumerable<object[]> CreateExerciseData()
        {
            yield return new object[]
            {
                null,
                true,
                DataModel.Resources.Errors.InternalException,
            };
            yield return new object[]
            {
                new Exercise
                {
                    Reps = 1,
                    CustomerId = string.Empty,
                    Description = "asdadas",
                    PrimaryId = 1,
                    Name = "name",
                    ExerciseSetId = 2,
                    Set = 1,
                    Weight = 10,
                },
                true,
                DataModel.Resources.Errors.InvalidRequest_PrimaryKeySet,
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
                    Weight = 3000,
                },
                true,
                DataModel.Resources.Errors.InvalidRequest,
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
                false,
                null,
            };
            yield return new object[]
            {
                new Exercise
                {
                    Reps = 1,
                    CustomerId = " ",
                    Description = "asdadas",
                    PrimaryId = 1,
                    Name = "name",
                    ExerciseSetId = 2,
                    Set = 1,
                    Weight = 10,
                },
                true,
                DataModel.Resources.Errors.InvalidRequest_PrimaryKeySet,
            };
            yield return new object[]
            {
                new Exercise
                {
                    Reps = 1,
                    CustomerId = "3 ",
                    Description = "asdadas",
                    PrimaryId = 1,
                    Name = "name",
                    ExerciseSetId = 2,
                    Set = 1,
                    Weight = 10,
                },
                true,
                DataModel.Resources.Errors.InvalidRequest_PrimaryKeySet,
            };
        }

        private static IEnumerable<object[]> ConstructorTestData()
        {
            var mock = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null).Object;

            yield return new object[] { null, Mock.Of<IDatabaseContext>(), Mock.Of<ILogger<ExerciseController>>() };
            yield return new object[] { mock, null, Mock.Of<ILogger<ExerciseController>>() };
            yield return new object[] { mock, Mock.Of<IDatabaseContext>(), null };
            yield return new object[] { null, Mock.Of<IDatabaseContext>(), null };
            yield return new object[] { null, null, Mock.Of<ILogger<ExerciseController>>() };
            yield return new object[] { mock, null, null };
            yield return new object[] { null, null, null };
        }
    }
}

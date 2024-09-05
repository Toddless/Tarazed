namespace ServerTests.Controllers
{
    using DataAccessLayer;
    using DataModel;
    using DataModel.Resources;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Server.Controllers;
    using Server.Controllers.Tests;

    [TestClass]
    public class ExerciseSetControllerTest : AbstractControllerTest<Unit, UnitController>
    {
        [DataTestMethod]
        [DynamicData(nameof(ConstructorTestData), DynamicDataSourceType.Method)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateInstanceTest(UserManager<ApplicationUser> manager, IDatabaseContext context, ILogger<UnitController> logger)
        {
            var controller = new UnitController(
                context,
                manager,
                logger);
        }

        [TestMethod]
        [DynamicData(nameof(DeleteExerciseSetData), DynamicDataSourceType.Method)]
        public async Task DeleteAsyncTest(Unit exerciseSets, long id, bool exceptionThrown, string expectedExceptions)
        {
            await OnDeleteAsyncTest(exerciseSets, id, exceptionThrown, expectedExceptions);
        }

        [TestMethod]
        [DynamicData(nameof(GetExerciseSetData), DynamicDataSourceType.Method)]
        public async Task GetAsyncTest(List<long> longs, List<Unit> exerciseSets)
        {
            await OnGetAsyncTest(longs, exerciseSets);
        }

        [DataTestMethod]
        [DynamicData(nameof(CreateExerciseSetData), DynamicDataSourceType.Method)]
        public async Task CreateAsyncTest(Unit exerciseSets, bool exceptionThrown, string expectedExceptions)
        {
            await OnCreateAsyncTest(exerciseSets, exceptionThrown, expectedExceptions);
        }

        [TestMethod]
        [DynamicData(nameof(UpdateExerciseSetData), DynamicDataSourceType.Method)]
        public async Task UpdateAsyncTest(Unit exerciseSets, bool exceptionsThrown, string expectedExceptions)
        {
            await OnUpdateAsyncTest(exerciseSets, exceptionsThrown, expectedExceptions);
        }

        protected override AbstractBaseController<Unit> SetupController(Mock<IDatabaseContext> context, ILogger<UnitController> logger, Mock<UserManager<ApplicationUser>> userManager)
        {
            return new UnitController(context.Object, userManager.Object, logger);
        }

        private static IEnumerable<object[]> ConstructorTestData()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type. Testing null exceptions, or some data must be null.
            var mock = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null).Object;

            yield return new object[] { null, Mock.Of<IDatabaseContext>(), Mock.Of<ILogger<UnitController>>() };
            yield return new object[] { mock, null, Mock.Of<ILogger<UnitController>>() };
            yield return new object[] { mock, Mock.Of<IDatabaseContext>(), null };
            yield return new object[] { null, Mock.Of<IDatabaseContext>(), null };
            yield return new object[] { null, null, Mock.Of<ILogger<UnitController>>() };
            yield return new object[] { mock, null, null };
            yield return new object[] { null, null, null };
        }

        private static IEnumerable<object[]> DeleteExerciseSetData()
        {
            yield return new object[] { null, null, true, Errors.NullObject };
            yield return new object[]
            {
                new Unit
                {
                    CustomerId = "1",
                    Id = 1,
                },
                0L,
                true,
                Errors.InvalidRequest_PrimaryKeyNotSet,
            };
            yield return new object[]
            {
                new Unit
                {
                    CustomerId = "2",
                    Id = 1,
                },
                1L,
                true,
                Errors.ElementNotExists,
            };
            yield return new object[]
            {
                new Unit
                {
                    CustomerId = "1",
                    Id = 1,
                },
                2L,
                true,
                Errors.ElementNotExists,
            };
            yield return new object[]
            {
                new Unit
                {
                    CustomerId = "1",
                    Id = 1,
                },
                1L,
                false,
                null,
            };
        }

        private static IEnumerable<object[]> GetExerciseSetData()
        {
            yield return new object[]
            {
                new List<long>
                {
                   1,
                   2,
                   6,
                   3,
                },
                new List<Unit>
                {
                    new Unit
                    {
                         CustomerId = "1",
                         Id = 1,
                    },
                    new Unit
                    {
                        CustomerId = "1",
                        Id = 2,
                    },
                    new Unit
                    {
                        CustomerId = "2",
                        Id = 2,
                    },
                    new Unit
                    {
                        CustomerId = "2",
                        Id = 3,
                    },
                    new Unit
                    {
                        CustomerId = "1",
                        Id = 3,
                    },
                },
            };
        }

        private static IEnumerable<object[]> UpdateExerciseSetData()
        {
            yield return new object[] { null, true, Errors.NullObject };

            yield return new object[]
            {
                new Unit
            {
                CustomerId = "2",
                Name = "Names",
                Id = 2,
            },
                true,
                Errors.ElementNotExists,
            };
            yield return new object[]
            {
                new Unit
            {
                CustomerId = "1",
                Name = "Names",
                Id = 1,
            },
                false,
                null,
            };
            yield return new object[]
            {
                new Unit
            {
                CustomerId = "1",
                Name = "Names",
                Id = 0,
            },
                true,
                Errors.InvalidRequest_PrimaryKeyNotSet,
            };
            yield return new object[]
            {
                new Unit
            {
                CustomerId = "3",
                Name = "Names",
                Id = 5,
            },
                true,
                Errors.ElementNotExists,
            };
        }

        private static IEnumerable<object[]> CreateExerciseSetData()
        {
            yield return new object[]
            {
                null,
                true,
                Errors.InternalException,
            };
            yield return new object[]
            {
                new Unit
                {
                    CustomerId = string.Empty,
                    Id = 1,
                    Name = "name",
                    CompletionDate = 3L,
                    TrainingPlanId = 1,
                },
                true,
                Errors.InvalidRequest_PrimaryKeySet,
            };
            yield return new object[]
            {
                new Unit
                {
                    CustomerId = string.Empty,
                    Id = 0,
                    Name = string.Empty,
                    CompletionDate = 3L,
                    TrainingPlanId = 1,
                },
                true,
                Errors.InvalidRequest,
            };
            yield return new object[]
            {
                new Unit
                {
                    CustomerId = string.Empty,
                    Id = 0,
                    Name = "name",
                },
                false,
                null,
            };
            yield return new object[]
            {
                new Unit
                {
                    CustomerId = " ",
                    Id = 1,
                    Name = "name",
                },
                true,
                Errors.InvalidRequest_PrimaryKeySet,
            };
            yield return new object[]
            {
                new Unit
                {
                    CustomerId = "3",
                    Id = 1,
                    Name = "name",
                },
                true,
                Errors.InvalidRequest_PrimaryKeySet,
            };
        }
    }
}

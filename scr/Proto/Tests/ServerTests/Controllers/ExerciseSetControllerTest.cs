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
    public class ExerciseSetControllerTest : AbstractControllerTest<ExerciseSet, ExerciseSetController>
    {
        [DataTestMethod]
        [DynamicData(nameof(ConstructorTestData), DynamicDataSourceType.Method)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateInstanceTest(UserManager<ApplicationUser> manager, IDatabaseContext context, ILogger<ExerciseSetController> logger)
        {
            var controller = new ExerciseSetController(
                context,
                manager,
                logger);
        }

        [TestMethod]
        [DynamicData(nameof(GetExerciseSetData), DynamicDataSourceType.Method)]
        public async Task GetAsyncTest(List<long> longs, List<ExerciseSet> exerciseSets)
        {
            await OnGetAsyncTest(longs, exerciseSets);
        }

        [DataTestMethod]
        [DynamicData(nameof(CreateExerciseSetData), DynamicDataSourceType.Method)]
        public async Task CreateAsyncTest(ExerciseSet exerciseSets, bool raisesException, string expectedMessage)
        {
            await OnCreateAsyncTest(exerciseSets, raisesException, expectedMessage);
        }

        [TestMethod]
        [DynamicData(nameof(UpdateExerciseSetData), DynamicDataSourceType.Method)]
        public async Task UpdateAsyncTest(ExerciseSet exerciseSets, bool exceptionsThrown, string expectedResult)
        {
            await OnUpdateAsyncTest(exerciseSets, exceptionsThrown, expectedResult);
        }

        [TestMethod]
        [DynamicData(nameof(DeleteExerciseSetData), DynamicDataSourceType.Method)]
        public async Task DeleteAsyncTest(ExerciseSet exerciseSets, long id, string expectedExceptions, bool exceptionThrown)
        {
            await OnDeleteAsyncTest(exerciseSets, id, expectedExceptions, exceptionThrown);
        }

        protected override AbstractBaseController<ExerciseSet> SetupController(Mock<IDatabaseContext> context, ILogger<ExerciseSetController> logger, Mock<UserManager<ApplicationUser>> userManager)
        {
            return new ExerciseSetController(context.Object, userManager.Object, logger);
        }

        private static IEnumerable<object[]> ConstructorTestData()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type. Testing null exceptions, or some data must be null.
            var mock = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null).Object;

            yield return new object[] { null, Mock.Of<IDatabaseContext>(), Mock.Of<ILogger<ExerciseSetController>>() };
            yield return new object[] { mock, null, Mock.Of<ILogger<ExerciseSetController>>() };
            yield return new object[] { mock, Mock.Of<IDatabaseContext>(), null };
            yield return new object[] { null, Mock.Of<IDatabaseContext>(), null };
            yield return new object[] { null, null, Mock.Of<ILogger<ExerciseSetController>>() };
            yield return new object[] { mock, null, null };
            yield return new object[] { null, null, null };
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
                new List<ExerciseSet>
                {
                    new ExerciseSet
                    {
                         CustomerId = "1",
                         PrimaryId = 1,
                    },
                    new ExerciseSet
                    {
                        CustomerId = "1",
                        PrimaryId = 2,
                    },
                    new ExerciseSet
                    {
                        CustomerId = "2",
                        PrimaryId = 2,
                    },
                    new ExerciseSet
                    {
                        CustomerId = "2",
                        PrimaryId = 3,
                    },
                    new ExerciseSet
                    {
                        CustomerId = "1",
                        PrimaryId = 3,
                    },
                },
            };
        }

        private static IEnumerable<object[]> DeleteExerciseSetData()
        {
            yield return new object[] { null, null, Errors.NullObject, true };
            yield return new object[]
            {
                new ExerciseSet
                {
                    CustomerId = "1",
                    PrimaryId = 1,
                },
                0L,
                Errors.InvalidRequest_PrimaryKeyNotSet,
                true,
            };
            yield return new object[]
            {
                new ExerciseSet
                {
                    CustomerId = "2",
                    PrimaryId = 1,
                },
                1L,
                Errors.ElementNotExists,
                true,
            };
            yield return new object[]
            {
                new ExerciseSet
                {
                    CustomerId = "1",
                    PrimaryId = 1,
                },
                2L,
                Errors.ElementNotExists,
                true,
            };
            yield return new object[]
            {
                new ExerciseSet
                {
                    CustomerId = "1",
                    PrimaryId = 1,
                },
                1L,
                null,
                false,
            };
        }

        private static IEnumerable<object[]> UpdateExerciseSetData()
        {
            yield return new object[] { null, true, Errors.NullObject };

            yield return new object[]
            {
                new ExerciseSet
            {
                CustomerId = "2",
                Name = "Names",
                PrimaryId = 2,
            },
                true,
                Errors.ElementNotExists,
            };
            yield return new object[]
            {
                new ExerciseSet
            {
                CustomerId = "1",
                Name = "Names",
                PrimaryId = 1,
            },
                false,
                null,
            };
            yield return new object[]
            {
                new ExerciseSet
            {
                CustomerId = "1",
                Name = "Names",
                PrimaryId = 0,
            },
                true,
                Errors.InvalidRequest_PrimaryKeyNotSet,
            };
            yield return new object[]
            {
                new ExerciseSet
            {
                CustomerId = "3",
                Name = "Names",
                PrimaryId = 5,
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
                new ExerciseSet
                {
                    CustomerId = string.Empty,
                    PrimaryId = 1,
                    Name = "name",
                    CompletionDate = 3L,
                    TrainingPlanId = 1,
                },
                true,
                Errors.InvalidRequest_PrimaryKeySet,
            };
            yield return new object[]
            {
                new ExerciseSet
                {
                    CustomerId = string.Empty,
                    PrimaryId = 0,
                    Name = string.Empty,
                    CompletionDate = 3L,
                    TrainingPlanId = 1,
                },
                true,
                Errors.InvalidRequest,
            };
            yield return new object[]
            {
                new ExerciseSet
                {
                    CustomerId = string.Empty,
                    PrimaryId = 0,
                    Name = "name",
                },
                false,
                null,
            };
            yield return new object[]
            {
                new ExerciseSet
                {
                    CustomerId = " ",
                    PrimaryId = 1,
                    Name = "name",
                },
                true,
                Errors.InvalidRequest_PrimaryKeySet,
            };
            yield return new object[]
            {
                new ExerciseSet
                {
                    CustomerId = "3",
                    PrimaryId = 1,
                    Name = "name",
                },
                true,
                Errors.InvalidRequest_PrimaryKeySet,
            };
        }
    }
}

namespace Server.Controllers.Tests
{
    using System.Collections.Generic;
    using DataAccessLayer;
    using DataModel;
    using DataModel.Resources;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Logging;
    using Moq;

    [TestClass]
    public class ExerciseControllerTests : AbstractControllerTest<Exercise, ExerciseController>
    {
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
        [DynamicData(nameof(DeleteExerciseData), DynamicDataSourceType.Method)]
        public async Task DeleteAsyncTest(Exercise data, long id, bool exceptionThrown, string expectedExceptions)
        {
            await OnDeleteAsyncTest(data, id, exceptionThrown, expectedExceptions);
        }

        [TestMethod]
        [DynamicData(nameof(GetExerciseData), DynamicDataSourceType.Method)]
        public async Task GetAsyncTest(List<long> longs, List<Exercise> exercises)
        {
            await OnGetAsyncTest(longs, exercises);
        }

        [DataTestMethod]
        [DynamicData(nameof(CreateExerciseData), DynamicDataSourceType.Method)]
        public async Task CreateAsyncTest(Exercise exercise, bool exceptionThrown, string expectedExceptions)
        {
            await OnCreateAsyncTest(exercise, exceptionThrown, expectedExceptions);
        }

        [TestMethod]
        [DynamicData(nameof(UpdateExerciseData), DynamicDataSourceType.Method)]
        public async Task UpdateAsyncTest(Exercise exercise, bool exceptionsThrown, string expectedExceptions)
        {
            await OnUpdateAsyncTest(exercise, exceptionsThrown, expectedExceptions);
        }

        protected override AbstractBaseController<Exercise> SetupController(Mock<IDatabaseContext> context, ILogger<ExerciseController> logger, Mock<UserManager<ApplicationUser>> userManager)
        {
            return new ExerciseController(userManager.Object, context.Object, logger);
        }

        private static IEnumerable<object[]> ConstructorTestData()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type. Testing null exceptions, or some data must be null.
            var mock = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null).Object;

            yield return new object[] { null, Mock.Of<IDatabaseContext>(), Mock.Of<ILogger<ExerciseController>>() };
            yield return new object[] { mock, null, Mock.Of<ILogger<ExerciseController>>() };
            yield return new object[] { mock, Mock.Of<IDatabaseContext>(), null };
            yield return new object[] { null, Mock.Of<IDatabaseContext>(), null };
            yield return new object[] { null, null, Mock.Of<ILogger<ExerciseController>>() };
            yield return new object[] { mock, null, null };
            yield return new object[] { null, null, null };
        }

        private static IEnumerable<object[]> DeleteExerciseData()
        {
            yield return new object[] { null, null, true, Errors.NullObject };
            yield return new object[]
            {
                new Exercise
                {
                    CustomerId = "1",
                    PrimaryId = 1,
                },
                0L,
                true,
                Errors.InvalidRequest_PrimaryKeyNotSet,
            };
            yield return new object[]
            {
                new Exercise
                {
                    CustomerId = "2",
                    PrimaryId = 1,
                },
                1L,
                true,
                Errors.ElementNotExists,
            };
            yield return new object[]
            {
                new Exercise
                {
                    CustomerId = "1",
                    PrimaryId = 1,
                },
                2L,
                true,
                Errors.ElementNotExists,
            };
            yield return new object[]
            {
                new Exercise
                {
                    CustomerId = "1",
                    PrimaryId = 1,
                },
                1L,
                false,
                null,
            };
        }

        private static IEnumerable<object[]> GetExerciseData()
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
                new List<Exercise>
                {
                    new Exercise
                    {
                         CustomerId = "1",
                         PrimaryId = 1,
                    },
                    new Exercise
                    {
                        CustomerId = "1",
                        PrimaryId = 2,
                    },
                    new Exercise
                    {
                        CustomerId = "2",
                        PrimaryId = 2,
                    },
                    new Exercise
                    {
                        CustomerId = "2",
                        PrimaryId = 3,
                    },
                    new Exercise
                    {
                        CustomerId = "1",
                        PrimaryId = 3,
                    },
                },
            };
        }

        private static IEnumerable<object[]> UpdateExerciseData()
        {
            yield return new object[] { null, true, Errors.NullObject };

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
                Errors.ElementNotExists,
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
                Errors.InvalidRequest_PrimaryKeyNotSet,
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
                Errors.ElementNotExists,
            };
        }

        private static IEnumerable<object[]> CreateExerciseData()
        {
            yield return new object[]
            {
                null,
                true,
                Errors.InternalException,
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
                Errors.InvalidRequest_PrimaryKeySet,
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
                Errors.InvalidRequest,
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
                Errors.InvalidRequest_PrimaryKeySet,
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
                Errors.InvalidRequest_PrimaryKeySet,
            };
        }
    }
}

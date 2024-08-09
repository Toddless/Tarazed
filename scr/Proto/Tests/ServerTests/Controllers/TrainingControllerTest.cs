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
    public class TrainingControllerTest : AbstractControllerTest<TrainingPlan, TrainingController>
    {
        [DataTestMethod]
        [DynamicData(nameof(ConstructorTestData), DynamicDataSourceType.Method)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateInstanceTest(UserManager<ApplicationUser> manager, IDatabaseContext context, ILogger<TrainingController> logger)
        {
            var controller = new TrainingController(
                context,
                manager,
                logger);
        }

        [TestMethod]
        [DynamicData(nameof(GetTrainingPlanData), DynamicDataSourceType.Method)]
        public async Task GetAsyncTest(List<long> longs, List<TrainingPlan> plan)
        {
            await OnGetAsyncTest(longs, plan);
        }

        [DataTestMethod]
        [DynamicData(nameof(CreateTrainingPlanData), DynamicDataSourceType.Method)]
        public async Task CreateAsyncTest(TrainingPlan plan, bool raisesException, string expectedMessage)
        {
            await OnCreateAsyncTest(plan, raisesException, expectedMessage);
        }

        [TestMethod]
        [DynamicData(nameof(UpdateTrainingPlanData), DynamicDataSourceType.Method)]
        public async Task UpdateAsyncTest(TrainingPlan plan, bool exceptionsThrown, string expectedResult)
        {
            await OnUpdateAsyncTest(plan, exceptionsThrown, expectedResult);
        }

        [TestMethod]
        [DynamicData(nameof(DeleteTrainingPlanData), DynamicDataSourceType.Method)]
        public async Task DeleteAsyncTest(TrainingPlan plan, long id, string expectedExceptions, bool exceptionThrown)
        {
            await OnDeleteAsyncTest(plan, id, expectedExceptions, exceptionThrown);
        }

        protected override AbstractBaseController<TrainingPlan> SetupController(Mock<IDatabaseContext> context, ILogger<TrainingController> logger, Mock<UserManager<ApplicationUser>> userManager)
        {
            return new TrainingController(context.Object, userManager.Object, logger);
        }

        private static IEnumerable<object[]> ConstructorTestData()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type. Testing null exceptions, or some data must be null.
            var mock = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null).Object;

            yield return new object[] { null, Mock.Of<IDatabaseContext>(), Mock.Of<ILogger<TrainingController>>() };
            yield return new object[] { mock, null, Mock.Of<ILogger<TrainingController>>() };
            yield return new object[] { mock, Mock.Of<IDatabaseContext>(), null };
            yield return new object[] { null, Mock.Of<IDatabaseContext>(), null };
            yield return new object[] { null, null, Mock.Of<ILogger<TrainingController>>() };
            yield return new object[] { mock, null, null };
            yield return new object[] { null, null, null };
        }

        private static IEnumerable<object[]> GetTrainingPlanData()
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
                new List<TrainingPlan>
                {
                    new TrainingPlan
                    {
                         CustomerId = "1",
                         PrimaryId = 1,
                    },
                    new TrainingPlan
                    {
                        CustomerId = "1",
                        PrimaryId = 2,
                    },
                    new TrainingPlan
                    {
                        CustomerId = "2",
                        PrimaryId = 2,
                    },
                    new TrainingPlan
                    {
                        CustomerId = "2",
                        PrimaryId = 3,
                    },
                    new TrainingPlan
                    {
                        CustomerId = "1",
                        PrimaryId = 3,
                    },
                },
            };
        }

        private static IEnumerable<object[]> DeleteTrainingPlanData()
        {
            yield return new object[] { null, null, Errors.NullObject, true };
            yield return new object[]
            {
                new TrainingPlan
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
                new TrainingPlan
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
                new TrainingPlan
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
                new TrainingPlan
                {
                    CustomerId = "1",
                    PrimaryId = 1,
                },
                1L,
                null,
                false,
            };
        }

        private static IEnumerable<object[]> UpdateTrainingPlanData()
        {
            yield return new object[] { null, true, Errors.NullObject };

            yield return new object[]
            {
                new TrainingPlan
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
                new TrainingPlan
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
                new TrainingPlan
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
                new TrainingPlan
            {
                CustomerId = "3",
                Name = "Names",
                PrimaryId = 5,
            },
                true,
                Errors.ElementNotExists,
            };
        }

        private static IEnumerable<object[]> CreateTrainingPlanData()
        {
            yield return new object[]
            {
                null,
                true,
                Errors.InternalException,
            };
            yield return new object[]
            {
                new TrainingPlan
                {
                    CustomerId = string.Empty,
                    PrimaryId = 1,
                    Name = "name",
                },
                true,
                Errors.InvalidRequest_PrimaryKeySet,
            };
            yield return new object[]
            {
                new TrainingPlan
                {
                    CustomerId = string.Empty,
                    PrimaryId = 0,
                    Name = string.Empty,
                },
                true,
                Errors.InvalidRequest,
            };
            yield return new object[]
            {
                new TrainingPlan
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
                new TrainingPlan
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
                new TrainingPlan
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

﻿namespace ServerTests.Controllers
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
        [DynamicData(nameof(DeleteTrainingPlanData), DynamicDataSourceType.Method)]
        public async Task DeleteAsyncTest(TrainingPlan plan, long id, bool exceptionThrown, string expectedExceptions)
        {
            await OnDeleteAsyncTest(plan, id, exceptionThrown, expectedExceptions);
        }

        [TestMethod]
        [DynamicData(nameof(GetTrainingPlanData), DynamicDataSourceType.Method)]
        public async Task GetAsyncTest(List<long> longs, List<TrainingPlan> plan)
        {
            await OnGetAsyncTest(longs, plan);
        }

        [DataTestMethod]
        [DynamicData(nameof(CreateTrainingPlanData), DynamicDataSourceType.Method)]
        public async Task CreateAsyncTest(TrainingPlan plan, bool raisesException, string expectedExceptions)
        {
            await OnCreateAsyncTest(plan, raisesException, expectedExceptions);
        }

        [TestMethod]
        [DynamicData(nameof(UpdateTrainingPlanData), DynamicDataSourceType.Method)]
        public async Task UpdateAsyncTest(TrainingPlan plan, bool exceptionsThrown, string expectedExceptions)
        {
            await OnUpdateAsyncTest(plan, exceptionsThrown, expectedExceptions);
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
                         Id = 1,
                    },
                    new TrainingPlan
                    {
                        CustomerId = "1",
                        Id = 2,
                    },
                    new TrainingPlan
                    {
                        CustomerId = "2",
                        Id = 2,
                    },
                    new TrainingPlan
                    {
                        CustomerId = "2",
                        Id = 3,
                    },
                    new TrainingPlan
                    {
                        CustomerId = "1",
                        Id = 3,
                    },
                },
            };
        }

        private static IEnumerable<object[]> DeleteTrainingPlanData()
        {
            yield return new object[] { null, null,  true, Errors.NullObject };
            yield return new object[]
            {
                new TrainingPlan
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
                new TrainingPlan
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
                new TrainingPlan
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
                new TrainingPlan
                {
                    CustomerId = "1",
                    Id = 1,
                },
                1L,
                false,
                null,
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
                Id = 2,
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
                Id = 1,
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
                Id = 0,
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
                Id = 5,
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
                    Id = 1,
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
                    Id = 0,
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
                    Id = 0,
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
                    Id = 1,
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
                    Id = 1,
                    Name = "name",
                },
                true,
                Errors.InvalidRequest_PrimaryKeySet,
            };
        }
    }
}

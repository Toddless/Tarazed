namespace Server.Controllers.Tests
{
    using System.Security.Claims;
    using DataAccessLayer;
    using DataModel;
    using DataModel.Resources;
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
    using ServerTests.Extensions;

    [TestClass]
    public abstract class AbstractControllerTest<TU, TV>
        where TV : class
        where TU : class, IEntity
    {
        protected Mock<IDatabaseContext>? Context { get; private set; }

        protected ILogger<TV>? Logger { get; private set; }

        protected Mock<UserManager<ApplicationUser>>? UserManager { get; private set; }

        protected AbstractBaseController<TU>? Controller { get; private set; }

        [TestInitialize]
        public void Setup()
        {
            Context = new Mock<IDatabaseContext>(MockBehavior.Strict);
            Logger = this.SetupLogger<TV>().Object;
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>(MockBehavior.Strict);
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type. Nur IUserStore ist wichtig, rest darf null sein.
            UserManager = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            Controller = SetupController(Context, Logger, UserManager);
        }

        [TestCleanup]
        public void Cleanup()
        {
            Controller?.Dispose();
            Controller = null;
        }

        public async Task OnDeleteAsyncTest(TU data, long id, bool exceptionThrown, string expectedExceptions)
        {
            if (exceptionThrown)
            {
                Assert.IsNotNull(expectedExceptions);
            }

            var mockDbSet = MockDbSet();
            var entityEntry = MockEntityEntry(data);

            var items = new List<TU>() { data };

            SetupApplicationUser();
            SetupSetMethod(items);
            SetupSaveChangesMethod();

            mockDbSet.Setup(o => o.Remove(It.IsAny<TU>())).Returns(entityEntry);

            bool result;

            try
            {
                if (exceptionThrown && expectedExceptions == Errors.NullObject.ToString())
                {
                    result = await Controller!.DeleteAsync(null);
                    Assert.IsFalse(result);
                }

                result = await Controller!.DeleteAsync(id);
                if (exceptionThrown)
                {
                    Assert.Fail("Did not throw specified Exception");
                }

                Assert.IsNotNull(result);
            }
            catch (ServerException ex)
            {
                Assert.AreEqual(expectedExceptions, ex.Message);
            }
            catch (InternalServerException ex)
            {
                Assert.AreEqual(expectedExceptions, ex.Message);
            }
        }

        public async Task OnGetAsyncTest(List<long> longs, List<TU> exercises)
        {
            var mockDbSet = MockDbSet();
            GetAsyncEnumerable(mockDbSet);
            SetupApplicationUser();
            SetupSetMethod(exercises);

            var resultWithNull = await Controller!.GetAsync(null);
            var resultWithIds = await Controller.GetAsync(longs);

            Assert.IsNotNull(resultWithNull);
            Assert.IsNotNull(resultWithIds);
            Assert.IsTrue(resultWithNull.Count() >= 3);
            Assert.IsTrue(resultWithIds.Count() >= 3);
        }

        public async Task OnCreateAsyncTest(TU data, bool exceptionThrown, string expectedExceptions)
        {
            if (exceptionThrown)
            {
                Assert.IsNotNull(expectedExceptions);
            }

            var mockDbSet = MockDbSet();
            var entityEntry = MockEntityEntry(data);
            var exercises = new List<TU>() { data };
            GetAsyncEnumerable(mockDbSet);
            SetupApplicationUser();
            SetupSetMethod(exercises);
            SetupSaveChangesMethod();
            mockDbSet.Setup(o => o.Add(It.IsAny<TU>())).Returns(entityEntry);

            TU? result;
            try
            {
                result = await Controller!.CreateAsync(data);
                if (exceptionThrown)
                {
                    Assert.Fail("Did not throw specified Exception");
                }

                Assert.IsNotNull(result);
                Assert.AreEqual(result, data);
                Assert.IsFalse(result.PrimaryId > 0);
            }
            catch (ServerException ex)
            {
                Assert.AreEqual(expectedExceptions, ex.Message);
            }
            catch (InternalServerException ex)
            {
                Assert.AreEqual(expectedExceptions, ex.Message);
            }
        }

        public async Task OnUpdateAsyncTest(TU data, bool exceptionsThrown, string expectedExceptions)
        {
            if (exceptionsThrown)
            {
                Assert.IsNotNull(expectedExceptions);
            }

            var mockDbSet = MockDbSet();
            var entityEntry = MockEntityEntry(data);

            var exercises = new List<TU>() { data };

            SetupApplicationUser();
            SetupSetMethod(exercises);
            SetupSaveChangesMethod();
            mockDbSet.Setup(m => m.Update(It.IsAny<TU>())).Returns(entityEntry);

            TU? result;
            try
            {
                result = await Controller!.UpdateAsync(data);
                if (exceptionsThrown)
                {
                    Assert.Fail("Did not throw specified Exception");
                }

                Assert.IsNotNull(result);
            }
            catch (ServerException ex)
            {
                Assert.AreEqual(expectedExceptions, ex.Message);
            }
            catch (InternalServerException ex)
            {
                Assert.AreEqual(expectedExceptions, ex.Message);
            }
        }

        protected static void GetAsyncEnumerable(Mock<DbSet<TU>> mockDbSet) => mockDbSet.As<IAsyncEnumerable<TU>>()
                      .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()));

        protected static Mock<DbSet<TU>> MockDbSet() => new Mock<DbSet<TU>>(MockBehavior.Strict);

        protected static EntityEntry<TU> MockEntityEntry(TU exercise)
        {
            var mockIStateManager = new Mock<IStateManager>();
            var mockIEntityType = new Mock<IEntityType>();
#pragma warning disable EF1001 // Internal EF Core API usage. Suppresing ok here only for Unittests
            var mockISnapshot = new Mock<ISnapshot>().Object;

            mockIEntityType.As<IRuntimeEntityType>().Setup(x => x.GetFlattenedProperties()).Returns(() => Array.Empty<IProperty>());
            mockIEntityType.As<IRuntimeEntityType>().Setup(x => x.EmptyShadowValuesFactory).Returns(() => mockISnapshot);

            var internalEntityEntry = new InternalEntityEntry(mockIStateManager.Object, mockIEntityType.Object, exercise);
            var entityEntry = new EntityEntry<TU>(internalEntityEntry);
#pragma warning restore EF1001 // Internal EF Core API usage.
            return entityEntry;
        }

        protected abstract AbstractBaseController<TU> SetupController(Mock<IDatabaseContext> context, ILogger<TV> logger, Mock<UserManager<ApplicationUser>> userManager);

        protected void SetupSaveChangesMethod()
        {
            Context!.Setup(o => o.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        }

        protected void SetupSetMethod(List<TU> data) => Context!.Setup(o => o.Set<TU>()).ReturnsDbSet(data);

        protected void SetupApplicationUser() => UserManager!.Setup(o => o.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new ApplicationUser() { Id = "1" });
    }
}

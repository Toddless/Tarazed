namespace ServerTests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using DataAccessLayer;
    using DataModel;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Controllers;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ChangeTracking;
    using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
    using Microsoft.EntityFrameworkCore.Metadata;
    using Microsoft.EntityFrameworkCore.Metadata.Internal;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Moq.EntityFrameworkCore;
    using Server.Controllers;
    using Server.Filters;
    using ServerTests.Extensions;

    [TestClass]
    public class CustomerControllerTest : Controller
    {
        private Mock<IDatabaseContext>? Context { get; set; }

        private Mock<ILogger<CustomerController>>? Logger { get; set; }

        private Mock<UserManager<ApplicationUser>>? UserManager { get; set; }

        private CustomerController? Controller { get; set; }

        [TestInitialize]
        public void Setup()
        {
            Context = new Mock<IDatabaseContext>(MockBehavior.Strict);
            Logger = this.SetupLogger<CustomerController>();
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>(MockBehavior.Strict);
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type. Nur IUserStore ist wichtig, rest darf null sein.
            UserManager = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            Controller = SetupController(Logger.Object, UserManager, Context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            Controller?.Dispose();
            Controller = null;
        }

        [TestMethod]
        public async Task DeleteCustomerAsync_UserRoleTest()
        {
            var user = new ApplicationUser()
            {
                Id = "1",
                Email = "mail@mail.de",
            };
            SetupApplicationUserInContext(user);
            SetupControllerContext(Controller!, false);

            var result = await Controller!.DeleteCustomerAsync();
            Assert.IsTrue(result);
            Context!.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(ServerException))]
        public async Task DeleteCustomerAsync_AdminRoleTest()
        {
            var user = new ApplicationUser()
            {
                Id = "1",
                Email = "mail@mail.de",
            };

            SetupControllerContext(Controller!, true);
            SetupApplicationUserInContext(user);
            await Controller!.DeleteCustomerAsync();
        }

        [TestMethod]
        [ExpectedException(typeof(ServerException))]
        public async Task DeleteCustomerAsync_UserIsNullTest()
        {
            // ensure precondition
            Assert.IsNull(Controller!.User);

            await Controller!.DeleteCustomerAsync();
        }

        [TestMethod]
        [DynamicData(nameof(ApplicationUserIsNullTestData), DynamicDataSourceType.Method)]
        public async Task DeleteCustomerAsync_ApplicationUserIsNullTest(ApplicationUser user)
        {
            SetupApplicationUserInContext(user);
            SetupControllerContext(Controller!, false);
            await Assert.ThrowsExceptionAsync<ServerException>(() => Controller!.DeleteCustomerAsync());
            UserManager!.Verify(o => o.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
        }

        [TestMethod]
        public async Task DeleteCustomerAsync_SuccessTest()
        {
            var user = new ApplicationUser()
            {
                Id = "2",
                Email = "mail@mail.de",
            };
            var users = new List<ApplicationUser>() { user };
            var mockDbSet = MockDbSet();
            var entityEntry = MockEntityEntry(user);
            SetupApplicationUserInContext(user);
            SetupControllerContext(Controller!, false);
            mockDbSet.Setup(o => o.Remove(user)).Returns(entityEntry);
            Context!.Setup(o => o.Set<ApplicationUser>()).Returns(mockDbSet.Object);

            var result = await Controller!.DeleteCustomerAsync();
            UserManager!.Verify(o => o.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
            Context!.Verify(o => o.Set<ApplicationUser>(), Times.Once);
            Context!.Verify(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            mockDbSet.Verify(o => o.Remove(user), Times.Once);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task GetCustomerAsync_SucceededTest()
        {
            var user = new ApplicationUser()
            {
                Email = "gmail@mail.de",
                Id = "asdNAISD12",
            };
            SetupApplicationUserInContext(user);
            SetupControllerContext(Controller!, false);
            var result = await Controller!.GetCustomerAsync();

            UserManager!.VerifyAll();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ServerException))]
        public async Task GetCustomerAsync_UserIsNullTest()
        {
            await Controller!.GetCustomerAsync();
        }

        [TestMethod]
        [DynamicData(nameof(ApplicationUserIsNullTestData), DynamicDataSourceType.Method)]
        public async Task GetCustomerAsync_ApplicationUserIsNull(ApplicationUser user)
        {
            SetupApplicationUserInContext(user);
            SetupControllerContext(Controller!, false);

            await Assert.ThrowsExceptionAsync<ServerException>(() => Controller!.GetCustomerAsync());

            UserManager!.Verify(o => o.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(ServerException))]
        public async Task UpdateCustomerAsync_UserIsNullTest()
        {
            Assert.IsNull(Controller!.User);

            // setup nullpointer fix
            await Controller!.UpdateCustomerAsync(new Customer());
        }

        [TestMethod]
        [ExpectedException(typeof(ServerException))]
        public async Task UpdateCustomerAsync_CustomerNullTest()
        {
            var user = new ApplicationUser()
            {
                Email = "gmail@mail.de",
                Id = "asdNAISD12",
            };
            SetupApplicationUserInContext(user);
            SetupControllerContext(Controller!, false);
            await Controller!.UpdateCustomerAsync(null);
        }

        [TestMethod]
        public async Task UpdateCustomerAsync_IdIsCorrectTest()
        {
            var customer = new Customer()
            {
                Email = "mail@mail.dom",
                Id = 0,
                UId = "1",
            };
            var user = new ApplicationUser()
            {
                Id = "1",
                Email = "mail@mail.de",
            };
            var mockDbSet = MockDbSet();
            var entityEntry = MockEntityEntry(user);
            SetupApplicationUserInContext(user);
            SetupControllerContext(Controller!, false);
            Context!.Setup(o => o.Set<ApplicationUser>()).Returns(mockDbSet.Object);
            UserManager!.Setup(o => o.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

            var result = await Controller!.UpdateCustomerAsync(customer);
            UserManager.VerifyAll();
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Email, user.Email);
        }

        [TestMethod]
        public async Task UpdateCustomerAsync_IdIsFalseTest()
        {
            var customer = new Customer()
            {
                Email = string.Empty,
                Id = 0,
                UId = "2",
            };
            var user = new ApplicationUser()
            {
                Id = "1",
                Email = "mail@mail.de",
            };
            var entityEntry = MockEntityEntry(user);
            SetupApplicationUserInContext(user);
            SetupControllerContext(Controller!, false);
            UserManager!.Setup(o => o.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

            await Assert.ThrowsExceptionAsync<ServerException>(() => Controller!.UpdateCustomerAsync(customer));

            UserManager.Verify(o => o.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
        }

        protected static EntityEntry<ApplicationUser> MockEntityEntry(ApplicationUser user)
        {
            var mockIStateManager = new Mock<IStateManager>();
            var mockIEntityType = new Mock<IEntityType>();
#pragma warning disable EF1001 // Internal EF Core API usage. Suppresing ok here only for Unittests
            var mockISnapshot = new Mock<ISnapshot>().Object;

            mockIEntityType.As<IRuntimeEntityType>().Setup(o => o.GetFlattenedProperties()).Returns(Array.Empty<IProperty>);
            mockIEntityType.As<IRuntimeEntityType>().Setup(o => o.EmptyShadowValuesFactory).Returns(() => mockISnapshot);

            var internalEntityEntry = new InternalEntityEntry(mockIStateManager.Object, mockIEntityType.Object, user);
            var entityEntry = new EntityEntry<ApplicationUser>(internalEntityEntry);
#pragma warning restore EF1001 // Internal EF Core API usage.
            return entityEntry;
        }

        protected static Mock<DbSet<ApplicationUser>> MockDbSet() => new Mock<DbSet<ApplicationUser>>(MockBehavior.Strict);

        protected CustomerController SetupController(ILogger<CustomerController> logger, Mock<UserManager<ApplicationUser>> userManager, Mock<IDatabaseContext> context)
        {
            return new CustomerController(context.Object, userManager.Object, logger);
        }

        private static IEnumerable<object[]> ApplicationUserIsNullTestData()
        {
            yield return new object[] { null };
        }

        private static void SetupControllerContext(CustomerController controller, bool isAdmin)
        {
            var httpContextMock = new Mock<HttpContext>();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, isAdmin ? "Admin" : "User"),
            };
            var identity = new ClaimsIdentity(claims);
            var userPrincipal = new ClaimsPrincipal(identity);

            httpContextMock.SetupGet(o => o.User).Returns(userPrincipal);
            var actionContext = new ActionContext(
                httpContextMock.Object,
                new Microsoft.AspNetCore.Routing.RouteData(),
                new ControllerActionDescriptor(),
                new Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary());
            var controllerContext = new ControllerContext(actionContext);
            controller!.ControllerContext = controllerContext;
        }

        private void SetupApplicationUserInContext(ApplicationUser user)
        {
            var users = new List<ApplicationUser>() { user };
            Context!.Setup(o => o.Set<ApplicationUser>()).ReturnsDbSet(users);
            Context!.Setup(o => o.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            UserManager!.Setup(o => o.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
        }
    }
}

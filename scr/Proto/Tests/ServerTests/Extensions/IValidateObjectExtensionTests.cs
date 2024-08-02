namespace Server.Extensions.Tests
{
    using System.ComponentModel.DataAnnotations;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass()]
    public class IValidateObjectExtensionTests
    {
        [TestMethod]
        public void ValidateObjectTest()
        {
            List<ValidationResult> results;
            IValidateObjectExtension.ValidateObject(new object(), out results);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidateObjectNullTest()
        {
            List<ValidationResult> results;
            IValidateObjectExtension.ValidateObject<object>(null, out results);
        }
    }
}

namespace Server.Extensions.Tests
{
    using System.ComponentModel.DataAnnotations;
    using DataModel;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class IValidateObjectExtensionTests
    {
        [TestMethod]
        [DataRow(null)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidateObjectNullTest(Exercise exercise)
        {
            var result = new List<ValidationResult>();

            var validatorResult = exercise.ValidateObject(out result);

            Assert.Fail();
        }

        [TestMethod]
        public void ValidateObjectTrueTest()
        {
            var testObj = new Exercise
            {
                Reps = 1,
                CustomerId = string.Empty,
                Description = "asdadas",
                PrimaryId = 0,
                Name = "name",
                ExerciseSetId = 2,
                Set = 1,
                Weight = 10,
            };

            var result = new List<ValidationResult>();

            var validatorResult = testObj.ValidateObject(out result);

            Assert.IsTrue(result != null);
            Assert.IsTrue(result.Count <= 0);
            Assert.IsTrue(validatorResult);
        }

        [TestMethod]
        public void ValidateObjectFalseTest()
        {
            var testObj = new Exercise
            {
                Reps = 1,
                CustomerId = string.Empty,
                Description = string.Empty,
                PrimaryId = 0,
                Name = "name",
                ExerciseSetId = 2,
                Set = 1,
                Weight = 10,
            };
            var result = new List<ValidationResult>();

            var validatorResult = testObj.ValidateObject(out result);
            Assert.IsFalse(validatorResult);
            Assert.IsTrue(result.Count == 1);
            Assert.IsTrue(result[0].MemberNames.Single() == nameof(Exercise.Description));
        }
    }
}

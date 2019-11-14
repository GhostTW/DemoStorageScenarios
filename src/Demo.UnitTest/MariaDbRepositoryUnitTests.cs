using System.Linq;
using Demo.Core;
using NUnit.Framework;

namespace Demo.UnitTest
{
    public class MariaDbRepositoryUnitTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GetUser_Should_Correct()
        {
            var sut = new MariaDbRepository();

            var result = sut.GetUsers();
            var firstUser = result.FirstOrDefault();
            
            Assert.IsNotNull(firstUser);
            Assert.AreEqual("Admin",firstUser.Code);
            Assert.AreEqual("Admin",firstUser.Code);
        }
    }
}
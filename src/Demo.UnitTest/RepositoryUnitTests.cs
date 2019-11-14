using System.Linq;
using Demo.Core;
using NUnit.Framework;

namespace Demo.UnitTests
{
    public class RepositoryUnitTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var sut = new Repository();

            var result = sut.GetUsers();
            var firstUser = result.FirstOrDefault();
            
            Assert.IsNotNull(firstUser);
            Assert.AreEqual("Admin",firstUser.Code);
            Assert.AreEqual("Admin",firstUser.Code);
        }
    }
}
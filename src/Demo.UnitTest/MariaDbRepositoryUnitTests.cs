using System.Linq;
using Demo.Core;
using Demo.Data;
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
        public void GetUserShouldCorrect()
        {
            var sut = new MariaDbRepository();

            var result = sut.GetUsers();
            var firstUser = result.FirstOrDefault();
            
            Assert.IsNotNull(firstUser);
            Assert.AreEqual("Admin",firstUser.Code);
            Assert.AreEqual("Admin",firstUser.Code);
        }

        [Test]
        public void CreateUserShouldCorrect()
        {
            var newUser = new UserEntity {Code = "FromUnitTest", Password = "pass.123", IsActive = false};
            
            var sut = new MariaDbRepository();

            var insertedUser = sut.InsertUser(newUser);
            
            Assert.NotZero(newUser.Id);
            Assert.NotZero(insertedUser.Id);
        }

        [Test]
        public void UpdateUser_Should_Correct()
        {
            var newUser = new UserEntity {Id = 4, Password = "pass.123", IsActive = true};
            
            var sut = new MariaDbRepository();

            Assert.DoesNotThrow(() => sut.UpdateUser(newUser));
            
        }
    }
}
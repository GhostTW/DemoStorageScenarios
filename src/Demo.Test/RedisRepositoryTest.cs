using Demo.Core.Entities;
using Demo.Core.Repositories;
using NUnit.Framework;

namespace Demo.Test
{
    public class RedisRepositoryUnitTests
    {
        [Test]
        public void InsertUserShouldCorrect()
        {
            var user = new UserEntity
                {Id = 1, Code = "Admin", Password = "pass.123", IsActive = true};

            var sut = new RedisRepository();

            Assert.DoesNotThrowAsync(async () => await sut.InsertUser(user));
        }
        
        [Test]
        public void TestConvert()
        {
            var actualResult = 0;
            var sut = new RedisRepository();

            Assert.DoesNotThrowAsync(async () => actualResult = await sut.ConvertTest("foo", 1));

            Assert.AreEqual(111, actualResult);
        }
        
        [Test]
        public void TestTable()
        {
            string[] actualResult = null;
            var sut = new RedisRepository();

            Assert.DoesNotThrowAsync(async () => actualResult = await sut.TestTable("user:1"));

            Assert.AreEqual(4, actualResult.Length);
        }
    }
}
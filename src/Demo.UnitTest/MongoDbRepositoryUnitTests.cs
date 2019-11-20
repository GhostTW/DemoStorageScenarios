using System.Collections.Generic;
using Demo.Core;
using Demo.Data;
using NUnit.Framework;

namespace Demo.UnitTest
{
    public class MongoDbRepositoryUnitTests
    {
        private MongoDbRepository _dbRepository;
        
        [SetUp]
        public void SetUp()
        {
            _dbRepository = new MongoDbRepository();
            _dbRepository.DropUserCollection();
        }
        
        [Test]
        public void InsertUserShouldSuccess()
        {
            var user = new UserEntity {Id = 1, Code = "Admin", Password = "pass.123", IsActive = true};
            
            var sut = _dbRepository;
            
            Assert.DoesNotThrow(() => sut.InsertUser(user));
        }
        
        [Test]
        public void InsertUsersShouldSuccess()
        {
            var users = new[]
            {
                new UserEntity {Id = 2, Code = "Test2", Password = "pass.123", IsActive = true},
                new UserEntity{Id = 3, Code = "Test3", Password = "pass.123", IsActive = true}
            };
            
            var sut = _dbRepository;
            
            Assert.DoesNotThrow(() => sut.InsertUsers(users));
        }
        
        [Test]
        public void GetAllUsersShouldSuccess()
        {
            IEnumerable<UserEntity> actualResult = null;
            var sut = _dbRepository;
            
            Assert.DoesNotThrow(() => actualResult = sut.GetAllUsers());
            Assert.IsEmpty(actualResult);
        }
    }
}
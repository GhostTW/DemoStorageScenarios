using System.Collections.Generic;
using System.Threading.Tasks;
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
            //_dbRepository.DropUserCollection();
        }
        
        [Test]
        public async Task InsertUser_should_success()
        {
            var user = new UserEntity {Id = 1, Code = "Admin", Password = "pass.123", IsActive = true};

            using (var session = await _dbRepository.Connection.StartSessionAsync())
            {
                var sut = _dbRepository;
                
                sut.SetSession(session);
                session.StartTransaction();

                Assert.DoesNotThrow(() => sut.InsertUser(user));

                await session.AbortTransactionAsync();
            }
        }
        
        [Test]
        public void InsertUsers_should_success()
        {
            var users = new UserEntity[]
            {
                new UserEntity {Id = 2, Code = "Test2", Password = "pass.123", IsActive = true},
                new UserEntity{Id = 3, Code = "Test3", Password = "pass.123", IsActive = true}
            };
            
            var sut = _dbRepository;
            
            Assert.DoesNotThrow(() => sut.InsertUsers(users));
        }
        
        [Test]
        public void GetAllUsers_should_success()
        {
            IEnumerable<UserEntity> actualResult = null;
            var sut = _dbRepository;
            
            Assert.DoesNotThrow(() => actualResult = sut.GetAllUsers());
            Assert.IsEmpty(actualResult);
        }
    }
}
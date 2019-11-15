using System.Collections.Generic;
using Demo.Data;
using MongoDB.Driver;

namespace Demo.Core
{
    public class MongoDbRepository
    {
        public readonly MongoClient Connection;
        private const string ConnectionString = "mongodb://127.0.0.1:27017/TestDB?retryWrites=false";
        private const string DatabaseName = "TestDB";
        private const string CollectionName = "Users";
        private IClientSessionHandle _session;

        public MongoDbRepository()
        {
            Connection = new MongoClient(ConnectionString);
        }

        public void SetSession(IClientSessionHandle session)
        {
            _session = session;
        }

        public void InsertUser(UserEntity user)
        {
            var collection = GetUserCollection();
            if(_session !=null)
                collection.InsertOne(_session, user);
            else
                collection.InsertOne(user);
        }

        public void InsertUsers(IEnumerable<UserEntity> users)
        {
            var collection = GetUserCollection();
            collection.InsertMany(users);
        }
        
        public IEnumerable<UserEntity> GetAllUsers()
        {
            var collection = GetUserCollection();

            return collection.Find(_ => true).ToEnumerable();
        }

        public void DropUserCollection()
        {
            var database = GetDatabase();
            database.DropCollection(CollectionName);
        }

        private IMongoDatabase GetDatabase() => _session != null ? _session.Client.GetDatabase(DatabaseName) : Connection.GetDatabase(DatabaseName);

        private IMongoCollection<UserEntity> GetUserCollection() =>
            GetDatabase().GetCollection<UserEntity>(CollectionName);
    }
}
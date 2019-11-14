using System.Collections.Generic;
using Demo.Data;
using MongoDB.Driver;

namespace Demo.Core
{
    public class MongoDbRepository
    {
        public readonly MongoClient Connection;
        private const string ConnectionString = "mongodb://127.0.0.1:27017";
        private const string DatabaseName = "TestDB";
        private const string CollectionName = "Users";

        public MongoDbRepository()
        {
            Connection = new MongoClient(ConnectionString);
        }

        public void InsertUser(UserEntity user)
        {
            var collection = GetUserCollection();
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

        private IMongoDatabase GetDatabase() => Connection.GetDatabase(DatabaseName);

        private IMongoCollection<UserEntity> GetUserCollection() =>
            GetDatabase().GetCollection<UserEntity>(CollectionName);
    }
}
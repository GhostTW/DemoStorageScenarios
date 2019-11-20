# C# MariaDb MySqlConnector 實作練習並應用 Transaction, TransactionScope, Isolation Level.

以下程式放在 `src/Demo.Core/MariaDbRepository.cs` 及 `src/Demo.UnitTest/MariaDbRepositoryUnitTests.cs`

使用前須先在 `database` 資料夾下執行 `docker-compose up -d`

## 基本使用

使用 UnitTest 測試基本的讀取、新增、修改方式．
以下範例是透過 MySqlConnector 取得 User 資料表的所有資料，並驗證是否有值．
其餘範例在相關檔案裡．

```c#
public IEnumerable<UserEntity> GetUsers()
{
    if(_connection == null)
    {
        using (var connection = new MySqlConnection(ConnectionString))
        {
            connection.Open();
            return Execute(connection);
        }
    }
    else // 使用外部產生的 connection，transaction 也交由外部控制．
    {
        return Execute(_connection);
    }

    // local function 實作資料轉換
    IEnumerable<UserEntity> Execute(MySqlConnection conn)
    {
        using (var command = new MySqlCommand(StoredProcedures.SpUserGetUsers, conn))
        {
            var parameter = new MySqlParameter("@OUT_ReturnValue", MySqlDbType.Int32)
            {
                Direction = ParameterDirection.Output
            };

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(parameter);
            var reader = command.ExecuteReader();

            if (!reader.HasRows) return Enumerable.Empty<UserEntity>();

            var result = new List<UserEntity>();
            while (reader.Read())
            {
                var id = reader.GetInt32("Id");
                var code = reader.GetString("Code");
                var password = reader.GetString("Password");
                var isActive = reader.GetBoolean("IsActive");
                var user = new UserEntity {Id = id, Code = code, Password = password, IsActive = isActive};
                result.Add(user);
            }

            return result;
        }
    }
}
```

對應的 unit test

```C#
[Test]
public void GetUserShouldCorrect()
{
    // arrange
    var sut = new MariaDbRepository();

    // action
    var result = sut.GetUsers();
    var firstUser = result.FirstOrDefault();

    // assert
    Assert.IsNotNull(firstUser);
    Assert.AreEqual("Admin", firstUser.Code);
}
```

## Transaction Commit, Rollback 的使用

這個例子使用 connection transaction 去實現 Rollback 的功能．
1. 準備一筆要塞入的資料
2. 開啟 transaction
3. 新增資料
4. Rollback
5. 驗證 rollback 前，新增資料後與原本資料數是否相同．
6. 驗證 rollback 後，筆數是否與原本資料數是否相同．

```C#
[Test]
public void CreateUserRollbackShouldCorrect()
{
    // arrange
    var newUser = new UserEntity {Code = "FromUnitTest", Password = "pass.123", IsActive = false};
    UserEntity insertedUser;
    int originUserCount = 0, tempUserCount = 0, finalUserCount = 0;

    // action
    using (var connection = new MySqlConnection(ConnectionString))
    {
        connection.Open();
        using (var transaction = connection.BeginTransaction())
        {
            var sut = new MariaDbRepository(connection, transaction);
            originUserCount = sut.GetUsers().Count();
            insertedUser = sut.CreateUser(newUser);
            tempUserCount = sut.GetUsers().Count();

            if (transaction.Connection != null) transaction.Rollback();
            finalUserCount = sut.GetUsers().Count();
        }
    }

    // assert
    Assert.NotZero(insertedUser.Id);
    Assert.AreEqual(originUserCount + 1, tempUserCount);
    Assert.AreEqual(originUserCount, finalUserCount);
}
```

## TransactionScope
測試一個交易對不同 connection 操作的 rollback
對 User, Product 開啟不同的連線，各新增一筆資料，最後在一次 rollback 驗證資料是否跟新增前相同．

```C#
[Test]
public void InsertUserAndProductRollback()
{
    // arrange
    var newUser = new UserEntity {Code = "UnitTest_Scope", Password = "pass.123", IsActive = false};
    var newProduct = new ProductEntity {Name = nameof(CreateProductShouldCorrect), Amount = 10, AccountId = 2};
    UserEntity insertedUser;
    ProductEntity insertedProduct;
    int originUserCount = 0, tempUserCount = 0, finalUserCount = 0;
    int originProductCount = 0, tempProductCount = 0, finalProductCount = 0;

    // action
    using (var transactionScope = new TransactionScope(TransactionScopeOption.Required,
        new TransactionOptions {IsolationLevel = IsolationLevel.RepeatableRead},
        TransactionScopeAsyncFlowOption.Enabled))
    {
        var userRepo = new MariaDbRepository();
        var productRepo = new MariaDbRepository();

        originUserCount = userRepo.GetUsers().Count();
        insertedUser = userRepo.CreateUser(newUser);
        tempUserCount = userRepo.GetUsers().Count();

        originProductCount = productRepo.GetProducts().Count();
        insertedProduct = productRepo.CreateProduct(newProduct);
        tempProductCount = productRepo.GetProducts().Count();

        transactionScope.Dispose();
        finalUserCount = userRepo.GetUsers().Count();
        finalProductCount = productRepo.GetProducts().Count();
    }

    // assert
    Assert.NotZero(insertedUser.Id);
    Assert.AreEqual(originUserCount + 1, tempUserCount);
    Assert.AreEqual(originUserCount, finalUserCount);

    Assert.NotZero(insertedProduct.Id);
    Assert.AreEqual(originProductCount + 1, tempProductCount);
    Assert.AreEqual(originProductCount, finalProductCount);
}
```
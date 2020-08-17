using System;
using Xunit;
using Moq;
using MongoDB.Driver;
using PassphraseManagerSvc.Models;
using PassphraseManagerSvc.Repo;
using System.Linq.Expressions;
using System.Threading;

namespace PassphraseManagerSvcTests
{
    public class PassStoreMongoTests
    {
        Mock<IMongoClient> GetMockClient(out Mock<IMongoCollection<PasswordStoreModel>> collection)
        {
            var mclient = new Mock<IMongoClient>();
            mclient.DefaultValue = DefaultValue.Mock;

            collection = new Mock<IMongoCollection<PasswordStoreModel>>() { DefaultValue = DefaultValue.Mock };
            var mdb = new Mock<IMongoDatabase>() { DefaultValue = DefaultValue.Mock };
            mdb.Setup(mdb => mdb.GetCollection<PasswordStoreModel>(
                It.IsAny<string>(),
                It.IsAny<MongoCollectionSettings>())
            ).Returns(collection.Object)
            .Verifiable();

            mclient.Setup(mc => mc.GetDatabase(It.IsAny<string>(), It.IsAny<MongoDatabaseSettings>()))
                .Returns(mdb.Object)
                .Verifiable();

            return mclient;
        }

        [Fact]
        public void AddStoreItem_with_valid_store_model()
        {


            // arrange
            var mclient = GetMockClient(out Mock<IMongoCollection<PasswordStoreModel>> mcollection);
            var mrepo = new PassStoreMongo(mclient.Object);

            // act 
            var rslt = mrepo.AddStoreItem("key-1", new StoreItemModel() { Title = "title", UserName = "uname-1", Password = "pwd-1" })
                .GetAwaiter().GetResult();

            // assert
            mclient.VerifyAll();
            mcollection.Verify(mc => mc.UpdateOneAsync(It.IsAny<FilterDefinition<PasswordStoreModel>>(),
                It.IsAny<UpdateDefinition<PasswordStoreModel>>(), default(UpdateOptions), default(CancellationToken)),
                Times.Exactly(2));

        }

        [Fact]
        public void AddStoreItem_with_invalid_data()
        {
            // arrange
            var mclient = GetMockClient(out Mock<IMongoCollection<PasswordStoreModel>> mcollection);
            var mrepo = new PassStoreMongo(mclient.Object);

            // act and assert
            Assert.Throws<ArgumentNullException>(() => mrepo.AddStoreItem("", new StoreItemModel() { Title = "title", UserName = "uname-1", Password = "pwd-1" })
                .GetAwaiter().GetResult());

            Assert.Throws<ArgumentNullException>(() => mrepo.AddStoreItem("key-1", default(StoreItemModel))
                .GetAwaiter().GetResult());

        }

        [Fact]
        public void Create_with_valid_data()
        {
            // arrange
            var mclient = GetMockClient(out Mock<IMongoCollection<PasswordStoreModel>> mcollection);
            var mrepo = new PassStoreMongo(mclient.Object);

            // act 
            var model = new PasswordStoreModel() { Key = "key-1", Name = "name-1", PassPhrase = "somepwd" };
            var rslt = mrepo.Create(model)
                .GetAwaiter().GetResult();

            // assert
            mclient.VerifyAll();
            mcollection.Verify(mc => mc.FindAsync(It.IsAny<FilterDefinition<PasswordStoreModel>>(),
               default(FindOptions<PasswordStoreModel>), default(CancellationToken)), Times.Once);
            mcollection.Verify(mc => mc.InsertOneAsync(model, default(InsertOneOptions), default(CancellationToken)), Times.Once);

        }

        [Fact]
        public void Create_with_invalid_data()
        {
            // arrange
            var mclient = GetMockClient(out Mock<IMongoCollection<PasswordStoreModel>> mcollection);
            var mrepo = new PassStoreMongo(mclient.Object);

            // act // assert
            Assert.Throws<ArgumentNullException>(() => mrepo.Create(null)
               .GetAwaiter().GetResult());

        }

        [Fact]
        public void Delete_with_valid_data()
        {
            // arrange
            var mclient = GetMockClient(out Mock<IMongoCollection<PasswordStoreModel>> mcollection);
            var mrepo = new PassStoreMongo(mclient.Object);

            // act 
            var model = new PasswordStoreModel() { Key = "key-1", Name = "name-1", PassPhrase = "somepwd" };
            var rslt = mrepo.Delete(model)
                .GetAwaiter().GetResult();

            // assert
            mclient.VerifyAll();
            mcollection.Verify(mc => mc.FindOneAndDeleteAsync(It.IsAny<FilterDefinition<PasswordStoreModel>>(),
               default(FindOneAndDeleteOptions<PasswordStoreModel, PasswordStoreModel>), default(CancellationToken)), Times.Once);

        }

        [Fact]
        public void Delete_with_invalid_data()
        {
            // arrange
            var mclient = GetMockClient(out Mock<IMongoCollection<PasswordStoreModel>> mcollection);
            var mrepo = new PassStoreMongo(mclient.Object);

            // act // assert
            Assert.Throws<ArgumentNullException>(() => mrepo.Delete(null)
               .GetAwaiter().GetResult());

        }


        [Fact]
        public void DeleteStoreItem_with_valid_store_model()
        {
            // arrange
            var mclient = GetMockClient(out Mock<IMongoCollection<PasswordStoreModel>> mcollection);
            var mrepo = new PassStoreMongo(mclient.Object);

            // act 
            var rslt = mrepo.DeleteStoreItem("key-1", new StoreItemModel() { Title = "title", UserName = "uname-1", Password = "pwd-1" })
                .GetAwaiter().GetResult();

            // assert
            mclient.VerifyAll();
            mcollection.Verify(mc => mc.UpdateOneAsync(It.IsAny<FilterDefinition<PasswordStoreModel>>(),
                It.IsAny<UpdateDefinition<PasswordStoreModel>>(), default(UpdateOptions), default(CancellationToken)),
                Times.Once);
        }

        [Fact]
        public void DeleteStoreItem_with_invalid_data()
        {
            // arrange
            var mclient = GetMockClient(out Mock<IMongoCollection<PasswordStoreModel>> mcollection);
            var mrepo = new PassStoreMongo(mclient.Object);

            // act and assert
            Assert.Throws<ArgumentNullException>(() => mrepo.DeleteStoreItem("", new StoreItemModel() { Title = "title", UserName = "uname-1", Password = "pwd-1" })
                .GetAwaiter().GetResult());

            Assert.Throws<ArgumentNullException>(() => mrepo.DeleteStoreItem("key-1", default(StoreItemModel))
                .GetAwaiter().GetResult());

        }

        [Fact]
        public void GetAll()
        {
            // arrange
            var mclient = GetMockClient(out Mock<IMongoCollection<PasswordStoreModel>> mcollection);
            var mrepo = new PassStoreMongo(mclient.Object);

            // act 
            var rslt = mrepo.GetAll()
                .GetAwaiter().GetResult();

            // assert
            mclient.VerifyAll();
            mcollection.Verify(mc => mc.FindAsync(FilterDefinition<PasswordStoreModel>.Empty,
                default(FindOptions<PasswordStoreModel>), default(CancellationToken)),
                Times.Once);
        }

        [Fact]
        public void GetByKeyName_with_valid_key()
        {
            // arrange
            var mclient = GetMockClient(out Mock<IMongoCollection<PasswordStoreModel>> mcollection);
            var mrepo = new PassStoreMongo(mclient.Object);

            // act 
            var rslt = mrepo.GetByKeyName("key-1123")
                .GetAwaiter().GetResult();

            // assert
            mclient.VerifyAll();
            mcollection.Verify(mc => mc.FindAsync(It.IsAny<FilterDefinition<PasswordStoreModel>>(),
                default(FindOptions<PasswordStoreModel>), default(CancellationToken)),
                Times.Once);
        }

        [Fact]
        public void GetByKeyName_with_inalid_key()
        {
            // arrange
            var mclient = GetMockClient(out Mock<IMongoCollection<PasswordStoreModel>> mcollection);
            var mrepo = new PassStoreMongo(mclient.Object);

            // act and assert
            Assert.Throws<ArgumentNullException>(() => mrepo.GetByKeyName(" ")
                .GetAwaiter().GetResult());

            Assert.Throws<ArgumentNullException>(() => mrepo.GetByKeyName(null)
                .GetAwaiter().GetResult());

        }

        [Fact]
        public void Update_with_valid_passstore()
        {
            // arrange
            var mclient = GetMockClient(out Mock<IMongoCollection<PasswordStoreModel>> mcollection);
            var mrepo = new PassStoreMongo(mclient.Object);
            var model = new PasswordStoreModel()
            {
                Key = "key-1",
                Name = "name-1",
                PassPhrase = "somepwd"
                ,
                IsEncrypted = true,
                EncryptedBy = "1"
            };

            // act 
            var rslt = mrepo.Update(model)
                .GetAwaiter().GetResult();

            // assert
            mclient.VerifyAll();
            mcollection.Verify(mc => mc.UpdateOneAsync(It.IsAny<FilterDefinition<PasswordStoreModel>>(),
                It.IsAny<UpdateDefinition<PasswordStoreModel>>(), default(UpdateOptions), default(CancellationToken)),
                Times.Once);
        }

        [Fact]
        public void Update_with_invalid_data()
        {
            // arrange
            var mclient = GetMockClient(out Mock<IMongoCollection<PasswordStoreModel>> mcollection);
            var mrepo = new PassStoreMongo(mclient.Object);

            // act and assert
            Assert.Throws<ArgumentNullException>(() => mrepo.Update(null)
                .GetAwaiter().GetResult());

        }


        [Fact]
        public void GetItemsByCategory_with_valid_store_model()
        {
            // arrange
            var mclient = GetMockClient(out Mock<IMongoCollection<PasswordStoreModel>> mcollection);
            var mrepo = new PassStoreMongo(mclient.Object);

            // act 
            var rslt = mrepo.GetItemsByCategory("key-217533")
                .GetAwaiter().GetResult();

            // assert
            mclient.VerifyAll();
            mcollection.Verify(mc => mc.AggregateAsync(It.IsAny<PipelineDefinition<PasswordStoreModel, PasswordsCategory>>(),
                default(AggregateOptions), default(CancellationToken)),
                Times.Once);
        }

        [Fact]
        public void GetItemsByCategory_with_invalid_data()
        {
            // arrange
            var mclient = GetMockClient(out Mock<IMongoCollection<PasswordStoreModel>> mcollection);
            var mrepo = new PassStoreMongo(mclient.Object);

            // act and assert
            Assert.Throws<ArgumentNullException>(() => mrepo.GetItemsByCategory(null)
                .GetAwaiter().GetResult());

        }

    }
}

using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using PassphraseManagerSvc.Models;
using MongoDB.Bson.Serialization.Conventions;
using System.Linq.Expressions;
using PassphraseManagerSvc.Dto;
using MongoDB.Bson;

namespace PassphraseManagerSvc.Repo
{
    public class PassStoreMongo : IPassStoreRepo<PasswordStoreModel>
    {
        IMongoClient _client {get; set;}
        //const string _const_dbName = "keys";
        const string _const_dbName = "dbtest";
        const string _const_colName = "passwordstore";
        string db {get; set;}
        readonly IMongoDatabase DataBase;
        IMongoCollection<PasswordStoreModel> Collection => DataBase.GetCollection<PasswordStoreModel>(_const_colName);
        public PassStoreMongo(IMongoClient client, string dbName= "")
        {
            _client = client?? throw new ArgumentNullException(nameof(client));
            
            db = string.IsNullOrWhiteSpace(dbName)?_const_dbName: dbName;

            var camelCaseConvention = new ConventionPack { new CamelCaseElementNameConvention() };
            ConventionRegistry.Register("CamelCase", camelCaseConvention, type => true);
            DataBase = _client.GetDatabase(db);
            if(!DataBase.ListCollectionNames().ToList().Contains(_const_colName))
            {
                DataBase.CreateCollection(_const_colName);
            }
        }

        public async Task<TStoreItem> AddStoreItem<TStoreItem>(string storeKey, TStoreItem item)
        {
            return await AddOrUpdate(storeKey, item);
        }

        public async Task<PasswordStoreModel> Create(PasswordStoreModel model)
        {
            if(model == default(PasswordStoreModel))
                throw new ArgumentNullException(nameof(model));

            var filter = Builders<PasswordStoreModel>.Filter.Eq(m => m.Key , model.Key);
            var rslt = await Collection.FindAsync<PasswordStoreModel>(filter);
            
            var ud = new UpdateDefinitionBuilder<PasswordStoreModel>();
            
            if(rslt.Any())
                throw new PassStoreException(PassStoreException.ErrorCategory.InvalidInput, $"keystore key: '{model.Key}' already exists.");

            await Collection.InsertOneAsync(model);
            
            return model;
        }

        public async Task<bool> Delete(PasswordStoreModel model)
        {
            if(model == default(PasswordStoreModel))
                throw new ArgumentNullException(nameof(model));

            var filter = Builders<PasswordStoreModel>.Filter.Eq(m => m.Key, model.Key);
            var dmodel = await Collection.FindOneAndDeleteAsync(filter);

            return true;
        }

        public async Task<bool> DeleteStoreItem<TStoreItem>(string storeKey, TStoreItem item)
        {
            if(string.IsNullOrWhiteSpace(storeKey))
                throw new ArgumentNullException(nameof(storeKey));
            
            if(item == null)
                throw new ArgumentNullException(nameof(item));

            var ud = new UpdateDefinitionBuilder<PasswordStoreModel>();
            var model = item as StoreItemModel;
            var filter = Builders<PasswordStoreModel>.Filter.Eq(m => m.Key , storeKey);
            
            var rstl = await Collection.UpdateOneAsync(filter, ud.PullFilter(p => p.Passwords, pi => pi.Title.Equals(model.Title)));

            return rstl.IsAcknowledged;
        }

        public async Task<IList<PasswordStoreModel>> GetAll()
        {
            var rslt = await Collection.FindAsync<PasswordStoreModel>(FilterDefinition<PasswordStoreModel>.Empty);
            
            return rslt.ToList();
        }

        public async Task<PasswordStoreModel> GetByKeyName(string key)
        {
            if(string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            var filter = Builders<PasswordStoreModel>.Filter.Eq(m => m.Key ,key);
            var rslt = await Collection.FindAsync<PasswordStoreModel>(filter);

            return rslt.FirstOrDefault();
        }

        public async Task<IList<PasswordsCategory>> GetItemsByCategory(string storeKey)
        {
            if(string.IsNullOrWhiteSpace(storeKey))
                throw new ArgumentNullException(nameof(storeKey));
            
            var stgMatch = new BsonDocument("$match", 
                new BsonDocument("key", storeKey));

            var stgUnwind = new BsonDocument("$unwind", 
                new BsonDocument("path", "$passwords"));

            var stgGroup = new BsonDocument("$group", 
                new BsonDocument
                {
                    { "_id", "$passwords.category" }, 
                    { "passwords", new BsonDocument("$push", "$passwords") }
                });

            var stages = new[]{
                stgMatch,
                stgUnwind,
                stgGroup
            };
           
            var rslt = await Collection.AggregateAsync<PasswordsCategory>(stages);

            return rslt.ToList();
        }

        public async Task<PasswordStoreModel> Update(PasswordStoreModel model)
        {
            if(model == default(PasswordStoreModel))
                throw new ArgumentNullException(nameof(model));

            var filter = Builders<PasswordStoreModel>.Filter.Eq(m => m.Key , model.Key);
            var ud = new UpdateDefinitionBuilder<PasswordStoreModel>();
            var uds = ud.Set(p => p.EncryptedBy, model.EncryptedBy)
                .Set(p => p.IsEncrypted, model.IsEncrypted)
                .Set(p => p.Name, model.Name)
                .Set(p => p.PassPhrase, model.PassPhrase);
            
            if(model.Passwords != default(StoreItemModel[]) && model.Passwords.Length > 0)
                uds = uds.PushEach(p => p.Passwords, model.Passwords);
            
            var rslt = await Collection.UpdateOneAsync(filter, uds);
             
            return rslt.IsAcknowledged? model: null;
        }

        public async Task<TStoreItem> UpdateStoreItem<TStoreItem>(string storeKey, TStoreItem item)
        {
            return await AddOrUpdate(storeKey, item);
        }

        async Task<TStoreItem> AddOrUpdate<TStoreItem>(string storeKey, TStoreItem item)
        {
            if(string.IsNullOrWhiteSpace(storeKey))
                throw new ArgumentNullException(nameof(storeKey));
            
            if(item == null)
                throw new ArgumentNullException(nameof(item));

            var filter = Builders<PasswordStoreModel>.Filter.Eq(m => m.Key , storeKey);
            var model = item as StoreItemModel;
            var ud = new UpdateDefinitionBuilder<PasswordStoreModel>();
            
            await Collection.UpdateOneAsync(filter, ud.PullFilter(p => p.Passwords, pi => pi.Title.Equals(model.Title)));
            var rslt = await Collection.UpdateOneAsync(filter, ud.Push(p => p.Passwords, model));
             
            return rslt.IsAcknowledged? item: default(TStoreItem);
        }

        
    }

}
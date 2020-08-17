using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using PassphraseManagerSvc.Models;

namespace PassphraseManagerSvc.Repo
{
    public class PassStoreInMemory : IPassStoreRepo<PasswordStoreModel>
    {
        public ConcurrentDictionary<string, PasswordStoreModel> _store {get; set;}

        public PassStoreInMemory()
        {
            _store = new ConcurrentDictionary<string, PasswordStoreModel>();
        }

        public async Task<PasswordStoreModel> Create(PasswordStoreModel model)
        {
            return await new TaskFactory().StartNew(() => {

            if(!string.IsNullOrWhiteSpace(model.Id))
            if(_store.ContainsKey(model.Id))
                throw new InvalidOperationException(nameof(model));
            
            if(string.IsNullOrWhiteSpace(model.Id))
                model.Id = Guid.NewGuid().ToString().Substring(10);
            
            //deep copy model
            var dmodel = model;

            _store.AddOrUpdate(model.Id, k => dmodel, (k, m) => dmodel);
            
            //deep copy
            return _store[model.Id];

            });
        }

        public async Task<bool> Delete(PasswordStoreModel model)
        {
            return await new TaskFactory().StartNew(() => {

            if(model == default(PasswordStoreModel) || !_store.ContainsKey(model.Id))
                throw new ArgumentNullException(nameof(model));
            
            if(!_store.ContainsKey(model.Id))
                return false;
            _store.Remove(model.Id, out PasswordStoreModel m);

            return true;
            });
        }

        public async Task<IList<PasswordStoreModel>> GetAll()
        {
            return await new TaskFactory().StartNew(() => {
            // deep copy all

                return _store.Values.ToList();
            });
        }

        public async Task<PasswordStoreModel> GetByKeyName(string key)
        {
            return await new TaskFactory().StartNew(() => {

            if(string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            var item = _store.Where(kvp => key.Equals(kvp.Value.Key)).FirstOrDefault();
            
            // deep copy
            return item.Value;
            });
        }

        public async Task<PasswordStoreModel> Update(PasswordStoreModel model)
        {
            return await new TaskFactory().StartNew(() => {

            if(model == default(PasswordStoreModel) || !_store.ContainsKey(model.Id))
                throw new ArgumentNullException(nameof(model));
            
            if(!_store.ContainsKey(model.Id))
                return null;
            
            // deep copy

            _store[model.Id] = model;

            return model;
            });
        }

        public async Task<TStoreItem> AddStoreItem<TStoreItem>(string storeKey, TStoreItem item)
        {
            return await new TaskFactory().StartNew(() => {

            if(string.IsNullOrWhiteSpace(storeKey))
                throw new ArgumentNullException(nameof(storeKey));

            lock(_store)
            {
                var store = _store.Where(kvp => storeKey.Equals(kvp.Value.Key)).FirstOrDefault();
                
                var lst = new StoreItemModel[store.Value.Passwords.Length +1];
                store.Value.Passwords.CopyTo(lst, 0);
                lst[lst.Length -1] = item as StoreItemModel;
                store.Value.Passwords = lst;
                _store[store.Key] = store.Value;
            }
            
            return item;
            });
        }

        public async Task<TStoreItem> UpdateStoreItem<TStoreItem>(string storeKey, TStoreItem item)
        {
            return await new TaskFactory().StartNew(() => {

            if(string.IsNullOrWhiteSpace(storeKey))
                throw new ArgumentNullException(nameof(storeKey));

            lock(_store)
            {
                var store = _store.Where(kvp => storeKey.Equals(kvp.Value.Key)).FirstOrDefault();
                var model = item as StoreItemModel;

                for(int idx = 0; idx < store.Value.Passwords.Length; idx++)
                {
                    if(store.Value.Passwords[idx].Title.Equals(model.Title)){
                        store.Value.Passwords[idx] = model;
                        break;
                    }
                }
                
                _store[store.Key] = store.Value;
            }
            
            return item;
            });
        }

        public async Task<bool> DeleteStoreItem<TStoreItem>(string storeKey, TStoreItem item)
        {
            return await new TaskFactory().StartNew(() => {

            if(string.IsNullOrWhiteSpace(storeKey))
                throw new ArgumentNullException(nameof(storeKey));

            lock(_store)
            {
                var store = _store.Where(kvp => storeKey.Equals(kvp.Value.Key)).FirstOrDefault();
                var lst = store.Value.Passwords.ToList();
                var model = item as StoreItemModel;
                var di = lst.Where(p => p.Title.Equals(model.Title)).FirstOrDefault();
                if(di != default(StoreItemModel))
                    lst.Remove(di);

                store.Value.Passwords = lst.ToArray();
                _store[store.Key] = store.Value;
            }
            
            return true;
            });
        }

        public async Task<IList<PasswordsCategory>> GetItemsByCategory(string storeKey)
        {
            var store = await GetByKeyName(storeKey);

            return await new TaskFactory().StartNew(() => {
                
                return store.Passwords.GroupBy(si => si.Category)
                .ToList().ConvertAll(gi => new PasswordsCategory() 
                    { Category = gi.Key, Passwords = gi.ToArray() }).ToList();
            });
        }
    }

}
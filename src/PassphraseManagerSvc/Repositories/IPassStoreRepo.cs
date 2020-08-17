using System.Collections.Generic;
using System.Threading.Tasks;
using PassphraseManagerSvc.Models;

namespace PassphraseManagerSvc.Repo
{
    public interface IPassStoreRepo<TModel>
        where TModel : IModel, new()
    {
        Task<IList<TModel>> GetAll();
        Task<TModel> GetByKeyName(string key);
        Task<TModel> Create(TModel model);
        Task<TModel> Update(TModel model);
        Task<bool> Delete(TModel model);

        Task<TStoreItem> AddStoreItem<TStoreItem>(string storeKey, TStoreItem item);
        Task<TStoreItem> UpdateStoreItem<TStoreItem>(string storeKey, TStoreItem item);
        Task<bool> DeleteStoreItem<TStoreItem>(string storeKey, TStoreItem item);

        Task<IList<PasswordsCategory>> GetItemsByCategory(string storeKey);
    }

}
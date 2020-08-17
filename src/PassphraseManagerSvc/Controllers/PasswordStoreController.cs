using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PassphraseManagerSvc.Dto;
using PassphraseManagerSvc.Models;
using PassphraseManagerSvc.Repo;

namespace PassphraseManagerSvc.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PasswordStoreController : ControllerBase
    {
        
        private readonly ILogger<PasswordStoreController> _logger;
        private readonly IPassStoreRepo<PasswordStoreModel> _repo;
        private readonly IMapper _mapper;
        public PasswordStoreController(ILogger<PasswordStoreController> logger,
            IPassStoreRepo<PasswordStoreModel> repo, IMapper mapper)
        {
            _logger = logger;
            _repo = repo?? throw new PassStoreException(PassStoreException.ErrorCategory.Configuration, 
                "PassStoreRepo is not configured.");
            _mapper = mapper;
        }

        [HttpGet]
        [Route("/passstore")]
        public async Task<ApiResponeDto<PasswordStore>> GetByKeyName([FromQuery]string key)
        {
            if(string.IsNullOrWhiteSpace(key))
                throw new PassStoreException(PassStoreException.ErrorCategory.InvalidInput, nameof(key));

            var passtore = await _repo.GetByKeyName(key);

            if(passtore == default(PasswordStoreModel))
                return new ApiResponeDto<PasswordStore>()
                {
                    Result = null,
                    HasError = true,
                    ErrorDetails = new Error() { ErrorCode = "001", Message = "Given key is not found in PasswordStore."}
                };

            return new ApiResponeDto<PasswordStore>()
            {
                Result = _mapper.Map<PasswordStore>(passtore),
                HasError = false
            };
        }

        [HttpPost]
        [Route("/passstore/create")]
        public async Task<ApiResponeDto<string>> CreatePassStore([FromBody]PasswordStore storeDetails)
        {
            if(!this.TryValidateModel(storeDetails))
                throw new PassStoreException(PassStoreException.ErrorCategory.InvalidInput, nameof(storeDetails));

            var passtore = await _repo.Create(_mapper.Map<PasswordStoreModel>(storeDetails));

            return new ApiResponeDto<string>()
            {
                Result = passtore?.Id,
                HasError = false
            };
        }

        [HttpPost]
        [Route("/passstore/add")]
        public async Task<ApiResponeDto<string>> CreatePassStoreItem([FromQuery]string key, [FromBody]StoreItem storeItem)
        {
            if(string.IsNullOrWhiteSpace(key))
                throw new PassStoreException(PassStoreException.ErrorCategory.InvalidInput, nameof(key));

            if(!this.TryValidateModel(storeItem))
                throw new PassStoreException(PassStoreException.ErrorCategory.InvalidInput, nameof(storeItem));
            
            var model = await _repo.AddStoreItem(key, _mapper.Map<StoreItemModel>(storeItem));
            
            return new ApiResponeDto<string>()
            {
                Result = model.Title,
                HasError = false
            };
        }

        [HttpPut]
        [Route("/passstore/update")]
        public async Task<ApiResponeDto<string>> UpdatePassStoreItem([FromQuery]string key, [FromBody]StoreItem storeItem)
        {
            if(string.IsNullOrWhiteSpace(key))
                throw new PassStoreException(PassStoreException.ErrorCategory.InvalidInput, nameof(key));

            if(!this.TryValidateModel(storeItem))
                throw new PassStoreException(PassStoreException.ErrorCategory.InvalidInput, nameof(storeItem));

            var model = await _repo.UpdateStoreItem(key, _mapper.Map<StoreItemModel>(storeItem));
            
            return new ApiResponeDto<string>()
            {
                Result = model.Title,
                HasError = false
            };
        }

        [HttpDelete]
        [Route("/passstore/delete")]
        public async Task<ApiResponeDto<bool>> UpdatePassStoreItem([FromQuery]string key, [FromQuery]string itemTitle)
        {
            if(string.IsNullOrWhiteSpace(key))
                throw new PassStoreException(PassStoreException.ErrorCategory.InvalidInput, nameof(key));

            if(string.IsNullOrWhiteSpace(itemTitle))
                throw new PassStoreException(PassStoreException.ErrorCategory.InvalidInput, nameof(itemTitle));

            var rslt = await _repo.DeleteStoreItem(key, new StoreItemModel(){Title = itemTitle});
            
            return new ApiResponeDto<bool>()
            {
                Result = rslt,
                HasError = false
            };
        }

        [HttpGet]
        [Route("/passstore/bycategory")]
        public async Task<ApiResponeDto<IList<PasswordsCategory>>> GetGroupByCategory([FromQuery]string key)
        {
            if(string.IsNullOrWhiteSpace(key))
                throw new PassStoreException(PassStoreException.ErrorCategory.InvalidInput, nameof(key));

            var rslt = await _repo.GetItemsByCategory(key);
            
            return new ApiResponeDto<IList<PasswordsCategory>>()
            {
                Result = rslt,
                HasError = false
            };
        }
        

    }
}

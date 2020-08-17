using AutoMapper;
using PassphraseManagerSvc.Dto;
using PassphraseManagerSvc.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<StoreItemModel, StoreItem>().ReverseMap();
        CreateMap<PasswordStoreModel, PasswordStore>().ReverseMap();
        //CreateMap<PasswordStore, PasswordStoreModel>();
    }

}
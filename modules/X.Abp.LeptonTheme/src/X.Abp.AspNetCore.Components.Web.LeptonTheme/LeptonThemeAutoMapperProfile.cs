using AutoMapper;
using Volo.Abp.AspNetCore.Components.Web.LeptonTheme.Components.ApplicationLayout.Navigation;
using Volo.Abp.AutoMapper;
using Volo.Abp.UI.Navigation;

namespace X.Abp.AspNetCore.Components.Web.LeptonTheme
{
    public class LeptonThemeAutoMapperProfile : Profile
    {
        public LeptonThemeAutoMapperProfile()
        {
            CreateMap<ApplicationMenu, MenuViewModel>()
                .ForMember(vm => vm.Menu, cnf => cnf.MapFrom(x => x));

            CreateMap<ApplicationMenuItem, MenuItemViewModel>()
                .ForMember(vm => vm.MenuItem, cnf => cnf.MapFrom(x => x))
                .Ignore(vm => vm.IsActive)
                .Ignore(vm => vm.IsOpen)
                .Ignore(vm => vm.Parent);
        }
    }
}

using System.Threading.Tasks;
using Volo.Abp.LeptonTheme.Management;

namespace X.Abp.AspNetCore.Components.Web.LeptonTheme
{
    public interface ILeptonSettingsProvider
    {
        Task<bool> IsBoxedAsync();

        Task<MenuPlacement> GetMenuPlacementAsync();

        Task<MenuStatus> GetMenuStatusAsync();

        Task<LeptonStyle> GetStyleAsync();
    }
}

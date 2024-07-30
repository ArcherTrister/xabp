// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace X.Abp.FileManagement.Files;

public interface IPublicFileDescriptorAppService : IApplicationService
{
    // Task<IRemoteStreamContent> PreViewAsync(Guid id, string token);
    Task<RawFileDto> GetRawFileAsync(string name);

    Task<IRemoteStreamContent> GetFileAsync(string name);

    Task<UploadOutput> UploadAsync(UploadInput input);
}

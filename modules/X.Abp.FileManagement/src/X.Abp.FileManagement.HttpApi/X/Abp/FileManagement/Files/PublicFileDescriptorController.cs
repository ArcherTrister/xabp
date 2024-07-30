// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Asp.Versioning;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Content;

namespace X.Abp.FileManagement.Files;

[RemoteService(Name = AbpFileManagementRemoteServiceConsts.RemoteServiceName)]
[Area(AbpFileManagementRemoteServiceConsts.ModuleName)]
[ControllerName("PublicFileDescriptors")]
[Route("api/file-management/public-file-descriptor")]
[Authorize]
public class PublicFileDescriptorController : AbpControllerBase, IPublicFileDescriptorAppService
{
    protected IPublicFileDescriptorAppService PublicFileDescriptorAppService { get; }

    public PublicFileDescriptorController(IPublicFileDescriptorAppService publicFileDescriptorAppService)
    {
        PublicFileDescriptorAppService = publicFileDescriptorAppService;
    }

    [HttpGet]
    [Route("raw-file/{name}")]
    public virtual async Task<RawFileDto> GetRawFileAsync(string name)
    {
        // TODO: output cache would be good
        return await PublicFileDescriptorAppService.GetRawFileAsync(name);
    }

    [HttpGet]
    [Route("www/{name}")]
    [AllowAnonymous]
    public virtual async Task<IRemoteStreamContent> GetFileAsync(string name)
    {
        return await PublicFileDescriptorAppService.GetFileAsync(name);
    }

    [HttpPost]
    [Route("upload")]
    [Authorize]
    public virtual async Task<UploadOutput> UploadAsync(UploadInput input)
    {
        return await PublicFileDescriptorAppService.UploadAsync(input);
    }

    // [HttpGet]
    // [Route("pre-view/{id}")]
    // [AllowAnonymous]
    // public virtual async Task<IRemoteStreamContent> PreViewAsync(Guid id, string token)
    // {
    //    var media = await FileDescriptorAppService.PreViewAsync(id, token);
    //    //Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.ContentType] = media.ContentType;
    //    Response.Headers.Add("Content-Disposition", $"attachment;filename=\"{media.FileName}\"");
    //    Response.Headers.Add("Accept-Ranges", "bytes");
    //    Response.ContentType = media.ContentType;
    //    return media;
    // }
}

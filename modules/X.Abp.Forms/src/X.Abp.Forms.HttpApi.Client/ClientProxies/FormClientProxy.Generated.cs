// This file is automatically generated by ABP framework to use MVC Controllers from CSharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Content;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Http.Client.ClientProxying;
using Volo.Abp.Http.Modeling;
using X.Abp.Forms.Forms;
using X.Abp.Forms.Questions;
using X.Abp.Forms.Responses;

// ReSharper disable once CheckNamespace
namespace X.Abp.Forms.Forms;

[Dependency(ReplaceServices = true)]
[ExposeServices(typeof(IFormAppService), typeof(FormClientProxy))]
public partial class FormClientProxy : ClientProxyBase<IFormAppService>, IFormAppService
{
    public virtual async Task<PagedResultDto<FormDto>> GetListAsync(GetFormListInputDto input)
    {
        return await RequestAsync<PagedResultDto<FormDto>>(nameof(GetListAsync), new ClientProxyRequestTypeValue
        {
            { typeof(GetFormListInputDto), input }
        });
    }

    public virtual async Task<PagedResultDto<FormResponseDetailedDto>> GetResponsesAsync(Guid id, GetResponseListInputDto input)
    {
        return await RequestAsync<PagedResultDto<FormResponseDetailedDto>>(nameof(GetResponsesAsync), new ClientProxyRequestTypeValue
        {
            { typeof(Guid), id },
            { typeof(GetResponseListInputDto), input }
        });
    }

    public virtual async Task<IRemoteStreamContent> GetCsvResponsesAsync(Guid id, GetResponseListInputDto input)
    {
        return await RequestAsync<IRemoteStreamContent>(nameof(GetCsvResponsesAsync), new ClientProxyRequestTypeValue
        {
            { typeof(Guid), id },
            { typeof(GetResponseListInputDto), input }
        });
    }

    public virtual async Task<long> GetResponsesCountAsync(Guid id)
    {
        return await RequestAsync<long>(nameof(GetResponsesCountAsync), new ClientProxyRequestTypeValue
        {
            { typeof(Guid), id }
        });
    }

    public virtual async Task DeleteAllResponsesOfFormAsync(Guid id)
    {
        await RequestAsync(nameof(DeleteAllResponsesOfFormAsync), new ClientProxyRequestTypeValue
        {
            { typeof(Guid), id }
        });
    }

    public virtual async Task SendInviteEmailAsync(FormInviteEmailInputDto input)
    {
        await RequestAsync(nameof(SendInviteEmailAsync), new ClientProxyRequestTypeValue
        {
            { typeof(FormInviteEmailInputDto), input }
        });
    }

    public virtual async Task<FormDto> CreateAsync(CreateFormDto input)
    {
        return await RequestAsync<FormDto>(nameof(CreateAsync), new ClientProxyRequestTypeValue
        {
            { typeof(CreateFormDto), input }
        });
    }

    public virtual async Task<FormWithDetailsDto> GetAsync(Guid id)
    {
        return await RequestAsync<FormWithDetailsDto>(nameof(GetAsync), new ClientProxyRequestTypeValue
        {
            { typeof(Guid), id }
        });
    }

    public virtual async Task<FormDto> UpdateAsync(Guid id, UpdateFormDto input)
    {
        return await RequestAsync<FormDto>(nameof(UpdateAsync), new ClientProxyRequestTypeValue
        {
            { typeof(Guid), id },
            { typeof(UpdateFormDto), input }
        });
    }

    public virtual async Task SetSettingsAsync(Guid id, UpdateFormSettingInputDto input)
    {
        await RequestAsync(nameof(SetSettingsAsync), new ClientProxyRequestTypeValue
        {
            { typeof(Guid), id },
            { typeof(UpdateFormSettingInputDto), input }
        });
    }

    public virtual async Task<FormSettingsDto> GetSettingsAsync(Guid formId)
    {
        return await RequestAsync<FormSettingsDto>(nameof(GetSettingsAsync), new ClientProxyRequestTypeValue
        {
            { typeof(Guid), formId }
        });
    }

    public virtual async Task<List<QuestionDto>> GetQuestionsAsync(Guid id, GetQuestionListDto input)
    {
        return await RequestAsync<List<QuestionDto>>(nameof(GetQuestionsAsync), new ClientProxyRequestTypeValue
        {
            { typeof(Guid), id },
            { typeof(GetQuestionListDto), input }
        });
    }

    public virtual async Task<QuestionDto> CreateQuestionAsync(Guid id, CreateQuestionDto input)
    {
        return await RequestAsync<QuestionDto>(nameof(CreateQuestionAsync), new ClientProxyRequestTypeValue
        {
            { typeof(Guid), id },
            { typeof(CreateQuestionDto), input }
        });
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        await RequestAsync(nameof(DeleteAsync), new ClientProxyRequestTypeValue
        {
            { typeof(Guid), id }
        });
    }
}

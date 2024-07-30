// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Volo.Abp.Features;

using X.Abp.Forms.Forms;

namespace X.Abp.Forms.Questions;

[RequiresFeature(FormsFeatures.Enable)]
public class QuestionAppService : FormsAppServiceBase, IQuestionAppService
{
    protected QuestionManager QuestionManager { get; }

    protected IFormRepository FormRepository { get; }

    protected IQuestionRepository QuestionRepository { get; }

    public QuestionAppService(
        QuestionManager questionManager,
        IFormRepository formRepository,
        IQuestionRepository questionRepository)
    {
        QuestionManager = questionManager;
        FormRepository = formRepository;
        QuestionRepository = questionRepository;
    }

    public async Task<List<QuestionDto>> GetListAsync(GetQuestionListDto input)
    {
        var items = await QuestionRepository.GetListAsync();
        return ObjectMapper.Map<List<QuestionBase>, List<QuestionDto>>(items);
    }

    public async Task<QuestionDto> GetAsync(Guid id)
    {
        var item = await QuestionRepository.GetAsync(id, includeDetails: true);

        return ObjectMapper.Map<QuestionBase, QuestionDto>(item);
    }

    public async Task<QuestionDto> UpdateAsync(Guid id, UpdateQuestionDto input)
    {
        var choiceList = new List<(Guid, string, bool)>();

        foreach (var choice in input.Choices)
        {
            if (choice.Id == Guid.Empty)
            {
                choice.Id = GuidGenerator.Create();
            }

            choiceList.Add((choice.Id, choice.Value, choice.IsCorrect));
        }

        var updatedQuestion = await QuestionManager.UpdateAsync(
            id,
            input.Title,
            input.Index,
            input.IsRequired,
            input.Description,
            input.QuestionType,
            input.HasOtherOption,
            choiceList);

        return ObjectMapper.Map<QuestionBase, QuestionDto>(updatedQuestion);
    }

    public async Task DeleteAsync(Guid id)
    {
        var item = await QuestionRepository.GetAsync(id);

        await QuestionManager.DeleteAsync(item);
    }
}

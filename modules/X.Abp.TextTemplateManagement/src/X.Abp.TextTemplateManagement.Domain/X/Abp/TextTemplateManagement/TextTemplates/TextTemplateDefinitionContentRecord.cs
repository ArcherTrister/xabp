// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace X.Abp.TextTemplateManagement.TextTemplates;

public class TextTemplateDefinitionContentRecord : Entity<Guid>
{
    public Guid DefinitionId { get; internal set; }

    public string FileName { get; set; }

    public string FileContent { get; set; }

    protected TextTemplateDefinitionContentRecord()
    {
    }

    public TextTemplateDefinitionContentRecord(
      Guid id,
      Guid definitionId,
      string fileName,
      string fileContent)
      : base(id)
    {
        DefinitionId = definitionId;
        FileName = Check.NotNullOrWhiteSpace(fileName, nameof(fileName), TemplateDefinitionContentRecordConsts.MaxFileNameLength);
        FileContent = fileContent;
    }

    public bool HasSameData(TextTemplateDefinitionContentRecord otherRecord)
    {
        return !(DefinitionId != otherRecord.DefinitionId) && !(FileName != otherRecord.FileName) && !(FileContent != otherRecord.FileContent);
    }

    public void Patch(TextTemplateDefinitionContentRecord otherRecord)
    {
        if (DefinitionId != otherRecord.DefinitionId)
        {
            DefinitionId = otherRecord.DefinitionId;
        }

        if (FileName != otherRecord.FileName)
        {
            FileName = otherRecord.FileName;
        }

        if (!(FileContent != otherRecord.FileContent))
        {
            return;
        }

        FileContent = otherRecord.FileContent;
    }
}

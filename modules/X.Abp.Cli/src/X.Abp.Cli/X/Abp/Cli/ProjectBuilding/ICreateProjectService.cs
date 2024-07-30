// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Volo.Abp.Cli.ProjectBuilding;

namespace X.Abp.Cli.ProjectBuilding;

public interface ICreateProjectService
{
    Task CreateAsync(ProjectBuildArgs createArgs);
}

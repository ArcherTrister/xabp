// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

using X.Abp.Forms.Forms;
using X.Abp.Forms.Questions;

namespace X.Abp.Forms.EntityFrameworkCore;

[DependsOn(
    typeof(AbpFormsDomainModule),
    typeof(AbpEntityFrameworkCoreModule))]
public class AbpFormsEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<FormsDbContext>(options =>
        {
            /* Add custom repositories here. Example:
             * options.AddRepository<Question, EfCoreQuestionRepository>();
             */
            options.AddDefaultRepositories<IFormsDbContext>();

            options.AddRepository<Form, EfCoreFormRepository>();
            options.AddRepository<QuestionBase, EfCoreQuestionRepository>();
        });
    }
}

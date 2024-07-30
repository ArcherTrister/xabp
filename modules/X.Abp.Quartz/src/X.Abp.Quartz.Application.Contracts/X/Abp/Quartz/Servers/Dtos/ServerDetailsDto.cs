// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;

using Quartz;

namespace X.Abp.Quartz.Servers.Dtos;

public class ServerDetailsDto
{
    public ServerDetailsDto(IEnumerable<IScheduler> schedulers)
    {
        Name = Environment.MachineName;
        Address = "localhost";
        Schedulers = schedulers.Select(x => x.SchedulerName).ToList();
    }

    public string Name { get; private set; }

    public string Address { get; private set; }

    public IReadOnlyList<string> Schedulers { get; set; }
}

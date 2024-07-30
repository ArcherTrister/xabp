// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

/*
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Quartz.Impl;

using Volo.Abp;

using X.Abp.Quartz.Servers.Dtos;

namespace X.Abp.Quartz
{
    /// <summary>
    /// Web API endpoint for scheduler information.
    /// </summary>
    [Area(QuartzRemoteServiceConsts.ModuleName)]
    [RemoteService(Name = QuartzRemoteServiceConsts.RemoteServiceName)]
    public class ServersController : QuartzController
    {
        [HttpGet]
        [Route("api/servers")]
        public IList<ServerHeaderDto> AllServers()
        {
            var servers = ServerRepository.LookupAll();

            return servers.Select(x => new ServerHeaderDto(x)).ToList();
        }

        [HttpGet]
        [Route("api/server/{serverName}/details")]
        public async Task<ServerDetailsDto> ServerDetails(string serverName)
        {
            var schedulers = await SchedulerRepository.Instance.LookupAll();
            return new ServerDetailsDto(schedulers);
        }
    }
}
*/

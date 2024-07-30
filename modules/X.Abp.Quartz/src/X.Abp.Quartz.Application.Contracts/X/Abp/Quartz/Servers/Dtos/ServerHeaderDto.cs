// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.Quartz.Servers.Dtos;

public class ServerHeaderDto
{
    public ServerHeaderDto(Server server)
    {
        Name = server.Name;
        Address = server.Address;
    }

    public string Name { get; private set; }

    public string Address { get; private set; }
}

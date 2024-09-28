// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

using Volo.Abp.DependencyInjection;

namespace X.Abp.Notification.RealTime
{
  public class InMemoryOnlineClientStore<T> : InMemoryOnlineClientStore, IOnlineClientStore<T>
  {
  }
}

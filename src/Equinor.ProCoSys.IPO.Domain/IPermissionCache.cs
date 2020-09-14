﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Equinor.ProCoSys.IPO.Domain
{
    public interface IPermissionCache
    {
        Task<IList<string>> GetPermissionsForUserAsync(string plantId, Guid userOid);
        Task<IList<string>> GetProjectNamesForUserOidAsync(string plantId, Guid userOid);
        void ClearAll(string plantId, Guid userOid);
    }
}

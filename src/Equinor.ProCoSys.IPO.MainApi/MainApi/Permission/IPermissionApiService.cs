﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Equinor.ProCoSys.IPO.ForeignApi.MainApi.Permission
{
    public interface IPermissionApiService
    {
        Task<IList<string>> GetPermissionsAsync(string plantId);
        Task<IList<ProCoSysProject>> GetAllOpenProjectsAsync(string plantId);
    }
}

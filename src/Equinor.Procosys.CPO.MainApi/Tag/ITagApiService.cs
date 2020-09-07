﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Equinor.Procosys.CPO.MainApi.Tag
{
    public interface ITagApiService
    {
        Task<IList<ProcosysTagDetails>> GetTagDetailsAsync(string plant, string projectName, IList<string> tagNos);
        Task<IList<ProcosysTagOverview>> SearchTagsByTagNoAsync(string plant, string projectName, string startsWithTagNo);
        Task<IList<ProcosysPreservedTag>> GetPreservedTagsAsync(string plant, string projectName);
        Task<IList<ProcosysTagOverview>> SearchTagsByTagFunctionsAsync(string plant, string projectName, IList<string> tagFunctionCodeRegisterCodePairs);
        Task MarkTagsAsMigratedAsync(string plant, IEnumerable<long> tagIds);
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Equinor.ProCoSys.IPO.MainApi.Client;
using Equinor.ProCoSys.IPO.MainApi.Plant;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Equinor.ProCoSys.IPO.MainApi.Tag
{
    public class MainApiTagService : ITagApiService
    {
        private readonly string _apiVersion;
        private readonly Uri _baseAddress;
        private readonly IBearerTokenApiClient _mainApiClient;
        private readonly IPlantCache _plantCache;
        private readonly int _tagSearchPageSize;

        public MainApiTagService(
            IBearerTokenApiClient mainApiClient,
            IPlantCache plantCache,
            IOptionsMonitor<MainApiOptions> options,
            ILogger<MainApiTagService> logger)
        {
            _mainApiClient = mainApiClient;
            _plantCache = plantCache;
            _apiVersion = options.CurrentValue.ApiVersion;
            _baseAddress = new Uri(options.CurrentValue.BaseAddress);
            if (options.CurrentValue.TagSearchPageSize < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(options.CurrentValue.TagSearchPageSize), "Must be a positive number.");
            }
            _tagSearchPageSize = options.CurrentValue.TagSearchPageSize;
            if (_tagSearchPageSize < 100)
            {
                logger.LogWarning("Tag search page size is set to a low value. This may impact the overall performance!");
            }
            if (_tagSearchPageSize <= 0)
            {
                throw new Exception($"{nameof(options.CurrentValue.TagSearchPageSize)} can't be zero or negative");
            }
        }

        public async Task<IList<ProCoSysTagDetails>> GetTagDetailsAsync(string plant, string projectName, IList<string> allTagNos)
        {
            if (allTagNos == null)
            {
                throw new ArgumentNullException(nameof(allTagNos));

            }
            if (!await _plantCache.IsValidPlantForCurrentUserAsync(plant))
            {
                throw new ArgumentException($"Invalid plant: {plant}");
            }

            var baseUrl = $"{_baseAddress}Tag/ByTagNos" +
                $"?plantId={plant}" +
                $"&projectName={WebUtility.UrlEncode(projectName)}" +
                $"&api-version={_apiVersion}";

            var tagDetails = new List<ProCoSysTagDetails>();
            var page = 0;
            // Use relative small page size since TagNos are added to querystring of url and maxlength is 2000
            var pageSize = 50;
            IEnumerable<string> pageWithTagNos;
            do
            {
                pageWithTagNos = allTagNos.Skip(pageSize * page).Take(pageSize).ToList();

                if (pageWithTagNos.Any())
                {
                    var url = baseUrl;
                    foreach (var tagNo in pageWithTagNos)
                    {
                        url += $"&tagNos={WebUtility.UrlEncode(tagNo)}";
                    }

                    var tagDetailsPage = await _mainApiClient.QueryAndDeserializeAsync<List<ProCoSysTagDetails>>(url);
                    tagDetails.AddRange(tagDetailsPage);
                }

                page++;

            } while (pageWithTagNos.Count() == pageSize);
            return tagDetails;
        }

        public async Task<IList<ProCoSysPreservedTag>> GetPreservedTagsAsync(string plant, string projectName)
        {
            if (!await _plantCache.IsValidPlantForCurrentUserAsync(plant))
            {
                throw new ArgumentException($"Invalid plant: {plant}");
            }

            var url = $"{_baseAddress}IPOTags" +
                      $"?plantId={plant}" +
                      $"&projectName={WebUtility.UrlEncode(projectName)}" +
                      $"&api-version={_apiVersion}";
            return await _mainApiClient.QueryAndDeserializeAsync<List<ProCoSysPreservedTag>>(url);
        }

        public async Task<IList<ProCoSysTagOverview>> SearchTagsByTagNoAsync(string plant, string projectName, string startsWithTagNo)
        {
            if (!await _plantCache.IsValidPlantForCurrentUserAsync(plant))
            {
                throw new ArgumentException($"Invalid plant: {plant}");
            }

            var items = new List<ProCoSysTagOverview>();
            var currentPage = 0;
            ProCoSysTagSearchResult tagSearchResult;
            do
            {
                var url = $"{_baseAddress}Tag/Search" +
                    $"?plantId={plant}" +
                    $"&startsWithTagNo={WebUtility.UrlEncode(startsWithTagNo)}" +
                    $"&projectName={WebUtility.UrlEncode(projectName)}" +
                    $"&currentPage={currentPage++}" +
                    $"&itemsPerPage={_tagSearchPageSize}" +
                    $"&api-version={_apiVersion}";
                tagSearchResult = await _mainApiClient.QueryAndDeserializeAsync<ProCoSysTagSearchResult>(url);
                if (tagSearchResult?.Items != null && tagSearchResult.Items.Any())
                {
                    items.AddRange(tagSearchResult.Items);
                }
            } while (tagSearchResult != null && items.Count < tagSearchResult.MaxAvailable);
            return items;
        }

        public async Task<IList<ProCoSysTagOverview>> SearchTagsByTagFunctionsAsync(string plant, string projectName, IList<string> tagFunctionCodeRegisterCodePairs)
        {
            if (!await _plantCache.IsValidPlantForCurrentUserAsync(plant))
            {
                throw new ArgumentException($"Invalid plant: {plant}");
            }

            var items = new List<ProCoSysTagOverview>();
            var currentPage = 0;
            ProCoSysTagSearchResult tagSearchResult;
            do
            {
                var url = $"{_baseAddress}Tag/Search/ByTagFunction" +
                          $"?plantId={plant}" +
                          $"&projectName={WebUtility.UrlEncode(projectName)}" +
                          $"&currentPage={currentPage++}" +
                          $"&itemsPerPage={_tagSearchPageSize}" +
                          $"&api-version={_apiVersion}";
                foreach (var tagFunctionCodeRegisterCodePair in tagFunctionCodeRegisterCodePairs)
                {
                    url += $"&tagFunctionCodeRegisterCodePairs={tagFunctionCodeRegisterCodePair}";
                }

                tagSearchResult = await _mainApiClient.QueryAndDeserializeAsync<ProCoSysTagSearchResult>(url);
                if (tagSearchResult?.Items != null && tagSearchResult.Items.Any())
                {
                    items.AddRange(tagSearchResult.Items);
                }
            } while (tagSearchResult != null && items.Count < tagSearchResult.MaxAvailable);
            return items;
        }

        public async Task MarkTagsAsMigratedAsync(string plant, IEnumerable<long> tagIds)
        {
            if (!await _plantCache.IsValidPlantForCurrentUserAsync(plant))
            {
                throw new ArgumentException($"Invalid plant: {plant}");
            }

            var url = $"{_baseAddress}IPOTags" +
                      $"?plantId={plant}" +
                      $"&api-version={_apiVersion}";

            var json = JsonSerializer.Serialize(tagIds);
            await _mainApiClient.PutAsync(url, new StringContent(json, Encoding.Default, "application/json"));
        }
    }
}

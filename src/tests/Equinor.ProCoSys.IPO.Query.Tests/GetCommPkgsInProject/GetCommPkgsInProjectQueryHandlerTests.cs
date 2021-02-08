﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.IPO.ForeignApi.MainApi.CommPkg;
using Equinor.ProCoSys.IPO.Infrastructure;
using Equinor.ProCoSys.IPO.Query.GetCommPkgsInProject;
using Equinor.ProCoSys.IPO.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ServiceResult;

namespace Equinor.ProCoSys.IPO.Query.Tests.GetCommPkgsInProject
{
    [TestClass]
    public class SearchCommPkgsByCommPkgNoQueryHandlerTests : ReadOnlyTestsBase
    {
        private Mock<ICommPkgApiService> _commPkgApiServiceMock;
        private IList<ProCoSysCommPkg> _mainApiCommPkgs;
        private GetCommPkgsInProjectQuery _query;

        private readonly string _projectName = "Pname";
        private readonly string _commPkgStartsWith = "C";
        private readonly int _defaultPageSize = 10;
        private readonly int _defaultCurrentPage = 0;

        protected override void SetupNewDatabase(DbContextOptions<IPOContext> dbContextOptions)
        {
            using (new IPOContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                _commPkgApiServiceMock = new Mock<ICommPkgApiService>();
                _mainApiCommPkgs = new List<ProCoSysCommPkg>
                {
                    new ProCoSysCommPkg
                    {
                        Id = 1, CommPkgNo = "CommPkgNo1", Description = "Desc1", CommStatus = "PB"
                    },
                    new ProCoSysCommPkg
                    {
                        Id = 2, CommPkgNo = "CommPkgNo2", Description = "Desc2", CommStatus = "OK"
                    },
                    new ProCoSysCommPkg
                    {
                        Id = 3, CommPkgNo = "CommPkgNo3", Description = "Desc3", CommStatus = "PA"
                    }
                };

                var result = new ProCoSysCommPkgSearchResult {MaxAvailable = 3, Items = _mainApiCommPkgs};

                _commPkgApiServiceMock
                    .Setup(x => x.SearchCommPkgsByCommPkgNoAsync(TestPlant, _projectName, _commPkgStartsWith, _defaultPageSize, _defaultCurrentPage))
                    .Returns(Task.FromResult(result));

                _query = new GetCommPkgsInProjectQuery(_projectName, _commPkgStartsWith, _defaultPageSize, _defaultCurrentPage);
            }
        }

        [TestMethod]
        public async Task Handle_ShouldReturnOkResult()
        {
            using (new IPOContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetCommPkgsInProjectQueryHandler(_commPkgApiServiceMock.Object, _plantProvider);
                var result = await dut.Handle(_query, default);

                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }
        }

        [TestMethod]
        public async Task Handle_ShouldReturnCorrectItems()
        {
            using (new IPOContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetCommPkgsInProjectQueryHandler(_commPkgApiServiceMock.Object, _plantProvider);
                var result = await dut.Handle(_query, default);

                Assert.AreEqual(3, result.Data.CommPkgs.Count);
                Assert.AreEqual(3, result.Data.MaxAvailable);
                var item1 = result.Data.CommPkgs.ElementAt(0);
                var item2 = result.Data.CommPkgs.ElementAt(1);
                var item3 = result.Data.CommPkgs.ElementAt(2);
                AssertCommPkgData(_mainApiCommPkgs.Single(c => c.CommPkgNo == item1.CommPkgNo), item1);
                AssertCommPkgData(_mainApiCommPkgs.Single(t => t.CommPkgNo == item2.CommPkgNo), item2);
                AssertCommPkgData(_mainApiCommPkgs.Single(t => t.CommPkgNo == item3.CommPkgNo), item3);
            }
        }

        [TestMethod]
        public async Task Handle_ShouldReturnEmptyList_WhenSearchReturnsNull()
        {
            using (new IPOContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetCommPkgsInProjectQueryHandler(_commPkgApiServiceMock.Object, _plantProvider);
                _commPkgApiServiceMock
                    .Setup(x => x.SearchCommPkgsByCommPkgNoAsync(TestPlant, _projectName, _commPkgStartsWith, _defaultPageSize, _defaultCurrentPage))
                    .Returns(Task.FromResult<ProCoSysCommPkgSearchResult>(new ProCoSysCommPkgSearchResult
                    {
                        MaxAvailable = 0,
                        Items = null
                    }));

                var result = await dut.Handle(_query, default);

                Assert.AreEqual(ResultType.Ok, result.ResultType);
                Assert.AreEqual(0, result.Data.CommPkgs.Count);
            }
        }

        private void AssertCommPkgData(ProCoSysCommPkg PCSCommPkg, ProCoSysCommPkgDto commPkgDto)
        {
            Assert.AreEqual(PCSCommPkg.Id, commPkgDto.Id);
            Assert.AreEqual(PCSCommPkg.CommPkgNo, commPkgDto.CommPkgNo);
            Assert.AreEqual(PCSCommPkg.Description, commPkgDto.Description);
            Assert.AreEqual(PCSCommPkg.CommStatus, commPkgDto.Status);
        }
    }
}

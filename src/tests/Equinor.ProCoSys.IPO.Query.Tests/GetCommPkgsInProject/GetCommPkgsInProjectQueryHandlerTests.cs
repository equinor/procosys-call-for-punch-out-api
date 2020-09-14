﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.IPO.Infrastructure;
using Equinor.ProCoSys.IPO.MainApi.CommPkg;
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

        private readonly int _projectId = 1;
        private readonly string _commPkgStartsWith = "C";

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

                _commPkgApiServiceMock
                    .Setup(x => x.SearchCommPkgsByCommPkgNoAsync(TestPlant, _projectId, _commPkgStartsWith))
                    .Returns(Task.FromResult(_mainApiCommPkgs));

                _query = new GetCommPkgsInProjectQuery(_projectId, _commPkgStartsWith);
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

                Assert.AreEqual(3, result.Data.Count);
                var item1 = result.Data.ElementAt(0);
                var item2 = result.Data.ElementAt(1);
                var item3 = result.Data.ElementAt(2);
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
                    .Setup(x => x.SearchCommPkgsByCommPkgNoAsync(TestPlant, _projectId, _commPkgStartsWith))
                    .Returns(Task.FromResult<IList<ProCoSysCommPkg>>(null));

                var result = await dut.Handle(_query, default);

                Assert.AreEqual(ResultType.Ok, result.ResultType);
                Assert.AreEqual(0, result.Data.Count);
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
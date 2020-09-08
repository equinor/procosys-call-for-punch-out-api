﻿using System;
using System.Diagnostics;

namespace Equinor.ProCoSys.IPO.MainApi.Tag
{
    [DebuggerDisplay("{TagNo}")]
    public class ProCoSysPreservedTag
    {
        public long Id { get; set; }
        public string CommPkgNo { get; set; }
        public string Description { get; set; }
        public string McPkgNo { get; set; }
        public string PurchaseOrderTitle { get; set; }
        public string RegisterCode { get; set; }
        public string TagFunctionCode { get; set; }
        public string TagNo { get; set; }
        public string MccrResponsibleCodes { get; set; }
        public string IPORemark { get; set; }
        public string StorageArea { get; set; }
        public string ModeCode { get; set; }
        public bool Heating { get; set; }
        public bool Special { get; set; }
        public DateTime? NextUpcommingDueTime { get; set; }
        public DateTime? StartDate { get; set; }
    }
}

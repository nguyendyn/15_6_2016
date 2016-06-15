using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PM_ASVN.Common
{
    public class Pagination
    {
        public int totalRecords { get; set; }
        public int pageNum { get; set; }
        public int pageSize { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PM_ASVN.Models
{
    public enum Types
    {
        Project = 1,
        Feature = 2,
        Function = 3,
        Table = 4,
        DataColumn = 5,
        Store = 6,
        Webpage = 7,
        Database = 8,
        BackendJob = 9,
        Ticket = 10,
        ReleasePackage = 11,
        Estimation = 12,
        WorkGroup = 14,
        WorkItem = 15,
        Process = 16,
        Task = 17,
        Component = 18,
        TestCase = 19,
		Bug = 20,
        User =21,
        Browser = 22,
        TypeTestCase = 23,
        TestCaseGroup = 24,
	    StepInTestCase = 25,
        TCPriority = 26,
        AttachFile = 27,
        StatusTicket = 28,
        BugType = 29,
        TicketPriority = 30,
        StatusTestCase = 31,
        StatusTask = 32,
        BugStatus = 33,
        BugReason = 34,
    }
}
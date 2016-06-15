using aperia.core.business;
using PM_ASVN.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages;

namespace PM_ASVN.Models
{
    public class ItemModel
    {
        public Item Data { get; set; }

        public Project ProjectData { get; set; }
        public Task TaskData { get; set; }
        public TestCase TestCaseData { get; set; }
        public int TestType { get; set; }
        public int Status { get; set; }
        public int Priority { get; set; }
        public List<StepInTestCaseModel> StepData { get; set; }
        public WorkItem WorkItemData { get; set; }
        public string Action { get; set; }
        public int ParentId { get; set; }
        public string ReturnUrl { get; set; }
        public List<TestCaseModel> TestCaseModel { get; set; }
        public List<Item> ListFile { get; set; }

    }
    public class ListItemModel
    {
        public Types Type { get; set; }
        public IEnumerable<PM_ASVN.Common.Item> List { get; set; }
        public List<String> Description { get; set; }
        public int ParentId { get; set; }
        public int FilterParentId { get; set; }
        public int PageIndex { get; set; }
        public int TotalPage { get; set; }
        public int PageSize { get; set; }
        public int TotalRows { get; set; }
        public bool Edit { get; set; }
        public bool Remove { get; set; }
        public Types TypeParent { get; set; }
        public int ChildId { get; set; }
        public Func<Item, HelperResult> Template { get; set; }
    }

    public class SelectedItemModel
    {
        public int ParentID { get; set; }
        public IEnumerable<PM_ASVN.Common.ItemRelation> Items { get; set; }
        public string ReturnUrl { get; set; }
    }
    public class TableModel
    {
        public PM_ASVN.Common.Item Item { get; set; }
        public List<PM_ASVN.Common.Item> Column { get; set; }
    }

    public class EstimationModel
    {
        public int ID { get; set; }
        public IEnumerable<WorkGroup> WorkGroupData { get; set; }
    }
    public class EstimationDetail
    {
        public int ID { get; set; }
        public string Feature { get; set; }
        public string WorkGroup { get; set; }
        public string Cost { get; set; }
    }
    public class WorkItemModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Cost { get; set; }
    }
    public class TestCaseModel
    {
        public int ID { get; set; }
        public string Feature { get; set; }
        public string Name { get; set; }
        public List<string> Users { get; set; }
        public List<bool> Browsers { get; set; }
        public List<TypeTestCase> TotalType { get; set; }
    }
    public class TypeTestCase
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int TotalTC { get; set; }
    }
    public class AccountModel
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public int IDRole { get; set; }
        public List<PM_ASVN.Common.Permission> Permission { get; set; }
    }
    public class StepInTestCaseModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string ExpectedResult { get; set; }
        public string TestData { get; set; }
    }
    public class CommentModel
    {
        public int ID { get; set; }
        public string Text { get; set; }
        public int IDUser { get; set; }
        public string Name { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
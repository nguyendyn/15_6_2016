using aperia.core.business;
using PM_ASVN.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace PM_ASVN.Common
{
    [EntityTable(true, "ID")]
    public partial class Item : BaseEntity<Item>
    {
        public int ID { get; set; }
        public int Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }
        public List<Item> GetChildList(ref ObjectParameter op, int pageIndex, int pageSize, int typeInt, int parentId, int totalRow)
        {
            op = new ObjectParameter();
            op.Add("PageIndex", pageIndex);
            op.Add("PageSize", pageSize);
            op.Add("Type", typeInt);
            op.Add("ParentId", parentId);
            op.Add("TotalRow", totalRow);
            return Db.ExecuteFunction<Item>("sp_PagingItem", op);
        }
        public List<Item> GetChildByParentIDAndFilterID(int typeInt, int ParentId, int FilterParentId)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("Type", typeInt);
            op.Add("ParentId", ParentId);
            op.Add("Filter", FilterParentId);
            return Db.ExecuteFunction<Item>("sp_ChildItem", op);
        }
        public List<Item> GetDatabaseInProject(int ID, int typeInt)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("ParentId", ID);
            op.Add("ChildType", typeInt);
            return Db.ExecuteFunction<Item>("sp_SelectDatabaseInProject", op);
        }
        public List<Item> GetChildNotInParentID(int typeInt, int ParentId)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("Type", typeInt);
            op.Add("ParentId", ParentId);
            return Db.ExecuteFunction<Item>("sp_ChildItemNotInParentID", op);
        }

        public List<Item> GetParentList(ref ObjectParameter op, int pageIndex, int pageSize, int childId, int typeInt, int totalRow)
        {
            op = new ObjectParameter();
            op.Add("PageIndex", pageIndex);
            op.Add("PageSize", pageSize);
            op.Add("ChildID", typeInt);
            op.Add("TypeParent", childId);
            op.Add("TotalRow", totalRow);
            return Db.ExecuteFunction<Item>("sp_SearchParentItemByChildID", op);

        }
        public List<Item> GetWorkItemInWorkGroup(int ID, int typeInt)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("IDParent", ID);
            op.Add("ChildType", typeInt);
            return Db.ExecuteFunction<Item>("sp_SelectWorkItemInWorkGroup", op);
        }
        public void RemoveFeatureInTicket(int IDFeature, int IDTicket)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("IDFeature", IDFeature);
            op.Add("IDTicket", IDTicket);
            Db.ExecuteFunction("sp_RemoveFeatureFromTicket", op);
        }
        public void RemoveItem(int ID)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("IDItem", ID);
            Db.ExecuteFunction("sp_RemoveItem", op);
        }
        public List<Item> SearchProject(string Query, int Type)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("SearchString", Query);
            op.Add("Type", Type);
            return Db.ExecuteFunction<Item>("sp_SearchProject", op);
        }
        public Item GetTestCaseProperties(int IDTestCase, int Type)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("parentId", IDTestCase);
            op.Add("type", Type);
            return Db.ExecuteFunction<Item>("sp_SearchItem", op).FirstOrDefault();
        }
        public Item GetTaskProperties(int IDTask, int Type)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("parentId", IDTask);
            op.Add("type", Type);
            return Db.ExecuteFunction<Item>("sp_SearchItem", op).FirstOrDefault();
        }
        public List<Item> GetFeatureInProject(int IDProject, int Type)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("parentId", IDProject);
            op.Add("type", Type);
            return Db.ExecuteFunction<Item>("sp_SearchItem", op);
        }
        public List<Item> GetFileInProject(int IDProject, int Type)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("parentId", IDProject);
            op.Add("type", Type);
            return Db.ExecuteFunction<Item>("sp_SearchItem", op);
        }
        public List<Item> GetListType(int Type)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("type", Type);
            return Db.ExecuteFunction<Item>("sp_GetItemByType", op);
        }
        public List<Item> GetListTestCaseByFeatureAndTypeTest(int IDFeature, int IDTypeTest)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("IDFeature", IDFeature);
            op.Add("IDType", IDTypeTest);
            return Db.ExecuteFunction<Item>("sp_GetTestCaseByFeatureAndTypeTest", op);
        }
        public List<Item> GetListTestCaseByUser(int IDUser, int IDFeature, int IDTypeTest)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("IDFeature", IDFeature);
            op.Add("IDType", IDTypeTest);
            op.Add("IDUser", IDUser);
            return Db.ExecuteFunction<Item>("sp_SearchTCByFeature", op);
        }
        public List<Item> GetUsersByTestCaseAndBrowser(int IDTestCase, int IDBrowser)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("IDBrowser", IDBrowser);
            op.Add("IDTestCase", IDTestCase);
            return Db.ExecuteFunction<Item>("sp_SearchUserByTC_Browser", op);
        }
        public List<Item> GetUserNotInTestCase(int IDTestCase, int Type)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("IDTestCase", IDTestCase);
            op.Add("Type", Type);
            return Db.ExecuteFunction<Item>("sp_GetUserNotInTestCase", op);
        }
        public List<Item> GetUserInTestCase(int IDTestCase, int Type)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("IDTestCase", IDTestCase);
            op.Add("Type", Type);
            return Db.ExecuteFunction<Item>("sp_GetUserInTestCase", op);
        }
        public List<TypeTestCase> CountTestCaseInType(int IDFeature, int IDTypeTestCase)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("IDFeature", IDFeature);
            op.Add("IDType", IDTypeTestCase);
            return Db.ExecuteFunction<TypeTestCase>("sp_CountTestCaseInType", op);
        }
    }
    [EntityTable(false, "ID")]
    public partial class Project : BaseEntity<Project>
    {
        public int ID { get; set; }
        public DateTime StartDate { get; set; }
        public string PrimaryBA { get; set; }
        public string PrimaryWeb { get; set; }
        public string PrimaryDB { get; set; }
    }
    [EntityTable("IDChild", "IDParent")]
    public partial class ItemRelation : BaseEntity<ItemRelation>
    {
        public int IDChild { get; set; }
        public int IDParent { get; set; }
        public string Description { get; set; }
    }
    [EntityTable("ID")]
    public partial class WorkGroup : BaseEntity<WorkGroup>
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public double PercentGroup { get; set; }
        public List<WorkGroup> GetListWorkGroup()
        {
            ObjectParameter op = new ObjectParameter();
            return Db.ExecuteFunction<WorkGroup>("sp_GetListWorkGroup", op);
        }
        public List<WorkGroup> GetTimeWorkGroup(int IDEstimation, int IDWorkGroup, int Type)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("IDEstimate", IDEstimation);
            op.Add("IDWorkGroup", IDWorkGroup);
            op.Add("ChildType", Type);
            return Db.ExecuteFunction<WorkGroup>("sp_GetTimeWorkGroup", op);
        }
    }
    [EntityTable("ID")]
    public partial class WorkItem : BaseEntity<WorkItem>
    {
        public int ID { get; set; }
        public int Status { get; set; }
        public List<WorkItem> GetListWorkItem()
        {
            ObjectParameter op = new ObjectParameter();
            return Db.ExecuteFunction<WorkItem>("sp_GetListWorkItem", op);
        }
        public List<WorkItemModel> GetDetailWorkItem(ref ObjectParameter op, int IDEstimation, int IDWorkGroup, int Type, double realTime)
        {
            op = new ObjectParameter();
            op.Add("IDParent1", IDEstimation);
            op.Add("IDParent2", IDWorkGroup);
            op.Add("ChildType", Type);
            op.Add("SumWG", realTime);
            return Db.ExecuteFunction<WorkItemModel>("sp_GetDetailWorkItem", op);
        }

    }
    [EntityTable("ID")]
    public partial class Estimation : BaseEntity<Estimation>
    {
        public int ID { get; set; }
        public string Description { get; set; }
        public string DateCreate { get; set; }
        public float Cost { get; set; }
        public List<EstimationDetail> GetEstimateDetail(int IDTicket)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("IDTicket", IDTicket);
            return Db.ExecuteFunction<EstimationDetail>("sp_SearchEstimateDetail", op);
        }
        public List<Estimation> GetDescriptionEstimation(int IDEstimation)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("IDEstimation", IDEstimation);
            return Db.ExecuteFunction<Estimation>("sp_GetDescriptionEstimation", op);
        }
    }
    [EntityTable("ID")]
    public partial class TestCase : BaseEntity<TestCase>
    {
        public int ID { get; set; }
        public string Screen { get; set; }
        public string Summary { get; set; }
        public string PreCondition { get; set; }
        public string Assigner { get; set; }
        public string TCExpectedResult { get; set; }
        public DateTime TestDate { get; set; }

    }
    [EntityTable("ID")]
    public partial class StepInTestCase : BaseEntity<StepInTestCase>
    {
        public int ID { get; set; }
        public string ExpectedResult { get; set; }
        public string TestData { get; set; }
        public List<StepInTestCaseModel> GetStepInTestCase(int ID)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("IDTestCase", ID);
            return Db.ExecuteFunction<StepInTestCaseModel>("sp_StepInTestCase", op);
        }
    }
    [EntityTable("ID_TestCase", "ID_User", "ID_Browser")]
    public partial class TestCaseRelation : BaseEntity<TestCaseRelation>
    {
        public int ID_TestCase { get; set; }
        public int ID_User { get; set; }
        public int ID_Browser { get; set; }
        public List<TestCaseRelation> CheckUserInBrowser(int IDUser, int IDBrowser, int IDTestCase)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("IDUser", IDUser);
            op.Add("IDBrowser", IDBrowser);
            op.Add("IDTestCase", IDTestCase);
            return Db.ExecuteFunction<TestCaseRelation>("sp_CheckUserInBrowser", op);
        }
    }
    [EntityTable("ID")]
    public partial class Account : BaseEntity<Account>
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int IDRole { get; set; }
        public bool RememberMe { get; set; }

        public List<AccountModel> GetListAccount(ref ObjectParameter op, int pageNum, int pageSize)
        {
            op = new ObjectParameter();
            op.Add("pageNum", pageNum);
            op.Add("pageSize", pageSize);
            op.Add("totalRecords", ParameterDirection.Output);
            return Db.ExecuteFunction<AccountModel>("sp_GetAccount", op);
        }
        public AccountModel GetAccountID(int ID)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("ID", ID);
            return Db.ExecuteFunction<AccountModel>("sp_GetAccountID", op).First();
        }
        public AccountModel GetAccountByName(string username)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("name", username);
            return Db.ExecuteFunction<AccountModel>("sp_GetAccountByName", op).First();
        }
        public int CheckAccount(string username)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("name", username);
            op.Add("result", ParameterDirection.Output);
            Db.ExecuteFunction<Account>("sp_CheckAccount", op);
            var output = op.FirstOrDefault(x => x.Key == "result");
            return (int)output.Value;
        }
        public int AddAccount(string username, string password, int IDRole)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("name", username);
            op.Add("pass", password);
            op.Add("idRole", IDRole);
            op.Add("result", ParameterDirection.Output);
            Db.ExecuteFunction<Account>("sp_AddAccount", op);
            var output = op.FirstOrDefault(x => x.Key == "result");
            return (int)output.Value;
        }

        public int GetRoleInAccount(int ID)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("id", ID);
            op.Add("result", ParameterDirection.Output);
            Db.ExecuteFunction<Role>("sp_GetRoleInAccount", op);
            var output = op.FirstOrDefault(x => x.Key == "result");
            return (int)output.Value;
        }
        public void EditAccount(int IDAccount, string username, int IDRole)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("idAccount", IDAccount);
            op.Add("name", username);
            op.Add("idRole", IDRole);
            Db.ExecuteFunction<Account>("sp_EditAccount", op);
        }
        public void DeleteAccount(int IDAccount)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("idAccount", IDAccount);
            Db.ExecuteFunction<Account>("sp_DeleteAccount", op);
        }
        public int Login(string username, string password)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("name", username);
            op.Add("pass", password);
            op.Add("result", ParameterDirection.Output);
            Db.ExecuteFunction<Account>("sp_AccountLogin", op);
            var output = op.FirstOrDefault(x => x.Key == "result");
            var rs = (int)output.Value;
            if (rs > 0)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
    [EntityTable("ID")]
    public partial class Role : BaseEntity<Role>
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Role> GetListRole()
        {
            return Db.ExecuteFunction<Role>("sp_GetListRole");
        }
        public int CheckRole(string name)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("name", name);
            op.Add("result", ParameterDirection.Output);
            Db.ExecuteFunction<Role>("sp_CheckRole", op);
            var output = op.FirstOrDefault(x => x.Key == "result");
            return (int)output.Value;
        }
        public int AddRole(string name, string description)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("role", name);
            op.Add("description", description);
            op.Add("result", ParameterDirection.Output);
            Db.ExecuteFunction<Role>("sp_AddRoles", op);
            var output = op.FirstOrDefault(x => x.Key == "result");
            return (int)output.Value;
        }
        public Role GetRoleID(int ID)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("ID", ID);
            return Db.ExecuteFunction<Role>("sp_GetRoleID", op).First();
        }
        public void EditRole(Role model)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("ID", model.ID);
            op.Add("name", model.Name);
            op.Add("description", model.Description);
            Db.ExecuteFunction<Role>("sp_EditRole", op);
        }
        public int CheckRoleExists(int IDRole)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("ID", IDRole);
            op.Add("result", ParameterDirection.Output);
            Db.ExecuteFunction<Role>("sp_ExistRoleInAccount", op);
            var output = op.FirstOrDefault(x => x.Key == "result");
            return (int)output.Value;
        }
        public void DeleteRole(int IDRole)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("ID", IDRole);
            Db.ExecuteFunction<Role>("sp_DeleteRole", op);
        }
    }
    [EntityTable("ID_Role", "ID_Permission")]
    public partial class RolePermission : BaseEntity<RolePermission>
    {
        public int ID_Role { get; set; }
        public int ID_Permission { get; set; }
    }
    [EntityTable("ID")]
    public partial class Permission : BaseEntity<Permission>
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Permission> GetPermissionInRole(int ID)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("ID", ID);
            return Db.ExecuteFunction<Permission>("sp_GetPermissionInRole", op);
        }

        public List<Permission> GetPermissionNotInRole(int ID)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("IDRole", ID);
            return Db.ExecuteFunction<Permission>("sp_GetPermissionNotInRole", op);
        }

        public void AddPermissionInRole(int IDRole, int IDPermission)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("IDRole", IDRole);
            op.Add("IDPermission", IDPermission);
            Db.ExecuteFunction("sp_AddPermissionInRole", op);
        }

        public void DeletePermissionInRole(int IDRole, int IDPermission)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("IDRole", IDRole);
            op.Add("IDPermission", IDPermission);
            Db.ExecuteFunction("sp_DeletePermissionInRole", op);
        }
        public List<Permission> GetListPermission(ref ObjectParameter op, int pageNum, int pageSize)
        {
            op = new ObjectParameter();
            op.Add("pageNum", pageNum);
            op.Add("pageSize", pageSize);
            op.Add("totalRecords", ParameterDirection.Output);
            return Db.ExecuteFunction<Permission>("sp_GetPermission", op);
        }
        public int CheckPermission(string name)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("name", name);
            op.Add("result", ParameterDirection.Output);
            Db.ExecuteFunction<Permission>("sp_CheckPermission", op);
            var output = op.FirstOrDefault(x => x.Key == "result");
            return (int)output.Value;
        }
        public int AddPermission(string name, string description)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("permission", name);
            op.Add("description", description);
            op.Add("result", ParameterDirection.Output);
            Db.ExecuteFunction<Permission>("sp_AddPermission", op);
            var output = op.FirstOrDefault(x => x.Key == "result");
            return (int)output.Value;
        }
        public Permission GetPermissionID(int ID)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("ID", ID);
            return Db.ExecuteFunction<Permission>("sp_GetPermissionID", op).First();
        }
        public void EditPermission(Permission model)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("ID", model.ID);
            op.Add("name", model.Name);
            op.Add("description", model.Description);
            Db.ExecuteFunction<Permission>("sp_EditPermission", op);
        }
        public void DeletePermission(int IDPermission)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("ID", IDPermission);
            Db.ExecuteFunction<Role>("sp_DeletePermission", op);
        }
    }
    [EntityTable(false, "ID")]
    public partial class Task : BaseEntity<Task>
    {
        public int ID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public double EstimatedTime { get; set; }
        public int Done { get; set; }
        public string Assignee { get; set; }
    }
    [EntityTable(true,"ID")]
    public partial class Comment : BaseEntity<Comment>
    {
        public int ID { get; set; }
        public string Text { get; set; }
        public int IDUser { get; set; }
        public int IDItemParent { get; set; }
        public DateTime CreateDate { get; set; }
        public List<CommentModel> GetListComment(int ID)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("IDParent", ID);
            return Db.ExecuteFunction<CommentModel>("sp_SearchCmtByIDParent", op);
        }
    }

}
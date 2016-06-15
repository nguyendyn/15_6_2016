using PM_ASVN.Common;
using PM_ASVN.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PM_ASVN.Controllers
{
     [CustomAuthorizeAttribute(Permission = "View_Database")]
    public class DatabaseController : BaseController
    {
        //
        // GET: /Database/
        public ActionResult Index(ItemModel model)
        {
            if (model.Data == null) model.Data = new PM_ASVN.Common.Item();
            model.Data.Type = (int)Types.Database;
            model.ReturnUrl = "/Database/List";
            return View(model);
        }
        public ActionResult List(ListItemModel model)
        {
            model.Type = Types.Database;
            model.Edit = true;
            return View(model);
        }
        public ActionResult SelectChild(ListItemModel model, SelectedItemModel selectedItem)
        {
            model.Type = Types.Database;
            return View(model);
        }

        public ActionResult GetDataByConnectionString(string ConnectionString)
        {
            //string cnString = @"workstation id=dcthangkhtn.mssql.somee.com;packet size=4096;user id=dcthangkhtn_SQLLogin_1;pwd=2s8zga86bm;data source=dcthangkhtn.mssql.somee.com;persist security info=False;initial catalog=dcthangkhtn";
            //ConnectionString = @"data source=.;initial catalog=TestDatabase;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework";
            SqlConnection conn = new SqlConnection(ConnectionString);
            conn.Open();
            string DatabaseName = conn.Database.ToString();
            List<TableModel> listTable = GetListTableAndColumn(conn);
            List<PM_ASVN.Common.Item> listStore = GetStoreProcedure(conn);

            conn.Close();

            string cs = ConfigurationManager.ConnectionStrings[0].ConnectionString;
            conn = new SqlConnection(cs);
            conn.Open();
            SqlCommand command = new SqlCommand("if not exists (SELECT * FROM Item where Item.Type='" + (int)Types.Database + "' and Item.Name = '" + DatabaseName + "') insert Item(Name, Type) values ('" + DatabaseName + "','" + (int)Types.Database + "') SELECT SCOPE_IDENTITY()", conn);
            int IDDatabase = 0;
            // check database exists
            // get ID database
            bool CheckID = Int32.TryParse(command.ExecuteScalar().ToString(), out IDDatabase);
            if (CheckID == false)
            {
                command = new SqlCommand("SELECT * FROM Item where Item.Type='" + (int)Types.Database + "' and Item.Name = '" + DatabaseName + "' SELECT SCOPE_IDENTITY()", conn);
                IDDatabase = Convert.ToInt32(command.ExecuteScalar());
            }
            else
            {
                IDDatabase = Convert.ToInt32(command.ExecuteScalar());
            }
            // add new store
            AddNewStore(IDDatabase, listStore, conn);
            // add new table
            AddNewTableAndColumn(IDDatabase, listTable, conn);
            conn.Close();
            return Redirect("/Database/List");
        }
        public List<TableModel> GetListTableAndColumn(SqlConnection conn)
        {
            SqlCommand command = new SqlCommand("select * from sys.tables where type = 'U'", conn);
            SqlDataAdapter sda = new SqlDataAdapter(command);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            List<TableModel> listTable = new List<TableModel>();
            foreach (DataRow row in dt.Rows)
            {
                TableModel tableModel = new TableModel();
                tableModel.Item = new PM_ASVN.Common.Item();
                tableModel.Column = new List<PM_ASVN.Common.Item>();
                tableModel.Item.Name = row[0].ToString();
                tableModel.Item.Type = (int)Types.Table;
                command = new SqlCommand("SELECT information_schema.COLUMNS.COLUMN_NAME FROM information_schema.COLUMNS WHERE TABLE_NAME = '" + tableModel.Item.Name + "'", conn);
                SqlDataAdapter sda2 = new SqlDataAdapter(command);
                DataTable dt2 = new DataTable();
                sda2.Fill(dt2);
                foreach (DataRow row2 in dt2.Rows)
                {
                    tableModel.Column.Add(new PM_ASVN.Common.Item { Name = row2[0].ToString(), Type = (int)Types.DataColumn });
                }
                listTable.Add(tableModel);
            }
            return listTable;
        }
        public List<PM_ASVN.Common.Item> GetStoreProcedure(SqlConnection conn)
        {
            SqlCommand command = new SqlCommand("SELECT * FROM sys.objects WHERE type in (N'P', N'PC')", conn);
            SqlDataAdapter sda = new SqlDataAdapter(command);
            DataTable dtStore = new DataTable();
            sda.Fill(dtStore);
            List<PM_ASVN.Common.Item> listStore = new List<PM_ASVN.Common.Item>();
            foreach (DataRow row in dtStore.Rows)
            {
                listStore.Add(new PM_ASVN.Common.Item { Name = row[0].ToString(), Type = (int)Types.Store });

            }
            return listStore;
        }
        public void AddNewStore(int IDDatabase, List<PM_ASVN.Common.Item> listStore, SqlConnection conn)
        {
            for (int i = 0; i < listStore.Count; i++)
            {
                SqlCommand command = new SqlCommand("if not exists (SELECT * FROM Item as I, ItemRelation as IR where I.Type=" + listStore[i].Type + " and I.ID = IR.IDChild and IR.IDParent = " + IDDatabase + " and I.Name = '" + listStore[i].Name + "') insert Item(Name, Type) values ('" + listStore[i].Name + "','" + listStore[i].Type + "'); ; SELECT SCOPE_IDENTITY();", conn);
                int IDStore = 0;
                bool CheckID = Int32.TryParse(command.ExecuteScalar().ToString(), out IDStore);
                if (CheckID == true)
                {
                    command = new SqlCommand("insert ItemRelation(IDChild, IDParent) values ('" + IDStore + "','" + IDDatabase + "')", conn);
                    command.ExecuteNonQuery();
                }

            }
        }
        public void AddNewTableAndColumn(int IDDatabase, List<TableModel> listTable, SqlConnection conn)
        {
            for (int i = 0; i < listTable.Count; i++)
            {
                SqlCommand command = new SqlCommand("if not exists (SELECT * FROM Item as I, ItemRelation as IR where I.Type=" + listTable[i].Item.Type + " and I.ID = IR.IDChild and IR.IDParent = " + IDDatabase + " and I.Name = '" + listTable[i].Item.Name + "') insert Item(Name, Type) values ('" + listTable[i].Item.Name + "','" + listTable[i].Item.Type + "'); SELECT SCOPE_IDENTITY();", conn);
                int IDTable = 0;
                // check table exists
                bool CheckID = Int32.TryParse(command.ExecuteScalar().ToString(), out IDTable);
                // not exists
                if (CheckID == true)
                {
                    command = new SqlCommand("insert ItemRelation(IDChild, IDParent) values ('" + IDTable + "','" + IDDatabase + "')", conn);
                    command.ExecuteNonQuery();
                }
                // exists
                else
                {
                    command = new SqlCommand("SELECT * FROM Item, ItemRelation where Item.Type='" + (int)Types.Table + "' and Item.Name = '" + listTable[i].Item.Name + "' and ItemRelation.IDChild = Item.ID and ItemRelation.IDParent = " + IDDatabase + " SELECT SCOPE_IDENTITY()", conn);
                    IDTable = Convert.ToInt32(command.ExecuteScalar());
                }
                // add more column
                for (int j = 0; j < listTable[i].Column.Count; j++)
                {
                    command = new SqlCommand("if not exists (SELECT * FROM Item as I, ItemRelation as IR where I.Type=" + listTable[i].Column[j].Type + " and I.ID = IR.IDChild and IR.IDParent = " + IDTable + " and I.Name = '" + listTable[i].Column[j].Name + "') insert item(Type, Name) values (" + listTable[i].Column[j].Type + ",'" + listTable[i].Column[j].Name + "'); SELECT SCOPE_IDENTITY();", conn);
                    int IDColumn = 0;
                    // check column exists in table
                    CheckID = Int32.TryParse(command.ExecuteScalar().ToString(), out IDColumn);
                    if (CheckID == true)
                    {
                        command = new SqlCommand("insert ItemRelation(IDChild, IDParent) values ('" + IDColumn + "','" + IDTable + "')", conn);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
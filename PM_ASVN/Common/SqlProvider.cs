using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

using System.Reflection;
using System.Configuration;

namespace aperia.core.business
{
    public class EntityTableAttribute : Attribute
    {
        public string[] PrimaryFields { get; private set; }
        public bool AutoID { get; private set; }
        public EntityTableAttribute(bool autoID, params string[] primaryFields)
        {
            PrimaryFields = primaryFields;
            AutoID = autoID;
        }
        public EntityTableAttribute(params string[] primaryFields) : this(false, primaryFields) { }
    }
    public abstract class BaseEntity<T>
    {
        public BaseEntity()
            : this(SqlProvider.Default)
        {
        }
        public BaseEntity(SqlProvider sqlProvider)
        {
            Db = sqlProvider;
        }
        protected SqlProvider Db { get; private set; }
        public void Copy(T source)
        {
            PropertyInfo[] props = Properties;

            foreach (var p in props)
            {
                if (p.CanRead && p.CanWrite)
                {
                    p.SetValue(this, p.GetValue(source));
                }

            }

        }
        protected PropertyInfo[] Properties
        {
            get
            {
                Type type = typeof(T);
                return type.GetProperties();

            }
        }
        public bool Get()
        {
            Type type = GetType();
            EntityTableAttribute att = type.GetCustomAttribute<EntityTableAttribute>();
            if (att == null) throw new Exception("Must have EntityTableAttribute");
            PropertyInfo[] props = Properties;
            string fieldQuery = "";
            foreach (var p in props)
            {
                fieldQuery += string.Format("[{0}],", p.Name);
            }
            fieldQuery = fieldQuery.Trim(',');

            string whereQuery = "1 = 1";
            foreach (var p in props)
            {
                if (att.PrimaryFields.Contains(p.Name))
                {
                    whereQuery += string.Format(" and [{0}] = @{0}", p.Name);
                }

            }
            string query = "";
            query = string.Format("SELECT {1} FROM [{0}] (nolock) where {2}", type.Name, fieldQuery, whereQuery);
            ObjectParameter parameters = new ObjectParameter();
            foreach (var p in props)
            {
                if (att.PrimaryFields.Contains(p.Name))
                {
                    parameters.Add(p.Name, p.GetValue(this));
                }
            }
            List<T> result = Db.ExecuteQuery<T>(query, parameters);
            if (result == null) return false;
            Copy(result[0]);
            return true;
        }
        public bool Delete()
        {
            Type type = GetType();
            EntityTableAttribute att = type.GetCustomAttribute<EntityTableAttribute>();
            if (att == null) throw new Exception("Must have EntityTableAttribute");
            PropertyInfo[] props = Properties;

            string whereQuery = "1 = 1";
            foreach (var p in props)
            {
                if (att.PrimaryFields.Contains(p.Name))
                {
                    whereQuery += string.Format(" and [{0}] = @{0}", p.Name);
                }

            }
            string query = "";
            query = string.Format("DELETE FROM [{0}] where {1}", type.Name, whereQuery);
            ObjectParameter parameters = new ObjectParameter();
            foreach (var p in props)
            {
                if (att.PrimaryFields.Contains(p.Name))
                {
                    parameters.Add(p.Name, p.GetValue(this));
                }
            }
            List<T> result = Db.ExecuteQuery<T>(query, parameters);
            if (result == null) return false;
            return true;
        }

        public bool Update()
        {
            return Save(false);
        }
        public bool Insert()
        {
            //return Save(false);
            return Save(true);
        }
        protected virtual bool Save(bool insert)
        {
            Type type = GetType();
            EntityTableAttribute att = type.GetCustomAttribute<EntityTableAttribute>();
            if (att == null) throw new Exception("Must have EntityTableAttribute");
            PropertyInfo[] props = Properties;
            string valueQuery = "", fieldQuery = "";
            if (insert)
            {
                foreach (var p in props)
                {
                    if (att.AutoID && att.PrimaryFields.Contains(p.Name)) continue;
                    valueQuery += string.Format("@{0},", p.Name);
                    fieldQuery += string.Format("[{0}],", p.Name);
                }
                fieldQuery = fieldQuery.Trim(',');
            }
            else
            {
                foreach (var p in props)
                {
                    if (att.PrimaryFields.Contains(p.Name)) continue;
                    valueQuery += string.Format("[{0}] = @{0},", p.Name);
                }
            }
            valueQuery = valueQuery.Trim(',');
            string whereQuery = "1 = 1";
            if (!insert)
            {
                foreach (var p in props)
                {
                    if (att.PrimaryFields.Contains(p.Name))
                    {
                        whereQuery += string.Format(" and [{0}] = @{0}", p.Name);
                    }

                }
            }
            string query = "";
            if (insert)
            {
                query = string.Format("INSERT INTO [{0}]({1}) VALUES ({2})", type.Name, fieldQuery, valueQuery);
            }
            else
            {
                query = string.Format("UPDATE [{0}] SET {1} WHERE {2}", type.Name, valueQuery, whereQuery);
            }
            ObjectParameter parameters = new ObjectParameter();
            foreach (var p in props)
            {
                if (insert && att.AutoID && att.PrimaryFields.Contains(p.Name)) continue;
                parameters.Add(p.Name, p.GetValue(this));
            }
            double result = 0;
            if (att.AutoID && insert)
            {
                query += ";select @@IDENTITY";
                List<int> newID = Db.ExecuteQuery<int>(query, parameters);

                foreach (var p in props)
                {
                    if (att.PrimaryFields.Contains(p.Name))
                    {
                        p.SetValue(this, newID[0]);
                    }
                }
                result = newID[0];
            }
            else
            {
                result = Db.ExecuteNonQuery(query, parameters);
            }
            return result > 0;
        }

    }
    public class ObjectParameter : Dictionary<string, object>
    {
        public ObjectParameter()
        {

        }
    }
    public class SqlProvider
    {
        static SqlProvider s_Default = null;
        static SqlProvider()
        {
            s_Default = new SqlProvider();
        }
        public static SqlProvider Default
        {
            get
            {
                return s_Default;
            }
        }

        string _conStr = null;
        protected SqlProvider()
            : this(ConfigurationManager.ConnectionStrings[0].ConnectionString)
        {

        }
        public SqlProvider(string conStr)
        {
            _conStr = conStr;
        }
        List<T> Execute<T>(CommandType commandType, string command, ObjectParameter parameters)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = command;
            cmd.CommandType = commandType;
            cmd.Connection = new SqlConnection(_conStr);
            cmd.Connection.Open();
            if (parameters != null && parameters.Count > 0)
            {
                if (commandType == CommandType.StoredProcedure)
                {
                    SqlCommandBuilder.DeriveParameters(cmd);
                    foreach (var p in parameters)
                    {
                        cmd.Parameters["@" + p.Key].Value = p.Value ?? DBNull.Value;
                    }
                }
                else
                {
                    foreach (var p in parameters)
                    {
                        cmd.Parameters.Add(new SqlParameter("@" + p.Key, p.Value));
                    }
                }
            }
            IDataReader reader = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(reader);
            cmd.Connection.Close();
            if (parameters != null)
            {
                foreach (var p in parameters.ToArray())
                {
                    switch (cmd.Parameters["@" + p.Key].Direction)
                    {
                        case ParameterDirection.InputOutput:
                        case ParameterDirection.Output: parameters[p.Key] = cmd.Parameters["@" + p.Key].Value;
                            break;
                    }
                }
            }

            return To<T>(dt);
        }
        public int ExecuteFunction(string storeName, ObjectParameter parameters = null)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = storeName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = new SqlConnection(_conStr);
            cmd.Connection.Open();
            SqlCommandBuilder.DeriveParameters(cmd);
            foreach (var p in parameters)
            {
                cmd.Parameters["@" + p.Key].Value = p.Value ?? DBNull.Value;
            }
            int result = cmd.ExecuteNonQuery();
            cmd.Connection.Close();
            foreach (var p in parameters)
            {
                switch (cmd.Parameters["@" + p.Key].Direction)
                {
                    case ParameterDirection.InputOutput:
                    case ParameterDirection.Output: parameters[p.Key] = cmd.Parameters["@" + p.Key].Value;
                        break;
                }
            }
            return result;
        }
        public List<T> ExecuteFunction<T>(string storeName, ObjectParameter parameters = null)
        {
            return Execute<T>(CommandType.StoredProcedure, storeName, parameters);
        }


        public List<T> ExecuteQuery<T>(string query, ObjectParameter parameters = null)
        {
            return Execute<T>(CommandType.Text, query, parameters);
        }

        public int ExecuteNonQuery(string query, ObjectParameter parameters = null)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = query;
            cmd.CommandType = CommandType.Text;
            cmd.Connection = new SqlConnection(_conStr);
            cmd.Connection.Open();
            if (parameters != null && parameters.Count > 0)
            {
                foreach (var p in parameters)
                {
                    cmd.Parameters.Add(new SqlParameter("@" + p.Key, p.Value));
                }
            }
            int result = cmd.ExecuteNonQuery();
            cmd.Connection.Close();
            return result;
        }

        static T To<T>(DataRow row)
        {
            Type type = typeof(T);
            var obj = (T)Activator.CreateInstance(type);
            foreach (PropertyInfo p in type.GetProperties())
            {
                if (p.CanWrite)
                {
                    if (row.Table.Columns.Contains(p.Name) && row[p.Name] != null && row[p.Name] != DBNull.Value)
                    {
                        p.SetValue(obj, row[p.Name]);
                    }
                    else
                    {
                        p.SetValue(obj, null);
                    }
                }
            }

            return obj;
        }

        static List<T> To<T>(DataTable table)
        {
            List<T> ret = new List<T>();
            if (table.Columns.Count > 1)
            {
                foreach (DataRow row in table.Rows)
                {
                    T t = To<T>(row);
                    ret.Add(t);
                }
            }
            else
            {
                Type type = typeof(T);
                foreach (DataRow row in table.Rows)
                {
                    T t = default(T);
                    if (row[0] != null && row[0] != DBNull.Value) t = (T)Convert.ChangeType(row[0], type);
                    ret.Add(t);
                }
            }
            if (ret.Count > 0) return ret;
            else return null;
        }
    }
}

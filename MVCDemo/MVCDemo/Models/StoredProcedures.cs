using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MVCDemo.Models
{
    public class StoredProcedures
    {
        private ModelDB db = new ModelDB();
        public int ProjectCreate(string name, string description)
        {
            var parameter = new SqlParameter[]{
                new SqlParameter("@ProjectName", name),
                new SqlParameter("@ProjectDescription", description),
                new SqlParameter("@result", SqlDbType.Int){
                   Direction= ParameterDirection.Output
                }
            };
            db.Database.ExecuteSqlCommand("sp_Project_Insert @ProjectName, @ProjectDescription, @result out", parameter);
            var output = parameter.FirstOrDefault(x => x.ParameterName == "@result");
            if (output != null)
            {
                return (int)output.Value;
            }

            return 0;
        }
        public int ProjectUpdate(int id, string name, string description, DateTime date)
        {
            var parameter = new SqlParameter[]{
                new SqlParameter("@id", id),
                new SqlParameter("@name", name),
                new SqlParameter("@description", description),
                new SqlParameter("@date", date),
                new SqlParameter("@result", SqlDbType.Int){
                   Direction= ParameterDirection.Output
                }
            };
            db.Database.ExecuteSqlCommand("sp_Project_Update @id,@name, @description,@date, @result out", parameter);
            var output = parameter.FirstOrDefault(x => x.ParameterName == "@result");
            if (output != null)
            {
                return (int)output.Value;
            }

            return 0;
        }
        public int ProjectDelete(int id)
        {
            var parameter = new SqlParameter[]{
                new SqlParameter("@id", id),
                new SqlParameter("@result", SqlDbType.Int){
                   Direction= ParameterDirection.Output
                }
            };
            db.Database.ExecuteSqlCommand("sp_Project_Delete @id, @result out", parameter);
            var output = parameter.FirstOrDefault(x => x.ParameterName == "@result");
            if (output != null)
            {
                return (int)output.Value;
            }

            return 0;
        }
        public int ModuleInsert(string name, string desc)
        {
            var parameter = new SqlParameter[]{
                new SqlParameter("@name", name),
                new SqlParameter("@description", desc),
                new SqlParameter("@result", SqlDbType.Int){
                    Direction=ParameterDirection.Output
                }
            };
            db.Database.ExecuteSqlCommand("sp_Module_Insert @name, @description, @result out", parameter);
            var output = parameter.FirstOrDefault(m => m.ParameterName == "@result");
            if (output != null)
            {
                return (int)output.Value;
            }
            return 0;
        }
        public int ModuleUpate(int id, string name, string desc)
        {
            var parameter = new SqlParameter[]{
                 new SqlParameter("@id", id),
                new SqlParameter("@name", name),
                new SqlParameter("@description", desc),
                new SqlParameter("@result", SqlDbType.Int){
                    Direction=ParameterDirection.Output
                }
            };
            db.Database.ExecuteSqlCommand("sp_Module_Update @id,@name, @description, @result out", parameter);
            var output = parameter.FirstOrDefault(m => m.ParameterName == "@result");
            if (output != null)
            {
                return (int)output.Value;
            }
            return 0;
        }

        public int ModuleDelete(int id)
        {
            var parameter = new SqlParameter[]{
                new SqlParameter("@id", id),
                new SqlParameter("@result", SqlDbType.Int){
                   Direction= ParameterDirection.Output
                }
            };
            db.Database.ExecuteSqlCommand("sp_Module_Delete @id, @result out", parameter);
            var output = parameter.FirstOrDefault(x => x.ParameterName == "@result");
            if (output != null)
            {
                return (int)output.Value;
            }

            return 0;
        }
        public int ModuleProject(int id,int?projectid)
        {
            var parameter = new SqlParameter[]{
                new SqlParameter("@id", id),
                new SqlParameter("@projectid", projectid),
                new SqlParameter("@result", SqlDbType.Int){
                   Direction= ParameterDirection.Output
                }
            };
            db.Database.ExecuteSqlCommand("sp_Module_InsertProject @id,@projectid, @result out", parameter);
            var output = parameter.FirstOrDefault(x => x.ParameterName == "@result");
            if (output != null)
            {
                return (int)output.Value;
            }

            return 0;
        }
        public int DropProject(int id,int?projectid)
        {
            var parameter = new SqlParameter[]{
                   new SqlParameter("@id", id),
                   new SqlParameter("@projectid", projectid),
                   new SqlParameter("@result", SqlDbType.Int){
                   Direction= ParameterDirection.Output
                }
            };
            db.Database.ExecuteSqlCommand("sp_Module_DropProject @id,@projectid, @result out", parameter);
            var output = parameter.FirstOrDefault(x => x.ParameterName == "@result");
            if (output != null)
            {
                return (int)output.Value;
            }

            return 0;
        }
        public int FeatureInsert(string name,string desc)
        {
            var parameter = new SqlParameter[]{
                new SqlParameter("@name",name),
                new SqlParameter("@desc",desc),
                new SqlParameter("@result",SqlDbType.Int){
                    Direction=ParameterDirection.Output
                }
            };
            db.Database.ExecuteSqlCommand("sp_Feature_Insert @name,@desc,@result out", parameter);
            var output = parameter.FirstOrDefault(f => f.ParameterName == "@result");
            if (output != null)
            {
                return (int)output.Value;
            }
            return 0;
        }
        public int FeatureUpdate(int id,string name, string desc)
        {
            var parameter = new SqlParameter[]{  //SqlParameter dung de truyen tham so xuong sql
                new SqlParameter("@id",id),
                new SqlParameter("@name",name),
                new SqlParameter("@desc",desc),
                new SqlParameter("@result",SqlDbType.Int){
                    Direction=ParameterDirection.Output
                }
            };
            db.Database.ExecuteSqlCommand("sp_Feature_Update @id,@name,@desc,@result out", parameter);
            var output = parameter.FirstOrDefault(f => f.ParameterName == "@result");
            if (output != null)
            {
                return (int)output.Value;
            }
            return 0;
        }
        public int FeatureDelete(int id)
        {
            var parameter = new SqlParameter[]{
                new SqlParameter("@id",id),
                new SqlParameter("@result",SqlDbType.Int){
                    Direction=ParameterDirection.Output
                }
            };
            db.Database.ExecuteSqlCommand("sp_Feature_Delete @id,@result out", parameter);
            var output = parameter.FirstOrDefault(f => f.ParameterName == "@result");
            if(output!=null)
            {
                return (int)output.Value;
            }
            return 0;
        }
        public int FeatureModule(int id, int? moduleid)
        {
            var parameter = new SqlParameter[]{
                new SqlParameter("@id",id),
                new SqlParameter("@moduleid",moduleid),
                new SqlParameter("@result",SqlDbType.Int){
                    Direction=ParameterDirection.Output
                }
            };
            db.Database.ExecuteSqlCommand("sp_Feature_InsertModule @id,@moduleid,@result out", parameter);
            var output = parameter.FirstOrDefault(f => f.ParameterName == "@result");
            if (output != null)
            {
                return (int)output.Value;
            }
            return 0;

        }
        public int DropModule(int id,int?moduleid)
        {
            var parameter = new SqlParameter[]{
                new SqlParameter("@id",id),
                 new SqlParameter("@moduleid",moduleid),
                    new SqlParameter("@result",SqlDbType.Int){
                    Direction=ParameterDirection.Output
                }
            };
            db.Database.ExecuteSqlCommand("sp_Feature_DropModule @id,@moduleid,@result out", parameter);
            var output = parameter.FirstOrDefault(f => f.ParameterName == "@result");
            if (output != null)
            {
                return (int)output.Value;
            }
            return 0;
        }
       
    }
}
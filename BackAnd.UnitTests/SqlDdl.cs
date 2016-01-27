using Durados;
using Durados.DataAccess;
using Durados.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackAnd.UnitTests
{
    public class SqlDdl
    {
        public static void AddBasicTable(string connectionString, string appName, string tableName)
        {
            MySqlAccess sqlAccess = new MySqlAccess();

            string sql = string.Format("CREATE TABLE `{0}` ( " +
  "`id` INT NOT NULL AUTO_INCREMENT, " +
  "`name` VARCHAR(45) NULL, " +
  "PRIMARY KEY (`id`));", tableName);

            sqlAccess.ExecuteNonQuery(connectionString, sql, SqlProduct.MySql);
        }
    }
}

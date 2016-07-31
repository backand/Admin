using Durados;
using Durados.Web.Mvc.Controllers;
using Durados.Web.Mvc.Infrastructure;

namespace Durados.Web.Mvc.UI.Helpers
{
    public  class NewDatabaseParameters
    {
        public string InstanceName { get; set; }
        public string DbName { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Engine { get; set; }
        public string EngineVersion { get; set; }

        private string characterSetName;
        public string CharacterSetName
        {
            get
            {
                return characterSetName;
            }
            set
            {
                characterSetName = value;
            }
        }

        private string zone;
        public string Zone
        {
            get
            {
                return zone;
            }
            set
            {
                zone = value;
            }
        }
        
        public NewDatabaseParameters()
        {
            characterSetName = System.Configuration.ConfigurationManager.AppSettings["NewcharacterSetName"] ?? "ASCII";
            zone = System.Configuration.ConfigurationManager.AppSettings["NewZoneName"]?? AmazonDatabaseRegion.us_east_1.ToString().Replace('_','-');
            
        }





        public SqlProduct SqlProduct { get; set; }
    }
    public  class NewDatabaseFactory
    {
        private string appId = string.Empty;
        public string  AppId { get; set; }
        public virtual NewDatabaseParameters GetNewParameters(SqlProduct product,string id)
        {
            AppId = id ?? string.Empty;
            NewDatabaseParameters parameters = GetNewBaseParameters();
            parameters.Port = GetDefaultProductPort(product);
            return parameters;
        }

        protected virtual NewDatabaseParameters GetNewBaseParameters()
        {
            
            return new NewDatabaseParameters
            {

                DbName = (GetPrefix() + Credentials.GenerateCredential(8)).ToLower()//comply with MySql,PostgreSql,SQlServer
               ,
                Password = Credentials.GenerateCredential(22)
               ,
                Username = (GetRandomLetter() + Credentials.GenerateCredential(14)).ToLower()
                ,
                InstanceName = (GetPrefix() + Credentials.GenerateCredential(8)).ToLower()
                ,
            };


        }

        private string GetRandomLetter()
        {
          System.Random random = new System.Random((int)System.DateTime.Now.Ticks);//thanks to McAden
       
            return  System.Convert.ToChar(System.Convert.ToInt32(System.Math.Floor(26 * random.NextDouble() + 65))).ToString();                 
            
        }

        public virtual string GetPrefix()
        {
            return   (System.Configuration.ConfigurationManager.AppSettings["NewDBPrefix"] ?? Database.LongProductName) + ((AppId.Length>10)?AppId.Substring(0,10):AppId);
        }
        public virtual string GetSqlServerEnginVersion()
        {
            return System.Configuration.ConfigurationManager.AppSettings["NewSqlServerEnginVersion"] ?? "10.50.2789.0.v1";
        }
        public virtual string GetMySQLEnginVersion()
        {
            return System.Configuration.ConfigurationManager.AppSettings["NewMySQLEnginVersion"] ?? "5.6.21";
        }
        public virtual string GetPostgreSQLEnginVersion()
        {
            return System.Configuration.ConfigurationManager.AppSettings["NewPostgreSQLEnginVersion"] ?? "9.3";
        }
        public virtual string GetOracleEnginVersion()
        {
            return System.Configuration.ConfigurationManager.AppSettings["NewOracleEnginVersion"] ?? "11.2.0.2.v2";
        }
        public  int GetDefaultProductPort(Durados.SqlProduct sqlProduct)
        {
            switch (sqlProduct)
            {
                case Durados.SqlProduct.MySql:
                    return 3306;
                case Durados.SqlProduct.Postgre:
                    return 5432;
                case Durados.SqlProduct.Oracle:
                    return 1542;

                default:
                    return 1433;
            }
        }
    }

    public class RDSNewDatabaseParameters : NewDatabaseParameters
    {
    }
    public class RDSNewDatabaseFactory : NewDatabaseFactory
    {
        public override NewDatabaseParameters GetNewParameters(SqlProduct product,string id)
        {
          
            NewDatabaseParameters parametes = base.GetNewParameters(product,id);
            switch (product)
            {
                case SqlProduct.SqlServer:
                    parametes.DbName = null;
                    parametes.Engine = RDSEngin.sqlserver_ee.ToString().Replace('_', '-');
                    parametes.EngineVersion = GetSqlServerEnginVersion();
                    return parametes;

                case SqlProduct.MySql:
                    parametes.Engine = RDSEngin.MySQL.ToString();
                    parametes.EngineVersion = GetMySQLEnginVersion();
                    return parametes;

                case SqlProduct.Postgre:
                    parametes.Engine = RDSEngin.postgres.ToString();
                    parametes.EngineVersion = GetPostgreSQLEnginVersion();
                    return parametes;

                case SqlProduct.Oracle:
                    parametes.Engine = RDSEngin.oracle_ee.ToString().Replace('_', '-');
                    parametes.EngineVersion = GetOracleEnginVersion();
                    parametes.DbName = "ORCL";
                    return parametes;

            }
            return null;
        }

        public static Durados.SqlProduct? GetSqlProductfromTemplate(string template)
        {
            switch (template)
            {
                case "10":
                    return Durados.SqlProduct.MySql;
                case "11":
                    return Durados.SqlProduct.Postgre;
                case "12":
                    return Durados.SqlProduct.SqlServer;

                default:
                    break;
            }
            return null;
        }





    }

    //enum RDSDBInstanceClass
    //{
    //    db_t1_micro, db_m1_small, db_m1_medium, db_m1_large, db_m1_xlarge, db_m2_xlarge, db_m2_2xlarge, db_m2_4xlarge, db_m3_medium, db_m3_large, db_m3_xlarge, db_m3_2xlarge, db_r3_large, db_r3_xlarge, db_r3_2xlarge, db_r3_4xlarge, db_r3_8xlarge, db_t2_micro, db_t2_small, db_t2_medium
    //}

    public enum RDSEngin
    {
        MySQL, oracle_se1, oracle_se, oracle_ee, sqlserver_ee, sqlserver_se, sqlserver_ex, sqlserver_web, postgres
    }
    enum AmazonDatabaseRegion
    {
        us_east_1   // US East (N. Virginia)
        ,
        us_west_2  // US West (Oregon)    //   ,
            ,
        us_west_1  // US West (N. California) 
            ,
        eu_west_1  //EU (Ireland)  
            ,
        eu_central_1 // EU (Frankfurt) 
            ,
        ap_southeast_1  // Asia Pacific (Singapore)    ap_southeast_1    
            ,
        ap_southeast_2  // Asia Pacific (Sydney)    ap_southeast_2    
            ,
        ap_northeast_1  // Asia Pacific (Tokyo)    ap_northeast_1   
        , sa_east_1     //South America (Sao Paulo)    sa_east_1 
    }
}
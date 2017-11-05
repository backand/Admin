using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Durados.DataAccess
{
    public interface  ISqlMainSchema
    {
        IDbConnection GetNewConnection();

        IDbConnection GetNewConnection(string connectionString);

        IDbCommand GetNewCommand();
        IDbCommand GetNewCommand(string sql, IDbConnection connection);
        string GetEmailBySocialIdSql();
        
         string GetEmailBySocialIdSql2();
        
         string GetSocialIdlByEmail();
        
         string GetSocialIdlByEmail2();

        string InsertNewUserSql(string tableName, string userTable);
        
         string GetInsertUserAppSql();
        
         string GetUserIdFromUsernameSql();
        
         string GetUserTempTokenSql();

         string GetUserNameByGuidSql();
        

         string GetDeleteUserSql();
        
         string GetUserBelongToMoreThanOneAppSql();
        
         string GetHasAppsSql();
        
         string GetInviteAdminBeforeSignUpSql(string username, string appId);
        
         string GetInviteAdminAfterSignupSql(string username);
        
         string GetInviteAdminAfterSignupSql(int userId, string appId, string role);
        
         string GetDeleteInviteUser(string username);
         
        string GetAppsPermanentFilter();

        string GetWakeupCallToAppSql();

        string GetAppsCountsql();

        string GetSqlProductSql();

        string GetAppsExistsSql(string appName);

        string GetAppsExistsForUserSql(string appName, int? userId);
        string GetPaymentStatusSql(string appName);
        string GetCurrentAppIdSql(string server, string catalog, string username, string userId);
        string GetPlanForAppSql(int appId);

        string GetHasOtherConnectiosSql(string appDatabase);
        string GetDropDatabaseSql(string name);

        string GetAppNamesWithPrefixSql(string appNamePrefix);

        string GetAppNameByGuidFromDb(string guid);

        string GetFindAndUpdateAppInMainSql(int? templateId);

        string GetUpdateLogModelExceptionSql();

        string GetSaveChangesIndicationFromDb2(string Id);

        string GetSetSaveChangesIndicationFromDbSql(int config, string Id);

        string GetLogModelSql();

        string GetAppLimitSql(string Id);//"select Name, Limit from durados_AppLimits with(nolock) where AppId = " + Id

        string GetDeleteUserSql(int userId, string appId); //string.Format("delete durados_UserApp where UserId = {0} and AppId = {1}"



        string GetUpdateAppSystemConnectionSql(int? sysConnId, string p);

        string GetUpdateDBStatusSql(int p, int appId);

        string GetAppIdSql(int templateId);

        string GetDeleteAppById(int id);

        string GetUpdateAppConnectionsSql(int? appConnId, int? sysConnId, string p);

        string GetExecCreateDB(string sysCatalog);

        string GetUpdateAppProduct();

        string GetDbStatusSql(string appId);

        string GetAppNameByIdSqlSql(int appId);

        string InsertNewConnectionToExternalServerTable();

        string GetValidateSelectFunctionExistsSql();

        
        string GetCreatorSql(int appId);

        string GetCreatorUsername(int appId);

        string GetNewDatabaseNameSql(int p, int templateAppId);

        string GetAppSql();

        string GetUserGuidSql();

        string GetAppRowByNameSql(string appName);

        string GetAppNameByTokenSql(string p);

        string GetUpdateAppToBeDeleted();

        string GetValidateUserSql(int appID, int userId);

        string GetLoadUserDataByGuidSql();

        string GetLoadUserDataByUsernameSql(string userFields, string userViewName, string userFieldName);

        string GetUserFieldsForSelectSql();

        string GetUsernameByUsernameSql();

        string GetUsernameByUsernameInUseSql();


        string InsertIntoPluginRegisterUsersSql();

        string InsertIntoUserSql();

        string GetExternalConnectionIdsSql();

        string GetDeleteAppByName(string id);

        string GetAppGuidByName();


        string GetAppGuidById();
        string GetUserAappIdSql();

        string GetAppsNameSql();

        string GetInsertLimitsSql(Limits limits, int limit, int? id);

        string GetUpdateAppConnectionAndProductSql(string newConnectionId, string image, string pk);

        string GetInsertIntoUsersSql(string viewName);
        string GetInsertIntoUsersSql2(string viewName);
    }
}

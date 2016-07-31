using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

using Durados;
using Durados.DataAccess;
using Durados.Web.Mvc.UI.Helpers;
using Durados.Web.Membership;


namespace Durados.Web.Mvc.Specifics.SanDisk.Allegro.Controllers
{
    public class AllegroPORController : AllegroHomeController
    {
        public JsonResult GetProductClassCapacities(string pk, string fk)
        {
            Durados.Web.Mvc.Specifics.SanDisk.Allegro.BusinessLogic.Capacity capacity =
                new Durados.Web.Mvc.Specifics.SanDisk.Allegro.BusinessLogic.Capacity();

            int porID = -1;
            int productClassID = -1;

            if (int.TryParse(pk, out porID) && int.TryParse(fk, out productClassID))
                return Json(capacity.GetCapacityInfo(productClassID, porID));
            else
                return Json(new Durados.Web.Mvc.Specifics.SanDisk.Allegro.BusinessLogic.Capacity());

        }

        //protected override Durados.Web.Mvc.Workflow.Engine CreateWorkflowEngine()
        //{
        //    return new BusinessLogic.Workflow.SanDiskWorkfowEngine();
        //}

        //protected override Durados.Web.Mvc.UI.Styler GetNewStyler()
        //{
        //    return new Durados.Web.Mvc.Specifics.SanDisk.Allegro.UI.PorStyler();
        //}
        protected override void AfterCreateBeforeCommit(CreateEventArgs e)
        {

            if (e.Command == null)
            {
                throw new DuradosException("Missing command in after edit before commit");
            }

            CreateCapacities(Convert.ToInt32(e.PrimaryKey), Convert.ToInt32(((Durados.Web.Mvc.Database)Database).GetUserID()), (SqlCommand)e.Command);

            base.AfterCreateBeforeCommit(e);
        }


        protected override void AfterEditBeforeCommit(EditEventArgs e)
        {

            if (e.Command == null)
            {
                throw new DuradosException("Missing command in after edit before commit");
            }

            CreateCapacities(Convert.ToInt32(e.PrimaryKey), Convert.ToInt32(((Durados.Web.Mvc.Database)Database).GetUserID()), (SqlCommand)e.Command);

            base.AfterEditBeforeCommit(e);

        }

        
        private void CreateCapacities(int id, int userId, System.Data.SqlClient.SqlCommand command)
        {
            
            List<int> capacities = new List<int>();

            // get capacities to create that not already exists
            string sql = "SELECT PORProductClassCapacity.ProductCapacityId as CapacityId " +
		"FROM     PORProductClassCapacity WITH (NOLOCK)  LEFT OUTER JOIN " +
						  "PORCapacity WITH (NOLOCK)  ON PORProductClassCapacity.PORId = PORCapacity.PORId AND PORProductClassCapacity.ProductCapacityId = PORCapacity.CapacityId " +
		"WHERE  (PORCapacity.CapacityId IS NULL) AND (PORProductClassCapacity.PORId = " + id.ToString() + ")";

            command.CommandText = sql;
            System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                capacities.Add(reader.GetInt32(0));

            }

            reader.Close();

            foreach (int capacity in capacities)
            {
                sql = "insert into PORCapacity (PORId, CapacityID) values (" + id + ", " + capacity + "); SELECT IDENT_CURRENT('[PORCapacity]') AS ID";
                command.CommandText = sql;
                int PORCapacityId = Convert.ToInt32(command.ExecuteScalar());

                
                sql = "insert into durados_ChangeHistory (ViewName, PK, ActionId, UpdateUserId) values ('v_PORCapacity', '" + PORCapacityId + "', 1, " + userId + ")";
                command.CommandText = sql;
                command.ExecuteNonQuery();

                sql = "insert into PMM (PORCapacityId) values (" + PORCapacityId + "); SELECT IDENT_CURRENT('[PMM]') AS ID";
                command.CommandText = sql;
                int PMMId = Convert.ToInt32(command.ExecuteScalar());

                sql = "insert into durados_ChangeHistory (ViewName, PK, ActionId, UpdateUserId) values ('v_SamplesRequest', '" + PMMId + "', 1, " + userId + ")";
                command.CommandText = sql;
                command.ExecuteNonQuery();

                sql = "insert into PLM (PORCapacityId,PLMBEStatusId) values (" + PORCapacityId + ",5); SELECT IDENT_CURRENT('[PLM]') AS ID";
                command.CommandText = sql;
                int PLMId = Convert.ToInt32(command.ExecuteScalar());

                sql = "insert into durados_ChangeHistory (ViewName, PK, ActionId, UpdateUserId) values ('v_PLM', '" + PLMId + "', 1, " + userId + ")";
                command.CommandText = sql;
                command.ExecuteNonQuery();

                CreatePLMParameterModes(PLMId, userId, command);

                sql = "insert into Test (PORCapacity) values (" + PORCapacityId + "); SELECT IDENT_CURRENT('[Test]') AS ID";
                command.CommandText = sql;
                int TestId = Convert.ToInt32(command.ExecuteScalar());

                sql = "insert into durados_ChangeHistory (ViewName, PK, ActionId, UpdateUserId) values ('v_TEST', '" + TestId + "', 1, " + userId + ")";
                command.CommandText = sql;
                command.ExecuteNonQuery();

                
            }
        }

        private void CreatePLMParameterModes(int plmId, int userId, System.Data.SqlClient.SqlCommand command)
        {
            List<BusinessLogic.Json.PlmParameterModeInfo> modes = new List<BusinessLogic.Json.PlmParameterModeInfo>();

            string sql = "SELECT Id, DefaultHostSpeedId, DefaultPowerId FROM PLMParameterModeType WHERE (Id NOT IN (SELECT Id FROM PLMParameterMode WHERE (PLMId = " + plmId + ")))";
            
            command.CommandText = sql;
            System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                BusinessLogic.Json.PlmParameterModeInfo mode = new Durados.Web.Mvc.Specifics.SanDisk.Allegro.BusinessLogic.Json.PlmParameterModeInfo();
                mode.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                if (!reader.IsDBNull(reader.GetOrdinal("DefaultHostSpeedId")))
                {
                    mode.DefaultHostSpeedId = reader.GetInt32(reader.GetOrdinal("DefaultHostSpeedId"));
                }
                if (!reader.IsDBNull(reader.GetOrdinal("DefaultPowerId")))
                {
                    mode.DefaultPowerId = reader.GetInt32(reader.GetOrdinal("DefaultPowerId"));
                }
                modes.Add(mode);

            }

            reader.Close();

            foreach (BusinessLogic.Json.PlmParameterModeInfo mode in modes)
            {

                sql = "insert into PLMParameterMode (PLMId, PLMParameterModeTypeId" + (mode.DefaultHostSpeedId.HasValue ? ",HostSpeedId" : "") + (mode.DefaultPowerId.HasValue ? ",PowerId" : "") + ") values (" + plmId + "," + mode.Id + (mode.DefaultHostSpeedId.HasValue ? "," + mode.DefaultHostSpeedId.Value : "") + (mode.DefaultPowerId.HasValue ? "," + mode.DefaultPowerId.Value : "") + "); SELECT IDENT_CURRENT('[PLMParameterMode]') AS ID";
                command.CommandText = sql;
                int id = Convert.ToInt32(command.ExecuteScalar());

                sql = "insert into durados_ChangeHistory (ViewName, PK, ActionId, UpdateUserId) values ('PLMParameterMode', '" + id + "', 1, " + userId + ")";
                command.CommandText = sql;
                command.ExecuteNonQuery();

            }
        }
    }
}

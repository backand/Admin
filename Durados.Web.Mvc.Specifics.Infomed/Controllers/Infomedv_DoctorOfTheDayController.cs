using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

using Durados;
using Durados.DataAccess;
using Durados.Web.Mvc.UI.Helpers;

namespace Durados.Web.Mvc.Specifics.Infomed.Controllers
{
    public class Infomedv_DoctorOfTheDayController : InfomedBaseController
    {
        Durados.Web.Mvc.Workflow.Notifier notifier = new Durados.Web.Mvc.Workflow.Notifier();

        protected override void BeforeCreate(CreateEventArgs e)
        {
            base.BeforeCreate(e);
            if (e.View.Name == "v_DoctorOfTheDay")
            {
                int? expertId = GetExpertId(e);
                if (expertId != null)
                {
                    SetPromotionPhrase(e, expertId);
                    SetPromotionId(e,expertId);
                }
                else
                {
                    throw new DuradosException("Faild to get Expert Id ");
                }
            }
           
        }

        private int? GetExpertId(CreateEventArgs e)
        {
            if (e.Values.ContainsKey("FK_v_DoctorOfTheDay_Expert_v_DoctorOfTheDay_Parent"))
            {
                return Convert.ToInt32(e.Values["FK_v_DoctorOfTheDay_Expert_v_DoctorOfTheDay_Parent"]);
            }
            else 
                return null;
        }

        private void SetPromotionId(CreateEventArgs e,int? expertId)
        {
            int? promotionId = GetPromotionId(e, expertId);
            if (promotionId.HasValue)
            {
                if (!e.Values.ContainsKey("FK_v_Promotion_v_DoctorOfTheDay_Parent"))
                    e.Values.Add("FK_v_Promotion_v_DoctorOfTheDay_Parent", promotionId.Value);
                else e.Values["FK_v_Promotion_v_DoctorOfTheDay_Parent"] = promotionId.Value;
            }
            else
            {
                throw new DuradosException("Faild To get new promotionId for this expert");
            }
        }

        private int? GetPromotionId(CreateEventArgs e, int? expertId)
        {
            IDbCommand command = e.Command;
            //check if there is a "today's doctor" prmotion already
            command.Parameters.Clear();
            command.Parameters.Add(new SqlParameter("@expertID", expertId));
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "p_promotions_IsExpert_DOT_Exist";

            int? promotionId = null;
            object scalar = command.ExecuteScalar();

            if (!(scalar == null || scalar == DBNull.Value))
            {
                promotionId = Convert.ToInt32(scalar);
            }

            if (!promotionId.HasValue)
            {

                promotionId = GetNewPromotion(command, expertId);

            }
            return promotionId;
        }

        private int? GetNewPromotion(IDbCommand command, int? expertId)
        {
            //IDbCommand command =e.Command;
            command.Parameters.Clear();
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@PromotionType", 10));//HomepageTodaysDoctor;
            command.Parameters.Add(new SqlParameter("@EntityID", expertId));
            command.Parameters.Add(new SqlParameter("@EntityType", 1));//expert entity type
            command.Parameters.Add(new SqlParameter("@EntityName", ""));
            command.Parameters.Add(new SqlParameter("@PromotionLevel", ""));
            command.Parameters.Add(new SqlParameter("@StartDate", DateTime.Today));
            command.Parameters.Add(new SqlParameter("@EndDate", DateTime.Today.AddYears(1)));
            //lstParams.Add(new SqlParameter("@AdminUserID", iAdminUserID));
            command.CommandText = "p_Promotions_AddPromotion";
            object scalar = command.ExecuteScalar();

            if (scalar == null || scalar == DBNull.Value)
            {
                return null;
            }
            int iNewPromotionID = Convert.ToInt32(scalar);

            return iNewPromotionID;
            
        }

        private static void SetPromotionPhrase(CreateEventArgs e, int? expertId)
        {
            if (e.Values.ContainsKey("PromotionPhrase") && !HasPromotionPhrase(e.Values["PromotionPhrase"]))
            {
                    e.Command.Parameters.Clear();
                    string sql = "select PromotionPhrase from tbl_experts with(nolock) where id=" + expertId;
                    e.Command.CommandText = sql;
                    try
                    {
                        object scalar = e.Command.ExecuteScalar();
                        if (scalar == null || scalar == DBNull.Value)
                        {
                            return;
                        }
                        e.Values["PromotionPhrase"] = scalar.ToString();
                    }
                    catch (Exception)
                    {

                        throw new DuradosException("Faild to get Promotion Phrase Id ");;
                    }

                }
            
        }

        private static bool HasPromotionPhrase(object obj)
        {
            bool hasPromotionPharse = true;

            if (obj != null && obj.ToString() == string.Empty)
            {
                hasPromotionPharse = false;
            }

            return hasPromotionPharse;
        }



       
        
    }
}

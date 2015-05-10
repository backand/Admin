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


                SetPromotionPhrase(e);
                //check if there is a "today's doctor" prmotion already
                e.Command.Parameters.Add(new SqlParameter("@expertID", e.Values["FK_v_DoctorOfTheDay_Expert_v_DoctorOfTheDay_Parent"]));
                e.Command.CommandType = CommandType.StoredProcedure;
                e.Command.CommandText = "p_promotions_IsExpert_DOT_Exist";

                int? promotionId = null;
                object scalar = e.Command.ExecuteScalar();

                if (!(scalar == null || scalar == DBNull.Value))
                {
                    promotionId = Convert.ToInt32(scalar);
                }



                if (!promotionId.HasValue)
                {
                    //BusinessLogic.BasePromotion promotion = new BusinessLogic.BasePromotion();
                    //// Promotion type
                    //promotion.PromotionType = 10;//HomepageTodaysDoctor;
                    //// Entity ID
                    //promotion.EntityID = (int)e.Values["FK_v_DoctorOfTheDay_Expert_v_DoctorOfTheDay_Parent"];
                    //// Entity type
                    //promotion.EntityType = 1;//expert entity type
                    //// Entity name
                    ////  promotion.EntityName = expert.FullName;
                    //// Start date
                    //promotion.StartDate = DateTime.Today;
                    //// End date
                    //promotion.EndDate = DateTime.Today.AddYears(1);

                    // Add promotion

                    promotionId = GetNewPromotion(e, e.Command);


                    // bSuccess = (promotionId > 0);
                }
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
           
        }

       

        private static void SetPromotionPhrase(CreateEventArgs e)
        {
            if (e.Values["PromotionPhrase"]!= null && e.Values["PromotionPhrase"] == string.Empty)
            {
                string sql = "select PromotionPhrase from tbl_experts with(nolock) where id=" + e.Values["FK_v_DoctorOfTheDay_Expert_v_DoctorOfTheDay_Parent"];
                e.Command.CommandText = sql;
                e.Values["PromotionPhrase"] = e.Command.ExecuteScalar();
            }
        }


        private int? GetNewPromotion(CreateEventArgs e, IDbCommand command)
        {

            command.Parameters.Clear();
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@PromotionType", 10));//HomepageTodaysDoctor;
            command.Parameters.Add(new SqlParameter("@EntityID", (int)e.Values["FK_v_DoctorOfTheDay_Expert_v_DoctorOfTheDay_Parent"]));
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
     
    
        
    }
}

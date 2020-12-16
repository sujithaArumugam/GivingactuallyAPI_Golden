using firstWebAPI.DataLayer;
using GivingActuallyAPI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.SqlClient;

namespace firstWebAPI.Services
{
    public class ArchivalService
    {
        GivingActuallyEntities Entity = new GivingActuallyEntities();
        public bool ArchivalDb()
        {
            try
            {
                bool result = insertBulk("Tbl_Campaign", "SELECT * FROM [dbo].[Tbl_Campaign] where Status=0 and DATEDIFF(day,  TargetDate,getdate()) >=20");
                result = insertBulk("Tbl_citydetails", "select * from dbo.tbl_citydetails where CityId  in ( Select BResidence from dbo.Tbl_BeneficiaryDetails ben join [dbo].[Tbl_Campaign] cam on ben.StoryId=cam.Id and  cam.Status=0 and DATEDIFF(day,  cam.TargetDate,getdate()) >=20 )");
                result = insertBulk("Tbl_BeneficiaryDetails", "Select * from dbo.Tbl_BeneficiaryDetails ben join [dbo].[Tbl_Campaign] cam on ben.StoryId=cam.Id and  cam.Status=0 and DATEDIFF(day,  cam.TargetDate,getdate()) >=20");
                result = insertBulk("tbl_CampaignDescription", "Select * from dbo.tbl_CampaignDescription ben join [dbo].[Tbl_Campaign] cam on ben.StoryId=cam.Id and  cam.Status=0 and DATEDIFF(day,  cam.TargetDate,getdate()) >=20");
                result = insertBulk("tbl_CampaignDescriptionUpdates", "Select * from dbo.tbl_CampaignDescriptionUpdates ben join [dbo].[Tbl_Campaign] cam on ben.StoryId=cam.Id and  cam.Status=0 and DATEDIFF(day,  cam.TargetDate,getdate()) >=20");
                result = insertBulk("tbl_campaigndonation", "Select * from dbo.tbl_campaigndonation ben join [dbo].[Tbl_Campaign] cam on ben.StoryId=cam.Id and  cam.Status=0 and DATEDIFF(day,  cam.TargetDate,getdate()) >=20");
                result = insertBulk("tbl_campaignwithdrawrequest", "Select * from dbo.tbl_campaignwithdrawrequest ben join [dbo].[Tbl_Campaign] cam on ben.CampaignId=cam.Id and  cam.Status=0 and DATEDIFF(day,  cam.TargetDate,getdate()) >=20");
                result = insertBulk("tbl_cmpnBenBankDetails", "Select * from dbo.tbl_cmpnBenBankDetails ben join [dbo].[Tbl_Campaign] cam on ben.CampaignId=cam.Id and  cam.Status=0 and DATEDIFF(day,  cam.TargetDate,getdate()) >=20");
                result = insertBulk("tbl_Endorse", "Select * from dbo.tbl_Endorse ben join [dbo].[Tbl_Campaign] cam on ben.CampaignId=cam.Id and  cam.Status=0 and DATEDIFF(day,  cam.TargetDate,getdate()) >=20");
                result = insertBulk("tbl_like", "Select * from dbo.tbl_like  ben join [dbo].[Tbl_Campaign] cam on ben.StoryId=cam.Id and  cam.Status=0 and DATEDIFF(day,  cam.TargetDate,getdate()) >=20");
                result = insertBulk("tbl_ParentComment", "Select * from dbo.tbl_ParentComment ben join [dbo].[Tbl_Campaign] cam on ben.StoryId=cam.Id and  cam.Status=0 and DATEDIFF(day,  cam.TargetDate,getdate()) >=20");
                result = insertBulk("tbl_Shares", "Select * from dbo.tbl_Shares ben join [dbo].[Tbl_Campaign] cam on ben.StoryId=cam.Id and  cam.Status=0 and DATEDIFF(day,  cam.TargetDate,getdate()) >=20");
                result = insertBulk("Tbl_StoriesAttachment", "Select * from dbo.Tbl_StoriesAttachment ben join [dbo].[Tbl_Campaign] cam on ben.StoryId=cam.Id and  cam.Status=0 and DATEDIFF(day,  cam.TargetDate,getdate()) >=20");
                result = insertBulk("tbl_UpdatesAttachment", "Select * from dbo.tbl_UpdatesAttachment ben join [dbo].[Tbl_Campaign] cam on ben.StoryId=cam.Id and  cam.Status=0 and DATEDIFF(day,  cam.TargetDate,getdate()) >=20");
                result = insertBulk("tbl_withdrawREquest_History", "Select * from dbo.tbl_withdrawREquest_History ben join [dbo].[Tbl_Campaign] cam on ben.CampaignId=cam.Id and  cam.Status=0 and DATEDIFF(day,  cam.TargetDate,getdate()) >=20");
                result = insertBulk("tbl_WithdrawRequestHistory", "Select * from dbo.tbl_WithdrawRequestHistory ben join [dbo].[Tbl_Campaign] cam on ben.CampaignId=cam.Id and  cam.Status=0 and DATEDIFF(day,  cam.TargetDate,getdate()) >=20");

                Entity.DeleteInactiveCampaign();
                return true;
            }
            catch (Exception ex) { throw ex; }
        }
        public bool insertBulk(string table, string query)
        {
            try
            {
                string Source = ConfigurationManager.ConnectionStrings["GivingActuallysource"].ConnectionString;
                string Destination = ConfigurationManager.ConnectionStrings["GivingActuallyArchive"]
                                     .ConnectionString;
                using (SqlConnection sourceCon = new SqlConnection(Source))
                {
                    SqlCommand cmd = new SqlCommand(query, sourceCon);
                    sourceCon.Open();
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        using (SqlConnection destinationCon = new SqlConnection(Destination))
                        {
                            using (SqlBulkCopy bc = new SqlBulkCopy(destinationCon))
                            {
                                bc.BatchSize = 10000;
                                bc.NotifyAfter = 5000;
                                bc.SqlRowsCopied += (sender, eventArgs) =>
                                {

                                };

                                bc.DestinationTableName = table;
                                destinationCon.Open();
                                bc.WriteToServer(rdr);
                            }
                        }
                    }

                }
                return true;
            }
            catch (Exception ex) { throw ex; }

        }
    }
}
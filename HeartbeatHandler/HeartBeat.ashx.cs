using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Configuration;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace HeartbeatHandler
{
    /// <summary>
    /// Summary description for HeartBeat
    /// </summary>
    public class HeartBeat : IHttpHandler
    {
        private static readonly string STATUS_OK = "ok";
        private static readonly string STATUS_FAIL = "fail";
        private static string overallStatus = STATUS_OK;

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Clear();
            context.Response.ContentType = "application/json";
            context.Response.ContentEncoding = Encoding.UTF8;

            JProperty dataBase = new JProperty("databaseStatuses", TestDatabaseStatuses());
            JProperty linkService =   new JProperty("linkStatuses", TestLinkStatuses());
            JProperty wsService = new JProperty("wsStatuses", TestWsStatuses());
            JProperty overAllStatus = new JProperty("overallStatus", overallStatus);

            JObject jsonResponse = new JObject(overAllStatus, dataBase, linkService, wsService);

            context.Response.Write(JsonConvert.SerializeObject(jsonResponse));
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        private static JArray TestDatabaseStatuses()
        {
            JArray databaseStatuses = new JArray();
            foreach (ConnectionStringSettings conSetting in ConfigurationManager.ConnectionStrings)
            {
                SqlConnection connection = new SqlConnection(conSetting.ToString());

                try
                {
                    connection.Open();
                }
                catch (Exception){
                }

                if ((connection.State & ConnectionState.Open) > 0)
                {
                    databaseStatuses.Add(constructStatusJson(true, conSetting.Name, 0.0d));
                    connection.Close();
                }
                else
                {
                    databaseStatuses.Add(constructStatusJson(false, conSetting.Name, 0.0d));
                    overallStatus = STATUS_FAIL;
                }
            }

            return databaseStatuses;
        }

        /// <summary>
        /// The link.sln needs to be running for these tests to pass
        /// </summary>
        private static JArray TestLinkStatuses()
        {
            JArray linkStatuses = new JArray();
            overallStatus = STATUS_FAIL;
            return linkStatuses;
        }

        /// <summary>
        /// The mipws.sln needs to be running for these tests to pass
        /// </summary>
        private static JArray TestWsStatuses()
        {
            JArray wsStatuses = new JArray();


            // find the demo campaign which tests mip_portal connectivity
            //VORoiCampaign demoCampaigns = new RoiCampaignCoreService().FindRoiCampaignById(1);

            //wsStatuses.Add(constructStatusJson(true, "mip_portal", 0.0d));

            // find a provider (andrew stiles) from mip_dw 
            //new ProviderProfileCoreService().FindFullProviderProfileByIdCampaign(1257455, 1);

            //wsStatuses.Add(constructStatusJson(true, "mip_dw", 0.0d));
            AddClass a = new AddClass();
            return wsStatuses;
        }


        private static JValue SetSatus()
        {
            JValue status = new JValue("true");
            return status;

        }
        private static JObject constructStatusJson(bool ok, string name, double timeInMilliseconds)
        {
            return new JObject(new JProperty("status", ok ? STATUS_OK : STATUS_FAIL),
                               new JProperty("name", name),
                               new JProperty("time", timeInMilliseconds));
        }
    }
}
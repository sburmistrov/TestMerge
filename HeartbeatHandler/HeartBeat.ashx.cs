using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization.Json;
using System.IO;

namespace HeartbeatHandler
{
    /// <summary>
    /// Summary description for HeartBeat
    /// </summary>
    public class HeartBeat : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.ContentEncoding = Encoding.UTF8;

            string strServer = "Data Source=(local);Initial Catalog=";
            string strSecurity = ";Integrated Security=SSPI";

            string[] Services = { "mip_dw", "mip_portal", "mip_user" };

            string statusOk = "OK";
            string statusNotOk = "Not OK";
            string strResponse = string.Empty;

            List<ConnectionStatus> results = new List<ConnectionStatus>();
            OverAllStatus overallStatus = new OverAllStatus();
            overallStatus.overallStatus = "success";

            foreach (string service in Services)
            {
                SqlConnection connection = new SqlConnection(strServer + service + strSecurity);

                try
                {
                    connection.Open();
                }
                catch (Exception)
                {
                }

                if ((connection.State & ConnectionState.Open) > 0)
                {
                    overallStatus.databases.Add(new ConnectionStatus(service, statusOk));
                    connection.Close();
                }
                else
                {
                    overallStatus.databases.Add(new ConnectionStatus(service, statusNotOk));
                    overallStatus.overallStatus = "fail";
                }
            }

            // check Mip Services
            //
            overallStatus.mipServices.Add(new ConnectionStatus("mip1", statusOk));
            overallStatus.mipServices.Add(new ConnectionStatus("mip2", statusOk));

            // check Link Services
            //
            overallStatus.linkServices.Add(new ConnectionStatus("link1", statusOk));
            overallStatus.linkServices.Add(new ConnectionStatus("link2", statusOk));

            // Serialize the results as JSON
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(overallStatus.GetType());
            MemoryStream memoryStream = new MemoryStream();
            try
            {
                serializer.WriteObject(memoryStream, overallStatus);
                strResponse = Encoding.Default.GetString(memoryStream.ToArray());
            }
            catch (Exception ex) 
            {
                strResponse = "Processing Error: " + ex.Message;
            }

            context.Response.Write(strResponse);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
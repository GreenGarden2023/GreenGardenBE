
using System.Net;
using System.Text;

namespace GreeenGarden.Business.Service.PaymentService
{
    internal class PaymentRequest
    {
        public PaymentRequest()
        {
        }
        public static string sendPaymentRequest(string endpoint, string postJsonString)
        {

            try
            {
#pragma warning disable SYSLIB0014 // Type or member is obsolete
                HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(endpoint);
#pragma warning restore SYSLIB0014 // Type or member is obsolete

                string postData = postJsonString;

                byte[] data = Encoding.UTF8.GetBytes(postData);

                httpWReq.ProtocolVersion = HttpVersion.Version11;
                httpWReq.Method = "POST";
                httpWReq.ContentType = "application/json";

                httpWReq.ContentLength = data.Length;
                httpWReq.ReadWriteTimeout = 30000;
                httpWReq.Timeout = 15000;
                Stream stream = httpWReq.GetRequestStream();
                stream.Write(data, 0, data.Length);
                stream.Close();

                HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse();

                string jsonresponse = "";

                using (StreamReader reader = new(response.GetResponseStream()))
                {

                    string temp = "";
                    while ((temp = reader.ReadLine()) != null)
                    {
                        jsonresponse += temp;
                    }
                }


                //todo parse it
                return jsonresponse;
                //return new MomoResponse(mtid, jsonresponse);

            }
            catch (WebException e)
            {
                return e.Message;
            }
        }
    }
}

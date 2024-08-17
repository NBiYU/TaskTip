using System;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TaskTip.Extends.ConnectClient.Model;

namespace TaskTip.Extends.ConnectClient
{
    public class HttpRequest
    {
        private HttpClient client;
        private bool _iHtmlFormatCode;
        public Encoding FormatCode = Encoding.UTF8;
        public async Task<ResponseModel> SendAsync(RequestModel req)
        {
            try
            {

                var request = new HttpRequestMessage(req.Method, req.Url)
                {
                    Content = new StringContent(req.Content)
                };

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var bytes = await response.Content.ReadAsByteArrayAsync();

                if (_iHtmlFormatCode)
                {
                    var formatText = Regex.Matches(await response.Content.ReadAsStringAsync(), "charset=.+(?=\")");
                    if (formatText != null)
                    {
                        FormatCode = Encoding.GetEncoding(formatText[0].Value.Split("=")[1]);
                    }
                }

                var objRes = new ResponseModel
                {
                    StatusCode = (int)response.StatusCode,
                    Content = FormatCode.GetString(bytes)
                };
                return objRes;
            }
            catch (Exception e)
            {
                return new ResponseModel()
                {
                    StatusCode = 999,
                    Message = e.Message
                };
            }
        }

        //SSL—È÷§
        private bool CheckSslResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }

        public HttpRequest(bool isSSL = true, bool isHtml = false)
        {
            if (isSSL)
            {
                var hander = new HttpClientHandler();
                hander.ServerCertificateCustomValidationCallback = CheckSslResult;
                client = new HttpClient(hander);
            }
            else
            {
                client = new HttpClient();
            }

            _iHtmlFormatCode = isHtml;
            client.Timeout = TimeSpan.FromSeconds(5);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        ~HttpRequest()
        {
            client.Dispose();
        }
    }
}

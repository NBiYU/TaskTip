using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TaskTip.Services;

namespace TaskTip.Common.ExecuteServices
{
    public class TcpRequestService
    {
        private string _ipAddress;
        private int _port = int.Parse(GlobalVariable.JsonConfiguration["TCPReceive:Port"]);
        private int _timeout = int.Parse(GlobalVariable.JsonConfiguration["TCPReceive:TimeOut"]);
        private AsyncCallback _act;
        private TcpClient _tcpClient;



        public bool Connected;

        public TcpRequestService(string ip, int port = 0, AsyncCallback act = null, int timeout = 0)
        {
            port = port == 0 ? _port : port;
            _timeout = timeout == 0 ? _timeout : timeout;

            _ipAddress = ip;
            _act = act;
            Init();
        }
        private void Init()
        {
            try
            {
                _tcpClient = new TcpClient();
                var result = _tcpClient.BeginConnect(IPAddress.Parse(_ipAddress), _port, _act, Connected);
                result.AsyncWaitHandle.WaitOne(_timeout);
                _tcpClient.EndConnect(result);
                Connected = _tcpClient.Connected;
            }
            catch (Exception e)
            {
                Connected = false;
                throw new Exception($"{_ipAddress}:{_port} 连接失败 - {e.Message}");
            }
        }

        public async Task SendAsync(object obj)
        {
            try
            {
                var content = await ContentFormat(obj);
                await SendAsync(content);
            }
            catch (Exception e)
            {
                GlobalVariable.LogHelper.Error($"【TCP发送】【{_ipAddress}:{_port}】发送失败：{e}");
            }
        }

        public async Task SendAsync(string content)
        {
            try
            {
                if (_tcpClient.Connected)
                {

                    var stream = _tcpClient.GetStream();
                    var sendData = Encoding.UTF8.GetBytes(content);

                    await stream.WriteAsync(sendData, 0, sendData.Length);
                    GlobalVariable.LogHelper.Info($"【TCP发送】【{_ipAddress}:{_port}】发送成功：{content}");
                }
            }
            catch (Exception e)
            {
                GlobalVariable.LogHelper.Error($"【TCP发送】【{_ipAddress}:{_port}】发送失败：{e}");
            }

        }

        public Task<string> ContentFormat(object obj)
        {
            return Task.FromResult(JsonConvert.SerializeObject(obj) + "&");
        }

        public void Close()
        {
            _tcpClient.Close();
        }
    }
}

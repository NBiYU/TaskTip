using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Net.Sockets;
using System.Reflection;
using System.Reflection.Metadata;
using TaskTip.Common.Extends;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TaskTip.Services;
using TaskTip.Enums;
using TaskTip.Models.DataModel;

namespace TaskTip.Common.ExecuteServices
{
    public class TcpListenerService : IHostedService
    {
        private NLog.Logger logger = GlobalVariable.LogHelper;
        private TcpListener tcpListener;
        private int Port;
        public TcpListenerService()
        {
            Port = int.Parse(GlobalVariable.JsonConfiguration["TCPReceive:Port"] ?? "11031");
            tcpListener = new TcpListener(System.Net.IPAddress.Any, Port);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            tcpListener.Start();
            logger.Info("【数据同步】【TCP监听】服务已启动");
            _ = Listener(cancellationToken);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            tcpListener.Stop();
            logger.Info("【数据同步】【TCP监听】服务已关闭");
            return Task.CompletedTask;
        }

        private async Task Listener(CancellationToken cancellationToken)
        {

            while (true)
            {
                TcpClient client = await tcpListener.AcceptTcpClientAsync(cancellationToken);
                _ = Task.Run(async () => await Execute(client), cancellationToken);
            }
        }

        private async Task Execute(TcpClient client)
        {
            try
            {
                if (client.Connected)
                {
                    var remote = (IPEndPoint)client.Client.RemoteEndPoint;
                    var ip = remote.Address.ToString();
                    var port = remote.Port;
                    var isKeep = true;
                    //如果是本地本身则跳出（未实现）

                    logger.Info($"【数据同步】【{ip}:{port}】已连接");

                    while (isKeep)
                    {
                        var networkStream = client.GetStream();

                        if (networkStream.CanRead && client.Available != 0)
                        {
                            var bytesContent = new byte[client.Available];
                            await networkStream.ReadAsync(bytesContent, 0, client.Available);

                            var jsonContent = Encoding.UTF8.GetString(bytesContent);
                            var reqContents = jsonContent.Split("&");
                            foreach (var content in reqContents)
                            {
                                var req = JsonConvert.DeserializeObject<TcpDataModel>(content);

                                if (req != null)
                                {
                                    logger.Info($"【数据同步】【{ip}:{port}】数据解析成功：{content}");

                                    isKeep = req.IsKeep;
                                    var reqKey = KeyHandler.DecryptionKey(req.Key);

                                    var localKey = GlobalVariable.LocalKey;
                                    var isSync = string.IsNullOrEmpty(localKey) && reqKey.Equals(localKey);

                                    switch (req.CommandType)
                                    {
                                        case SyncCommand.SyncRequest:
                                            await SyncRequestHandler(req.RequestData, ip);
                                            break;
                                        case SyncCommand.FileRequest:
                                            await FileRequestHandler(req.RequestData, ip);
                                            break;
                                        case SyncCommand.Push:
                                            await SyncPushHandler(req.RequestData, !isKeep);
                                            break;
                                        case SyncCommand.Modify:
                                            await ModifyHandler(req.RequestData);
                                            break;
                                        case SyncCommand.Keep:
                                            await KeepHandler(req.RequestData);
                                            break;
                                    }

                                    logger.Info($"【数据同步】【{ip}:{port}】【{req.CommandType.GetDesc()}】【{req.RequestData.SyncCategory.GetDesc()}】完成同步");
                                }
                            }
                        }
                        System.Threading.Thread.Sleep(1000);
                    }
                    client.Close();
                }
            }
            catch (Exception ex)
            {
                logger.Error($"【数据同步】【TCP监听】处理数据异常：{ex}");
            }
            finally
            {
                client.Close();
            }
        }

        private async Task FileRequestHandler(TcpRequestData reqData, string ip, int port = 0)
        {
            port = port == 0 ? Port : port;
            var client = new TcpRequestService(ip, port);
            switch (reqData.SyncCategory)
            {
                case SyncFileCategory.RecordFile:
                    var files = Directory.GetFiles(GlobalVariable.RecordFilePath).ToList();
                    var target = files.FirstOrDefault(x => x.Contains(reqData.GUID));

                    if (string.IsNullOrEmpty(target)) return;

                    var targetFileName = Path.GetFileName(target);

                    var path = Path.Combine(GlobalVariable.RecordFilePath, targetFileName);
                    var xamlPath = Path.Combine(GlobalVariable.RecordFilePath, "Xaml", targetFileName);
                    var title = new RecordFileModel();
                    var text = string.Empty;
                    if (File.Exists(path))
                        title = JsonConvert.DeserializeObject<RecordFileModel>(await File.ReadAllTextAsync(path));
                    if (File.Exists(xamlPath)) text = await File.ReadAllTextAsync(xamlPath);


                    await client.SendAsync(new TcpDataModel()
                    {
                        Key = GlobalVariable.LocalKey,
                        CommandType = SyncCommand.Push,
                        IsKeep = true,
                        RequestData = new TcpRequestData()
                        {
                            GUID = targetFileName.Split(".")[0],
                            OperationType = OperationRequestType.Add,
                            SyncCategory = SyncFileCategory.RecordFile,
                            FileData = new RecordFileModel()
                            {
                                Title = title.Title,
                                Text = text
                            }
                        }
                    });
                    break; ;
            }


        }
        //        await client.SendAsync(new TcpDataModel()
        //{
        //    Key = GlobalVariable.LocalKey,
        //    CommandType = SyncCommand.Push,
        //    IsKeep = false,
        //    RequestData = new TcpRequestData()
        //    {
        //        SyncCategory = SyncFileCategory.RecordFile
        //    }
        //});

        //轮询
        private async Task SimileHandler(string ip, int port = 0)
        {
            try
            {
                port = port == 0 ? Port : port;
                var client = new TcpRequestService(ip, port);
                await client.SendAsync(new TcpDataModel()
                {
                    Key = GlobalVariable.LocalKey,
                    CommandType = SyncCommand.Support,
                    IsKeep = false,
                    RequestData = null
                });
            }
            catch (Exception e)
            {
                logger.Error($"【数据同步】【TCP监听】轮询异常：{e}");
            }

        }


        private Task KeepHandler(TcpRequestData requestData)
        {
            throw new NotImplementedException();
        }

        private Task ModifyHandler(TcpRequestData reqData)
        {
            dynamic reqCategory = reqData.SyncCategory switch
            {
                SyncFileCategory.TaskPlan => JsonConvert.DeserializeObject<TaskFileModel>(reqData.FileData.ToString()),
                SyncFileCategory.Record => JsonConvert.DeserializeObject<TreeInfo>(reqData.FileData.ToString()),
            };

            var sendToken = reqData.SyncCategory switch
            {
                SyncFileCategory.TaskPlan => Const.CONST_LISTITEM_CHANGED,
                SyncFileCategory.Record => Const.CONST_NOTIFY_RECORD_ITEM,
            };

            WeakReferenceMessenger.Default.Send(new CorrespondenceModel() { GUID = reqData.GUID, Operation = reqData.OperationType, Message = reqCategory }, sendToken);
            return Task.CompletedTask;
        }

        private Task SyncRequestHandler(TcpRequestData requestData, string ip, int port = 0)
        {
            port = port == 0 ? Port : port;

            Task.Run(async () =>
            {
                try
                {
                    OperationRecord.isRecording = false;
                    var operationList = OperationRecord.OperationRecordRead();
                    var client = new TcpRequestService(ip);
                    var content = new StringBuilder();

                    if (operationList is { Count: > 0 })
                    {

                        foreach (var op in operationList)
                        {
                            content.Append(
                                await client.ContentFormat(new TcpDataModel()
                                {
                                    Key = GlobalVariable.LocalKey,
                                    IsKeep = true,
                                    CommandType = SyncCommand.Push,
                                    RequestData = op
                                }));
                        }
                    }

                    content.Append(await client.ContentFormat(new TcpDataModel()
                    {
                        Key = GlobalVariable.LocalKey,
                        IsKeep = false,
                        CommandType = SyncCommand.Push,
                        RequestData = new TcpRequestData()
                        {
                            SyncCategory = SyncFileCategory.Record
                        }
                    }));

                    await client.SendAsync(content.ToString());

                    logger.Info($"【数据同步】【{ip}:{port}】【{SyncCommand.Push.GetDesc()}】{operationList.Count} 完成推送");
                }
                catch (Exception e)
                {
                    GlobalVariable.LogHelper.Error($"【数据同步】【{ip}:{port}】【{SyncCommand.Push.GetDesc()}】推送异常：{e}");
                }

                OperationRecord.isRecording = true;
            });
            return Task.CompletedTask;
        }

        private Task SyncPushHandler(TcpRequestData requestData, bool completed)
        {
            WeakReferenceMessenger.Default.Send(new CorrespondenceModel() { Operation = (completed ? OperationRequestType.Completed : OperationRequestType.Default), Message = requestData }, Const.CONST_SYNC_RECEIVE);
            return Task.CompletedTask;
        }
    }
}

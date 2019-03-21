using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.ServiceProcess;
using System.Threading;
using System.Xml;

namespace DNSPod.Service
{
    public partial class DNSPodService : ServiceBase
    {
        public DNSPodService()
        {
            InitializeComponent();
        }
        string currentPath = string.Empty;
        string logPath = string.Empty;

        static LoginInfo login = new LoginInfo();
        static List<Record> listRecord = new List<Record>();
        static int Time = 0;
        static string ip = string.Empty;
        protected override void OnStart(string[] args)
        {
            try
            {
                InitData();
                new Thread(delegate ()
                {
                    while (true)
                    {
                        ReferData();
                        Thread.Sleep(Time);
                    }
                }).Start();
                Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                Helpter.WriteLineLog(logPath, LogCategory.Config, ex.Message);
                throw;
            }
        }

        protected override void OnStop()
        {
        }
        void ReferData()
        {
            try
            {
                string _ip = Helpter.GetPublicIP();
                foreach (Record item in listRecord)
                {
                    item.ip = _ip;
                    if (item.ip == item._ip)
                    {
                        Helpter.WriteLineLog(logPath, LogCategory.Api, $"上次轮训ip[{item._ip}]与本次获取ip:[{item.ip}]一致");
                        return;
                    }
                    var json = Helpter.RecordList(login.login_token, item.domain, item.subdomain);
                    if (json.status.code != 1)//操作失败，立刻结束操作
                    {
                        Helpter.WriteLineLog(logPath, LogCategory.Api, "登陆认证失败,轮询已停止");
                        this.Stop();
                        return;
                        //Thread.CurrentThread.Abort();
                    }
                    item._ip = item.ip;
                    int recordId = 0;
                    if (json.records != null && json.records.Count > 0)
                    {
                        if (json.records[0].value == item.ip)//与记录中的ip一致，不需要操作
                        {
                            Helpter.WriteLineLog(logPath, LogCategory.Api, $"[{item.subdomain}.{item.domain}] 记录值 [{item.ip}] 与服务器记录一致不需要修改");
                            continue;
                        }
                        recordId = json.records[0].id;
                    }
                    else
                        recordId = Helpter.RecordCreate(login.login_token, item.domain, item.subdomain, item.ip);
                    if (recordId > 0)
                        json = Helpter.RecordDdns(login.login_token, recordId, item.domain, item.subdomain, item.ip);
                    Helpter.WriteLineLog(logPath, LogCategory.Api, $"[{item.subdomain}.{item.domain}] 记录值 [{item.ip}] 更新成功");
                }
            }
            catch (Exception ex)
            {
                Helpter.WriteLineLog(logPath, LogCategory.Api, ex.Message);
            }
        }

        void InitData()
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                //currentPath = Directory.GetCurrentDirectory() + "\\";
                //currentPath=System.Environment.CurrentDirectory;
                currentPath = AppDomain.CurrentDomain.BaseDirectory;
                logPath = $"{currentPath}logs\\";
                string xml = currentPath + "conf\\conf.xml";
                Directory.CreateDirectory(logPath);
                Helpter.WriteLineLog(logPath, LogCategory.Config, "-------------------------------");
                Helpter.WriteLineLog(logPath, LogCategory.Config, currentPath);
                if (!File.Exists(xml))
                {
                    Helpter.WriteLineLog(logPath, LogCategory.Config, $"配置文件不存在{xml}");
                    throw new Exception($"配置文件不存在{xml}");
                }

                XmlDocument doc = new XmlDocument();
                doc.Load(xml);
                var node = doc.SelectSingleNode("DNSPod.Service");

                Time = Convert.ToInt32(node.SelectSingleNode("Time").Attributes["value"].Value);
                if (Time < 5)
                {
                    throw new Exception("间隔时间太短，必须大于等于5，建议为10");
                }
                Time = Time * 60 * 1000;
                login.id = node.SelectSingleNode("LoginDNSPod/id").InnerText;
                login.token = node.SelectSingleNode("LoginDNSPod/token").InnerText;

                var nodelist = node.SelectNodes("RecordList/Record");

                foreach (XmlNode item in nodelist)
                {
                    listRecord.Add(new Record()
                    {
                        subdomain = item.Attributes["sub_domain"].Value,
                        domain = item.Attributes["domain"].Value,
                    });
                }
            }
            catch (Exception ex)
            {
                Helpter.WriteLineLog(logPath, LogCategory.Config, ex.Message);
                throw;
            }
        }

        public void TestStartupAndStop(string[] args)
        {
            OnStart(args);
            Console.ReadLine();
            OnStop();
        }



    }
}

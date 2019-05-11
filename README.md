# DNSPod.Service

## 项目说明
- 找遍了各个地方，发现基于dnspod的ddns软件都只能运行在linux的shell脚本，于是自己动手写了这个
- 使用DNSPod官方API接口
  - 会检测登陆id与token能否登陆成功，登陆验证失败时，服务不能正常启动
  - 会检测从ip138获取的ip与dnspod已设置的记录值是否一致，防止重复设置相同记录被禁用api
- 基于.NET Framework 4.5
  - .NET 4.5下载地址： [在线安装包](http://go.microsoft.com/fwlink/?LinkId=225704) [离线安装包](http://go.microsoft.com/fwlink/?LinkId=225702)

## 配置说明
- 使用xml文件进行配置
- 轮询时间，必须大于等于5，建议为10，<b>单位分钟</b>

~~~xml
<Time value="10"/>
~~~

- 登陆信息

  - token 申请地址 <a href=https://www.dnspod.cn/console/user/security>https://www.dnspod.cn/console/user/security</a>

  ~~~ xml
   <LoginDNSPod>
      <id>id</id>
      <token>token</token>
    </LoginDNSPod>
  ~~~

- 记录值（可多条，多个域名指向本机ip）

  - sub_domain，主机记录
  - domain，域名

  ~~~xml
  <Record sub_domain="www" domain="domain.com"/>
  ~~~

  

## 运行方式

1. 双击运行后，会以控制台程序方式展示日志信息
   - 多次双击运行，也只会有一个实例，
2. windows服务安装<b>（推荐）</b>
   - 打开cmd，运行命令安装windows服务，在服务管理中设置开机启动运行

~~~ cmd
sc create DNSPod.Service binPath= "C:\DNSPod.Service\DNSPod.Service.exe"
~~~

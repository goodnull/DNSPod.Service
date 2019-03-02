# DNSPod.Service

## 项目说明
- 使用DNSPod官方API接口
- 基于.NET Framework 4.5
- 使用JSON
- 基础信息可配置

## 配置说明
- 轮询时间，必须大于等于5，建议为10

~~~xml
<Time value="10"/>
~~~

- 登陆信息

  - 申请地址 <a href=https://www.dnspod.cn/console/user/security>https://www.dnspod.cn/console/user/security</a>

  ~~~ xml
   <LoginDNSPod>
      <id>id</id>
      <token>token</token>
    </LoginDNSPod>
  ~~~

- 记录值

  - sub_domain，主机记录
  - domain，域名
  - ip，记录值

  ~~~xml
  <RecordList>
      <Record sub_domain="www" domain="domain.com" ip="$ip"/>
    </RecordList>
  ~~~

  
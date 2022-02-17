# MorePractice Malody Server

<p>
状态：
master
<a href="https://github.com/RhythmCodec/MorePracticeMalodyServer/actions/workflows/build.yml?query=branch%3Amaster">
<img src="https://github.com/RhythmCodec/MorePracticeMalodyServer/actions/workflows/build.yml/badge.svg?branch=master">
</a>
&nbsp dev
<a href="https://github.com/RhythmCodec/MorePracticeMalodyServer/actions/workflows/build.yml?query=branch%3Adev">
<img src="https://github.com/RhythmCodec/MorePracticeMalodyServer/actions/workflows/build.yml/badge.svg?branch=dev">
</a>

</p>

## 基本

API版本：**202112**  
程序版本：**1.1.0**  
程序运行时：**ASP.NET Core**

开源的 Malody V 谱面(Chart)服务器。~~低负载基本能用，高负载肯定完蛋~~。  
开箱即用的服务器程序，不需配置即可使用 sqlite 与本地文件系统主持服务器。

## 功能

**目前已经实现的功能：**  
* 谱面上传/下载  
* 谱面搜索（关键字/按id）  
* 谱面列表
* 推荐列表
* 自提供存储  
* 活动列表  
* MySql/SQLServer/sqlite支持

**目前已知有bug的功能：**
* 一切正常 :)
* 1.1.0目前没有完全测试

**目前未实现的功能：**
* 202112的皮肤列表与下载

**将在未来添加的功能：**
* 上传身份限制
* 用户身份限制
* 第三方存储提供
* 皮肤列表与下载
* 支持 docker
* 前端管理界面

## 安装

可以用以下两种方式安装：**二进制文件**或**从源代码编译**

**使用二进制文件安装：**  
1. 在[Release页面](https://github.com/RhythmCodec/MorePracticeMalodyServer/releases)下载最新的二进制文件。
2. 解压到本地，修改配置文件。
3. 运行MorePracticeMalodyServer。根据平台不同，文件名略有差异。
  - **Linux:** MorePracticeMalodyServer
  - **Windows:** MorePracticeMalodyServer.exe
 
**从源代码编译：**  
1. 克隆或下载MorePracticeMalodyServer仓库。
2. 确保已经安装.NET 6 SDK。您可以[从此下载](https://dotnet.microsoft.com/download/dotnet/6.0)。关于如何安装.NET 6 SDK，您可以[阅读此文章](https://docs.microsoft.com/en-us/dotnet/core/install/)。
3. 运行`dotnet restore`。
4. 默认使用sqlite提供数据库，本地文件系统提供存储。若要使用其他提供程序，请参考[修改配置](#修改配置)章节。
5. 若切换了数据库提供程序，请先删除 MorePracticeMalodyServer\Migrations 文件夹内所有内容。之后运行`dotnet ef migrations add InitialCreate`，执行完成后运行`dotnet ef database update`来更新数据库。对于生产数据库，建议运行`dotnet ef migrations script --idempotent`生成SQL脚本，在数据库上运行该脚本以构建数据表。
6. 运行`dotnet build --configuration Release --no-restore`构建应用程序。
7. 在  MorePracticeMalodyServer\bin\Release 下复制构建好的应用程序。

## 修改配置

应用程序配置是appsettings.json。该文件的结构以及修改方式如下：
* Logging
  * LogLevel : 可全部设为`Warning`减少输出
    * Default
    * Microsoft
    * Microsoft.Hosting.Lifetime
  * Data : 数据库相关设置
    * Provider : 数据库提供程序，可设置为`sqlserver`,`mysql`,`sqlite`。默认为sqlite。
    * PoolSize : 连接池的最大连接数。
    * ConnectionString : 连接字符串，具体设置请参考数据库文档。
    * ServerVersion : MySql使用，确定服务器版本。
  * Storage : 存储提供程序相关配置
    * Provider : 存储提供程序的链接，self时为自提供。有关如何扩展存储提供，请参考[自定义存储提供](#自定义存储提供)章节。
  * CheckUid : 如果是202112以上，则可对UID进行验证。true开启，false关闭。

## 修改数据库提供程序的注意事项

如果您修改了数据库提供程序，您必须使用源代码编译的方式来生成数据库迁移以及数据库脚本，来构建您的数据表。  
请参考[安装](#安装)中的**从源代码编译**部分进行操作。
由于目前程序仅测试了使用sqlite，因此其它提供程序可能会出现不可预料的行为。如果程序在您使用时发生异常，请[提交Issue](https://github.com/RhythmCodec/MorePracticeMalodyServer/issues/new/choose)。

## 自定义存储提供

**此部分目前尚未实现！**  
由于各种网络存储供应商提供的访问方式不同，因此此部分需要根据需求进行开发。如果需要对某个提供商进行支持，请[提交Issue](https://github.com/RhythmCodec/MorePracticeMalodyServer/issues/new/choose)。
请将Storage:Provider设置为受支持的选项。目前仅支持`self`。  
游戏使用application/x-www-form-urlencoded传送文件，每次只会发送一个文件，同时附带额外的meta信息。
meta信息为文本键值对。在程序中，我们提供了文件的cid，sid，hash数据，您可以在表单中检索。  
由于文件在您的提供程序上保存，服务器不会尝试下载您的文件进行解析。您需要自行解析谱面文件(chart file | *.mc)的元数据。
解析过后请调用服务器API上传歌曲元数据。（**此API尚未实现**）

## docker支持

程序支持在 docker 上运行。目前没有打包 docker 镜像，但是提供了 dockerfile ，因此需要您自行运行`docker build`进行构建。  
在 docker 上运行的服务器时，如果使用本地文件系统存储，同时需要持久化数据，请映射`/wwwroot`文件夹与`data.sqlite`。他们均位于程序根目录。

## 许可

本程序使用 GPL v3.0 (GNU GENERAL PUBLIC LICENSE v3.0)许可开放源代码，使用本程序或源代码即代表您了解并同意该协议。

**Microsoft.EntityFrameworkCore**
* Apache-2.0

**Microsoft.EntityFrameworkCore.Sqlite**
* Apache-2.0

**Microsoft.EntityFrameworkCore.SqlServer**
* Apache-2.0

**Microsoft.EntityFrameworkCore.Tools**
* Apache-2.0

**Microsoft.VisualStudio.Azure.Containers.Tools.Targets**
* [MICROSOFT SOFTWARE LICENSE TERMS](https://www.nuget.org/packages/Microsoft.VisualStudio.Azure.Containers.Tools.Targets/1.11.1/license)

**Pomelo.EntityFrameworkCore.MySql**
* MIT

**NVorbis**
* Mit


## 感谢
* [@soloopooo](https://github.com/soloopooo)
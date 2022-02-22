# MorePractice Malody Server

<p>
State:
master
<a href="https://github.com/RhythmCodec/MorePracticeMalodyServer/actions/workflows/build.yml?query=branch%3Amaster">
<img src="https://github.com/RhythmCodec/MorePracticeMalodyServer/actions/workflows/build.yml/badge.svg?branch=master">
</a>

&nbsp; dev
<a href="https://github.com/RhythmCodec/MorePracticeMalodyServer/actions/workflows/build.yml?query=branch%3Adev">
<img src="https://github.com/RhythmCodec/MorePracticeMalodyServer/actions/workflows/build.yml/badge.svg?branch=dev">
</a>

</p>

## Basic

API version：**202112**  
Application version：**1.1.0**  
Runtime：**ASP.NET Core**

An open-source Malody V Chart Server. ~Can be run on low loads, but surely booms at high loads.~  
You can use it immediately as you download, no more configuration, while it can be directly run on the server using sqlite and local file system.

## Features

**Features Now Usable:**  
* Charts upload/download
* Charts Searching (using Keywords or id) 
* Chart list
* Promotion list
* Saves file on local server  
* Event list
* Supports MySql/SQLServer/sqlite

**Currently Known Bugs:**
* All right :) EXCEPT 1.1.0 :(
* Ver 1.1.0 is not fully tested.


**Features Under Construction：**
* Skin list and downloading in API ver 202112

**Will be Added：**
* Upload identity restructions
* User identity restructions
* Save on third-party device or website
* Skin list and downloading
* Supports docker
* Frontend management page

## Installation

You can install it in two ways: **binary** or **compile from source code**.

**Binary：**  
1. Download the latest binary files at [release page](https://github.com/RhythmCodec/MorePracticeMalodyServer/releases).
2. Decompress it, modify the profile.
3. Run MorePracticeMalodyServer. File names is slightly different depending on the platform.
  - **Linux:** MorePracticeMalodyServer
  - **Windows:** MorePracticeMalodyServer.exe
 
**Compile from Source Code：**  
1. Clone or download MorePracticeMalodyServer repo.
2. Make sure you have installed .NET 6 SDK. You can [download from here](https://dotnet.microsoft.com/download/dotnet/6.0). You can [read this](https://docs.microsoft.com/en-us/dotnet/core/install/) to get the way about installing .NET 6 SDK.
3. Run `dotnet restore`.
4. Program will use sqlite as default database and local filesystem as default storage. If you are using other providers, please refer to [Modifying the profile](#Modifying the profile).
5. As the database provider changing, Please delete ALL THINGS under MorePracticeMalodyServer\Migrations, then run `dotnet ef migrations add InitialCreate` and `dotnet ef database update` to update it. For production environment databases, `dotnet ef migrations script --idempotent` is promoted, while generates the SQL script to construct the data table.
6. Run `dotnet build --configuration Release --no-restore` to build.
7. Copy bulit files at MorePracticeMalodyServer\bin\Release. 

## Modifying the Profile

Profile: appsettings.json. The following are the structure and modify ways:
* Logging
  * LogLevel : Set `Warning` to reduce logs
    * Default
    * Microsoft
    * Microsoft.Hosting.Lifetime
  * Data : setup databases
    * Provider : Database provider, you can set to `sqlserver`,`mysql`,`sqlite`. sqlite is default.
    * PoolSize : The maximum connection number of connection pool.
    * ConnectionString : Strings of connection. Refer to databases' documentation.
    * ServerVersion : Used by MySql, define version.
  * Storage : setup storage
    * Provider : The storage provider link. Local storage is `self`. For information on how to extend storage provisioning, refer to the section on [custom storage provisioning](#Custom Storage Provisioning).
  * CheckUid : If client use api version 202112, server can check uid validation. Set to `true` to enable and `false` to disable.

## Notes for Modifying the Database Provider

If you modify the database provider, you must use source code compilation to build database migrations and database scripts to build your data tables.  
Refer to **Compile from Source Code** in [Installation](#Installation).
Now only sqlite is tested, as a result, other providers may behave unpredictably. If an exception occurs while the program is in use，please [open a new issue](https://github.com/RhythmCodec/MorePracticeMalodyServer/issues/new/choose).

## Custom Storage Provisioning

**This is now under construction!**  
As there are lots of ways of accessing the web storage provided by various providers, this part will be developed according to the needs. If you need a support for a provider, please [open issue](https://github.com/RhythmCodec/MorePracticeMalodyServer/issues/new/choose).
Please set Storage:Provider as supported options. Currently we only support `self`.
Games use application/x-www-form-urlencoded to send files. Only one file with metadata is sent at a time.
Metadata is text key-value. We provide the cid, sid and hash. It can be retrieved in the form.
As the files are saved on your storage provider, the server won't try to download your files to analyze. You need to parse the metadata of chart files | *.mc.
After parsing, please call server api to upload it(**This Api is under construstion**).

## Docker support

Application can be run on Docker. Although docker images are not currently packaged, there is dockerfile, so you need to run `docker build`.  
When run server on Docker, please map the directory `/wwwroot` and `data.sqlite` if you need local filesystem as storage and persist data. These directorys are all in program's root.

## License

This program uses GPL v3.0 (GNU GENERAL PUBLIC LICENSE v3.0) Agreement. By using this program or source code, you understand and agree to this agreement.

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

## Special thanks to
* [@soloopooo](https://github.com/soloopooo)

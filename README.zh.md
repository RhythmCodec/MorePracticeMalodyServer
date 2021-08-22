# MorePractice Malody Server

<p>
״̬��
<div>master
<a href="https://github.com/RhythmCodec/MorePracticeMalodyServer/actions/workflows/build.yml?query=branch%3Amaster">
<img src="https://github.com/RhythmCodec/MorePracticeMalodyServer/actions/workflows/build.yml/badge.svg?branch=master">
</a>
</div>
<div>dev
<a href="https://github.com/RhythmCodec/MorePracticeMalodyServer/actions/workflows/build.yml?query=branch%3Adev">
<img src="https://github.com/RhythmCodec/MorePracticeMalodyServer/actions/workflows/build.yml/badge.svg?branch=dev">
</a>
</div>
</p>

## ����

API�汾��**202103**  
����汾��**1.0.0**
��������ʱ��**ASP.NET Core**

��Դ�� Malody V ����(Chart)��������~~�͸��ػ������ã��߸��ؿ϶��군~~��  
���伴�õķ��������򣬲������ü���ʹ�� sqlite �뱾���ļ�ϵͳ���ַ�������

## ����

**Ŀǰ�Ѿ�ʵ�ֵĹ��ܣ�**  
* �����ϴ�/����  
* �����������ؼ���/��id��  
* ���ṩ�洢  
* �����б�
* ��б�  
* MySql/SQLServer/sqlite֧��

**Ŀǰ��֪��bug�Ĺ��ܣ�**
* ����������  
  * ���ؼ���λ�ڿ�ͷʱ���᷵�ؽ��������EF Core��bug���������Ȥ������[�ڴ�׷��](https://github.com/dotnet/efcore/issues/25644)��

**Ŀǰδʵ�ֵĹ��ܣ�**
* �Ƽ��б�

**����δ����ӵĹ��ܣ�**
* �Ƽ��б�
* �ϴ��������
* �û��������
* �������洢�ṩ
* ֧�� 202108 API�汾
* ֧�� docker

## ��װ

�������������ַ�ʽ��װ��**�������ļ�**��**��Դ�������**

**ʹ�ö������ļ���װ��**  
1. ��[Releaseҳ��](https://github.com/RhythmCodec/MorePracticeMalodyServer/releases)�������µĶ������ļ���
2. ��ѹ�����أ��޸������ļ���
3. ����MorePracticeMalodyServer������ƽ̨��ͬ���ļ������в��졣
  - **Linux:** MorePracticeMalodyServer
  - **Windows:** MorePracticeMalodyServer.exe
 
**��Դ������룺**  
1. ��¡������MorePracticeMalodyServer�ֿ⡣
2. ȷ���Ѿ���װ.NET 5 SDK��������[�Ӵ�����](https://dotnet.microsoft.com/download/dotnet/5.0)��������ΰ�װ.NET 5 SDK��������[�Ķ�������](https://docs.microsoft.com/en-us/dotnet/core/install/)��
3. ����`dotnet restore`��
4. Ĭ��ʹ��sqlite�ṩ���ݿ⣬�����ļ�ϵͳ�ṩ�洢����Ҫʹ�������ṩ������ο�[�޸�����](#�޸�����)�½ڡ�
5. ���л������ݿ��ṩ��������ɾ�� MorePracticeMalodyServer\Migrations �ļ������������ݡ�֮������`dotnet ef migrations add InitialCreate`��ִ����ɺ�����`dotnet ef database update`���������ݿ⡣�����������ݿ⣬��������`dotnet ef migrations script --idempotent`����SQL�ű��������ݿ������иýű��Թ������ݱ�
6. ����`dotnet build --configuration Release --no-restore`����Ӧ�ó���
7. ��  MorePracticeMalodyServer\bin\Release �¸��ƹ����õ�Ӧ�ó���

## �޸�����

Ӧ�ó���������appsettings.json�����ļ��Ľṹ�Լ��޸ķ�ʽ���£�
* Logging
  * LogLevel : ��ȫ����Ϊ`Warning`�������
    * Default
    * Microsoft
    * Microsoft.Hosting.Lifetime
  * Data : ���ݿ��������
    * Provider : ���ݿ��ṩ���򣬿�����Ϊ`sqlserver`,`mysql`,`sqlite`��Ĭ��Ϊsqlite��
    * PoolSize : ���ӳص������������
    * ConnectionString : �����ַ���������������ο����ݿ��ĵ���
    * ServerVersion : MySqlʹ�ã�ȷ���������汾��
  * Storage : �洢�ṩ�����������
    * Provider : �洢�ṩ��������ӣ�selfʱΪ���ṩ���й������չ�洢�ṩ����ο�[�Զ���洢�ṩ](#�Զ���洢�ṩ)�½ڡ�

## �޸����ݿ��ṩ�����ע������

������޸������ݿ��ṩ����������ʹ��Դ�������ķ�ʽ���������ݿ�Ǩ���Լ����ݿ�ű����������������ݱ�  
��ο�[��װ](#��װ)�е�**��Դ�������**���ֽ��в�����
����Ŀǰ�����������ʹ��sqlite����������ṩ������ܻ���ֲ���Ԥ�ϵ���Ϊ�������������ʹ��ʱ�����쳣����[�ύIssue](https://github.com/RhythmCodec/MorePracticeMalodyServer/issues/new/choose)��

## �Զ���洢�ṩ

**�˲���Ŀǰ��δʵ�֣�**  
�뽫Storage:Provider����Ϊ�����ļ��ṩ������ϴ��ӿڡ�  
�ϴ���ɺ�����÷������ص�֪ͨ�������ļ������ӡ���**�˻ص���δʵ��**��  
��Ϸʹ��application/x-www-form-urlencoded�����ļ���ÿ��ֻ�ᷢ��һ���ļ���ͬʱ���������meta��Ϣ��
meta��ϢΪ�ı���ֵ�ԡ��ڳ����У������ṩ���ļ���cid��sid��hash���ݣ��������ڱ��м�����  
�����ļ��������ṩ�����ϱ��棬���������᳢�����������ļ����н���������Ҫ���н��������ļ�(chart file | *.mc)��Ԫ���ݡ�
������������÷�����API�ϴ�����Ԫ���ݡ���**��API��δʵ��**��

## docker֧��

����֧���� docker �����С�Ŀǰû�д�� docker ���񣬵����ṩ�� dockerfile �������Ҫ����������`docker build`���й�����  
�� docker �����еķ�������֧��ʹ�ñ����ļ�ϵͳ��Ҳ��֧��ʹ��sqlite�����滻���ݿ��ṩ������洢�ṩ���򡣹�������滻���ݿ��ṩ�������ļ��ṩ������[�ο�����](#�޸����ݿ��ṩ�����ע������)��

## ���

������ʹ�� GPL v3.0 (GNU GENERAL PUBLIC LICENSE v3.0)��ɿ���Դ���룬ʹ�ñ������Դ���뼴�������˽Ⲣͬ���Э�顣

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

## ��л
* [@soloopooo](https://github.com/soloopooo)
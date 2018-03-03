# SQLCLR_getSPList
## SQL Server CLR Stored Procedure to retrieve a SharePoint list via a webservice call, and return it as a result set
This feature is for times when you are building a SQL script or stored procedure, and you need to access data in a SharePoint list from within your SQL code. This stored procedure will return a result set that you can save to a temp table and work with as you need to. **See below for an example**

### Note about .net, SQL and SharePoint versions
The two dlls in the release folder were built with .net 3.0, and tested both on SQLServer 2008 R2, and 2016.  Results were tested against SharePoint 2010 as well as SharePoint 2013. It will probably work just fine on SQL Server version 2012, and it might not need the XmlSerializers.dll file (see below) but I didn't test it.

### Deployment and executing
There are two dlls in the release folder:
* SQLCLR_getSPList.dll 
* SQLCLR_getSPList.XmlSerializers.dll *<-- This file is only needed when deploying to 2008 R2.*

*About the XmlSerializers.dll file: This project uses the .net XML libraries, and the way they work they need to write their intermediate steps to disk, which the assemblies in 2008 R2 didn't allow.  That XmlSerializers.dll file gets around that limitation.  SQLServer 2016 doesn't have that same limitation so you won't need that file.*

Being redundant for clarity:
* To deploy to the Sql Server 2008R2, copy both dll files to a folder on the database server
* To deploy to SQL Server 2016, only copy SQLCLR_getSPList.dll to a folder on the database server

Once the files are copied you'll need to run some configuration statements on the database server.  This script is `install.sql` and also below.

Open the Management Studio editor, connected to that database server, open a new query window and run the script (for your path, my path was c:\sqlclrfiles)
```sql
use tempdb
go
create database testdb
go
use testdb
go
ALTER DATABASE testdb
SET TRUSTWORTHY ON
GO

if object_id('sp_getsplist','PC') is not NULL
  drop proc sp_getsplist
go

if exists (select * from sys.assemblies where name = 'SQLCLR_getSPList.XmlSerializers')
  drop assembly [SQLCLR_getSPList.XmlSerializers]
go
if exists (select * from sys.assemblies where name = 'GetSPList')
  drop assembly GetSPList
go

CREATE ASSEMBLY GetSPList
FROM 'c:\sqlclrfiles\SQLCLR_getSPList.dll'
--WITH PERMISSION_SET = EXTERNAL_ACCESS
WITH PERMISSION_SET = UNSAFE
GO

--this is only needed for SQL Server 2008R2
create assembly [SQLCLR_getSPList.XmlSerializers]
from 'c:\sqlclrfiles\SQLCLR_getSPList.XmlSerializers.dll'
with permission_set = safe
go

CREATE PROCEDURE sp_getsplist
  @url nvarchar(255), @listname nvarchar(255)
AS
EXTERNAL NAME GetSPList.StoredProcedures.GetSPList
go


Commands completed successfully.
```
## Testing

For a small test, consider two two lists, the first list is in SharePoint 2010:
![](https://raw.githubusercontent.com/matt-jk/SQLCLR_getSPList/master/images/SP2010_testlist.jpg "SharePoint 2010 test list")

And the second list in SharePoint 2013:
![](https://raw.githubusercontent.com/matt-jk/SQLCLR_getSPList/master/images/SP2013_testlist.jpg "SharePoint 2013 test list")

Below is the sql window where I ran the stored proc, once for each list, and the output:
![](https://raw.githubusercontent.com/matt-jk/SQLCLR_getSPList/master/images/test_output.jpg "test output")

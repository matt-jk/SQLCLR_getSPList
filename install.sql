--
-- Script to install the assemblies
-- Modify lines 32 and 40 with the location of the dll files
-- Matt Kalal
--

--
-- un-rem these next lines if you want to create a test db for testing
--
--use tempdb
--go
--create database testdb
--go
--use testdb
--go
--ALTER DATABASE testdb
--SET TRUSTWORTHY ON
--GO

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
FROM 'c:\path_on_dbserver\SQLCLR_getSPList.dll'
--WITH PERMISSION_SET = EXTERNAL_ACCESS
WITH PERMISSION_SET = UNSAFE
GO
--
--This is requred for SQL Server 2008R2, you won't need it for 2016
--
create assembly [SQLCLR_getSPList.XmlSerializers]
from 'c:\path_on_dbserver\SQLCLR_getSPList.XmlSerializers.dll'
with permission_set = safe
go

CREATE PROCEDURE sp_getsplist
  @url nvarchar(255), @listname nvarchar(255)
AS
EXTERNAL NAME GetSPList.StoredProcedures.GetSPList
go

exec sp_getsplist 'url to the SharePoint _vti_bin/lists.asmx', 'name_of_list'
go



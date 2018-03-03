# SQLCLR_getSPList
## SQL Server CLR Stored Procedure to retrieve a SharePoint list via a webservice call, and return it as a result set
This feature is for times when you are building a SQL script or stored procedure, and you need to access data in a SharePoint list from within your SQL code.  This stored procedure will return a result set that you can save to a temp table and work with as you need to.

### Note about SQL and SharePoint versions
The two dlls in the release folder were built with .net 3.0, and tested both on SQLServer 2008 R2, and 2016.  Results were tested against SharePoint 2010 as well as SharePoint 2013.

### Installation and executing
![](https://raw.githubusercontent.com/matt-jk/SQLCLR_getSPList/master/images/test_output.jpg)

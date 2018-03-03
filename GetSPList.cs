using System;
using System.Data;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using System.Collections.Generic;
using SP2010WS;

/*
 * This creates the assembly that gets loaded in the SqlServer database
 * My idea was to pull from the SharePoint webservice the list as two collections, the rows and column definitions
 * and then do a conversion to prepare to return them to the context pipe.
 * 
 * There is probably a more elegant way to do these conversion operations, but this works and it was straightforward
 * to get working, and easy to test
 * Matt Kalal
 */

public partial class StoredProcedures
{
    [Microsoft.SqlServer.Server.SqlProcedure]

    public static void GetSPList(SqlString SPUrl, SqlString list_name)
    {

        SPWebService WebService = new SPWebService(SPUrl.ToString());
        SPList MyList = WebService.GetSPList(list_name.ToString());

        List<SqlMetaData> Cols = new List<SqlMetaData>();
        //
        // The logic is to iterate through each ColumDef from the SharePoint list and add a corresponding
        // column metadata for the result set to SQL Server.  Basically, this loop matches up data types
        // instead of having the SQL resultset have all columns with varchar (which probably would have been fine
        // for most purposes)
        //
        foreach (ColumnDef CD in MyList.GetColumnDefs())
        {
            switch (CD.ColType.ToUpper())
            {
                case "TEXT":
                    Cols.Add(new SqlMetaData(CD.ColName, SqlDbType.NVarChar, CD.ColLen));
                    break;
                case "NUMBER":
                    Cols.Add(new SqlMetaData(CD.ColName, SqlDbType.Decimal, 18, 5));
                    break;
                case "CURRENCY":
                    Cols.Add(new SqlMetaData(CD.ColName, SqlDbType.Money));
                    break;
                case "DATETIME":
                    Cols.Add(new SqlMetaData(CD.ColName, SqlDbType.DateTime2));
                    break;
                case "BOOLEAN":
                    Cols.Add(new SqlMetaData(CD.ColName, SqlDbType.Bit));
                    break;
                default:
                    Cols.Add(new SqlMetaData(CD.ColName, SqlDbType.NVarChar, 4000));
                    break;
            }
        }

        SqlPipe pipe = SqlContext.Pipe;

        SqlDataRecord rec = new SqlDataRecord(Cols.ToArray());
        pipe.SendResultsStart(rec);

        int i;
        decimal tempdec;
        DateTime tempdt;

        //
        // The logic here is similar to the columns above, but it is being done for each row
        // in the result set.  The list data comes out of the webservice as XML which I left as strings.
        // For each row, it iterates through each column and compares what the datatype is in SharePoint (ColumnDef)
        // and does a conversion for the resultset
        //

        foreach (var listrow in MyList.GetRows())
        {
            i = -1;
            foreach (ColumnDef CD in MyList.GetColumnDefs())
            {
                i++;
                tempdec = 0;
                switch (CD.ColType.ToUpper())
                {
                    case "TEXT":
                        rec.SetSqlString(i, listrow[i]);
                        break;
                    case "NUMBER":
                        decimal.TryParse(listrow[i], out tempdec);
                        rec.SetSqlDecimal(i, tempdec);
                        break;
                    case "CURRENCY":
                        decimal.TryParse(listrow[i], out tempdec);
                        rec.SetSqlMoney(i, tempdec);
                        break;
                    case "DATETIME":
                        if (!DateTime.TryParse(listrow[i], out tempdt))
                        {
                            tempdt = new DateTime(1900, 1, 1);
                        }
                        rec.SetSqlDateTime(i, tempdt);
                        break;
                    case "BOOLEAN":
                        if (listrow[i].Equals("1"))
                        {
                            rec.SetSqlBoolean(i, true);
                        }
                        else
                        {
                            rec.SetSqlBoolean(i, false);
                        }
                        break;
                    default:
                        rec.SetSqlString(i, listrow[i]);
                        break;
                }
            }
            pipe.SendResultsRow(rec);
        }
        pipe.SendResultsEnd();
    }
}

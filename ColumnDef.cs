/*
 * This is to hold the column definitions.  The strategy is to pull out of the SharePoint list the columns
 * we want (there are many columns, most we don't care about) store them as a collection of this ColumnDef
 * and then use that collection to pull the columns we want to see
 * Matt Kalal
 */

namespace SP2010WS
{
    public class ColumnDef
    {
        public string ColName;
        public string ColType;
        public int ColLen;
        private string ColInternalName;

        public ColumnDef(string cname, string ciname, string ctype, int clen)
        {
            ColName = cname;
            ColInternalName = ciname;
            ColType = ctype;
            ColLen = clen;
        }
        public ColumnDef(string cname, string ciname, string ctype, string clen)
        {
            ColName = cname;
            ColInternalName = ciname;
            ColType = ctype;
            if (!int.TryParse(clen, out ColLen))
            {
                ColLen = 4000;
            }
        }
        public string GetInternalColumnName()
        {
            return ColInternalName;
        }
    }
}

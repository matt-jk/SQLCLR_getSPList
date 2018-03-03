using System;
using System.Collections.Generic;

/*
 * My idea behind this class is it is basically a container for a collection of columns, and a collection of rows
 * It knows how to create itself if you give it a valid xml output from the GetList web service call, and the GetListItems web service call
 * 
 * Matt Kalal
 */
namespace SP2010WS
{
    public class SPList
    {
        private List<ColumnDef> ColumnDefs;
        private List<List<string>> Rows;

        public SPList()
        {
            ColumnDefs = new List<ColumnDef>();
            Rows = new List<List<string>>();
        }

        public void SetError()
        {
            ColumnDefs.Add(new ColumnDef("col1", "col1", "TEXT", 80));
            Rows.Add(new List<string>() { "Error, there is a problem getting the data set" });
        }

        public int LoadList(System.Xml.XmlNode ListXML, System.Xml.XmlNode ListItemsXML)
        {
            ColumnDefs = new List<ColumnDef>();
            Rows = new List<List<string>>();

            List<string> tempLst = new List<string>();

            string ListID = GetAttribute(ListXML, "ID");

            foreach (System.Xml.XmlNode xnode in ListXML)
            {
                // I really only care about the "Fields" node, but the "selectSingleNode" with an xPath of "/List/Fields" isn't working
                // probably because of a namespace; perhaps later I'll figure it out but a quick loop looking at the name seems to be OK as a workaround
                if (xnode.Name.ToString().Equals("Fields"))
                {
                    foreach (System.Xml.XmlNode FX in xnode)
                    {
                        //
                        // It's a little long, but the logic that works is the field is not hidden, and [the static name is "title", or the sourceid is the same as the ListID]
                        //
                        if (!(GetAttribute(FX, "Hidden").Equals("TRUE", StringComparison.CurrentCultureIgnoreCase)) && (GetAttribute(FX, "StaticName").Equals("title", StringComparison.CurrentCultureIgnoreCase) || GetAttribute(FX, "SourceID").Equals(ListID, StringComparison.CurrentCultureIgnoreCase)))
                        {
                            ColumnDefs.Add(new ColumnDef(GetAttribute(FX, "DisplayName"), GetAttribute(FX, "StaticName"), GetAttribute(FX, "Type"), GetAttribute(FX, "MaxLength")));
                        }
                    }
                }
            }

            foreach (System.Xml.XmlNode linode in ListItemsXML)
                //
                // We just want the rs:data nodes, and there is probably a better way to do this, but it works
                //
            {
                if (linode.Name.ToString().Equals("rs:data", StringComparison.CurrentCultureIgnoreCase))
                {
                    foreach (System.Xml.XmlNode rownode in linode)
                    {
                        if (rownode.Name.ToString().Equals("z:row", StringComparison.CurrentCultureIgnoreCase))
                        {
                            foreach (ColumnDef cd in ColumnDefs)
                            {
                                //
                                // The point with this "ows_" is the columns out of SharePoint web service call
                                // all started with "ows_" and there is probably a reason for that but I didn't want
                                // to go through the spec.  This works just fine.
                                //
                                tempLst.Add(GetAttribute(rownode, "ows_" + cd.GetInternalColumnName()));
                            }
                            Rows.Add(new List<string>(tempLst));
                            tempLst.Clear();
                        }
                    }

                }
            }
            return 1;
        }

        public List<List<string>> GetRows()
        {
            return Rows;
        }

        public List<ColumnDef> GetColumnDefs()
        {
            return ColumnDefs;
        }

        //
        // I made this function because not all of the attributes are present, and when they aren't
        // it was easier just to return a string "NULL" than it was to try and program around it
        //
        private string GetAttribute(System.Xml.XmlNode XmlNode, string AttName)
        {
            try
            {
                return XmlNode.Attributes[AttName].Value;
            }
            catch (NullReferenceException)
            {
                return "NULL";
            }
        }
    }
}

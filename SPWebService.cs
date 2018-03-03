using System;
using System.Collections.Generic;

/*
 * This class has the features of calling the webservice and returning a SPList object
 * As an FYI, my first idea was to use the SharePoint Client Object Model, (CSOM) but in early
 * testing, the SQL Server didn't have the CLR assemblies to support it, and it was going to be a nightmare
 * to add them all and configure, not to mention unsupported and a bad idea.
 * 
 * The different pages on this website were very helpful:
 * https://msdn.microsoft.com/en-us/library/ms458094(v=office.14).aspx
 * You create a webservice proxy, and then call those functions and pick through the XML that gets returned.
 * That's what this class does.
 * 
 * Note that it only supports authenticated connections, not by passing credentials.  I only tested it
 * with a connection, I didn't even try it with a user and password.  You'll have to change the SPservice.Credentials
 * line if you want to try it.
 * 
 * I also put in a 10000 line limit.
 * 
 * One more comment, there is also a CAML specification available to more precisely query the webservice.  I didn't use it as you
 * can see below.  I pulled everything back and parsed out the XML return.
 * 
 * Matt Kalal
 */
namespace SP2010WS
{
    public class SPWebService
    {
        private SharePoint2010WS.Lists SPservice;

        public SPWebService(string url)
        {
            /*Declare and initialize a variable for the Lists Web service.*/
            SPservice = new SharePoint2010WS.Lists();

            /*Authenticate the current user by passing their default 
            credentials to the Web service from the system credential 
            cache. */
            SPservice.Credentials = System.Net.CredentialCache.DefaultCredentials;

            /*Set the Url property of the service for the path to a subsite. 
            Not setting this property will return the lists in the root Web site.*/
            SPservice.Url = url;
        }

        public SPList GetSPList(string ListName)
        {
            SPList MyList = new SPList();
            string ListID = GetListID(ListName);

            if (ListID.Equals("notfound", StringComparison.CurrentCultureIgnoreCase))
            {
                MyList.SetError();
            }
            else
            {
                System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();

                string viewName = "";
                string rowLimit = "10000";

                System.Xml.XmlElement query = xmlDoc.CreateElement("Query");
                System.Xml.XmlElement viewFields = xmlDoc.CreateElement("ViewFields");
                System.Xml.XmlElement queryOptions = xmlDoc.CreateElement("QueryOptions");

                MyList.LoadList(SPservice.GetList(ListID), SPservice.GetListItems(ListID, viewName, query, viewFields, rowLimit, queryOptions, null));
            }
            return MyList;
        }

        public Dictionary<string, string> GetLists()
        {
            Dictionary<string, string> OutList = new Dictionary<string, string>();

            /*Declare an XmlNode object and initialize it with the XML 
            response from the GetListCollection method. */
            System.Xml.XmlNode node = SPservice.GetListCollection();
            /*Loop through XML response and parse out the value of the
            Title attribute for each list. */
            foreach (System.Xml.XmlNode xmlnode in node)
            {
                OutList.Add(xmlnode.Attributes["Title"].Value, xmlnode.Attributes["ID"].Value);
            }
            return OutList;
        }

        public System.Xml.XmlNode GetListCollection()
        {
            return SPservice.GetListCollection();
        }

        public string GetListID(string ListName)
        {
            string OutString = "NotFound";

            try
            {
                foreach (System.Xml.XmlNode xn in SPservice.GetListCollection())
                {
                    if (xn.Attributes["Title"].Value.Equals(ListName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        OutString = xn.Attributes["ID"].Value;
                    }
                }
            }
            catch (Exception)
            {
                ;
            }
            return OutString;
        }
    }
}

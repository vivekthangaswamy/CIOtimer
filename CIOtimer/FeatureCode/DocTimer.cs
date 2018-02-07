using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using System.Xml;

namespace CIOtimer
{
    public class DocTimer : SPJobDefinition
    {
        public DocTimer() : base() 
        { 
        }

        public DocTimer(string jobName, SPService service, SPServer server, SPJobLockType targetType)
            : base(jobName, service, server, targetType)
        {
        }

        public DocTimer(string jobName, SPWebApplication webApplication)
            : base(jobName, webApplication, null, SPJobLockType.ContentDatabase)
        {
            this.Title = jobName;
        }

         // current jobs site guid 
        public override void Execute(Guid contentDbId)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                // get a reference to the current site collection's content database
                SPList objList = null;

                SPWebApplication webApplication = this.Parent as SPWebApplication;
                SPContentDatabase contentDb = webApplication.ContentDatabases[contentDbId];


                // read the source information
                XmlDocument xDocs = new XmlDocument();
                xDocs.Load(@"C:\Program Files\Common Files\Microsoft Shared\web server extensions\12\TEMPLATE\XML\TimerSource.xml");

                XmlNodeList nodeList;
                XmlNode root = xDocs.DocumentElement;
                nodeList = root.SelectNodes("descendant::SOURCE/DOCUMENT[@name]");

                // Change the price on the books.
                foreach (XmlNode source in nodeList)
                {
                    //source.Attributes["name"].Value.ToString();
                    //source.Attributes["url"].Value.ToString();
                    //source.Attributes["guid"].Value.ToString();

                    // get a reference to the Document Library in the RootWeb of the first site collection in the content database
                    objList = contentDb.Sites[0].RootWeb.Lists[source.Attributes["name"].Value.ToString()];

                    SPListItemCollection listItems = objList.Items;
                    int itemCount = listItems.Count;

                    // build the output XML

                    XmlDocument xoutDoc = new XmlDocument();
                    XmlElement xoEle = xoutDoc.CreateElement("DOCUMENTDETAILS");

                    for (int k = 0; k < itemCount; k++)
                    {
                        SPListItem item = listItems[k];

                        xoEle.SetAttribute("Title", item.Title.ToString());
                        xoEle.SetAttribute("Name", item.Name.ToString());
                        xoEle.SetAttribute("URL", item.Url.ToString());
                        xoEle.SetAttribute("Guid", item.UniqueId.ToString());
                        xoEle.SetAttribute("Properties", item.Properties.Values.ToString());
                        xoutDoc.AppendChild(xoEle);
                    }

                    // local path
                    xoutDoc.Save(@"c:/DocumentDetails.xml");
                }

            });
        }
    }
}

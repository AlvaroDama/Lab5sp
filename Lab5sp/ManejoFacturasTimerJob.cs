using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace Lab5sp
{
    public class ManejoFacturasTimerJob: SPJobDefinition
    {
        public ManejoFacturasTimerJob() { }

        public ManejoFacturasTimerJob(string name, SPWebApplication webApplication,
            SPServer server, SPJobLockType lockType) : base(name, webApplication, server, lockType) { }

        public override void Execute(Guid targetInstanceId)
        {
            using (SPSite managerSite = new SPSite("http://pruebassp2/"))
            {
                using (SPWeb managerWeb = managerSite.RootWeb)
                {
                    SPList overviewList = managerWeb.Lists["ResumenFacturas"];

                    while (overviewList.Items.Count > 0)
                    {
                        overviewList.Items[0].Delete();
                        overviewList.Update();
                    }

                    foreach (SPSite departmentSite in this.WebApplication.Sites)
                    {
                        using (SPWeb departmentWeb = departmentSite.RootWeb)
                        {
                            SPList expensesList = departmentWeb.Lists.TryGetList("Facturas");

                            if (expensesList != null)
                            {
                                double departmentTotal = 0;
                                foreach (SPListItem expense in expensesList.Items)
                                {
                                    departmentTotal += (double)expense["Importe"];
                                }

                                Uri url = new Uri(departmentWeb.Url);

                                string hostName = url.GetComponents(UriComponents.Host, UriFormat.Unescaped);

                                string[] hostNameComponents = hostName.Split('.');

                                SPListItem overviewItem = overviewList.Items.Add();

                                overviewItem["Title"] = hostNameComponents[0];
                                overviewItem["ImporteTotal"] = departmentTotal;

                                overviewItem.Update();
                                overviewList.Update();
                            }
                        }

                        departmentSite.Dispose();
                    }
                }
            }
        }
    }
}
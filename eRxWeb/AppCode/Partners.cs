using System;
using System.Xml;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using Allscripts.ePrescribe.DatabaseSelector;
using DAL = Allscripts.ePrescribe.Data;

namespace eRxWeb
{
/// <summary>
/// Summary description for Partners
/// </summary>
public class Partners : List<Partner>
{
	public Partners()
	{
        loadPartners();
	}

    public Partners(List<Partner> partners)
    {
        partners.ForEach(Add);
    }

    private void loadPartners()
    {
        DataSet ds = DAL.Partner.GetPartners(ConnectionStringPointer.SHARED_DB);

        if (ds.Tables.Count > 0)
        {
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    try
                    {
                        Partner partner = new Partner(dr);

                        this.Add(partner);
                    }
                    catch (Exception ex)
                    {
                        //if this one partner load bombs, do an easy catch and continue so one bad partner doesn't spoil the lot
                        try
                        {
                            //could not load partners
                            Allscripts.Impact.Audit.AddException(Guid.Empty.ToString(), Guid.Empty.ToString(), "Could not load partner " + dr["PartnerName"].ToString() + " from App_Start " + ex.ToString(), "", "", "", ConnectionStringPointer.ERXDB_DEFAULT);
                        }
                        catch (Exception finalex)
                        {
                            try
                            {
                                //we've pretty much caught everything we're gonna catch
                                System.IO.TextWriter writer = new System.IO.StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\eRxNOWErrorLog.txt", true);
                                writer.WriteLine(DateTime.Now.ToString() + " An error occurred while creating an error: " + finalex.ToString());
                                writer.Close();
                            }
                            catch { }
                        }
                    }
                }
            }
        }
    }

}

}
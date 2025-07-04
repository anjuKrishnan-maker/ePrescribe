using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace eRxWeb
{
    public partial class Help_Add_on : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.Form.Attributes.Add("autocomplete", "off");
            ((HelpMasterPageNew)this.Master).CurrentPage = HelpMasterPageNew.HelpPage.ADD_ONS;
                Repeater1.DataSource = GetImages();
                Repeater1.DataBind();    
        }
    }

    private List<dynamic> GetImages() {
            var retrunList = new List<dynamic>();

            var path = Server.MapPath("Images/Addon");
            if (Directory.Exists(path))
            {
                DirectoryInfo dinfo = new DirectoryInfo(path);
               
                foreach (var info in dinfo.GetFiles("*.png"))
                {
                    retrunList.Add(new {
                        FileName = $"Images/Addon/" + info.Name,
                        ExpandedFile = $"Images/Addon/" + info.Name.Replace(info.Extension, ".jpg")
                    });
                }

            }
            return retrunList;
    }
}

}
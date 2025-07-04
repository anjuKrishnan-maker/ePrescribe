using System;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Collections.Generic;
using System.Xml;
using Microsoft.IdentityModel.Claims;
using Allscripts.ePrescribe.Common;
using System.Configuration;
using eRxWeb.State;
using System.Web.UI;

namespace eRxWeb
{
    /// <summary>
    /// Summary description for Helper
    /// </summary>
    public class Helper
    {
        public const string NO_CLAIM_FOUND_MESSAGE = "Claim wasn’t found or there were more than one claim found";
	    public Helper()
	    {
		
	    }
        public static string GetClaimValue(ClaimCollection claimCollection, string claimType)
        {
            string value = null;

            try
            {
                value = (from c in claimCollection
                         where c.ClaimType.Equals(claimType, StringComparison.OrdinalIgnoreCase)
                         select c.Value).Single();
            }
            catch (InvalidOperationException)
            {
                value = NO_CLAIM_FOUND_MESSAGE;
            }

            return value;
        }

        /// <summary>
        /// Checks the passed in ClaimsCollection for the existance of the claimValue. Not case-sensitive.
        /// </summary>
        /// <param name="claimCollection">List of claims</param>
        /// <param name="claimValue">Claim URI</param>
        /// <returns>Bool indicating if the claim exists in the ClaimsCollection</returns>
        public static bool DoesClaimExist(ClaimCollection claimCollection, string claimValue)
        {
            bool exists = false;
            string value = null;

            try
            {
                value = (from c in claimCollection
                         where c.Value.Equals(claimValue, StringComparison.OrdinalIgnoreCase)
                         select c.Value).Single();
            }
            catch (InvalidOperationException)
            {
                exists = false;
                value = null;
            }

            exists = !string.IsNullOrEmpty(value);

            return exists;
        }

	    /// <summary>
	    /// Checks the passed in ClaimsCollection for the existance of the claimValue. Not case-sensitive.
	    /// </summary>
	    /// <param name="permissionCollection">List of permissions</param>
	    /// <param name="permissionUri">Permission URI</param>
	    /// <returns>Bool indicating if the permission exists in the collection</returns>
	    public static bool DoesClaimExist(ePrescribeSvc.Permission[] permissionCollection, string permissionUri)
	    {
		    bool exists = false;
		    string value = null;

		    try
		    {
			    value = (from c in permissionCollection
					     where c.Value.Equals(permissionUri, StringComparison.OrdinalIgnoreCase)
					     select c.Value).Single();
		    }
		    catch (InvalidOperationException)
		    {
			    exists = false;
			    value = null;
		    }

		    exists = !string.IsNullOrEmpty(value);

		    return exists;
	    }

        /// <summary>
        /// Compares 2 sets of ClaimCollections for equality. Only looks for the roles and permissions claim values.
        /// </summary>
        /// <param name="oldClaims">Old/current set of claims</param>
        /// <param name="newClaims">New set of claims</param>
        /// <returns>True if the collections are the same; false otherwise</returns>
        public static bool CompareClaimCollections(ClaimCollection oldClaims, ClaimCollection newClaims)
        {
		    bool areEqual = false;

		    if (oldClaims != null && newClaims != null)
		    {
			    //put the old claimcollection into a strongly typed list
			    List<Claim> oldClaimsList = new List<Claim>(oldClaims);

			    //create a new filtered list and populate it with only roles
                List<Claim> oldClaimsListFiltered = new List<Claim>(oldClaims).FindAll(c => c.ClaimType.Equals(ConfigurationManager.AppSettings["CLAIM_ROLE_TYPE"].ToString(), StringComparison.OrdinalIgnoreCase));

			    //add permissions to the newly filtered list
                oldClaimsListFiltered.AddRange(new List<Claim>(oldClaims).FindAll(c => c.ClaimType.Equals(ConfigurationManager.AppSettings["CLAIM_PERMISSION_TYPE"].ToString(), StringComparison.OrdinalIgnoreCase)));

			    //put the new claimcollection into a strongly typed list
			    List<Claim> newClaimsList = new List<Claim>(newClaims);

			    //create a new filtered list and populate it with only roles
                List<Claim> newClaimsListFiltered = new List<Claim>(newClaims).FindAll(c => c.ClaimType.Equals(ConfigurationManager.AppSettings["CLAIM_ROLE_TYPE"].ToString(), StringComparison.OrdinalIgnoreCase));

			    //add permissions to the newly filtered list
			    newClaimsListFiltered.AddRange(new List<Claim>(newClaims).FindAll(c => c.ClaimType.Equals(ConfigurationManager.AppSettings["CLAIM_PERMISSION_TYPE"].ToString(), StringComparison.OrdinalIgnoreCase)));

			    //we only care about the claim values, so put the values into their own list for the final compare
			    var oldClaimsListFilteredStrings =
				    from claim in oldClaimsListFiltered
				    select new { claim.Value };

			    var newClaimsListFilteredStrings =
				    from claim in newClaimsListFiltered
				    select new { claim.Value };

			    //the collections are "equal" if 
			    //   1. the count of claims on the original claimcollections are equal
			    //   2. the filtered list of claim values are the same
			    areEqual = (oldClaimsList.Count == newClaimsList.Count) && !oldClaimsListFilteredStrings.Except(newClaimsListFilteredStrings).Any();
		    }

            return areEqual;
        }

	    public static IClaimsPrincipal GetIClaimsPrincipalFromSAMLToken(string samlToken, out DateTime validFrom, out DateTime validTo)
	    {
		    Microsoft.IdentityModel.Configuration.ServiceConfiguration serviceConfig
			    = new Microsoft.IdentityModel.Configuration.ServiceConfiguration();

		    // Now read the token and convert it to an IPrincipal
		    SecurityToken theToken = null;
		    ClaimsIdentityCollection claimsIdentity = null;

		    using (XmlReader reader = XmlReader.Create(new System.IO.StringReader(samlToken)))
		    {
			    theToken = serviceConfig.SecurityTokenHandlers.ReadToken(reader);
			    claimsIdentity = serviceConfig.SecurityTokenHandlers.ValidateToken(theToken);
		    }

		    validFrom = theToken.ValidFrom;
		    validTo = theToken.ValidTo;

		    IClaimsPrincipal icp = new ClaimsPrincipal(claimsIdentity);

		    return icp;
	    }
        public static string GetHelpText(string pageName) {
            return Allscripts.Impact.SystemConfig.GetHelp(pageName);
        }

        internal static void SetHelpTextForPane(AjaxControlToolkit.AccordionPane paneHelp, string path)
        {
            string paneContent = GetHelpText(path);            
            paneHelp.ContentContainer.Controls.Add(new LiteralControl(paneContent));
            SetHelpWithScreenContainerVisiblity(paneHelp, paneContent);
        }

        internal static void SetHelpTextForPanel(System.Web.UI.WebControls.Panel panelHelp, string path)
        {
            string panelContent = GetHelpText(path);
            panelHelp.Controls.Add(new LiteralControl(panelContent));
            SetHelpWithScreenContainerVisiblity(panelHelp, panelContent);
        }

        internal static void SetHelpWithScreenContainerVisiblity(Control pane, string content)
        {
            if (String.IsNullOrWhiteSpace(content))
            {
                pane.Visible = false;
            }
        }

        internal static void SetHelpWithScreenContainerVisiblity(Control pane, Control panelHeader, AjaxControlToolkit.CollapsiblePanelExtender cpe, string content)
        {
            if (String.IsNullOrWhiteSpace(content))
            {
                pane.Visible = false;
                panelHeader.Visible = false;
                cpe.Enabled = false;
            }
        }

        public static string GetStringOrEmpty(string val)
        {
            if (!string.IsNullOrEmpty(val))
                return val;
            else
                return string.Empty;
        }
        public static bool IsAngularMode
        {
            get
            {
                bool isSpa = false;
                bool.TryParse(ConfigurationManager.AppSettings["SPAMode"], out isSpa);

                return isSpa;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.ServerModel
{
    public class PharmacyModel
    {
        public string RatingImageUrl { get; internal set; }
        public string RatingText { get; internal set; }
        public string TotalErrors { get; internal set; }
        public string TotalRx { get; internal set; }

        public string Address1 { get; internal set; }
        public string City { get; internal set; }
        public string DeliveryRate { get; internal set; }
        public string EPCS { get; internal set; }
        public string Fax { get; internal set; }
        public string Name { get; internal set; }
        public string Phone { get; internal set; }
        public string State { get; internal set; }
        public string TransMethod { get; internal set; }
        public string ZipCode { get; internal set; }
        public string CrossStreet { get; set; }
        public string Comment { get; internal set; }
        public string Address2 { get; internal set; }
        public string NABP { get; internal set; }
    }
    public class PharmacyRequestModel
    {
        public string PharmacyID { get; set; }
        public string Address1 { get; set; }
        public string City { get; set; }
        public string Fax { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Comment { get; set; }
        public string Address2 { get; set; }
    }
}
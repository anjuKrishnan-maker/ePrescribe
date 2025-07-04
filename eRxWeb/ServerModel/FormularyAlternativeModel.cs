
namespace eRxWeb.ServerModel
{
    public class FormularyAlternativeModel
    {
    }
    class FormularyAlternative
    {
        public string DrugName { get; set; }
        public string Status { get; set; }
        public bool IsGeneric { get; set; }
        public string ImageUrl { get; set; }
        public string ToolTip { get; set; }
        public string LevelOfPreferedness { get; set; }
        public string Copay { get; set; }
    }
}
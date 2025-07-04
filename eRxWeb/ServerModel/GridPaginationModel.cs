namespace eRxWeb.ServerModel
{
    public class GridPaginationModel
    {
        public GridPaginationModel()
        {

            CurrentPageNo = 0;
            TotalNumberOfRecord = 0;
        }
        public int PageSize { get; set; }
        public int CurrentPageNo { get; set; }
        public int TotalNumberOfRecord { get; set; }
    }
}
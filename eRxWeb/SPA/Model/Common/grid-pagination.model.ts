export class GridPaginationModel {
    HasNext: boolean;
    HasPrevious: boolean;
    ToTalPageCount: number;
    constructor(
        public PageSize: number,
        public CurrentPageNo: number,
        public TotalNumberOfRecord: number
    ) {
        this.ToTalPageCount = Math.ceil(this.TotalNumberOfRecord / this.PageSize);
        this.HasNext = (this.CurrentPageNo+1 )<this.ToTalPageCount;
        this.HasPrevious = this.CurrentPageNo > 0;
        
    }
}
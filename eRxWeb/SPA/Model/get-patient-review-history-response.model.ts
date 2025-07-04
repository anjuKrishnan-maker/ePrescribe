import Reviewhistorymodel = require("./review-history.model");
import ReviewHistoryItem = Reviewhistorymodel.ReviewHistoryItem;

export class GetPatientReviewHistoryResponse {
    public HistoryItems: Array<ReviewHistoryItem>;
    public MoreRowsAvailable: boolean;
    public ActiveMedsPresent: boolean;
}
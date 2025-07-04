import { PPTPlusDetailsUserChangeStatus, PptDetailContext} from '../../tools/constants';

export class PptPlusDetailsUserChangesResponse {
    public Status: PPTPlusDetailsUserChangeStatus;
    public DrugName: string;
    public Message: string;
    public PageContext: PptDetailContext;
    public MedSearchIndexes: string[];
}
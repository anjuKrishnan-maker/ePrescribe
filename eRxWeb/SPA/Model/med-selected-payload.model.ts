import { RobustLinkMesssagel, CopayCoverageModel, FormularyAlternative, GenericAlternative, PptPlusSummaryRequest } from './model.import.def';

export class MedicationSelectedPayload {
    RobustLinkPayload: RobustLinkMesssagel[];
    CopayCoveragePayload: CopayCoverageModel;
    FormularyAlternativesPayload: FormularyAlternative[];
    GenericAlternativesPayload: GenericAlternative[];
    PptPlusSummaryRequest: PptPlusSummaryRequest[];
    selectedMedicationRowIndex?: number[]
}
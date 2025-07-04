export class PatientUploadResponse {
    JobInFlight: boolean;
    UploadSuccess: boolean;
    ImportBatchJobHistory: ImportBatchModel[];
    CurrentJob: ImportBatchModel;
    ErrorMessage: string;
}

export class ImportBatchModel {
    ID: number;
    LicenseID: string;
    ProcessBegin: Date;
    ProcessEnd: Date;
    BatchSize: number;
    ImportBatchStatusID: number;
    StatusDescription: string;
    ErrorLines: string;
}

export class UploadError {
    Message: string;
    Type: UploadErrorType
}

export enum UploadErrorType {
    InvalidFileError = 1,
    ParseFileError
}
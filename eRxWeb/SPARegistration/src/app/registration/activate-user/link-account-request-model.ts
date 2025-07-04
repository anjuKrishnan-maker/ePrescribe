export class LinkAccountRequestModel {
   shieldUserName:string
   shieldPassword: string
}
export class LinkExistingUserModel {
    ErrorReason: ErrorReason;
    FirstName: string;
    LastName: string;
    Email: string;
    AccountName: string;
    IsValid: boolean;
    WorkflowType: Workflow;
}

export enum ErrorReason {    
    NOT_AUTHENTICATED,
    FAILED_TO_LINK_USER,
    ERX_UPDATE_FAILURE,
    FAILED_TO_RETRIEVE_USER_GUID,
    USER_PROFILE_ALREADY_LINKED_INSIDE_THIS_TENANT,
    EXTERNAL_ID_IS_NOT_USER_GUID,
    USERNAME_ALREADY_EXISTS_FOR_LICENSE
}
export enum Workflow {
    None,
    Sso,
    Utilities,
    Main
}
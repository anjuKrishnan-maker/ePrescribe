export class HomeAddressStartUpModel {
    States: State[];
}
export class State {
    State: string;
    Description: string
}

export class HomeAddressDetail {
    HomeAddress: string;
    HomeAddress2: string;
    City: string;
    State: string;
    ZipCode: string;
}

export class HomeAddressResponse {
    IsSaveSuccessful: boolean
    RedirectUrl: string;
}
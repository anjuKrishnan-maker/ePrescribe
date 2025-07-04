import Dictionarymodel = require("./Dictionary.model");
import Dictionary = Dictionarymodel.Dictionary;
import Messagemodel = require("./message.model");
import MessageIcon = Messagemodel.MessageIcon;

export class SupervisingProvider {
    SupervisingProviders: Dictionary<string> 
}

export class SupervisorProviderInfoRequest {
    SupervisorProviderId: string;
}

export class SupervisorProviderInfoResponse {
    IsSupervisorProviderInfoSet: boolean;
    Message: string;
    MessageIcon: MessageIcon;
}
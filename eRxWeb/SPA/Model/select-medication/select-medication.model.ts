import Scriptpadmodel1 = require("../right-panel/script-pad.model");
import ScriptPadModel = Scriptpadmodel1.ScriptPadModel;
import Messagemodel1 = require("../message.model");
import MessageModel = Messagemodel1.MessageModel;
import Constants = require("../../tools/constants");
import SelectMedicationReturnAction = Constants.SelectMedicationReturnAction;

export class SelectMedicationModel {
    public ReturnAction: SelectMedicationReturnAction;
    public ScriptPadModel: ScriptPadModel;
    public RedirectUrl: string;
    public MessageModel: MessageModel;
}
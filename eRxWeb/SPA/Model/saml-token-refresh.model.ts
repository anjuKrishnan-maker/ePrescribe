import Constants = require("../tools/constants");

export class SamlTokenRefresh {
    public Result: Constants.SamlRefreshResult;
    public NewExpirationTimeMs: number;
}
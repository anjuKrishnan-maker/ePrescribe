import Constants = require("../tools/constants");

export interface ColumnSortedEvent {
    sortColumn: string;
    sortDirection: Constants.ColumnSortDirection;
}

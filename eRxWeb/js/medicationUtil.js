function RowClick(sender, eventArgs) {
    var index = eventArgs.get_itemIndexHierarchical();
    var masterTable = sender.get_masterTableView();
    var row = masterTable.get_dataItems()[index];
    var csSchedule = row.getDataKeyValue("controlledsubstancecode");
    
    var msg = document.getElementById("epcsNotAllowedErrorMessage");
    if (csSchedule != null && msg != null && csSchedule.trim().length > 0)
    {
        msg.style.display = 'inline';
    } else
    {
        msg.style.display = 'none';
    }
}
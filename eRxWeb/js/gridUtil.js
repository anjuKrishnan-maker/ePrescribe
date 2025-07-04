// JScript File
function GridSingleSelect(keyValue, dataKeyName, gridClientID, selectOnGrid)
{
    var controlIndex = 0;        
    var items = $find(gridClientID).get_dataItems();
    for (i = 0; i < items.length; i++)
    {
        if (keyValue.toString() == items[i].getDataKeyValue(dataKeyName).toString())
        {   
            selectRadio(keyValue, dataKeyName)                
            if (selectOnGrid)
            {                     
                items[i].set_selected(true);
            }                        
        }
        else
        {
            items[i].set_selected(false);
        }
    }
}

function selectRadio(keyValue, dataKeyName)
{
    for (j = 0; j < document.forms[0].elements.length; j=j+1)
    {
        var control = document.forms[0].elements[j];
        if (control.type == "radio")
        {      
            if (control.getAttribute(dataKeyName) != null)
            {       
                if (control.getAttribute(dataKeyName).toString() == keyValue)
                {
                    control.checked = "checked";
                }
                else
                {
                    control.checked = "";
                }
            }
        }
    }
}     


function grdViewFormularyMedSingleSelect(keyValue, dataKeyName, gridClientID, selectOnGrid) {
    var controlIndex = 0;
    var grid = $find(gridClientID);
    var masterTableView = grid.get_masterTableView();
    if (masterTableView != null) {
        var items = masterTableView.get_dataItems();
        for (i = 0; i < items.length; i++) {
            if (keyValue.toString() == items[i].getDataKeyValue(dataKeyName).toString()) {
                grdViewFormularyMedSelectRadio(keyValue, dataKeyName)
                if (selectOnGrid) {
                    items[i].set_selected(true);
                }
            }
            else {
                items[i].set_selected(false);
            }
        }
    }
}
function grdViewFormularyMedSelectRadio(keyValue, dataKeyName) {
    for (j = 0; j < document.forms[0].elements.length; j = j + 1) {
        var control = document.forms[0].elements[j];
        if (control.type == "radio") {
            if (control.getAttribute(dataKeyName) != null) {
                if (control.getAttribute(dataKeyName).toString() == keyValue) {
                    control.checked = "checked";
                }
                else {
                    control.checked = "";
                }
            }
        }
    }
}

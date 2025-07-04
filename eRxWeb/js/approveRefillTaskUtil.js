function ShowPAApprovalTextBox(rdbYes, txtPaApprovalCode, lblPaApprovalCode) {
    if (txtPaApprovalCode != null && rdbYes != null && lblPaApprovalCode != null) {
        if (rdbYes.checked) {
            txtPaApprovalCode.style.display = "inline";
            lblPaApprovalCode.style.display = "inline";
        } else {
            txtPaApprovalCode.style.display = "none";
            lblPaApprovalCode.style.display = "none";
        }
    }
}

function ShowHideControls(rdbDeny, ddlDenial, txtNotes, lblNotes, lblNotesCount) {
    if (rdbDeny != null && ddlDenial != null)
    {
        if(lblNotesCount != null) lblNotesCount.children[0].innerText = "70";
        if (rdbDeny.checked)
        {
            ddlDenial.style.display = "inline";
            ddlDenial.selectedIndex = 0;
            if (txtNotes != null && lblNotesCount != null && lblNotes != null) {
                txtNotes.style.display = lblNotes.style.display = lblNotesCount.style.display = "none";
            }
        }
        else
        {
            ddlDenial.style.display = "none";

            if (txtNotes != null && lblNotesCount != null && lblNotes != null) {
                txtNotes.value = "";
                txtNotes.style.display = lblNotes.style.display = lblNotesCount.style.display = "inline";
            }
        }
    }
}


function DisableNonApprovalRows(eventArgs, approvalTaskType) {
    var mt = eventArgs.get_tableView();
    var rows = mt.get_dataItems();
    for (var i = 0; i < rows.length; i++) {
        var taskType = rows[i].getDataKeyValue("tasktype");
        if (taskType !== approvalTaskType) {
            DisableRow(rows[i], mt);
        }
        else {
            EnableRow(rows[i]);
        }
    }
}

function HandleControlsForSelection(eventArgs) {
    var mt = eventArgs.get_tableView();
    var rows = mt.get_dataItems();
    var rowSelected = eventArgs.get_itemIndexHierarchical();

    for (var i = 0; i < rows.length; i++) {

        if (i !== parseInt(rowSelected)) {
            DisableRow(rows[i], mt);
        }
        else {
            EnableRow(rows[i]);
        }
    }
}

function EnableRow(row) {
    var inputs = row.get_element().getElementsByTagName("input");
    var selects = row.get_element().getElementsByTagName("select");
    var aTags = row.get_element().getElementsByTagName("a");
    var textArea = row.get_element().getElementsByTagName("textarea");

    disableElements(inputs, false);
    disableElements(selects, false);
    disableElements(textArea, false);
    disableTags(aTags, false);
}

function DisableRow(row, table) {
    var inputs = row.get_element().getElementsByTagName("input");
    var selects = row.get_element().getElementsByTagName("select");
    var aTags = row.get_element().getElementsByTagName("a");
    var textArea = row.get_element().getElementsByTagName("textarea");

    disableElements(inputs, true);
    disableElements(selects, true);
    disableElements(textArea, true);
    disableTags(aTags, true);
    var cell = table.getCellByColumnUniqueName(row, "NotesDenialCol");
    var elms = cell.getElementsByClassName("hideWhenNotSelected");
    row.get_element().getElementsByClassName("resetSpan")[0].innerHTML = "70";
    hideElements(elms);
}

function hideElements(elements) {
    for (var i = 0; i < elements.length; i++) {
        elements[i].style.display = "none";
    }
}

function disableElements(elements, disabled) {
    for (var j = 0; j < elements.length; j++) {
        if (disabled) {
            if (!elements[j].hasAttribute('originalValueSaved')) {
                elements[j].setAttribute('wasDisabled', elements[j].disabled);
                elements[j].setAttribute('originalValueSaved', true);
                elements[j].disabled = true;
            }
            else {
                elements[j].disabled = true;
            }
            elements[j].checked = false;
            elements[j].selectedIndex = 1;
            elements[j].value = "";
        }
        else {
            if (elements[j].hasAttribute('originalValueSaved')) {
                elements[j].disabled = elements[j].getAttribute('wasDisabled') === 'true';
            }
            else {
                elements[j].setAttribute('wasDisabled', elements[j].disabled);
                elements[j].setAttribute('originalValueSaved', true);
            }
        }
    }
}

function disableTags(elements, disabled) {
    for (var j = 0; j < elements.length; j++) {
        if (disabled) {
            elements[j].setAttribute('hrefRemoved', elements[j].getAttribute("href"));
            if (elements[j].style.color !== 'lightgrey') elements[j].setAttribute('colorRemoved', elements[j].style.color);
            elements[j].removeAttribute('href');
            elements[j].style.color = 'lightgrey';
        }
        else {
            if (elements[j].hasAttribute('colorRemoved')) {
                elements[j].setAttribute('href', elements[j].getAttribute("hrefRemoved"));
                elements[j].style.color = elements[j].getAttribute("colorRemoved");
            }
        }
    }
}



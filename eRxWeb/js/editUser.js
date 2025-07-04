function updateCheckBoxCheckStatus(chkParent, chkChild) {
    chkChild.checked = chkParent.checked;
    chkChild.disabled = !chkParent.checked;
    return chkChild;
}

function btnSubmitEpcsClientClick(btnSubmitEpcs)
{
    if (!pageIsValid())
    {
        return false;
    }
    btnSubmitEpcs.disabled = true;
    btnSubmitEpcs.value = "Processing...";
}
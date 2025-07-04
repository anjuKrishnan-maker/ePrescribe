function CheckLength(txt, maxLen) {
    try {
        if (txt != null) {
            var iLength = txt.value.length
            if (iLength <= maxLen) //Check the Limit.
            {
                //Display the remaining characters
                var lblCountChar = document.getElementById('ctl00_ContentPlaceHolder2_ucPrivacyOverride_lblCountChar');
                var lblCountChar1 = document.getElementById('ctl00_ContentPlaceHolder1_ucPrivacyOverride_lblCountChar');
                if (lblCountChar != null) {
                    lblCountChar.innerHTML = maxLen - iLength;
                }

                if (lblCountChar1 != null) {
                    lblCountChar1.innerHTML = maxLen - iLength;
                }

            }
            else {
                txt.value = txt.value.substring(0, maxLen);
                return false;
            }
        }
    }
    catch (e) {
        return false;
    }
}


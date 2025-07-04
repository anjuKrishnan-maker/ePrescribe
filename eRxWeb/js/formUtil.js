// Form utilities

function capitilizeInitial(inputObject)
{
	var inputString; 
	inputString = inputObject.value;
	if (inputString.length > 0)
	{
		inputObject.value = inputString.replace(/\b([a-z])/gi, function(c){return c.toUpperCase();});
	}
}

function numericKeyPressOnly(inputObject, e)
{
	var keyCode = window.event ? e.keyCode : e.which;
	
	if (keyCode != null)
	{
		//Numeric, tab, backspace are valid key strokes
		if (keyCode == 0 ||
			keyCode == 8 || 
			keyCode == 9 ||
			(keyCode >= 48 && keyCode <= 57))
		{
			return true;
		}
	}
	
	return false;
}
function normalizeStringToNumberOnly(inputString)
{
	return inputString.replace(/[^0-9]/g, "");
}
function parseNumberInput(inputObject)
{
	var inputString = inputObject.value;
	
	if (inputString.length > 0)
	{
		inputObject.value = normalizeStringToNumberOnly(inputString);
	}
}
function formatPhoneInput(inputObject)
{
	var inputString = inputObject.value; 
	//take the input value first trim off all the non numeric characters
	inputString = normalizeStringToNumberOnly(inputString);
	if (inputString.length >= 10)
	{
		var formattedPhone = inputString.substring(0, 3) + "-" + inputString.substring(3, 6) + "-" + inputString.substring(6, 10); 
		inputObject.value = formattedPhone;
	}
}

function formatSSNInput(inputObject)
{
	var inputString = inputObject.value;
	inputString = normalizeStringToNumberOnly(inputString);
	
	if (inputString.length == 9)
	{
		var formattedSSN = inputString.substring(0, 3) + "-" + inputString.substring(3, 5) + "-" + inputString.substring(5);
		inputObject.value = formattedSSN;
	}
}

function LimitInput(targetField, maxLen, eventObj)
{
    var isPermittedKeystroke;
    var enteredKeystroke;
    var maximumFieldLength;
    var currentFieldLength;
    var inputAllowed = true;
    var selectionLength = 0;
    
    if (document.selection)
		selectionLength = parseInt(document.selection.createRange().text.length);
	else if (document.getSelection)
		selectionLength = parseInt(document.getSelection().length);
	
    if ( maxLen != null )
    {
     // Get the current and maximum field length
     currentFieldLength = parseInt(targetField.value.length);
        maximumFieldLength = parseInt(maxLen);

        // Allow non-printing, arrow and delete keys
        enteredKeystroke = window.event ? eventObj.keyCode : eventObj.which;
        isPermittedKeystroke = ((enteredKeystroke < 32)                                // Non printing
                     ||(enteredKeystroke >= 33 && enteredKeystroke <= 40))    // Page Up, Down, Home, End, Arrow

        // Decide whether the keystroke is allowed to proceed
        if ( !isPermittedKeystroke )
        {
            if ( ( currentFieldLength - selectionLength ) >= maximumFieldLength ) 
            {
                inputAllowed = false;
                
            }
        }
    } 
    
    if (window.event)
		window.event.returnValue = inputAllowed;
	else if (eventObj.preventDefault && !inputAllowed)
		eventObj.preventDefault();
	
    return (inputAllowed);
}

//
// Limit the text input in the specified field on a paste event. 
// The onpaste event only works on IE browsers. Firefox, Opera will not trigger the onpaste event. 
// In order to get access to the Firefox, Opera browser's clipboard data, browser needs to be set up with higher security access. 
// For the time being limit the paste length by the onchange event instead of the onpaste event. 
function LimitPaste(targetField, maxLen)
{
    var clipboardText;
    var resultantLength;
    var maximumFieldLength;
    var currentFieldLength;
    var pasteAllowed = true;
    var selectionLength = 0;
    
    if (document.selection)
		selectionLength = parseInt(document.selection.createRange().text.length);
	else if (document.getSelection)
		selectionLength = parseInt(document.getSelection().length);

    if ( maxLen != null )
    {
     // Get the current and maximum field length
     currentFieldLength = parseInt(targetField.value.length);
        maximumFieldLength = parseInt(maxLen);

        clipboardText = window.clipboardData.getData("Text");
        resultantLength = currentFieldLength + clipboardText.length - selectionLength;
        if ( resultantLength > maximumFieldLength)
        {
            pasteAllowed = false;
        }    
    }    
    
    if (window.event)
		window.event.returnValue = pasteAllowed;
		
    return (pasteAllowed);
}

function LimitChange(targetField, maxLen, eventObj)
{
	var maximumFieldLength;
    var currentFieldLength;
    
    if ( maxLen != null )
    {
		// Get the current and maximum field length
		currentFieldLength = parseInt(targetField.value.length);
        maximumFieldLength = maxLen;
        
        if (currentFieldLength > maximumFieldLength)
        {
            targetField.value = targetField.value.substring(0, maximumFieldLength);
        }
    }
}

function checkFieldLength(limitField, limitCountID, limitNum) {
    var len = limitField.value.length;
    if (len > limitNum) {
        limitField.value = limitField.value.substring(0, limitNum);
        len = limitNum
    }

    var limitCount = document.getElementById(limitCountID);
    if (limitCount != null) {
        limitCount.innerHTML = limitNum - len;
    }
}

function onlyAllowNumericZipPress(event)
{
    return new RegExp(/^\d{0,9}$/).test(event.key);
}
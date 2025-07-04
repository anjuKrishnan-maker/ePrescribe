export function IsStringNullUndefinedEmpty(input) {
    if (input == undefined
        || input == null) {
        return true;
    }

    let inputString = String(input);
    if (inputString == '') {
        return true;
    }

    return false;
}


declare global {
    interface String {
        PasswordValidator(): boolean;
    }
}
String.prototype.PasswordValidator = function (): boolean {
    
    let inputString = String(this);
    let options: number = 0;
    if (inputString) {
        if (/[a-z]/.test(inputString))
            options++;
        if (/[A-Z]/.test(inputString))
            options++;
        if (/[!@#\$%\^\&*\)\(+=._-]/.test(inputString))
            options++;
        if (/[0-9]/.test(inputString))
            options++;
        if (inputString.length < 8 || inputString.length > 25 || options < 3) {            
            return false;
        }
        else {
            return true;
        }

    }


}

export { };   
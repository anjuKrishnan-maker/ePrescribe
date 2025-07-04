
    export function DateValidator(date: string) {
        let regex = RegExp("^((0?[1-9]|1[012])[- /.](0?[1-9]|[12][0-9]|3[01])[- /.](19|20)?[0-9]{2})*$");
        let result = regex.test(date);
        return result;
    }

    export function convertDateToString(date: Date): string {
        let dateString = '';
        try {
            if (typeof (date) == "string") {
                date = new Date(date);
            }
            //why doesn't js have a simpler date to string formatter!!
            dateString = (date.getMonth() + 1) + '/' + date.getDate() + '/' + date.getFullYear();
        }
        catch{
            dateString = '';
        }
        return dateString;
    }
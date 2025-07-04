
export function isNullOrWhiteSpace(value: string) {
    if (value == null) return true;

    for (let i = 0; i < value.length; i++) {
        if (!(value[i] == ' ' || value[i] == '')) return false;
    }
    return true;
}  
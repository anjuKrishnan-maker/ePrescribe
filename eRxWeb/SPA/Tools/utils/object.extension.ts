
export default class ObjectUtil {
    private constructor() { }


    public static IsEmpty(val: Object) {
        return val && Object.keys(val).length === 0;
    }

    public static ConvertToPascalCase(val: Object, except: string[]=[]): Object {
        if (ObjectUtil.IsEmpty(val))
            return val;

        let pascalCasePattern = new RegExp("^([A-Z])([a-z]+)")

        let nameTransformer = (propname) => {
            if (pascalCasePattern.test(propname)) {
                return propname;
            }
            else {
                return propname.charAt(0).toUpperCase() + propname.slice(1);
            }
        }

        let transformed, value, type = Object.prototype.toString.apply(val);
        if (type == "[object Object]") {
            transformed = {};
            let isExceptConsidered = except && except.length > 0;
            for (var propname in val) {
                let prp: string = propname;
                value = val[propname];
                if (isExceptConsidered && except.indexOf(prp) > -1) {
                    transformed[propname] = value;
                    continue;
                };
                transformed[nameTransformer(propname)] = value;
            }
            return transformed;
        }
    }
}
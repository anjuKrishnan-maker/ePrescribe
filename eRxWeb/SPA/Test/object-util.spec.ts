import { TestBed,  async } from '@angular/core/testing';
import { BrowserDynamicTestingModule, platformBrowserDynamicTesting } from '@angular/platform-browser-dynamic/testing';
import { } from 'jasmine';
import ObjectUtil from '../tools/Utils/object.extension';
describe('Object Util unit tests', () => {

    beforeEach(async(() => {
        TestBed.resetTestEnvironment();
        TestBed.initTestEnvironment(BrowserDynamicTestingModule, platformBrowserDynamicTesting());
    }));

    it('handle null checks wil get what you send', async(() => {
        //arrange
        let camelCaseObjectUndefined = undefined;
        let camelCaseObjectNull = null;
        let camelCaseObjectEmpty = {  };

        //Act
        let objUndefined = ObjectUtil.ConvertToPascalCase(camelCaseObjectUndefined);
        let objNull = ObjectUtil.ConvertToPascalCase(camelCaseObjectNull);
        let objEmpty = ObjectUtil.ConvertToPascalCase(camelCaseObjectEmpty);

        expect(objUndefined).toBeUndefined(objUndefined);
        expect(objNull).toBe(objNull);
        expect(objEmpty).toBe(camelCaseObjectEmpty, objEmpty);
       
    }));

    it('get object keys in pascal case', async(() => {
        //arrange
        let camelCaseObject = {firstName:"hello",LastName:"lastNamevalue"};

        //Act
        let pascalCaseObject = ObjectUtil.ConvertToPascalCase(camelCaseObject);
        console.log(pascalCaseObject);
        expect(pascalCaseObject["FirstName"]).toEqual("hello");
        expect(pascalCaseObject["firstName"]).toBeUndefined();
        expect(pascalCaseObject["LastName"]).toEqual("lastNamevalue");
        expect(pascalCaseObject["lastName"]).toBeUndefined();
    }));

    it('get object keys in pascal case, with exempted as it is', async(() => {
        //arrange
        let camelCaseObject = { firstName: "hello", LastName: "lastNamevalue" };

        //Act
        let pascalCaseObject = ObjectUtil.ConvertToPascalCase(camelCaseObject, ["firstName"]);
        console.log(pascalCaseObject);
        expect(pascalCaseObject["firstName"]).toEqual("hello");
        expect(pascalCaseObject["FirstName"]).toBeUndefined();
        expect(pascalCaseObject["LastName"]).toEqual("lastNamevalue");
        expect(pascalCaseObject["lastName"]).toBeUndefined();
    }));

});


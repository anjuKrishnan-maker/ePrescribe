import { Injectable } from '@angular/core';
import { BaseService } from './base.service';
import { StateLicense, UserStateLicenseRequest, MessageModel } from '../model/model.import.def';
import { HttpClient } from '@angular/common/http';
import { UserRequest, UserModel, SaveUserResponseModel, UserActivationInfoModel, UserBackButtonModel } from '../model/user/user.model';
import { SecretQuestion, SecretQuestionAnswer, SecretQuestionAnswerResponse, SecretQuestionAnswerRequest } from '../model/secret-question-answer.model';
import { API_ROUTE_NAME } from '../tools/constants';

@Injectable()
export class UserService extends BaseService {
    constructor(http: HttpClient) {
        super(http);
    }

    userInit(userRequest: UserRequest) {
        return this.InvokeApiMethod<UserModel>(API_ROUTE_NAME.EDIT_USER_INIT, userRequest);
    }

    saveUser(saveUserRequest: UserModel) {
        return this.InvokeApiMethod<SaveUserResponseModel>(API_ROUTE_NAME.EDIT_USER_SAVE, saveUserRequest);
    }    

    getUserStateLicenses(userStateLicenseRequest: UserStateLicenseRequest) {
        return this.InvokeApiMethod<StateLicense[]>('/api/UserApi/GetUserStateLicenses', userStateLicenseRequest);        
    }

    deleteUserStateLicense(userStateLicenseRequest: UserStateLicenseRequest) {
        return this.InvokeApiMethod<MessageModel>('/api/UserApi/DeleteUserStateLicense', userStateLicenseRequest);
    }

    updateUserStateLicense(userStateLicenseRequest: UserStateLicenseRequest) {
        return this.InvokeApiMethod<MessageModel>('/api/UserApi/UpdateUserStateLicense', userStateLicenseRequest);
    }

    addUserStateLicense(userStateLicenseRequest: UserStateLicenseRequest) {
        return this.InvokeApiMethod<MessageModel>('/api/UserApi/AddUserStateLicense', userStateLicenseRequest);
    }

    getSecretQuestionsList() {
        return this.InvokeApiMethod<SecretQuestion[]>('/api/UserApi/GetSecretQuestionsList', null);
    }

    getUserSecretQuestions() {
        return this.InvokeApiMethod<SecretQuestion[]>('/api/UserApi/GetUserSecretQuestions', null);
    }

    saveSecretQuestionAnswers(secretQuestionAnswerRequest: SecretQuestionAnswerRequest) {
        return this.InvokeApiMethod<SecretQuestionAnswerResponse>('api/UserApi/SaveSecretQuestionAnswers', secretQuestionAnswerRequest);
    }

    processUserActivation(newUserActivationData: UserActivationInfoModel) {
        return this.InvokeApiMethod<MessageModel>("/api/userapi/processnewuser", newUserActivationData);
    }
}
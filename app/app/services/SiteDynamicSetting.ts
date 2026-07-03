import type {IApiResult} from "~~/types";
export enum DynamicSettings {
    babak = "babak",
    motivation = "motivation",
}

export interface IDynamicSetting {
    id: number,
    key: string,
    type: string,
    jsonValue: string,
    createDate: Date,
    updateDate: Date
}
export default class SiteDynamicSetting {
    constructor(private httpClient: LocalFetch) {
    }

    setSiteDynamicSetting(payload: { id: number, jsonValue: string }): Promise<IApiResult> {
        return this.httpClient(`DynamicSiteSetting/Update`, {
            method: "PUT",
            body: payload,
        });
    }

    getSiteDynamicSettingByKeyAndType(params: { type: string, key: string }): Promise<IApiResult> {
        return this.httpClient(`DynamicSiteSetting/GetByTypeAndKey/by-type-key`, {
            method: "GET",
            params,
        });
    }


}

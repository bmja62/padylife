import type {IApiResult} from "~~/types";

export interface ICountry {
    countryId: number,
    countryName: string,
    countryNameFa: string,
    countryCode: string,
    phoneCode: string,
    isActive: true,
    id: number
}

export interface IProvince {
    countryId: null,
    provinceName: string,
    provinceNameFa: string,
    provinceCode: string,
    isActive: boolean,
    id: null
}

export interface ICity {
    provinceId: number,
    cityName: string,
    cityNameFa: string,
    cityCode: string,
    isActive: boolean,
    id: null
}

export interface IGetProvincesParams extends IGlobalGridRequest {
    countryId: number
}

export interface IGetCitiesParams extends IGlobalGridRequest {
    provinceId: number
}

export default class SettingService {
    constructor(private httpClient: LocalFetch) {
    }

    getCountries(params: IGlobalGridRequest): Promise<IApiResult> {
        return this.httpClient(`Countries/Get`, {
            method: "GET",
            params,
        });
    }

    getProvinces(params: IGetProvincesParams): Promise<IApiResult> {
        return this.httpClient(`Provinces/GetByFilter`, {
            method: "GET",
            params,
        });
    }

    getCities(params: IGetCitiesParams): Promise<IApiResult> {
        return this.httpClient(`Cities/GetByFilter`, {
            method: "GET",
            params,
        });
    }

}

import type {IApiResult} from "~~/types";

export enum AddressType { Home = 'Home', Work ='Work', Other = 'Other' }

export interface IGetAddressesParams extends IGlobalGridRequest {
    userId: number;
}

export interface IAddress {
    userId: number,
    countryId: number,
    provinceId: number,
    cityId: number,
    postalCode: string,
    addressText: string,
    plaque: number,
    unit: number,
    floor: number,
    recipientName: string,
    recipientPhone: string,
    landlinePhone: string,
    isDefault: boolean,
    addressType: AddressType,
    geoLocation?: string,
    id: number

}

export interface ICreateAddressPayload extends Omit<IAddress, 'id'> {
}


export default class AddressService {
    constructor(private httpClient: LocalFetch) {
    }

    getUserAddresses(params: IGetAddressesParams): Promise<IApiResult> {
        return this.httpClient(`Addresses/GetByFilter`, {method: "GET", params});
    }

    createAddress(payload: IAddress): Promise<IApiResult> {
        return this.httpClient(`Addresses/Create`, {method: "POST", body: payload});
    }

    updateAddress(payload: IAddress): Promise<IApiResult> {
        return this.httpClient(`Addresses/Update`, {
            method: "PUT", body: payload, params: {
                id: payload.id
            }
        });
    }

    deleteAddress(addressId: number): Promise<IApiResult> {
        return this.httpClient(`Addresses/Delete/${addressId}`, {method: "DELETE"});
    }


}

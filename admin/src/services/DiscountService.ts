import type {AxiosInstance, AxiosResponse} from 'axios'
import type {IApiResult} from '@/models/IApiResult'
import {IGlobalGridRequest} from "@/models/IGlobalGridRequest";

export interface IDiscount {
    code: string,
    discountAmount: number,
    discountPercentage: number,
    startDate: Date,
    endDate: Date,
    isSpecial: boolean,
    id: number
}

export interface ICreateDiscountPayload extends Omit<IDiscount, 'id'> {
}

export default class DiscountService {
    constructor(private axiosInstance: AxiosInstance) {
    }

    getAllCoupons(discountsFilter: IGlobalGridRequest): Promise<AxiosResponse<IApiResult<IGlobalGridResult<object[]>>>> {
        return this.axiosInstance.get('Discounts/Get', {
            params: discountsFilter,
        })
    }

    createCoupon(payload: ICreateDiscountPayload): Promise<AxiosResponse<IApiResult>> {
        return this.axiosInstance.post('Discounts/Create', payload)
    }

    updateCoupon(payload: IDiscount): Promise<AxiosResponse<IApiResult>> {
        return this.axiosInstance.put('Discounts/Update', payload)
    }

    deleteCoupon(id: string | number): Promise<AxiosResponse<IApiResult>> {
        return this.axiosInstance.delete(`Discounts/Delete/${id}`)
    }
}

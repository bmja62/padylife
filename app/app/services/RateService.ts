import type {IApiResult} from "~~/types";

export enum RateType {
    Product = 'Product',
    Variant = 'Variant',
    Blog = 'Blog',
    Plan = 'Plan',
    Excersie = 'Excersie',
    UserPlanExerciesAnswers = 'UserPlanExerciesAnswers',
    Step = 'Step',
}

export interface ICreateRate {
    entityId: number
    entityType: RateType
    ratingValue: number
}

export interface IGetEntityRateParams {
    entityId: number
    entityType: RateType
}

export default class RateService {
    constructor(private httpClient: LocalFetch) {
    }

    createRating(payload: ICreateRate): Promise<IApiResult> {
        return this.httpClient("Rates/Create", {method: "POST", body: payload});
    }
    getMyRatingStats(filters: IGetEntityRateParams): Promise<AxiosResponse<IApiResult<IGlobalGridResult>>> {
        return this.httpClient('Rates/IsRateExist', {
            method: "POST",
            params: filters
        })
    }
    getAverageRating(filters: IGetEntityRateParams): Promise<AxiosResponse<IApiResult<IGlobalGridResult>>> {
        return this.httpClient('Rates/GetAverageRating', {
            method: "POST",
            params: filters
        })
    }
}

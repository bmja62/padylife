import type {IApiResult} from "~~/types";
import {DailyFeelingsEnum, DailyFeelingsKeys} from "~/models/Enums/DailyFeelings";

export const feelingsIconMap = {
    1: "new-better",
    2: "new-good",
    3: "new-meh",
    4: "new-sad",
    5: "new-sadder",
};
export const feelingsMap = [
    {
        type:DailyFeelingsEnum.Glad
    },
    {
        type:DailyFeelingsEnum.Happy
    },
    {
        type:DailyFeelingsEnum.Poker
    },
    {
        type:DailyFeelingsEnum.Sad
    },
    {
        type:DailyFeelingsEnum.Bad
    },
]

export interface ICreateDailyFeelingPayload {
    type: DailyFeelingsEnum
    description: string
    voiceUrl: string
}

export interface IDailyFeelingUser {
    createdByUserId: number,
    userName: string,
    fullName: string,
    age: number,
    email: string
}

export interface IDailyFeeling {
    id: number,
    type: DailyFeelingsKeys,
    description: string,
    voiceUrl: string,
    createdAt: Date,
    userInfo: IDailyFeelingUser
}

export interface IGetDailyFeelingsParams extends IGlobalGridRequest {
    userId: number,
    startDate: Date | string,
    endDate: Date | string
}
export default class DailyFeelings {
    constructor(private httpClient: LocalFetch) {
    }

    createDailyFeeling(payload: ICreateDailyFeelingPayload): Promise<IApiResult> {
        return this.httpClient("DailyFeelings/Create", {method: "POST", body:payload});
    }

    getAllDailyFeelings(params: IGetDailyFeelingsParams): Promise<IApiResult> {
        return this.httpClient("DailyFeelings/GetAll", {method: "GET", params});
    }
}

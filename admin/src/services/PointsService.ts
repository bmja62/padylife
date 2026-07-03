import type {AxiosInstance, AxiosResponse} from 'axios'
import type {IApiResult} from '@/models/IApiResult'
import type {IGlobalGridResult} from '@/models/IGlobalGridResult'

export interface IUserPoints {
  availablePoints: number
  consumedPoints: number
  earnedPoints: number
  moneyValue: number
  pointsToMoneyRatio: number
  userId: number
}

export interface IPointsPayload {
  userId: number,
  amount: number,
  reason: string,
  referenceId: number,
  referenceType: "Product"
}

export enum InternalPointsActionType {
  Earn = 1,
  Consume = 2,
}

export default class PointsService {
  constructor(private axiosInstance: AxiosInstance) {
  }

  getUserPoints(userId: number): Promise<AxiosResponse<IApiResult<IGlobalGridResult>>> {
    return this.axiosInstance.get(`Points/GetUserPoints/${userId}`)
  }

  earnUserPoints(payload: IPointsPayload): Promise<AxiosResponse<IApiResult<IGlobalGridResult>>> {
    return this.axiosInstance.post(`Points/EarnPoints/earn`, payload)
  }

  consumeUserPoints(payload: IPointsPayload): Promise<AxiosResponse<IApiResult<IGlobalGridResult>>> {
    return this.axiosInstance.post(`Points/ConsumePoints/consume`, payload)
  }

}

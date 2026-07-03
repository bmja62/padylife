import type {AxiosInstance, AxiosResponse} from 'axios'
import type {IApiResult} from '@/models/IApiResult'
import type {IGlobalGridResult} from '@/models/IGlobalGridResult'
import {IGlobalGridRequest} from "@/models/IGlobalGridRequest";

export enum ChallengeType {
  Single = 'Single',
  Group = 'Group',
}

export const challengeTypesShow = {
  Single: "فردی",
  Group: "گروهی",
};

export interface IChallenge {
  title: string,
  description: string,
  imageUrl: string,
  type: ChallengeType,
  id: number
}
export interface IGetChallangeFilters extends IGlobalGridRequest{
  type: ChallengeType,
}
export interface ICreateChallengePayload extends Omit<IChallenge, 'id'> {
}

export interface IUpdateChallengePayload extends IChallenge {
}

export default class ChallengeService {
  constructor(private axiosInstance: AxiosInstance) {
  }

  getAllChallenges(filters: IGlobalGridRequest): Promise<AxiosResponse<IApiResult<IGlobalGridResult<IChallenge[]>>>> {
    return this.axiosInstance.get('Challenges/Get', {
      params: filters
    })
  }
  getAllChallengesByFilter(filters: IGetChallangeFilters): Promise<AxiosResponse<IApiResult<IGlobalGridResult<IChallenge[]>>>> {
    return this.axiosInstance.get('Challenges/GetAllByFilter', {
      params: filters
    })
  }
  getAllChallengesByToken(filters: IGetChallangeFilters): Promise<AxiosResponse<IApiResult<IGlobalGridResult<IChallenge[]>>>> {
    return this.axiosInstance.post('Challenges/GetChallengesByToken', {
      params: filters
    })
  }
  getChallengeById(challengeId: number): Promise<AxiosResponse<IApiResult<IGlobalGridResult<IChallenge>>>> {
    return this.axiosInstance.get(`Challenges/Get/${challengeId}`)
  }

  createChallenge(payload: ICreateChallengePayload): Promise<AxiosResponse<IApiResult<IGlobalGridResult>>> {
    return this.axiosInstance.post('Challenges/Create', payload)
  }

  updateChallenge(payload: IUpdateChallengePayload): Promise<AxiosResponse<IApiResult<IGlobalGridResult>>> {
    return this.axiosInstance.put('Challenges/Update', payload,{
      params:{
        id:payload.id
      }
    })
  }

  deleteChallenge(challengeId: number): Promise<AxiosResponse<IApiResult<IGlobalGridResult>>> {
    return this.axiosInstance.delete(`Challenges/Delete/${challengeId}`)
  }

}

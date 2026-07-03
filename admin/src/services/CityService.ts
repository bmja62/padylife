import type {AxiosInstance, AxiosResponse} from 'axios'
import type {IApiResult} from '@/models/IApiResult'
import {IGlobalGridRequest} from "@/models/IGlobalGridRequest";


export interface IEntity {
  provinceId: number,
  cityName: string,
  cityNameFa: string,
  cityCode: string,
  isActive: boolean,
  id: null
}

export interface ICreateEntityPayload extends Omit<IEntity, 'id'> {

}

export interface IUpdateEntityPayload extends Omit<IEntity, 'id'> {

}

export default class CityService {
  constructor(private axiosInstance: AxiosInstance) {
  }

  getAll(params: IGlobalGridRequest): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.get('Cities/Get', {
      params
    })
  }

  create(payload: ICreateEntityPayload): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post('Cities/Create', payload)
  }

  update(payload: IUpdateEntityPayload, entityId: number): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.put('Cities/Update', payload, {
      params: {
        id: entityId,
      }
    })
  }

  delete(entityId: number): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.delete(`Cities/Delete/${entityId}`)
  }
}

import type {AxiosInstance, AxiosResponse} from 'axios'
import type {IApiResult} from '@/models/IApiResult'
import {IGlobalGridRequest} from "@/models/IGlobalGridRequest";



export interface IEntity {
  countryId: number,
  countryName: string,
  countryNameFa: string,
  countryCode: string,
  phoneCode: string,
  isActive: true,
  id: number
}

export interface ICreateEntityPayload extends Omit<IEntity, 'id'> {

}

export interface IUpdateEntityPayload extends Omit<IEntity, 'id'> {

}

export default class CountriesService {
  constructor(private axiosInstance: AxiosInstance) {
  }

  getAll(params: IGlobalGridRequest): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.get('Countries/Get', {
      params
    })
  }

  create(payload: ICreateEntityPayload): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post('Countries/Create', payload)
  }

  update(payload: IUpdateEntityPayload, entityId: number): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.put('Countries/Update', payload, {
      params: {
        id: entityId,
      }
    })
  }

  delete(entityId: number): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.delete(`Countries/Delete/${entityId}`)
  }
}

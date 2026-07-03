import type {AxiosInstance, AxiosResponse} from 'axios'
import type {IApiResult} from '@/models/IApiResult'
import {IGlobalGridRequest} from "@/models/IGlobalGridRequest";


export interface IEntity {
  countryId: null,
  provinceName: string,
  provinceNameFa: string,
  provinceCode: string,
  isActive: boolean,
  id: null
}

export interface ICreateEntityPayload extends Omit<IEntity, 'id'> {

}

export interface IUpdateEntityPayload extends Omit<IEntity, 'id'> {

}

export default class ProvinceService {
  constructor(private axiosInstance: AxiosInstance) {
  }

  getAll(params: IGlobalGridRequest): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.get('Provinces/Get', {
      params
    })
  }

  create(payload: ICreateEntityPayload): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post('Provinces/Create', payload)
  }

  update(payload: IUpdateEntityPayload, entityId: number): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.put('Provinces/Update', payload, {
      params: {
        id: entityId,
      }
    })
  }

  delete(entityId: number): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.delete(`Provinces/Delete/${entityId}`)
  }
}

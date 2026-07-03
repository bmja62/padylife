import type {AxiosInstance, AxiosResponse} from 'axios'
import type {IApiResult} from '@/models/IApiResult'
import type {IGlobalGridResult} from '@/models/IGlobalGridResult'

export enum DynamicSettings {
  babak = "babak",
  motivation = "motivation"
}

export interface IDynamicSetting {
  id: number,
  key: string,
  type: string,
  jsonValue: string,
  createDate: Date,
  updateDate: Date
}
export default class SettingsService {
  constructor(private axiosInstance: AxiosInstance) {
  }

  getAllSettings(filters: any): Promise<AxiosResponse<IApiResult<IGlobalGridResult<any[]>>>> {
    return this.axiosInstance.get('/SiteDynamicSetting/GetByOption', {
      params: filters,
    })
  }

  siteDynamicSettingCreate(payload: any): Promise<AxiosResponse<IApiResult<any>>> {
    return this.axiosInstance.post('/SiteDynamicSetting/Create', payload)
  }

  getAllBanners(): Promise<AxiosResponse<IApiResult<IGlobalGridResult<any[]>>>> {
    return this.axiosInstance.post('/Banner/GetAll')
  }
  getAllSliders(): Promise<AxiosResponse<IApiResult<IGlobalGridResult<any[]>>>> {
    return this.axiosInstance.post('/Media/GetSliders')
  }

  getDefaultFirstBanner(): Promise<AxiosResponse<IApiResult<IGlobalGridResult<any[]>>>> {
    return this.axiosInstance.post('/SiteDynamicSetting/GetFirstPageBanner')
  }

  setDefaultFirstBanner(payload: any): Promise<AxiosResponse<IApiResult<any>>> {
    return this.axiosInstance.post('/SiteDynamicSetting/UpdateFirstPageBanner', payload)
  }

  getDefaultMetaDescription(): Promise<AxiosResponse<IApiResult<IGlobalGridResult<any[]>>>> {
    return this.axiosInstance.post('/SiteDynamicSetting/GetMeta')
  }

  setDefaultMetaDescription(payload: any): Promise<AxiosResponse<IApiResult<any>>> {
    return this.axiosInstance.post('/SiteDynamicSetting/UpdateSiteMeta', {
      value: payload
    })
  }

  getSiteLogo(): Promise<AxiosResponse<IApiResult<IGlobalGridResult<any[]>>>> {
    return this.axiosInstance.post('/Dashboard/GetLogo')
  }

  setSiteLogo(payload: any): Promise<AxiosResponse<IApiResult<any>>> {
    return this.axiosInstance.post('/Dashboard/ChangeLogo', payload)
  }

  getDefaultAffiliatePercentage(): Promise<AxiosResponse<IApiResult<IGlobalGridResult<any[]>>>> {
    return this.axiosInstance.post('/SiteDynamicSetting/GetAffiliatePercentage')
  }

  setDefaultAffiliatePercentage(payload: any): Promise<AxiosResponse<IApiResult<any>>> {
    return this.axiosInstance.post('/SiteDynamicSetting/UpdateSiteAffiliatePercentage', {
      value: payload
    })
  }

  getPaymentStatus(): Promise<AxiosResponse<IApiResult<IGlobalGridResult<any[]>>>> {
    return this.axiosInstance.post('/SiteDynamicSetting/GetPaymentGatewayAccess')
  }

  setPaymentStatus(payload: any): Promise<AxiosResponse<IApiResult<any>>> {
    return this.axiosInstance.post('/SiteDynamicSetting/TogglePaymentGatewayAccess')
  }

  getDefaultAddress(): Promise<AxiosResponse<IApiResult<IGlobalGridResult<any[]>>>> {
    return this.axiosInstance.post('/SiteDynamicSetting/GetSiteAddress')
  }

  setDefaultAddress(payload: any): Promise<AxiosResponse<IApiResult<any>>> {
    return this.axiosInstance.post('/SiteDynamicSetting/UpdateSiteAdress', payload)
  }

  setSiteDynamicSetting(payload: { id: number, jsonValue: string }): Promise<AxiosResponse<IApiResult<any>>> {
    return this.axiosInstance.put('DynamicSiteSetting/Update', payload)
  }

  getSiteDynamicSettingByKeyAndType(params: { type: string, key: string }): Promise<AxiosResponse<IApiResult<any>>> {
    return this.axiosInstance.get('DynamicSiteSetting/GetByTypeAndKey/by-type-key', {
      params
    })
  }

}

import type {AxiosInstance, AxiosResponse} from 'axios'
import type {IApiResult} from '@/models/IApiResult'
import type {IGlobalGridResult} from '@/models/IGlobalGridResult'
import type {IGlobalGridRequest} from '@/models/IGlobalGridRequest'

export interface IDynamicPage {
  title: string
  slug: string
  content: string
  seoTitle: string
  seoDescription: string
  id: number
}

export interface IGetAllDynamicPagesFilters extends IGlobalGridRequest {
}

export interface IUpdateDynamicPagePayload extends IDynamicPage {
}

export interface ICreateDynamicPagePayload extends Omit<IDynamicPage, 'id'> {
  id?: number | null
}

export default class DynamicPageService {
  constructor(private axiosInstance: AxiosInstance) {
  }

  getAllDynamicPages(filters: IGetAllDynamicPagesFilters): Promise<AxiosResponse<IApiResult<IGlobalGridResult<IDynamicPage[]>>>> {
    return this.axiosInstance.post('/StaticPage/GetList', filters)
  }

  createANewDynamicPage(payload: ICreateDynamicPagePayload): Promise<AxiosResponse<IApiResult<IDynamicPage>>> {
    return this.axiosInstance.post('/StaticPage/Create', payload)
  }

  updateADynamicPage(payload: IUpdateDynamicPagePayload): Promise<AxiosResponse<IApiResult<IDynamicPage>>> {
    return this.axiosInstance.post('/StaticPage/Update', payload)
  }

  getASingleDynamicPage(id: string | number): Promise<AxiosResponse<IApiResult<IDynamicPage>>> {
    return this.axiosInstance.post(`/StaticPage/GetById`, null, {
      params: {
        id
      }
    })
  }

  deleteADynamicPage(id: string | number): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post(`/StaticPage/Delete`, null, {
      params: {
        id
      }
    })
  }
}

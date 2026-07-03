import type {AxiosInstance, AxiosResponse} from 'axios'
import type {IApiResult} from '@/models/IApiResult'
import {IGlobalGridRequest} from "@/models/IGlobalGridRequest";

export interface IProductCategory {
  name: string,
  description: string,
  parentCategoryId?: number | null,
  parentName: string,
  childCategories: string[],
  productCategoryAttributes: string[],
  imageUrl:string,
  id: number
}

export interface ICreateProductCategoryPayload extends Omit<IProductCategory, 'parentName' | 'childCategories' | 'productCategoryAttributes' | 'id'> {
}

export interface IUpdateProductCategoryPayload extends Omit<IProductCategory, 'parentName' | 'childCategories' | 'productCategoryAttributes' | 'id'> {
}

export default class ProductCategories {
  constructor(private axiosInstance: AxiosInstance) {
  }

  getAllProductCategories(params: IGlobalGridRequest): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.get('ProductCategories/Get', {
      params
    })
  }

  createProductCategory(payload: ICreateProductCategoryPayload): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post('ProductCategories/Create', payload)
  }

  updateProductCategory(payload: IUpdateProductCategoryPayload, productCategoryId: number): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.put('ProductCategories/Update', payload, {
      params: {
        id: productCategoryId,
      }
    })
  }

  deleteProductCategory(productCategoryId: number): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.delete(`ProductCategories/Delete/${productCategoryId}`)
  }
}

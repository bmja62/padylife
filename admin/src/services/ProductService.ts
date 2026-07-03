import type {AxiosInstance, AxiosResponse} from 'axios'
import type {IApiResult} from '@/models/IApiResult'
import {IGlobalGridRequest} from "@/models/IGlobalGridRequest";
import {UploaderTypes} from "@/models/Enums/UploaderTypes";
import {useUtils} from "@/composables/useUtils";

export enum ProductType {
  Simple = 'Simple',
  Variant = 'Variant',
}
export interface IProductBasic {
  id: number,
  name: string,
  description: string,
  price: number,
  stockQuantity: number,
  categoryId: number,
  categoryName: string,
}

export interface IProduct extends IProductBasic {
  productImages: IProductImage[],
  attributes: IProductAttribute[],
  variants: IProductVariant[],
  type:ProductType,
  userInfo: IProductUserInfo
}


export interface IProductImage {
  id: number,
  url: string,
  type: UploaderTypes,
  typeName: keyof UploaderTypes,
  objectId: number
}

export interface IProductImageMain {
  objectId: number,
  type: UploaderTypes,
  mainImage: File
}

export interface IProductImageGallery {
  objectId: number,
  type: UploaderTypes,
  galleryImage: File
}

export interface IProductAttribute {
  attributeId: number,
  attributeName: string,
  value: string
}

export interface IProductAttributeLite extends Omit<IProductAttribute, 'attributeName'> {

}

export interface IAddProductAttribute {
  productId: number,
  attributeId: number,
  value: string
}

export interface IProductVariant {
  id: number,
  sku: string,
  price: number,
  stockQuantity: number,
  productVariantImages: {
    main: IProductImage,
    gallery: IProductImage[]
  },
  attributeValues: IProductAttribute[]
}

export interface IProductVariantLite extends Omit<IProductVariant, 'id' | 'attributeValues'> {
  attributeValues: IProductAttributeLite[]
}

export interface IProductUserInfo {
  userId: number,
  userName: string,
  fullName: string
}

export interface IProductBasicPayload {
  name: string,
  description: string,
  categoryId: number | null,
  type: ProductType
}

export interface ICreateProductPayload extends Omit<IProductBasic, 'categoryName' | 'id'> {
  productImage: File,
  gallery: File[],
  AttributeValues: IProductAttributeLite[],
  variants: IProductVariantLite[]
}

export interface IProductGetParams extends IGlobalGridRequest {
  categoryId: number | null

}

export interface ISimplePricePayload {
  basePrice: number,
  variantPrices: [] | null
}

export interface ICreateVariantPayload {
  sku: string,
  price: null | number,
  attributeValues: { attributeId: null | number, value: string }[]
}
export default class ProductService {
  constructor(private axiosInstance: AxiosInstance) {
  }

  getAllProduct(params: IProductGetParams): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.get('Products/GetAll', {
      params
    })
  }

  getProductById(productId: number): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.get(`Products/GetById/${productId}`)
  }

  createProduct(payload: IProductBasicPayload): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post('Products/CreateBasic/basic', payload)
  }

  updateProduct(payload: ICreateProductPayload, productId: number): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.put(`Products/Update/${productId}`, payload)
  }

  deleteProduct(productId: number): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.delete(`Products/Delete/${productId}`)
  }

  addImageToGallery(payload: IProductImageGallery): Promise<AxiosResponse<IApiResult>> {
    const myForm = useUtils().generateAnyFormData(payload)
    return this.axiosInstance.post(`Products/AddImageToGallery`, myForm)
  }

  changeMainImage(payload: IProductImageMain): Promise<AxiosResponse<IApiResult>> {
    const myForm = useUtils().generateAnyFormData(payload)
    return this.axiosInstance.post(`Products/ChangeMainImage`, myForm)
  }

  addProductAttributeValue(payload: IAddProductAttribute): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post(`Products/AddProductAttributeValue`, payload)
  }
  addProductVariant(payload:ICreateVariantPayload,productId:number): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post(`Products/AddVariants/${productId}/variants`, [payload])
  }
  RemoveProductAttributeValue(payload: { productId: number, attributeId: number }): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post(`Products/RemoveProductAttributeValue`, payload)
  }
  setSimplePrice(payload:ISimplePricePayload,productId:number): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post(`Products/SetPricing/${productId}/pricing`, payload)
  }

}

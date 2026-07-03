import type {AxiosInstance, AxiosResponse} from 'axios'
import type {IApiResult} from '@/models/IApiResult'
import {IGlobalGridRequest} from "@/models/IGlobalGridRequest";


export enum ProductAttributesType {
  Text = 1,
  Number,
  Decimal,
  Boolean,
  Color,
  Size,
  Date,
  Time,
  DateTime,
  Image,
  File,
  Dropdown,
  MultiSelect,
  Tags,
  Range,
  RichText,
  Email,
  PhoneNumber,
  Url,
  VideoUrl,
  Json,
  Location,
  Barcode,
  Rating,
  Currency,
  Percentage,
  TimeDuration,
  CustomObject
}

export const productAttributeTypes = [
  {value: "Text", label: "متن ساده"},
  {value: "Number", label: "عدد صحیح"},
  {value: "Decimal", label: "عدد اعشاری"},
  {value: "Boolean", label: "درست یا نادرست"},
  {value: "Color", label: "رنگ"},
  {value: "Size", label: "سایز"},
  {value: "Date", label: "تاریخ"},
  {value: "Time", label: "زمان"},
  {value: "DateTime", label: "تاریخ و زمان"},
  {value: "Image", label: "تصویر"},
  {value: "File", label: "فایل"},
  {value: "Dropdown", label: "فهرست کشویی"},
  {value: "MultiSelect", label: "انتخاب چندتایی"},
  {value: "Tags", label: "برچسب‌ها"},
  {value: "Range", label: "بازه عددی"},
  {value: "RichText", label: "متن با فرمت"},
  {value: "Email", label: "ایمیل"},
  {value: "PhoneNumber", label: "شماره تلفن"},
  {value: "Url", label: "آدرس اینترنتی"},
  {value: "VideoUrl", label: "لینک ویدیو"},
  {value: "Json", label: "داده ساخت‌یافته (JSON)"},
  {value: "Location", label: "موقعیت مکانی"},
  {value: "Barcode", label: "بارکد یا کد کالا"},
  {value: "Rating", label: "امتیاز عددی"},
  {value: "Currency", label: "مقدار پولی"},
  {value: "Percentage", label: "درصد"},
  {value: "TimeDuration", label: "مدت زمان"},
  {value: "CustomObject", label: "شیء دلخواه"}
];
export const productAttributeTypesShow = {
  Text: "متن ساده",
  Number: "عدد صحیح",
  Decimal: "عدد اعشاری",
  Boolean: "درست یا نادرست",
  Color: "رنگ",
  Size: "سایز",
  Date: "تاریخ",
  Time: "زمان",
  DateTime: "تاریخ و زمان",
  Image: "تصویر",
  File: "فایل",
  Dropdown: "فهرست کشویی",
  MultiSelect: "انتخاب چندتایی",
  Tags: "برچسب‌ها",
  Range: "بازه عددی",
  RichText: "متن با فرمت",
  Email: "ایمیل",
  PhoneNumber: "شماره تلفن",
  Url: "آدرس اینترنتی",
  VideoUrl: "لینک ویدیو",
  Json: "داده ساخت‌یافته (JSON)",
  Location: "موقعیت مکانی",
  Barcode: "بارکد یا کد کالا",
  Rating: "امتیاز عددی",
  Currency: "مقدار پولی",
  Percentage: "درصد",
  TimeDuration: "مدت زمان",
  CustomObject: "شیء دلخواه"
};
export interface IAttribute {
  name: string,
  description: string,
  type: keyof ProductAttributesType,
  id: number
}

export interface ICreateAttributePayload extends Omit<IAttribute, 'id'> {

}

export interface IUpdateAttributePayload extends Omit<IAttribute, 'id'> {

}

export default class ProductAttributes {
  constructor(private axiosInstance: AxiosInstance) {
  }

  getAllProductAttributes(params: IGlobalGridRequest): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.get('ProductAttributes/Get', {
      params
    })
  }

  createProductAttribute(payload: ICreateAttributePayload): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post('ProductAttributes/Create', payload)
  }

  updateProductAttribute(payload: IUpdateAttributePayload, attributeId: number): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.put('ProductAttributes/Update', payload, {
      params: {
        id: attributeId,
      }
    })
  }
  deleteProductAttribute( attributeId: number): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.delete(`ProductAttributes/Delete/${attributeId}`)
  }
}

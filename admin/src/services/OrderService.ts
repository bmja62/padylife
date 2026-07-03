import type {AxiosInstance, AxiosResponse} from 'axios/index'
import type {IApiResult} from '@/models/IApiResult'
import type {IGlobalGridResult} from '@/models/IGlobalGridResult'

export enum OrderStatus {
  Pending = 1,
  Processing = 2,
  Shipped = 3,
  Delivered = 4,
  Canceled = 5,
  Verified = 6
}

export const orderStatusPersianMap = {
  'Pending': 'در حال بررسی',
  'Processing': 'در حال آماده سازی',
  'Shipped': 'ارسال شده',
  'Delivered': 'تحویل داده شده',
  'Canceled': 'لغو شده',
}
export const orderStatusBgMap = {
  'Pending': 'secondary',
  'Processing': 'secondary',
  'Shipped': 'success',
  'Delivered': 'success',
  'Canceled': 'error',
}
export const orderStatus = {
  1: {
    title: 'در حال بررسی',
    bgClass: 'primary',
    value: 1

  },
  2: {
    title: 'پرداخت شده',
    bgClass: 'success',
    value: 2

  },
  3: {
    title: 'تحویل به شرکت های پستی',
    bgClass: 'primary',
    value: 3

  },
  4: {
    title: 'تحویل داده شده',
    bgClass: 'success',
    value: 4

  },
  5: {
    title: 'لغو شده',
    bgClass: 'error',
    value: 5

  },
  6: {
    title: 'آماده سازی و به واحد ارسال',
    bgClass: 'primary',
    value: 6
  },
  7: {
    title: 'در انتظار تاییدیه سامانه دشت',
    bgClass: 'primary',
    value: 7
  }
}

export const orderStatusesPicker = [
  {
    name: 'در حال بررسی',
    value: 'Pending'
  },
  {
    name: 'تحویل داده شده',
    value: 'Delivered'
  },
  {
    name: 'لغو شده',
    value: 'Canceled'
  },

]


export default class OrderService {
  constructor(private axiosInstance: AxiosInstance) {
  }

  getAllOrders(ordersFilters: IOrdersListFilters): Promise<AxiosResponse<IApiResult<IGlobalGridResult<IOrdersListItem[]>>>> {
    return this.axiosInstance.get('/orders', {
      params: ordersFilters,
    })
  }

  changeOrderStatus(payload: {
    orderId: number,
    newStatus: string
  }): Promise<AxiosResponse<IApiResult<IGlobalGridResult<IOrdersListItem[]>>>> {
    return this.axiosInstance.put(`orders/${payload.orderId}/status`, {
      newStatus: payload.newStatus
    })
  }

  getOrderById(orderId: number): Promise<AxiosResponse<IApiResult<IGlobalGridResult<IOrdersListItem[]>>>> {
    return this.axiosInstance.get(`/orders/${orderId}`)
  }
}

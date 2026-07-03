import type {IApiResult} from "~~/types";
import type {BasketItemType} from "~/services/BasketService";

export enum OrderStatus {
    Pending = "Pending",
    Processing = "Processing",
    Shipped = "Shipped",
    Delivered = "Delivered",
    Canceled = "Canceled"
}

export interface IOrderItem {
    objectId: number,
    title: string,
    quantity: number,
    unitPrice: number,
    itemType: BasketItemType
}

export interface IOrderUser {
    fullName: string,
    userName: string,
    email: null | string,
    phoneNumber: string
}

export interface IOrder {
    orderId: number,
    orderDate: string | Date,
    status: OrderStatus,
    totalPrice: number,
    userId: number,
    userInfo: IOrderUser
    id: number,
    items: IOrderItem[],
    address: "-"
}

export const statusMap = {
    Pending: {label: 'در انتظار', color: 'bg-yellow-100 text-yellow-700'},
    Processing: {label: 'در حال پردازش', color: 'bg-blue-100 text-blue-700'},
    Shipped: {label: 'ارسال شده', color: 'bg-indigo-100 text-indigo-700'},
    Delivered: {label: 'تحویل داده‌شده', color: 'bg-green-100 text-green-700'},
    Canceled: {label: 'لغو شده', color: 'bg-gray-200 text-gray-600'},
}
export const persianStatusMap = {
    'در انتظار': {label: 'در انتظار', color: 'bg-yellow-100 text-yellow-700'},
    'در حال پردازش': {label: 'در حال پردازش', color: 'bg-blue-100 text-blue-700'},
    'بسته بندی و ارسال': {label: 'بسته بندی و ارسال', color: 'bg-indigo-100 text-indigo-700'},
    'ارسال شده': {label: 'ارسال ‌شده', color: 'bg-green-100 text-green-700'},
    'کنسل شده': {label: 'کنسل شده', color: 'bg-gray-200 text-gray-600'}
}


export interface IGetOrdersParams {
    page: number,
    pageSize: number,
    userId: number,
    status?: OrderStatus | null,
    orderId?: null | number
}

export default class OrderService {
    constructor(private httpClient: LocalFetch) {
    }

    getAllOrders(params: IGetOrdersParams): Promise<IApiResult> {
        return this.httpClient(`orders`, {
            method: "GET",
            params,
        });
    }

    getOrderById(orderId: number): Promise<IApiResult> {
        return this.httpClient(`orders/${orderId}`, {
            method: "GET"
        });
    }

    getOrderInvoiceById(orderId: number): Promise<IApiResult> {
        return this.httpClient(`orders/${orderId}/invoice`, {
            method: "GET"
        });
    }
}

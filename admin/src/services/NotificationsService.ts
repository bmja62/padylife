import type {AxiosInstance, AxiosResponse} from 'axios'
import type {IApiResult} from '@/models/IApiResult'
import type {IGlobalGridResult} from '@/models/IGlobalGridResult'
import {IGlobalGridRequest} from "@/models/IGlobalGridRequest";

export enum NotificationType {
  SystemNotification = 0,
  SMS = 1,
  Email = 2,
}

export const notificationTypeMap = {
  'SystemNotification': 'سیستمی',
  'SMS': 'پیامک',
  "Email": 'ایمیل',
}
export interface INotificationPayload {
  senderId: number,
  subject: string,
  description: string,
  allusers: true,
  reciverIds: number[],
  notificationType: NotificationType,
  isFromSystem: boolean
}

export interface INotification {
  id: number,
  subject: string,
  description: string,
  notificationTypes: number,
  createdAt: Date
}

export interface IGetNotificationParams extends IGlobalGridRequest {
  userId: number
}
export default class NotificationsService {
  constructor(private axiosInstance: AxiosInstance) {
  }

  sendNotification(payload:INotificationPayload): Promise<AxiosResponse<IApiResult<IGlobalGridResult>>> {
    return this.axiosInstance.post(`Notifications/SendNotification`,payload)
  }

  getAllNotifications(filters: IGetNotificationParams): Promise<AxiosResponse<IApiResult<IGlobalGridResult>>> {
    return this.axiosInstance.get(`Notifications/GetSystemNotificationByUserId`, {
      params: filters
    })
  }



}

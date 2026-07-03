import type {IApiResult} from '@/models/IApiResult'
import type {IGlobalGridRequest} from "~~/types";

export interface IGetNotificationsParams extends Omit<IGlobalGridRequest, 'search'> {
    userId: number
}

export interface IUserDataEvent {
    userId: null,
    fullName: string,
    email: null,
    phoneNumber: string,
    unReadNotificationCount: null
}

export interface IUserNotificationEvent {
    id: number,
    subject: string,
    description: string,
    notificationTypes: number,
    createdAt: Date
}

export interface INotificationsGet {
    readNotification: []
    readTotalCount: null
    unReadNotification: IUserNotificationEvent[]
    unReadTotalCount: null
}

export interface IMarkAsReadNotificationEvent {
    userId: number,
    notificationId: number
}

export default class NotificationService {
    constructor(private httpClient: LocalFetch) {
    }

    getAllNotificationsForUI(filters: IGetNotificationsParams): Promise<IApiResult> {
        return this.httpClient("Notifications/GetUserSystemNotificationsForUI", {method: "GET", params: filters});

    }

    markAsRead(payload: IMarkAsReadNotificationEvent): Promise<IApiResult> {
        return this.httpClient("Notifications/MarkAsRead", {method: "POST", body: payload});

    }


}

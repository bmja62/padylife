import type {IApiResult} from "~~/types";

export interface ITicketDetail {
    id: number,
    content: string,
    supportUserId: number,
    responseType: string,
    createdAt: Date
}


export interface ITicketReplyPayload {
    id: number,
    content: string
}

export interface ITicketCreatePayload {
    title: string,
    content: string,
    ticketType: TicketType
}

export enum TicketStatus {
    WaitingForSupport = 'WaitingForSupport',
    WaitingForUser = 'WaitingForUser',
    Closed = 'Closed',
}

export enum TicketStatusesColor {
    WaitingForSupport = 'bg-primary',
    WaitingForUser = 'bg-secondary',
    Closed = 'bg-red-500',
}

export enum TicketStatusesPersian {
    WaitingForSupport = 'در انتظار پاسخ پشتیبانی',
    WaitingForUser = 'در انتظار پاسخ مشتری',
    Closed = 'بسته شده',
}

export interface ITicketItem {
    id: number,
    userId: number,
    status: TicketStatus,
    title: string,
    createdAt: Date,
    ticketDetails: ITicketDetail[],
    updatedAt: Date,
    ticketType: TicketType
}


export enum TicketTypesPersian {
    Expert = 'کارشناس',
    Financial = 'مالی',
    Suggestion = 'پیشنهادات',
    Other = 'سایر',
}

export enum TicketType {
    Expert = 'Expert',
    NutritionSpecialist = "NutritionSpecialist",
    Financial = "Financial",
    Suggestion = "Suggestion",
    Other = "Other",
}

export default class TicketService {
    constructor(private httpClient: LocalFetch) {
    }

    getMyTickets(params: IGlobalGridRequest): Promise<IApiResult> {
        return this.httpClient(`Tickets/GetMyTickets/GetMyTickets`, {method: "GET", params});
    }

    getTicketDetail(ticketId: number): Promise<IApiResult> {
        return this.httpClient(`Tickets/Get/${ticketId}`, {method: "GET"});
    }

    reply(payload: ITicketReplyPayload): Promise<IApiResult> {
        return this.httpClient(`Tickets/Answer/Answer`, {method: "POST", body: payload});
    }

    create(payload: ITicketCreatePayload): Promise<IApiResult> {
        return this.httpClient(`Tickets/Create`, {
            method: "POST", body: payload
        });
    }
}

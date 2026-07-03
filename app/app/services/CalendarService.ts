import type {IApiResult} from '@/models/IApiResult'


export interface IGetCalendarParams {
    shamsiYear: number
    shamsiMonth: number
}

export enum ECalendarEvent {
    Personal = 1,
    Work = 2,
    Meeting = 3,
    Reminder = 4,
    Other = 5
}

export interface ICreateEventPayload {
    date: Date,
    title: string,
    description: string,
    type: ECalendarEvent
}

export interface ICalendarDayEvent {
    id: number,
    title: string,
    type: ECalendarEvent,
    description: string
}

export interface ICalendarDay {
    shamsiDay: number,
    dayId:number,
    shamsiMonth: number,
    miladiDay: number,
    miladiMonth: number,
    weekday: number,
    weekOfMonth: number,
    isHoliday: boolean,
    occasions: string[],
    date: string,
    events: ICalendarDayEvent[],
    "isInMonth": true
}

export interface ICalendar {
    shamsiYear: number,
    shamsiMonth: number,
    days: ICalendarDay[],
    monthOccasions: string[]
}

export default class CalendarService {
    constructor(private httpClient: LocalFetch) {
    }

    getCalendar(filters: IGetCalendarParams): Promise<IApiResult> {
        return this.httpClient("CalendarManager/GetMonth", {method: "GET", params: filters});

    }

    getEventsByDay(date: Date): Promise<IApiResult> {
        return this.httpClient("CalendarManager/GetEventsByDay", {
            method: "GET", params: {
                date
            }
        });
    }

    createEvent(payload: ICreateEventPayload): Promise<IApiResult> {
        return this.httpClient("CalendarManager/CreateEvent", {
            method: "POST",
            body: payload
        });
    }

    updateEvent(payload: ICalendarDayEvent): Promise<IApiResult> {
        return this.httpClient("CalendarManager/UpdateEvent", {
            method: "PUT",
            body:payload
        });
    }

    deleteEvent(id: number): Promise<IApiResult> {
        return this.httpClient("CalendarManager/DeleteEvent", {
            method: "DELETE", params: {
                id
            }
        });
    }
}

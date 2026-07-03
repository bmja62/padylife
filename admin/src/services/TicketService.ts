import type {AxiosInstance, AxiosResponse} from 'axios'
import type {IApiResult} from '@/models/IApiResult'
import type {IGlobalGridRequest} from '@/models/IGlobalGridRequest'
import type {IGlobalGridResult} from '@/models/IGlobalGridResult'
import type {TicketResponseTypes, TicketStatuses, TicketTypes,} from '@/models/Enums/TicketTypes'

export interface ITicketCreatePayload {
  title: string | null
  content: string | null
  ticketType: TicketTypes | null
}

export interface ITicketsFilter extends IGlobalGridRequest {
  type?: TicketTypes | null
  status?: TicketStatuses | null
}
export enum TicketResponseTypes{

}

export interface ITicket {
  id: number
  userId: number
  status: TicketStatuses
  title: string
  createdAt: string | Date
  ticketDetails: ITicketDetails[]
  updatedAt: string | Date
  ticketType: TicketTypes
}

export interface ITicketCount {
  count: number
}

export interface ITicketListItem extends Omit<ITicket, 'ticketDetails'> {
  ticketDetails: null
}

export interface ITicketDetails {
  id: number
  content: string
  supportUserId: number
  responseType: TicketResponseTypes
  createdAt: Date | string
}

export interface ITicketAnswerPayload {
  id: number | string
  content: string
}

export default class TicketService {
  constructor(private axiosInstance: AxiosInstance) {
  }

  getTicketsList(filters: ITicketsFilter): Promise<AxiosResponse<IApiResult<IGlobalGridResult<ITicketListItem[]>>>> {
    return this.axiosInstance.get('Tickets/GetTickets',  {
      params: filters,
    })
  }

  createTicket(
    payload: ITicketCreatePayload,
  ): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post('/Tickets', payload)
  }

  getTicketDetails(
    parentId: number | string,
  ): Promise<AxiosResponse<IApiResult<ITicket>>> {
    return this.axiosInstance.get(`Tickets/Get/${parentId}`)
  }

  sendTicketAnswer(
    payload: ITicketAnswerPayload,
  ): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post('Tickets/Answer/Answer', payload)
  }

  getUnreadTicketCount(): Promise<AxiosResponse<IApiResult<ITicketCount>>> {
    return this.axiosInstance.get('/Tickets/GetNotAnsweredTicketCount')
  }

  closeATicket(ticketId: string): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post(`Tickets/Close/Close/${ticketId}`)
  }
}

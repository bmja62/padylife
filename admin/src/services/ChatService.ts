import type {AxiosInstance, AxiosResponse} from 'axios'
import type {IApiResult} from '@/models/IApiResult'
import {IGlobalGridRequest} from "@/models/IGlobalGridRequest";

export interface IChatUser {
  userId: number,
  userFullName: string,
  profileImage: null | string
}

export interface IChatDetail {
  chatId: number,
  chatRoomName: null | string,
  chatRoomUserPlanTitle: null | string,
  userFullNames: IChatUser[],
  isGroupChatRoom: boolean
}

export interface IChat {
  chat: IChatDetail,
  messageCount: number,
  lastMessage: null | string,
  lastMessageTime: null | Date
}

export interface IMessage {
  messageId: number,
  roomId: number,
  senderId: number,
  encryptedContent: string,
  type: string,
  replyToMessageId: number,
  createdAt: Date,
  senderName: string,
  reactions: string[],
  status: string
}

export interface IGetChatParams extends IGlobalGridRequest {
  roomId: number,
}

export default class ChatService {
  constructor(private axiosInstance: AxiosInstance) {
  }

  getAllUserChats(params: IGlobalGridRequest): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.get('Chat/GetAllChatForAdmin', {
      params
    })
  }

  getUserChatById(params: IGetChatParams): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.get('Chat/GetChatByIdQueryForAdmin', {
      params
    })
  }


}

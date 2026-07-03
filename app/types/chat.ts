export interface IChatInfo {
    chat: IChat,
    userInfo: IChatUserInfo,
    messageCount: number,
    unReadMessageCount: number,
    lastMessage: string
}

export interface IChat {
    chatId: number,
    chatRoomName: null | string,
    chatRoomUserPlanTitle: null | string,
    isGroupChatRoom: boolean,
    isExpert: boolean,
}

export interface IChatUserInfo {
    userId: number,
    userFullName: string,
    profileImage: string,
    isOnline: boolean
}

export enum ChatFilters {
    Companion = 1,
    Specialist = 2,
}

export enum ChatMessageStatus { Sent = 1, Received = 2, Delivered = 3, Read = 4 }

export interface IChatMessage {
    createdAt: Date
    encryptedContent: string
    messageId: number
    reactions: []
    replyToMessageId: null | number
    roomId: number
    senderId: number
    senderName: string
    type: number
    status: ChatMessageStatus

}

export interface IMessageDTO {
    roomId: number,
    encryptedContent: string,
    replyToMessageId: null | number,
    type: number,
    encryptionMetadata: null | string
}

export interface IGenericConversationsList<T> {
    data: T,
    totalCount: number
}
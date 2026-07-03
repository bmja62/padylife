import type {IApiResult} from "~~/types";
import type {CommentType} from "~/models/Enums/CommentType";

export enum ReactTypes {
    Like = 'Like',
    DisLike = 'DisLike'
}
export interface ICreateCommentPayload {
    entityId: number
    entityType: CommentType
    text: string
    parentCommentId: number | null
}

export interface IEntityCommentsListFilters {
    entityId: number
    entityType: CommentTypes,
    search: string,
    pageNumber: number,
    count: number,
    isApproved: boolean,
}

export interface IEntityComment {
    id: number,
    text: string,
    isApproved: boolean,
    isLikedByMe: boolean,
    isMe: boolean,
    isReactedByLoginUser: boolean,
    likeCount: number,
    dislikeCount: number,
    userInfo: {
        id: number,
        fullName: string
    },
    replies: string[]
}

export default class CommentService {
    constructor(private httpClient: LocalFetch) {
    }

    createComment(payload: ICreateCommentPayload): Promise<IApiResult> {
        return this.httpClient("Comments/Create", {method: "POST", body: payload});
    }

    reactToComment(commentId: number, type:ReactTypes): Promise<IApiResult> {
        return this.httpClient("Comments/ReactToComment", {
            method: "POST",
            params: {
                commentId
            },
            body: {
                type
            }
        });
    }

    getEntityComments(filters
                          :
                          IEntityCommentsListFilters
    ):
        Promise<AxiosResponse<IApiResult<IGlobalGridResult>>> {
        return this.httpClient('Comments/GetEntityComments', {
            params: {...filters, isApproved: true}
        })
    }
}

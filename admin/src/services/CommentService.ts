import type {AxiosInstance, AxiosResponse} from 'axios'
import type {IApiResult} from '@/models/IApiResult'
import type {IGlobalGridResult} from '@/models/IGlobalGridResult'
import {IGlobalGridRequest} from "@/models/IGlobalGridRequest";

export enum CommentTypes {
  Product = 'Product',
  Variant = 'Variant',
  Blog = 'Blog',
  Plan = 'Plan',
  Excersie = 'Excersie',
  Step = 'Step',
  Challenge = 'Challenge',
  Specialist = 'Specialist',
}

export interface IEntityCommentsListFilters extends IGlobalGridRequest {
  entityType: CommentTypes,
  isApproved: boolean | null,
}

export interface IEntityComment {
  id: number,
  text: string,
  likeCount: number,
  dislikeCount: number,
  userInfo: {
    id: number,
    fullName: string
  },
  replies: string[]
}

export default class CommentService {
  constructor(private axiosInstance: AxiosInstance) {
  }

  getEntityComments(filters: IEntityCommentsListFilters): Promise<AxiosResponse<IApiResult<IGlobalGridResult>>> {
    return this.axiosInstance.get('Comments/GetEntityComments', {
      params: filters
    })
  }
  getEntityCommentsForAdmin(filters: IEntityCommentsListFilters): Promise<AxiosResponse<IApiResult<IGlobalGridResult>>> {
    return this.axiosInstance.get('Comments/GetEntityCommentsForAdmin', {
      params: filters
    })
  }
  approve(commentId: number): Promise<AxiosResponse<IApiResult<IGlobalGridResult>>> {
    return this.axiosInstance.put(`Comments/Approve/${commentId}`)
  }

  delete(commentId: number): Promise<AxiosResponse<IApiResult<IGlobalGridResult>>> {
    return this.axiosInstance.delete(`Comments/Delete/${commentId}`)
  }

}

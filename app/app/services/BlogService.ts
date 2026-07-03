import type {IApiResult} from '@/models/IApiResult'
import type {IGlobalGridRequest} from "~~/types";

export enum BlogStatus {
    Publish = 'Publish',
    PreRelease = 'PreRelease',
}

export interface IBlogLite {
    id: number
    image: string
    seoURL: string
    shortDescription: string
    title: string
}
export interface IBlog {
    id: number,
    blogCategoryId: number,
    blogCategoryTitle: string,
    title: string,
    seoURL: string,
    content: string,
    shortDescription?: string,
    mainImageUrl?: string,
    createdAt: Date,
    status: BlogStatus
}

export interface IBlogDetail extends IBlog {
    author: {fullName: string, profilePictureUrl: null|string}
    canonicalLink: string
    mainId: null
    metaAuthor: string
    metaContent: null | string
    metaKeywords: string
    metaTags:string
    metas: null | object[]
    ogMainPicUrl: null | string
    ogTitle: string
    ogurl: string
    relatedBlogs: []
    scriptContent: null
    seoDescription: string
    seoTitle: string
    spendTimeForRead: string
    tableOfContent: string
    videoThumbnailImageUrl: null | string
}

export interface IGetBlogsParams extends IGlobalGridRequest {
    searchByTitle: string
    blogCategoryId?: number
}
export default class BlogService {
    constructor(private httpClient: LocalFetch) {
    }

    getAllBlogs(filters: IGetBlogsParams): Promise<IApiResult> {
        return this.httpClient("Blog/GetAllForWeb", {method: "GET", params: filters});

    }

    getBlogByseourl(seoURL: string): Promise<IApiResult> {
        return this.httpClient("Blog/GetBySeoURL", {
            method: "GET", params: {
                seoURL
            }
        });

    }


}

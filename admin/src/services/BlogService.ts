import type {AxiosInstance, AxiosResponse} from 'axios'
import type {IApiResult} from '@/models/IApiResult'
import {IGlobalGridRequest} from "@/models/IGlobalGridRequest";

export interface IBlogListFilters {
  pageNumber: number,
  count: number,
  searchByTitle: string
}

export interface ICreateOrUpdateBlogDTO {
  title: string,
  spendTimeForRead: string,
  type: string,
  seoURL: string,
  seoTitle: string,
  seoDescription: string,
  tableOfContent: string,
  content: string,
  shortDescription: string,
  metaKeywords: string,
  metaAuthor: string,
  ogTitle: string,
  ogMainPicUrl: string,
  canonicalLink: string,
  ogurl: string,
  status: string,
  mainImageFile: string
  blogCategoryId: null
}

export interface IBlogCategory {
  title: string,
  description: string,
  imageUrl: string,
  id: number
}

export interface ICreateBlogCategoryPayload extends Omit<IBlogCategory, 'id'> {
}

export interface IUpdateBlogCategoryPayload extends IBlogCategory {
}

export default class BlogService {
  constructor(private axiosInstance: AxiosInstance) {
  }

  getAllBlogCategories(filters: IGlobalGridRequest): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.get('/BlogCategories/Get', {
      params: filters
    })
  }

  getAllBlog(filters): Promise<AxiosResponse<IStatesListItem[]>> {
    return this.axiosInstance.get('/Blog/GetAll', {
      params: filters
    })
  }

  getBlogById(id: number): Promise<AxiosResponse<IStatesListItem[]>> {
    return this.axiosInstance.get(`/Blog/GetBy/GetBy/${id}`)
  }

  createBlogCategories(payload: ICreateBlogCategoryPayload): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post('/BlogCategories/Create', payload)
  }

  updateBlogCategories(payload: IUpdateBlogCategoryPayload): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.put('/BlogCategories/Update', payload, {
      params: {
        id: payload.id
      }
    })
  }

  deleteBlogCategories(id: string | number): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.delete(`BlogCategories/Delete/${id}`)
  }
  createBlog(payload): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post('/Blog/Create', payload)
  }


  updateBlog(payload): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.put('/Blog/Update', payload)
  }

  updateBlogSEO(payload): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post('/Blog/UpdateBlogSEO', payload)
  }

  updateBlogCategoriesSEO(payload): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post('/BlogCategories/UpdateBlogCategoriesSEO', payload)
  }


  deleteBlog(id: string | number): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.delete(`/Blog/Delete`,  {
      params: {
         id,
      }
    })
  }
}

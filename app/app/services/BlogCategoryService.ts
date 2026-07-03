import type {IApiResult} from '@/models/IApiResult'
import type {IGlobalGridRequest} from "~~/types";

export interface IBlogCategory {
    title: string,
    description: string,
    imageUrl: string,
    id: number
}

export default class BlogCategoryService {
    constructor(private httpClient: LocalFetch) {
    }

    getAllBlogCategories(filters: IGlobalGridRequest): Promise<IApiResult> {
        return this.httpClient("BlogCategories/Get", {method: "GET", params: filters});

    }


}

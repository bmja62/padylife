import type {IApiResult} from "~~/types";

export interface IProductCategory {
    name: string,
    description: string,
    parentCategoryId?: number | null,
    parentName: string,
    childCategories: string[],
    productCategoryAttributes: string[],
    imageUrl: string,
    id: number
}


export default class ProductCategoryService {
    constructor(private httpClient: LocalFetch) {
    }


    getAllProductCategories(params: IGlobalGridRequest): Promise<AxiosResponse<IApiResult>> {
        return this.httpClient('ProductCategories/Get', {
            method: "GET",
            params
        })
    }

    getProductCategoryById(productCategoryId: number): Promise<AxiosResponse<IApiResult>> {
        return this.httpClient(`ProductCategories/Get/${productCategoryId}`, {
            method: "GET",})
    }
}

import type {IApiResult} from "~~/types";
import type {UploaderTypes} from "~/models/Enums/UplaoderTypes";

export interface IGetProductsParams extends IGlobalGridRequest {
    categoryId?: number | null
    searchTerm?: number | null
}

export enum ProductType {
    Simple = 'Simple',
    Variant = 'Variant',
}
export enum ProductPersianType{
    Simple = 'ساده',
    Variant = 'متغییر',
}

export interface IProductBasic {
    id: number,
    name: string,
    description: string,
    price: number,
    stockQuantity: number,
    categoryId: number,
    categoryName: string,
}

export interface IProduct extends IProductBasic {
    productImages: IProductImages,
    attributes: IProductAttribute[],
    basketQuantity:number,
    variants: IProductVariant[],
    type: ProductType,
    userInfo: IProductUserInfo
}

export interface IProductImages {
    main: IProductImage,
    gallery: IProductImage[]
}

export interface IProductImage {
    id: number,
    url: string,
    type: UploaderTypes,
    typeName: keyof UploaderTypes,
    objectId: number
}


export interface IProductAttribute {
    attributeId: number,
    attributeName: string,
    value: string
}

export type IProductAttributeLite = Omit<IProductAttribute, 'attributeName'>

export interface IAddProductAttribute {
    productId: number,
    attributeId: number,
    value: string
}

export interface IProductVariant {
    id: number,
    basketQuantity:number
    sku: string,
    price: number,
    stockQuantity: number,
    productVariantImages: {
        main: IProductImage,
        gallery: IProductImage[]
    },
    attributeValues: IProductAttribute[]
}

export interface IProductVariantLite extends Omit<IProductVariant, 'id' | 'attributeValues'> {
    attributeValues: IProductAttributeLite[]
}

export interface IProductUserInfo {
    userId: number,
    userName: string,
    fullName: string
}

export interface IGroupedVariant {
    attributeId: number,
    attributeName: string,
    value: string,
    variantId: number
}

export interface IVariantGroup {
    keys: string[],
    attributeId: number,
    name: string,
    variants: IGroupedVariant[]
}

export interface IInventoryStock {
    data: IWarehouseStock[],
    totalCount: number,
    summery: IWarehouseStockSummary
}

export interface IWarehouseStockSummary {
    totalQuantity: number,
    totalReserved: number,
    totalAvailable: number,
    warehouseCount: number,
    zoneCount: number
}

export interface IWarehouseStock {
    warehouseId: number,
    warehouseName: string,
    zoneId: number,
    zoneName: string,
    quantity: number,
    reservedQuantity: number,
    availableQuantity: number,
    minimumStock: number,
    reorderPoint: number,
    lastStockUpdate: Date,
    productName: string,
    variantName: null | string
}

export default class ProductService {
    constructor(private httpClient: LocalFetch) {
    }

    getAllProduct(payload: IGetProductsParams): Promise<IApiResult> {
        return this.httpClient("Products/GetAll", {method: "GET", params: payload});
    }

    getProductById(productId: number): Promise<IApiResult> {
        return this.httpClient(`Products/GetById/${productId}`, {method: "GET"});
    }

    getProductOrVariantStock(productId: number,variantId?:number) {
        return this.httpClient(`Warehouse/GetProductInventory/inventory/${productId}`, {
            method: "GET", params: {
                variantId
            }
        });

    }
}

import type {IApiResult} from "~~/types";

export enum BasketItemType {
    Product = 'Product',
    Variant = "Variant",
    Plan = "Plan",
    Service = "Service",
    ExpertPlanPrice = "ExpertPlanPrice",
}

export enum BasketStatus {

}

export interface IAddBasketItemPayload {
    objectId: number,
    basketItemType: BasketItemType,
    quantity: number
}

export interface IUpdateBasketItemPayload extends Omit<IAddBasketItemPayload, 'quantity' | 'objectId'> {
    itemId: number
    newQuantity: number
}

export interface IBasketItem {
    id: number
    itemType: BasketItemType
    objectId: number
    quantity: number
    unitPrice: number
}
export interface IBasket {
    id: number,
    userId: number,
    items: IBasketItem[],
    productTotalPrice: number,
    discountAmount: number,
    shippingCost: number,
    finalPrice: number,
    status: BasketStatus,
    createdAt: Date,
    lastUpdated: Date
}

export interface IBasketItemDetail {
    id: number,
    title: string,
    type: string,
    unitPrice: number,
    quantity: number,
    totalPrice: number,
    brand: string,
    variantAttributes: string,
    imageUrl: {
        main: null | string,
        gallery: string[]
    }
}

export interface IBasketDetailPayload {
    items: {
        objectId: number,
        itemType: BasketItemType,
        quantity: number
    }[]
}

export default class BasketService {
    constructor(private httpClient: LocalFetch) {
    }

    addBasketItem(payload: IAddBasketItemPayload, userId?: number): Promise<IApiResult> {
        const authStore = useAuthStore()
        return this.httpClient(`Basket/AddItem/${userId ? userId : authStore.getUser.id}/items`, {
            method: "POST",
            body: payload,
        });
    }

    removeBasketItem(payload: {
        itemId: number,
        basketItemType: BasketItemType
    }, userId?: number): Promise<IApiResult> {
        const authStore = useAuthStore()
        return this.httpClient(`Basket/RemoveItem/${userId ? userId : authStore.getUser.id}/items/${payload.itemId}`, {
            method: "DELETE",
            body: payload,
        });
    }

    updateItemQuantity(payload: IUpdateBasketItemPayload, userId?: number): Promise<IApiResult> {
        const authStore = useAuthStore()
        return this.httpClient(`Basket/UpdateItemQuantity/${userId ? userId : authStore.getUser.id}/items/${payload.itemId}`, {
            method: "PUT",
            body: payload,
        });
    }

    getUserBasket(userId?: number): Promise<IApiResult> {
        const authStore = useAuthStore()
        return this.httpClient(`Basket/GetOrCreateBasket/${userId ? userId : authStore.getUser.id}`, {
            method: "GET",
        });
    }

    getUserBasketItems(payload?: IBasketDetailPayload): Promise<IApiResult> {
        return this.httpClient(`Basket/GetBasketItemDetailsAsProduct/basket/details`, {
            method: "POST",
            body: payload,
        });
    }
    basketToOrder(discountCode?: string | ''): Promise<IApiResult> {
        return this.httpClient(`Basket/BasketToOrder`, {
            method: "POST",
            body: {
                discountCode
            }
        });
    }
    createPaymentLink(orderId: string | ''): Promise<IApiResult> {
        return this.httpClient(`Payments/GetPaymentLink/GetPaymentLink`, {
            method: "GET",
            params: {
                orderId
            }
        });
    }
    payByWallet(orderId: string | ''): Promise<IApiResult> {
        return this.httpClient(`Payments/PayWithWallet/PayWithWallet`, {
            method: "POST",
            params: {
                orderId
            }
        });
    }

}

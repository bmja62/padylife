import type {IApiResult} from "~~/types";

export interface IWalletDetail {
    id: number,
    userId: number,
    user: {
        id: number,
        fullName: string,
        phoneNumber: string
    },
    credit: number,
    createdAt: Date,
    updatedAt: null | Date
}

export default class WalletService {
    constructor(private httpClient: LocalFetch) {
    }

    getMyWallet(): Promise<IApiResult> {
        return this.httpClient(`Wallets/GetMyWallet`, {
            method: "GET",
        });
    }

    chargeWallet(amount: string): Promise<IApiResult> {
        return this.httpClient(`Wallets/ChargeMyWallet`, {
            method: "POST",
            params: {
                amount
            }
        });
    }

}

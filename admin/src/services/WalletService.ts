import type {AxiosInstance, AxiosResponse} from 'axios'
import type {IApiResult} from '@/models/IApiResult'
import {IGlobalGridRequest} from "@/models/IGlobalGridRequest";

export interface IGetWalletsParams extends IGlobalGridRequest {
  userFullName: string,
  roleName : string
}

export enum WalletOperation {
  Deposit = 'Deposit',
  Withdraw = 'Withdraw',
}

export interface IWallet {
  id: number,
  userId: number,
  user: {
    id: number,
    fullName: string,
    phoneNumber: string
  },
  credit: number,
  createdAt: Date,
  updatedAt: Date
}

export interface IWalletTransaction {
  id: number,
  walletId: number,
  amount: number,
  description: string,
  createdAt: Date,
  createdByUser: {
    id: number,
    fullName: string,
    phoneNumber: string
  },
  operation: WalletOperation
}

export interface ICreateTransactionPayload {
  walletId: number,
  description: string,
  credit: number
}

export default class WalletService {
  constructor(private axiosInstance: AxiosInstance) {
  }

  getAllWallets(filters: IGetWalletsParams): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.get('Wallets/GetWallets', {
      params: filters
    })
  }

  getWalletTransactionsById(filters: IGlobalGridRequest, walletId: number): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.get(`Wallets/GetWalletTransactions/${walletId}`, {
      params: filters
    })
  }

  getWalletById(walletId: number): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.get(`Wallets/GetWalletById/${walletId}`)
  }

  withdraw(payload: ICreateTransactionPayload): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post(`Wallets/Withdraw`, payload)
  }

  deposit(payload: ICreateTransactionPayload): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post(`Wallets/Deposit`, payload)
  }
}

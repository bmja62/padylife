import type {AxiosInstance, AxiosResponse} from 'axios'
import type {IApiResult} from '@/models/IApiResult'

export interface IIncreaseOrDecreaseStockPayload {
  productId: number | null,
  variantId: number | null,
  warehouseId: number | null,
  quantity: number | null,
  reason: string


}

export interface ITransferStockPayload extends IIncreaseOrDecreaseStockPayload {
  toWarehouseId: number | null,
  fromWarehouseId: number | null,
}

export interface IReserveOrReleaseStockPayload extends Omit<IIncreaseOrDecreaseStockPayload, 'reason'> {
  referenceId: string
}

export default class InventoryService {
  constructor(private axiosInstance: AxiosInstance) {
  }

  increaseStock(payload: IIncreaseOrDecreaseStockPayload): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post('Inventory/IncreaseStock', payload)
  }

  decreaseStock(payload: IIncreaseOrDecreaseStockPayload): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post('Inventory/DecreaseStock', payload)
  }

  transferStock(payload: ITransferStockPayload): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post('Inventory/TransferStock', payload)
  }

  releaseStock(payload: IReserveOrReleaseStockPayload): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post('Inventory/ReleaseStock', payload)
  }

  reserveStock(payload: IReserveOrReleaseStockPayload): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post('Inventory/ReserveStock', payload)
  }
}

import type {AxiosInstance, AxiosResponse} from 'axios'
import type {IApiResult} from '@/models/IApiResult'
import {IGlobalGridRequest} from "@/models/IGlobalGridRequest";


export interface IWarehouse {
  id?:number,
  isActive: boolean,
  name: string,
  code: string,
  address: string,
  contactPhone: string,
  managerName: string,
  latitude: number,
  longitude: number
}

export interface IZone {
  id: number,
  name: string,
  code: string,
  capacity: number,
  currentOccupancy: number
}

export interface IWarehouseDetail extends IWarehouse {
  zones: IZone[]

}
export interface ICreateWarehousePayload {
  name: string,
  code: string,
  address: string,
  contactPhone: string,
  managerName: string,
  latitude: number,
  longitude: number
}

export interface ICreateZonePayload extends Omit<IZone, 'id' | 'currentOccupancy'> {
}

export interface IGetWarehouseParams extends IGlobalGridRequest {
  isActive: boolean
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


export default class WarehouseService {
  constructor(private axiosInstance: AxiosInstance) {
  }

  getWarehouses(params: IGetWarehouseParams): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.get('Warehouse/GetWarehouses', {
      params
    })
  }

  getWarehouseDetail(warehouseId: number): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.get(`Warehouse/GetWarehouse/${warehouseId}`)
  }
  createWarehouse(payload: ICreateWarehousePayload): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post('Warehouse/Create', payload)
  }

  createWarehouseZone(payload: ICreateZonePayload, warehouseId: number): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post(`Warehouse/AddZone/${warehouseId}/zones`, payload)
  }
  removeWarehouseZone(zoneId:number): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post(`Warehouse/RemoveZone/zones`, {
      id: zoneId,
    })
  }
  updateWarehouse(payload: IWarehouse, warehouseId: number): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.put(`Warehouse/Update/${warehouseId}`, payload)
  }

  deactiveWarehouse(warehouseId: number): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.delete(`Warehouse/Deactivate/${warehouseId}`)
  }

  getProductInventory(productId: number, variantId: number): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.get(`Warehouse/GetProductInventory/inventory/${productId}`, {
      params: {
        variantId
      }
    })
  }
}

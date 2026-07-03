import type {AxiosInstance, AxiosResponse} from 'axios'
import type {IApiResult} from '@/models/IApiResult'
import type {IGlobalGridResult} from '@/models/IGlobalGridResult'

export interface IGetPaymentsListParams {


}


export default class PaymentService {
  constructor(private axiosInstance: AxiosInstance) {
  }


  getAllPayments(filters: IGetPaymentsListParams): Promise<AxiosResponse<IApiResult<IGlobalGridResult>>> {
    return this.axiosInstance.get('Payments/GetPayments', {
      params: filters
    })
  }

}

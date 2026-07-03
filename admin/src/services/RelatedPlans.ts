import type {AxiosInstance, AxiosResponse} from 'axios'
import type {IApiResult} from '@/models/IApiResult'


export interface IRelatedPlan {
  sourcePlanId: null | number,
  targetPlanId: null | number,
  order: null | number
}

export interface IRelatedPlanDetail {
  planId: number,
  title: string,
  nextPlans: IRelatedPlan[],
  previousPlans: IRelatedPlan[]
}

export interface IDeleteRelatedPlan extends Omit<IRelatedPlan, 'order'> {
}

export default class RelatedPlansService {
  constructor(private axiosInstance: AxiosInstance) {
  }

  getPlanRelatedPlans(planId: number): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.get('Plans/GetPlanRelations', {
      params: {
        planId
      }
    })
  }

  createPlanRelation(payload: IRelatedPlan): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post('Plans/CreatePlanRelation', payload)
  }

  updatePlanRelation(payload: IRelatedPlan): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.put('Plans/UpdatePlanRelation', payload)
  }

  deletePlanRelation(payload: IDeleteRelatedPlan): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.delete(`Plans/DeletePlanRelation`, {
      data: payload
    })
  }
}

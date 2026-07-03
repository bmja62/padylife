import type {AxiosInstance, AxiosResponse} from 'axios'
import type {IApiResult} from '@/models/IApiResult'
import type {IGlobalGridResult} from '@/models/IGlobalGridResult'
import {IGlobalGridRequest} from "@/models/IGlobalGridRequest";

export enum PlanStatus {
  Draft = 1,
  Active = 2,
  Completed = 3
}

export const planStatusesForShow = {
  Draft: 'پیش نویس',
  Active: 'فعال',
  Completed: 'انجام شده',
}

export const planStatusesForPicker = [
  {
    name: 'پیش نویس',
    value: 'Draft'
  },
  {
    name: 'فعال',
    value: 'Active'
  },
  // {
  //   name: 'انجام شده',
  //   value: 'Completed'
  // },
]

export interface IPlanCategoryListFilters {
  pageNumber: number,
  count: number,
  search: string,
}
export interface IPlansListFilters {
  pageNumber: number,
  count: number,
  search: string,
}
export interface IUserPlansListFilters {
  pageNumber: number,
  count: number,
  search?: string,
  userId: number,
}
export interface ICreatePlan {
  title: string,
  planCategoryId: number | null,
  description: string
  imageUrl: string
  level: string
  price: string
}

export interface IPricePayload {
  expertId: number,
  planId: number,
  price: number,
}

export interface IActivityPayload {
  planId: string | string[],
  expertId: number|null,
  isActive?:boolean|null
}

export interface IPrice {
  expertId: number,
  expertFullName: string,
  planId: number,
  planTitle: string,
  price: number,
  isActive: boolean
}

export interface IPlan {
  id: number | null
  title: string,
  planCategoryId: number | null,
  description: string,
  status: PlanStatus
  isSignUpPlan: boolean
  imageUrl: string
  level: string | null
  planCategoryName: string
  price: string
  planQuestions: null | []

}

export interface IPlanGet extends Omit<IPlan, 'planQuestions'> {
  planCategoryName: string
  planQuestions: IPlanQuestion,
  description: string
}

export interface IUpdatePlanQuestionOption {
  questionOptionId: null | number,
  type: string,
  objectId: null | number
}

export interface ICreateOrUpdateLinkedQuestion {
  planId: number,
  planQuestionId: number,
  questionOptionId: number,
  linkedQuestionId: number,
  linkedExerciseId: number
}

export interface IWeeklyCommitmentReportRequest {
  userId: number
  weeks: number
}

export interface IPlanAnswersRequestParams {
  planId: number
  onlyCompleted?: boolean
  search?: string | null
  pageNumber: number
  count: number
}

export interface IPlanQuestion {
  id: number
  isMain: boolean
  questionId: number
  questionOptions: IPlanQuestionOption[]
  readOnlyQuestionOptions: IPlanQuestionOption[]
  questionText: string
}

export interface IPlanQuestionOption {
  hasValidLinks: boolean
  id: number,
  linkedQuestion: null,
  linkedExercise: null,
  linkedExerciseId?: null | number
  linkedQuestionId?: null | number
  questionId: number
  text: string
}

export interface IGetUserPlansParams {
  pageNumber: number,
  count: number,
  userId: number,
}


export interface IReadOnlyQuestionOption extends Omit<IPlanQuestionOption, 'linkedQuestionId' | 'linkedExerciseId' | 'questionId' | 'hasValidLinks'> {

}
export interface IAddPlanQuestionPayload {
  planId: null | number,
  questionId: null | number,
  isMainQuestion: boolean
}

export interface IPlanPricesListFilters extends IGlobalGridRequest {
  planId?: number | null
}

export interface INestedPlanQuestion {
  questionId: number,
  isMainQuestion: boolean
}

export interface IAddPlanQuestionToPlanPayload {
  planId: number,
  nestedPlanQuestions: INestedPlanQuestion[]
}

export interface ICreatePlanCategory {
  name: string
}

export interface IChangePlanStatus {
  id: number,
  status: PlanStatus
}
export default class PlanService {
  constructor(private axiosInstance: AxiosInstance) {
  }


  getAllPlanCategories(filters: IPlanCategoryListFilters): Promise<AxiosResponse<IApiResult<IGlobalGridResult>>> {
    return this.axiosInstance.get('PlanCategories/Get', {
      params: filters
    })
  }
  getAllPlans(filters:IPlansListFilters): Promise<AxiosResponse<IApiResult<IGlobalGridResult>>> {
    return this.axiosInstance.post('Plans/GetAll', null, {
      params: filters
    })
  }

  getUserPlans(filters: IGetUserPlansParams): Promise<AxiosResponse<IApiResult<IGlobalGridResult>>> {
    return this.axiosInstance.get('Plans/GetUserPlans', {
      params: filters,
    })
  }

  toggleSignUpPlan(planId: number): Promise<AxiosResponse<IApiResult<IGlobalGridResult>>> {
    return this.axiosInstance.post('Plans/ToogleIsSginUpPlan', null, {
      params: {
        planId
      }
    })
  }
  getPlanById(planId: number): Promise<AxiosResponse<IApiResult<IGlobalGridResult>>> {
    return this.axiosInstance.post('Plans/Get', null, {
      params: {
        id: planId
      }
    })
  }

  getSpecialistPriceForPlan(params: {
    expertId: number,
    id: number
  }): Promise<AxiosResponse<IApiResult<IGlobalGridResult>>> {
    return this.axiosInstance.get(`Plans/GetPlanPriceForUI`, {
      params: {
        expertId:params.expertId,
        planId: params.id
      }
    })
  }

  getPlanPrices(params: IPlanPricesListFilters): Promise<AxiosResponse<IApiResult<IGlobalGridResult>>> {
    return this.axiosInstance.get(`Plans/GetAllPlanPrices`, {
      params
    })
  }

  setExpertPlanPrice(payload: IPricePayload): Promise<AxiosResponse<IApiResult<IGlobalGridResult>>> {
    return this.axiosInstance.post('Plans/SetPlanPrice', payload)
  }

  setExpertPlanActivity(payload: IActivityPayload): Promise<AxiosResponse<IApiResult<IGlobalGridResult>>> {
    return this.axiosInstance.post('Plans/DiactiveExpertPlanByExpert', payload)
  }

  getPlanQuestions(filters:object): Promise<AxiosResponse<IApiResult<IGlobalGridResult>>> {
    return this.axiosInstance.post('Plans/GetPlanQuestionsByPlanId', null, {
      params: filters
    })
  }
  createPlanCategory(payload: ICreatePlanCategory): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post('PlanCategories/Create', payload)
  }

  changePlanStatus(payload: IChangePlanStatus): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post('Plans/ChangePlanStatus',null,{
      params:payload
    })
  }
  createPlan(payload: ICreatePlan): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post('Plans/Create', payload)
  }

  createOrUpdatePlan(payload: IPlan): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post('Plans/CreateOrUpdate', payload)
  }
  deletePlanCategory(planCategoryId: number): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.delete(`PlanCategories/Delete/${planCategoryId}`)
  }
  deletePlan(planId: number): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post(`Plans/Delete`, null, {
      params: {
        id: planId
      }
    })
  }

  updatePlanCategory(payload: object, id: number): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.put('PlanCategories/Update', payload, {
      params: {
        id
      }
    })
  }
  updatePlan(payload: object, id: number): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.put('PlanCategories/Update', payload, {
      params: {
        id
      }
    })
  }

  addPlanQuestion(payload: IAddPlanQuestionPayload): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post('Plans/AddPlanQuestion', payload)
  }

  addPlanQuestionToPlan(payload: IAddPlanQuestionToPlanPayload): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post('Plans/AddPlanQuestionToPlan', payload)
  }

  setPlanDiscount(payload: {
    planId: number,
    discountPrice: number,
    discountPriceStartDate: Date,
    discountPriceEndDate: Date
  }): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post('Plans/SetPlanDiscount', payload)
  }
  getPlanDiscount(id:number): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.get('Plans/GetPlanDiscount',{
      params:{
        id
      }
    })
  }

  linkQuestionToPlan(payload: IUpdatePlanQuestionOption): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post('Questions/UpdateQuestionOption', payload)
  }

  linkExerciseToPlan(payload: object, id: number): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.put('Plans/AddPlanQuestion', payload, {
      params: {
        id
      }
    })
  }

  createOrUpdateLinkedQuestion(payload: ICreateOrUpdateLinkedQuestion): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post('Plans/CreateOrUpdateLinkedQuestion', payload)
  }

  getWeeklyCommitmentReport(payload: IWeeklyCommitmentReportRequest): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post('Report/GetWeeklyCommitmentReport', payload)
  }
  
  getPlanAnswersRequest(params: IPlanAnswersRequestParams): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.get('Plans/GetPlanAnswersRequest', {
      params,
    })
  }
  getPlanExcersisesRequest(params: IPlanAnswersRequestParams): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.get('Plans/GetPlanExerciseAnswers', {
      params,
    })
  }
}

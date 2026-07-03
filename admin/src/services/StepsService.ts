import type {AxiosInstance, AxiosResponse} from 'axios'
import type {IApiResult} from '@/models/IApiResult'
import type {IGlobalGridResult} from '@/models/IGlobalGridResult'


export interface IStepsListFilters {
  pageNumber: number,
  count: number,
  search: string,
  allUsers?:boolean
}

export interface ITempStep {
  id: number
  name: string

}

export interface IRemoveStepFromExercise {
  excersiesId: number,
  stepId: number
}
export interface IAddStepToExercise extends IRemoveStepFromExercise{}
export interface IGetStep {

  createdAt: Date
  exerciseId: number
  name: string
  stepId: number
}

export interface ICreateStepPayload {
  name: string,
  createdByUserId:number
}

export default class StepsService {
  constructor(private axiosInstance: AxiosInstance) {
  }

  getAllSteps(filters: IStepsListFilters): Promise<AxiosResponse<IApiResult<IGlobalGridResult>>> {
    return this.axiosInstance.get('Steps/Get', {
      params: filters
    })
  }
  getAllStepsByFilter(filters: IStepsListFilters): Promise<AxiosResponse<IApiResult<IGlobalGridResult>>> {
    return this.axiosInstance.get('Steps/GetAllByFilter', {
      params: filters
    })
  }
  addStepToExercise(payload: IAddStepToExercise): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post('Excersies/AddStepToExcersie', payload)
  }
  createStep(payload: ICreateStepPayload): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post('Steps/Create', payload)
  }

  deleteStep(stepId: number): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.delete(`Steps/Delete/${stepId}`)
  }

  removeStepFromExercise(payload: IRemoveStepFromExercise): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post(`Excersies/RemoveStepFromExcersie`,payload)
  }

  updateStep(payload: object, id: number): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.put('Steps/Update', payload, {
      params: {
        id
      }
    })
  }

}

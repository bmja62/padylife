import type {AxiosInstance, AxiosResponse} from 'axios'
import type {IApiResult} from '@/models/IApiResult'
import type {IGlobalGridResult} from '@/models/IGlobalGridResult'
import {ExerciseTypes} from "@/models/Enums/ExerciseTypes";

export interface IExercisesListFilters {
  pageNumber: number,
  count: number,
  search: string,
}

export interface IExercise {
  title: string,
  exerciseType: string,
  documentLink: string,
  exerciseGoal: string,
  practiceMethod: string,
  imageUrl: string,
  exerciseCount: null | number,
  exerciseEstimate: null | number,
  exerciseCategoryId: null | number,
  stepIds: []
}

export interface IUpdateExercise extends Omit<IExercise, 'stepIds'> {
  id: number
}

export interface IGetExercise {

  createdAt: "Date"
  documentLink: string
  exerciseCategoryId: number
  exerciseCount: number
  exerciseEstimate: string
  exerciseGoal: string
  imageUrl: string
  exerciseStepsDTOs: []
  exerciseType: ExerciseTypes
  id: number
  name: string
  practiceMethod: string
  title: string
  updatedAt: null | Date
}

export interface IExerciseCategoryListFilters {
  pageNumber: number,
  count: number,
  search: string,
}

export interface ICreateOrUpdateExerciseCategoryPayload {
  name: string
}

export default class ExerciseService {
  constructor(private axiosInstance: AxiosInstance) {
  }

  getAllExercises(filters: IExercisesListFilters): Promise<AxiosResponse<IApiResult<IGlobalGridResult<[]>>>> {
    return this.axiosInstance.get('Excersies/GetAll', {
      params: filters
    })
  }

  getExerciseById(exerciseId: number): Promise<AxiosResponse<IApiResult<IGlobalGridResult<[]>>>> {
    return this.axiosInstance.get('Excersies/Get', {
      params: {
        id: exerciseId
      }
    })
  }

  getAllExerciseCategories(filters: IExerciseCategoryListFilters): Promise<AxiosResponse<IApiResult<IGlobalGridResult<IDynamicPage[]>>>> {
    return this.axiosInstance.get('ExerciseCategories/Get', {
      params: filters
    })
  }

  createExerciseCategory(payload: ICreateOrUpdateExerciseCategoryPayload): Promise<AxiosResponse<IApiResult<IDynamicPage>>> {
    return this.axiosInstance.post('ExerciseCategories/Create', payload)
  }

  createExercise(payload: IExercise): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post('Excersies/Create', payload)
  }

  deleteExerciseCategory(exerciseCategoryId: number): Promise<AxiosResponse<IApiResult<IDynamicPage>>> {
    return this.axiosInstance.delete(`ExerciseCategories/Delete/${exerciseCategoryId}`)
  }

  deleteExercise(exerciseId: number): Promise<AxiosResponse<IApiResult<IDynamicPage>>> {
    return this.axiosInstance.delete(`Excersies/Delete`,{
      params:{
        id:exerciseId
      }
    })
  }

  updateExercise(payload: object, id: number): Promise<AxiosResponse<IApiResult<IDynamicPage>>> {
    return this.axiosInstance.put('Excersies/Update', payload, {
      params: {
        id
      }
    })
  }

  updateExerciseCategory(payload: object, id: number): Promise<AxiosResponse<IApiResult<IDynamicPage>>> {
    return this.axiosInstance.put('ExerciseCategories/Update', payload, {
      params: {
        id
      }
    })
  }

}

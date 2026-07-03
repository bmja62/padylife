import type { AxiosInstance, AxiosResponse } from 'axios'
import type { IApiResult } from '@/models/IApiResult'
import type { IGlobalGridResult } from '@/models/IGlobalGridResult'

export interface IQuestionsListFilters {
  pageNumber: number,
  count: number,
  search: string,
}

export interface IAddOptionToQuestion {
  questionId: number,
  text: string
}

export interface IQuestion {
  questionCategoryId: null | number,
  text: string,
  displayText: string,
  questionOptions: Record<string, string>[]
}

export interface IQuestionOption {
  id: number,
  questionId: number,
  text: string
}

export interface IGetQuestion {
  id: number
  options: IQuestionOption[]
  questionCategoryId: number
  questionCategoryName: string
  text: string
  displayText: string
}

export interface IQuestionCategoryListFilters {
  pageNumber: number,
  count: number,
  search: string,
}

export interface ICreateOrUpdateQuestionCategoryPayload {
  name: string
}

export default class QuestionService {
  constructor(private axiosInstance: AxiosInstance) {
  }

  getAllQuestions(filters: IQuestionsListFilters): Promise<AxiosResponse<IApiResult<IGlobalGridResult<[]>>>> {
    return this.axiosInstance.get('Questions/GetAll', {
      params: filters
    })
  }

  getQuestionById(questionId: number): Promise<AxiosResponse<IApiResult<IGlobalGridResult<[]>>>> {
    return this.axiosInstance.get('Questions/GetBy', {
      params: {
        id: questionId
      }
    })
  }
  getAllQuestionCategories(filters: IQuestionCategoryListFilters): Promise<AxiosResponse<IApiResult<IGlobalGridResult<IDynamicPage[]>>>> {
    return this.axiosInstance.get('QuestionCategories/Get', {
      params: filters
    })
  }

  createQuestionCategory(payload: ICreateOrUpdateQuestionCategoryPayload): Promise<AxiosResponse<IApiResult<IDynamicPage>>> {
    return this.axiosInstance.post('QuestionCategories/Create', payload)
  }

  createQuestion(payload: IQuestion): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post('Questions/Create', payload)
  }

  updateQuestion(payload: IQuestion, questionId: number): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.put('Questions/Update', payload, {
      params: {
        id: questionId
      }
    })
  }
  deleteQuestionCategory(QuestionCategoryId: number): Promise<AxiosResponse<IApiResult<IDynamicPage>>> {
    return this.axiosInstance.delete(`QuestionCategories/Delete/${QuestionCategoryId}`)
  }
  removeQuestionOption(optionId: number): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.delete(`Questions/RemoveQuestionOptionToQuestion`, {
      params: {
        optionId
      }
    })
  }

  addOptionToQuestion(payload: IAddOptionToQuestion): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post(`Questions/AddQuestionOptionToQuestion`, payload)
  }
  deleteQuestion(QuestionId: number): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.delete(`Questions/Delete`, {
      params: {
        id: QuestionId
      }
    })
  }

  updateQuestionCategory(payload: object, id: number): Promise<AxiosResponse<IApiResult<IDynamicPage>>> {
    return this.axiosInstance.put('QuestionCategories/Update', payload, {
      params: {
        id
      }
    })
  }
  updateQuestionOption(payload: object): Promise<AxiosResponse<IApiResult<IDynamicPage>>> {
    return this.axiosInstance.post('Questions/UpdateQuestionOption', payload)
  }

}

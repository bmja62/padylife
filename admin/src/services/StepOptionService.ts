import type {AxiosInstance, AxiosResponse} from 'axios'
import type {IApiResult} from '@/models/IApiResult'

export const optionsForShow = {
  'MultipleChoice': 'چند گزینه ای',
  'Video': 'ویدیو',
  'Task': 'تکلیف',
  'Action': 'تعاملی',
  'Text': 'متن‌دار',
  'Image': 'عکس',
}
export const optionsForPicker = {
  'CreateMultiple': 1,
  'CreateVideo': 2,
  'CreateTask': 3,
  'CreateAction': 4,
  'CreateText': 5,
  'CreateImage': 6,
}
export interface IBaseStepOption {
  stepId: null | number,
  title: string,
  description: string,
  order: null | number
}

export interface ICreateVideoOption extends IBaseStepOption {
  videoUrl: string,
  thumbnailUrl: string,
  duration: string,
  allowDownload: boolean,

}

export interface ICreateImageOption extends IBaseStepOption {
  imageUrl: string,
  altText: string,
  caption: string,
  width: null | number,
  height: null | number,
}

export interface ICreateTextOption extends IBaseStepOption {
  content: "string",
  isHtml: true,
  textFormat: "string",
}

export interface ICreateActionOption extends IBaseStepOption {
  actionCommand: string,
  actionParameters: string,
  requiresConfirmation: boolean,
}

export interface ICreateTaskOption extends IBaseStepOption {
  deadlineDays: null | number,
  assigneeRole: "",
  taskInstructions: "",
  estimatedMinutes: null | number,
}
export interface ICreateMultiChoiceOption extends IBaseStepOption {
  choices: IChoice[],
  allowMultipleSelection: boolean,
  correctAnswerHint: string,
}

export interface IChoice {
  id: number,
  text: string,
  isCorrect: boolean,
  order: number
}


export interface IStepOptionsListFilters {
  stepId: number,
  type?: number,
  search?: string,
  pageNumber: number,
  count: number,
}

export default class StepOptionService {
  constructor(private axiosInstance: AxiosInstance) {
  }

  getAllStepOptions(filters: IStepOptionsListFilters): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.get('StepOption/GetAll', {
      params: filters
    })
  }
  createVideo(payload: ICreateVideoOption): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post('StepOption/CreateVideo', payload)
  }

  createImage(payload: ICreateImageOption): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post('StepOption/CreateImage', payload)
  }

  createText(payload: ICreateTextOption): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post('StepOption/CreateText', payload)
  }

  createAction(payload: ICreateActionOption): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post('StepOption/CreateAction', payload)
  }

  createTask(payload: ICreateTaskOption): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post('StepOption/CreateTask', payload)
  }

  createMultiChoice(payload: ICreateMultiChoiceOption): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post('StepOption/CreateMultipleChoice', payload)
  }

  deleteStepOption(params: { id: number, confrim: boolean }): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.delete('StepOption/Delete', {
      params
    })
  }

}

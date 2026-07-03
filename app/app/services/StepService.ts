import type {IApiResult} from "~~/types";

export interface IStepDetail {
    id: number,
    name: string,
    stepOptions:IStepOptionLite[]
}
export interface IChoice {
    id: number,
    text: string,
    isCorrect: boolean,
    order: number
}

export interface IStepOption {
    stepId: null | number,
    title: string,
    description: string,
    order: null | number
    id: number,
    type: string,
}
export interface IVideoOption extends IStepOption {
    videoUrl: string,
    thumbnailUrl: string,
    duration: string,
    allowDownload: boolean,

}

export interface IImageOption extends IStepOption {
    imageUrl: string,
    altText: string,
    caption: string,
    width: null | number,
    height: null | number,
}

export interface ITextOption extends IStepOption {
    content: "string",
    isHtml: true,
    textFormat: "string",
}

export interface IActionOption extends IStepOption {
    actionCommand: string,
    actionParameters: string,
    requiresConfirmation: boolean,
}

export interface ITaskOption extends IStepOption {
    deadlineDays: null | number,
    assigneeRole: "",
    taskInstructions: "",
    estimatedMinutes: null | number,
}
export interface IMultiChoiceOption extends IStepOption {
    choices: IChoice[],
    allowMultipleSelection: boolean,
    correctAnswerHint: string,
}

export type IStepOptionLite = Omit<IStepOption, 'choices'|'order'|'type'|'description'|'stepId'>


export interface IUserStepAnswerPayload {
    userPlanId: number,
    excersieId: number,
    stepId: number,
    selectedStepOptionId: number,
    text: string,
    imageUrl: string,
    selectedChoiceIds: number[]
}

export interface IHasAnswered extends Omit<IUserStepAnswerPayload, 'selectedStepOptionId' | 'selectedChoiceIds' | 'text' | 'imageUrl'> {
}
export default class StepService {
    constructor(private httpClient: LocalFetch) {
    }

    getStepDetailById(stepId: number): Promise<IApiResult<IStepDetail>> {
        return this.httpClient(`Steps/Get/${stepId}`, {method: "GET"});
    }

    createUserStepAnswer(payload: IUserStepAnswerPayload): Promise<IApiResult> {
        return this.httpClient(`UserStepAnswers/SubmitAnswer`, {method: "POST", body: payload});
    }

    hasAnsweredStepOption(payload: IHasAnswered): Promise<IApiResult> {
        return this.httpClient(`UserStepAnswers/HasSubmitAnswer`, {method: "POST", body: payload});
    }
    getStepOptions(stepId: number): Promise<IApiResult<IStepOption>> {
        return this.httpClient(`StepOption/GetAll`, {
            method: "GET", params: {
                stepId: stepId,
                pageNumber: 1,
                count: 100
            }
        });
    }
}

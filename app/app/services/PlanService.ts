import type { IApiResult } from "~~/types";
import type { IExercise } from "~/services/ExerciseService";

export enum PlanStatus {
    Draft = 1,
    Active = 2,
    Completed = 3
}

export interface IQuestionOption {
    id: number,
    text: string
}

export interface IPlanCategory {
    name: string,
    id: number
}

export interface IPlanQuestion {
    id: number,
    questionId: number,
    text: string,
    isMain: boolean,
    questioOptions: IQuestionOption[]
}

export interface ISignUpPlan {
    id: number,
    planCategoryId: number,
    name: string,
    planMainQuestion: IPlanQuestion
    description: string      
    isSignUpPlan: boolean
    status: PlanStatus
}

export interface IAnsweredQuestion {

    answerDate: Date
    planId: number
    planQuestionId: number
    planTitle: string
    questionText: string
    selectedOptionId: number
    selectedOptionText: string
    userPlanId: number
}

export interface ILastAnswerExercise {
    id: number,
    title: string,
    categoryName: string
    imageUrl: string
}

export interface IUserPlan {
    answeredQuestions: IAnsweredQuestion[]
    lastAnswerExercises: ILastAnswerExercise[]
    nextUnansweredQuestion: null | IAnsweredQuestion,
    planId: number
    imageUrl: string
    planName: string
    planLevel: string
    userPlanId: number
}

export interface ICreateUserAnswer {
    userPlanId: null | number,
    planQuestionId: null | number,
    selectedQuestionOptionId: null | number
}

export interface ILinkExercisesToUserPayload {
    questionLinkedId: number
    userPlanId: number
}

export interface IGetUserPlansFilters {
    userId: number
    search?: string
    pageNumber: number
    count: number,
    planId?: number,
}

export interface IGetNextUnAnsweredQuestion {
    planId: number,
    userId: number
}
export interface IExpert {
    expertId: number
    fullName: string
    profileImage: string
    specialization: string
}
export interface IUserPlanInfo {
    planExperts: IExpert[]
    exercises: IExercise[]
    planId: number
    isCompleted: boolean
    planTitle: string
    planLevel: string
    userFullName: string | null
    userId: number
    userPlanId: number
}
export interface IPrice {
    expertId: number,
    expertFullName: string,
    jobTitle: string,
    planId: number,
    planTitle: string,
    price: number,
    id: number,
    isActive: boolean
}

export interface IPlan {
    id: number,
    title: string,
    planCategoryId: number,
    planCategoryName: string,
    description: string,
    isSignUpPlan: boolean,
    planQuestions: null,
    imageUrl: string | null,
    price: number,
    finalPrice: number,
    discountPrice: number,
    status: PlanStatus,
    level: string
}

export interface IPlanPricesListFilters extends IGlobalGridRequest {
    planId?: number | null
}

export interface IGetPlanPriceParams {
    expertId: number,
    planId: number,
    isActive: boolean
}

export interface IGetPlansForUIParams extends IGlobalGridRequest {
    containUserPlans: boolean
    containSginUpPlans: boolean,
    categoryId?: number | null
}
export interface IAddCompanion {
    userPlanId: number,
    companionUserId: number
}

export interface IGetPlanSubscribersParams extends IGlobalGridRequest {
    planId: number
}

export interface IPlanSubscriber {
    imageUrl: string
    userFullName: string
}

export interface IGetPlanAnswersRequestFilters {
    planId?: number
    onlyCompleted?: boolean
    search?: string
    pageNumber: number
    count: number
}

export interface IPlanAnswerItemOption {
    planQuestionId: number
    isMain: boolean
    questionId: number
    questionText: string
    selectedOptionId: number
    selectedOptionText: string
}

export interface IPlanAnswersRequestItem {
    userPlanId: number
    userId: number
    userFullName: string
    startDate: string
    isCompleted: boolean
    answers: IPlanAnswerItemOption[]
}

export interface IGetUserPlanCompanionsFilters {
    userPlanId: number,
    search: string
    pageNumber: number
    count: number
}

export interface IRelatedPlan {
    sourcePlanId: null | number,
    targetPlanId: null | number,
    order: null | number
}

export interface IPlanItem {
    order: number
    finalPrice:number
    targetPlanId: number
    targetTitle: string
    hasPlan: boolean
}

export interface IRelatedPlanDetail {
    planId: number,
    title: string,
    nextPlans: IPlanItem[],
    previousPlans: IPlanItem[]
}

export default class PlanService {
    constructor(private httpClient: LocalFetch) {
    }

    addUserCompanion(payload: IAddCompanion): Promise<IApiResult<ISignUpPlan>> {
        return this.httpClient("Plans/AddCompanion", { method: "POST", body: payload });
    }
    removeUserCompanion(UserPlanCompanionId: number): Promise<IApiResult<ISignUpPlan>> {
        return this.httpClient("Plans/DeleteUserPlanCompanion", {
            method: "DELETE", params: {
                UserPlanCompanionId
            }
        });
    }
    getUserPlanCompanions(params: IGetUserPlanCompanionsFilters): Promise<IApiResult<ISignUpPlan>> {
        return this.httpClient("Plans/GetUserPlanCompanions/list", { method: "GET", params });
    }

    getPlansReportsForExpert(params: IGlobalGridRequest): Promise<IApiResult<ISignUpPlan>> {
        return this.httpClient("Report/GetReportPlanForExpert", { method: "GET", params });
    }

    getPlanUsers(params: {
        globalGrid: IGlobalGridRequest,
        planId: number
    }): Promise<IApiResult<ISignUpPlan>> {
        return this.httpClient("Plans/GetUsersThatAlreadyInPlanId", { method: "POST", body: params });
    }
    getPlanExperts(params: {
        globalGrid: IGlobalGridRequest,
        planId: number
    }): Promise<IApiResult<ISignUpPlan>> {
        return this.httpClient("Plans/GetExpertsThatPeresentPlanId", { method: "POST", body: params });
    }
    getSignUpPlan(): Promise<IApiResult<ISignUpPlan>> {
        return this.httpClient("Plans/GetSignUpPlan", { method: "POST" });
    }
    getPlanById(planId: number): Promise<IApiResult<ISignUpPlan>> {
        return this.httpClient("Plans/Get", { method: "POST", params: { id: planId } });
    }

    getAllPlanCategories(params: IGlobalGridRequest): Promise<IApiResult<ISignUpPlan>> {
        return this.httpClient("PlanCategories/Get", { method: "GET", params });
    }
    getAllPlansForUI(params: IGetPlansForUIParams): Promise<IApiResult<ISignUpPlan>> {
        return this.httpClient("Plans/GetAllForUI", { method: "POST", params });
    }
    linkExercisesToUser(params: ILinkExercisesToUserPayload): Promise<IApiResult<ISignUpPlan>> {
        return this.httpClient("Excersies/AssginExcersieToUser", { method: "POST", params });
    }

    getUserPlanExercises(UserPlanId: number): Promise<IApiResult<ISignUpPlan>> {
        return this.httpClient("Plans/GetUserPlanExercises", { method: "GET", params: { UserPlanId } });
    }

    getExpertCompanionCount(expertId?: number, isComplete: boolean): Promise<IApiResult<ISignUpPlan>> {
        const authStore = useAuthStore()
        return this.httpClient("Plans/GetExpertCompanionsCount", {
            method: "GET", params: {
                expertId: expertId ? expertId : authStore.getUser.id,
                isCompleted: isComplete
            }
        });
    }

    getPlanSubscribers(params: IGetPlanSubscribersParams): Promise<IApiResult<ISignUpPlan>> {
        return this.httpClient("Plans/GetPlanSubscribers", {
            method: "GET", params: params
        });
    }
    getUserPlans(params: IGetUserPlansFilters): Promise<IApiResult<ISignUpPlan>> {
        return this.httpClient("Plans/GetUserPlansStatus", { method: "GET", params });
    }
    getPlanPricesForUI(params: IPlanPricesListFilters): Promise<IApiResult<ISignUpPlan>> {
        return this.httpClient("Plans/GetAllPlanPricesForUI", { method: "GET", params });
    }

    getPlanPriceForUI(params: IGetPlanPriceParams): Promise<IApiResult<ISignUpPlan>> {
        return this.httpClient("Plans/GetPlanPriceForUI", { method: "GET", params });
    }
    getNextUnAnsweredQuestion(filters: IGetNextUnAnsweredQuestion): Promise<IApiResult<ISignUpPlan>> {
        return this.httpClient("Plans/GetNextUnansweredQuestion", { method: "GET", params: filters });
    }

    assignPlanToUser(payload): Promise<IApiResult<ISignUpPlan>> {
        return this.httpClient("Plans/AssginPlanToUser", { method: "POST", params: payload });
    }

    createUserAnswer(payload: ICreateUserAnswer): Promise<IApiResult<ISignUpPlan>> {
        return this.httpClient("Plans/CreateUserPlanAnswer", { body: payload, method: "POST" });
    }
    completeExerciseForUser(payload: ICreateUserAnswer): Promise<IApiResult<ISignUpPlan>> {
        return this.httpClient("Plans/CompletedUserExercise", { body: payload, method: "POST" });
    }

    completePlanForUser(userPlanId: number): Promise<IApiResult<ISignUpPlan>> {
        return this.httpClient("Plans/CompleteUserPlan", {
            params: {
                userPlanId
            }, method: "POST"
        });
    }

    getPlanRelatedPlans(planId: number): Promise<IApiResult<ISignUpPlan>> {
        return this.httpClient("Plans/GetPlanRelations", {
            params: {
                planId
            }, method: "GET"
        });
    }

    getPlanAnswersRequest(params: IGetPlanAnswersRequestFilters): Promise<IApiResult<IPlanAnswersRequestItem[]>> {
        return this.httpClient("Plans/GetPlanAnswersRequest", { method: "GET", params });
    }
}

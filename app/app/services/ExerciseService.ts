import type {IApiResult} from "~~/types";

export interface IExercise {
    count: number
    documentLink: string
    exerciseEstimate: string
    exerciseGoal: string
    exerciseId: number
    exerciseType: string
    id: number
    practiceMethod: string
    title: string
}

export interface IExerciseStep {
    stepId: number,
    exerciseId: number,
    name: string,
    createdAt: Date
}

export interface IExerciseDetail {
    id: number,
    title: string,
    exerciseCategoryId: number,
    name: string,
    documentLink: string,
    createdAt: Date,
    exerciseEstimate: string,
    exerciseGoal: string,
    practiceMethod: string,
    questionLinkedId: number,
    exerciseCount: number,
    exerciseType: string,
    updatedAt: null,
    exerciseStepsDTOs: IExerciseStep[]
}

export default class ExerciseService {
    constructor(private httpClient: LocalFetch) {
    }

    getExerciseById(exerciseId: number): Promise<IApiResult<ISignUpPlan>> {
        return this.httpClient("Excersies/Get", {method: "GET", params: {id:exerciseId}});
    }

}

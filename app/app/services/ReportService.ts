import type {IApiResult} from "~~/types";
import type {DailyFeelingsEnum} from "~/models/Enums/DailyFeelings";

export interface IActivityReport {
    userId: number,
    userName: string,
    totalExercises: number,
    completedExercises: number,
    exerciseCompletionRate: number,
    activePlans: number,
    totalPoints: number,
    leaderPercentile: number,
    healthStatus: string
}

export interface IGetWeeklyParams {
    userId: number,
    weeks: number
}

export interface IFeelingReport {
    averageFeeling: number
    feelingEntries: number
    feelingTypes: Record<number, DailyFeelingsEnum>
    weekLabel: string
    weekNumber: number
}

export interface IWeeklyReportResult<T> {
    weeklyData: T
    weeks: number
}

export interface IWeeklyCommitmentReport {

    averageAnswersPerWeek: number
    averageCompletionRate: number
    weeklyData: IWeeklyCommitment[]
    weeks: number
}

export interface IWeeklyCommitment {
    weekNumber: number,
    weekLabel: string,
    answersCount: number,
    exercisesAssigned: number,
    exercisesCompleted: number,
    completionRate: number
}

export interface ICourseProgress {
    completedExercises: number
    planId: number
    planTitle: string
    progressPercentage: number
    totalExercises: number
}

export interface ICoursesProgressReport {
    overallProgress: number
    plansProgress: ICourseProgress[]
}

export interface IGetPlanTeamChart {
    topPlans: number,
    periods: number,
    grouping: string
}

export interface IPlanTeamChart {

    answerCount: number
    earnedPoints: number
    periodLabel: Date
    trend: null
}

export interface IPlanTeamChartsReport {
    dataPoints: IPlanTeamChart[]
    planName: string
    planImageUrl: string
}

export interface IGetTeamPerformancePayload {
    teams: Record<string, number[]>,
    periods: nunmber,
    grouping: string
}

export default class ReportService {
    constructor(private httpClient: LocalFetch) {
    }

    getUserActivityReport(userId: number): Promise<IApiResult> {
        return this.httpClient(`Report/GetUserActivityReport`, {
            method: "POST", body: {
                userId
            }
        });
    }

    getWeeklyFeelingReport(payload: IGetWeeklyParams): Promise<IApiResult> {
        return this.httpClient(`Report/GetWeeklyFeelingsReport`, {
            method: "POST", body: payload
        });
    }

    getWeeklyCommitmentReport(payload: IGetWeeklyParams): Promise<IApiResult> {
        return this.httpClient(`Report/GetWeeklyCommitmentReport`, {
            method: "POST", body: payload
        });
    }

    getCourseProgressReport(userId: number): Promise<IApiResult> {
        return this.httpClient(`Report/GetCourseProgressReport`, {
            method: "POST", body: {
                userId
            }
        });
    }

    getPlanTeamChart(payload: IGetPlanTeamChart): Promise<IApiResult> {
        return this.httpClient(`Report/AutoGeneratePlanTeamChart`, {
            method: "POST", body: payload
        });
    }

    getTeamPerformanceChart(payload: IGetTeamPerformancePayload): Promise<IApiResult> {
        return this.httpClient(`Report/GetTeamPerformanceChart`, {
            method: "POST", body: payload
        });
    }

    getUserActivityForAllQuestions(): Promise<IApiResult> {
        return this.httpClient(`Report/GetUserActivityForAllQuestionsReport`,{
         method:   'POST',
            body:{}
        });
    }
}

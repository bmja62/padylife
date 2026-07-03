import type {IApiResult} from "~~/types";

export interface ILeaderboardUser {
    rank: number,
    userId: number,
    userName: string,
    profileImage: null | string,
    totalPoints: number,
    answerCount: number,
    badges: string[]
}

export interface IMedal {
    id: number,
    title: string,
    description: string,
    iconUrl: string,
    isLocked: boolean
}

export default class LeaderBoardService {
    constructor(private httpClient: LocalFetch) {
    }

    getLeaderBoard(TopCount: number): Promise<IApiResult> {
        return this.httpClient(`Leaderboard/GetLeaderboard`, {
            method: "GET", params: {
                TopCount
            }
        });
    }

    getExpertLeaderboard(TopCount: number): Promise<IApiResult> {
        return this.httpClient(`Leaderboard/GetExpertLeaderboard`, {
            method: "GET", params: {
                TopCount
            }
        });
    }

    getPlanLeaderboard(TopCount: number = 10, PlanId: number): Promise<IApiResult> {
        return this.httpClient(`Leaderboard/GetPlanLeaderboard`, {
            method: "GET", params: {
                TopCount,
                PlanId
            }
        });
    }
    getAllMedals(params: IGlobalGridRequest): Promise<IApiResult> {
        return this.httpClient(`Medals/GetAll`, {
            method: "GET", params
        });
    }

    checkAndAssignMedals(userId?: number): Promise<IApiResult> {
        const authStore = useAuthStore()
        return this.httpClient(`Medals/CheckAndAssign/assign`, {
            method: "POST", params: {
                userId: userId ? userId : authStore.getUser.id
            }
        });
    }

}

import type {AxiosInstance, AxiosResponse} from 'axios'
import type {IApiResult} from '@/models/IApiResult'
import type {IGlobalGridResult} from '@/models/IGlobalGridResult'
import type {IGlobalGridRequest} from "~~/types";

export enum ChallengeType {
    Single = 'Single',
    Group = 'Group',
}

export const challengeTypesShow = {
    Single: "فردی",
    Group: "گروهی",
};

export interface IChallenge {
    title: string,
    description: string,
    imageUrl: string,
    type: ChallengeType,
    participantCount:number,
    hasParticipantByMe:boolean,
    id: number
}

export interface IGetChallengeFilters extends IGlobalGridRequest{
    type: ChallengeType,
}
export default class ChallengeService {
    constructor(private httpClient: LocalFetch) {
    }

    getAllChallenges(filters: IGlobalGridRequest):Promise<IApiResult> {
        return this.httpClient("Challenges/Get", {method: "GET",params:filters});

    }
    getAllChallengesByFilter(filters: IGetChallengeFilters):Promise<IApiResult> {
        return this.httpClient("Challenges/GetAllByFilter", {method: "GET",params:filters});

    }
    getChallengeById(challengeId: number): Promise<AxiosResponse<IApiResult<IGlobalGridResult<IChallenge>>>> {
        return this.httpClient(`Challenges/Get/${challengeId}`, {method: "GET"});
    }
    participate(challengeId: number): Promise<AxiosResponse<IApiResult<IGlobalGridResult<IChallenge>>>> {
        return this.httpClient(`Challenges/JoinChallenge`, {method: "POST",params:{challengId:challengeId}});
    }

}

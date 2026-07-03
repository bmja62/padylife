import type {AxiosInstance, AxiosResponse} from 'axios'
import type {IApiResult} from '@/models/IApiResult'
import type {IGlobalGridResult} from '@/models/IGlobalGridResult'

export interface IVisitsStatsParams {
  entityType?: string,
  entityId?: number | null,
  section?: string,
  fromDate: Date,
  toDate: Date,
  pageUrlPattern?: string
}

export interface IVisitEntity {
  entityType: "",
  entityId: number,
  pageUrl: "",
  section: "",
  uniqueVisits: number,
  totalVisits: number,
  statDate: Date


}

export interface IPopularVisitEntity {
  pageUrl: string,
  pageTitle: string,
  entityType: string,
  entityId: number,
  totalVisits: number,
  uniqueVisits: number
}

export interface IPopularVisitsParams {
  top: number,
  fromDate: Date,
  toDate: Date,
}

export default class VisitService {
  constructor(private axiosInstance: AxiosInstance) {
  }

  getVisitsStats(filters: IVisitsStatsParams): Promise<AxiosResponse<IApiResult<IGlobalGridResult<IVisitEntity[]>>>> {
    return this.axiosInstance.get('VisitTracking/GetStats/stats', {
      params: filters
    })
  }

  getVisitsChart(filters: { days: number }): Promise<AxiosResponse<IApiResult<IGlobalGridResult<IVisitEntity[]>>>> {
    return this.axiosInstance.get('VisitTracking/GetDailyChartData/stats/chart/daily', {
      params: filters
    })
  }

  getVisitsSummary(filters: IVisitsStatsParams): Promise<AxiosResponse<IApiResult<IGlobalGridResult<IVisitEntity[]>>>> {
    return this.axiosInstance.get('VisitTracking/GetVisitSummary/stats/summary', {
      params: filters
    })
  }

  getDashboardSummary(): Promise<AxiosResponse<IApiResult<IGlobalGridResult<IVisitEntity[]>>>> {
    return this.axiosInstance.get('Dashboard/GetSystemSummary/summary')
  }

  getPopularVisits(filters: IPopularVisitsParams): Promise<AxiosResponse<IApiResult<IGlobalGridResult<IPopularVisitEntity[]>>>> {
    return this.axiosInstance.get('VisitTracking/GetPopularPages/stats/popular', {
      params: filters
    })
  }


}

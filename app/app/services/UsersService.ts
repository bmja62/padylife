import type {IApiResult} from "~~/types";
import type {ISignUpPlan} from "~/services/PlanService";
import type {UserRoles} from "~/models/Enums/UserRoles";


export interface IRegisterPayload {
  userName: string;
  email?: string;
  phoneNumber?: string;
  password: string;
  type: UserRoles;
  verificationCode:string
}

export interface ILoginPayload {
  username: string;
  password: string;
}

export interface IRole {
  name: string,
  description: string
}


export interface IUserSession {
  email: string
  emailConfirmed: boolean
  fullName: null | string
  gender: null | number
  id: number
  introduceCode: null | string
  isActive: boolean
  phoneNumber: string
  roles: IRole[]
  shabaNo: null | string
  userName: string
  walletCredit: number
  addresses: []
  age: number
  birthdate: string
  hight: number
  instagramId: null | string
  jobTitle: string
  maritalStatus: string
  profileImage: string
  signUpPlanInfo: { hasSignUpPlan: boolean, lastQuestion: null }
  userPlans: []
  wight: number
}

export interface IUpdatePayload {
  id: number,
  fullName: string,
  age: number,
  gender: string,
  isActive: boolean,
  birthdate: Date,
  hight: number,
  wight: number,
  jobTitle: string,
  maritalStatus: "Single",
  instagramId: string
}

export interface ISimilarUser {
  userId: number,
  fullName: string,
  userName: string,
  sharedAnswerCount: number,
  similarityPercent: number,
  badgeTitle: string,
  badgeIcon: string
}
export interface IUserPoint{
  userId: number,
  availablePoints: number,
  earnedPoints: number,
  consumedPoints: number,
  pointsToMoneyRatio: number,
  moneyValue: number
}

export interface IGoogleCredentials{
  clientId: string,
  client_id: string,
  credential: string,
  select_by: string
}

export interface IForgetPasswordPayload{
  phoneNumber: string
  resetCode: string,
  newPassword: string
}

export interface IExchangePointsPayload {
  userId: number | null,
  pointsToConvert: number | null,
  description: string
}
export default class UserService {
  constructor(private httpClient: LocalFetch) {}

  signUp(signUpPayload: IRegisterPayload): Promise<IApiResult<ISignUpPlan>> {
    return this.httpClient("Users/Register", {
      body: signUpPayload,
      method: "POST"
    });
  }

  signIn(signInPayload: ILoginPayload): Promise<IApiResult<any>> {
    return this.httpClient("Users/Token/Token", {
      body: signInPayload,
      method: "POST"
    });
  }

  updateUser(payload: IUpdatePayload): Promise<IApiResult> {
    return this.httpClient(`Users/Update/${payload.id}`, {
      body: payload,
      method: "PUT"
    });
  }
  getUserByToken(): Promise<IApiResult<IUserSession>> {
    return this.httpClient("Users/GetByToken", {method: "GET"});
  }
  setNewPassword(payload:IForgetPasswordPayload): Promise<IApiResult<IUserSession>> {
    return this.httpClient("Users/ResetPassword/reset-password", {method: "POST",body:payload});
  }

  sendOtpForgetPassword(phoneNumber: string): Promise<IApiResult<IUserSession>> {
    return this.httpClient("Users/ForgotPassword/forgot-password", {
      method: "POST", body: {
        phoneNumber
      }
    });
  }
  sendOtpRegister(phoneNumber: string): Promise<IApiResult<IUserSession>> {
    return this.httpClient("Users/SendOTPCodeToPhoneNumber/send-verification-code", {
      method: "POST", body: {
        phoneNumber
      }
    });
  }
  getUserPoints(userId: number): Promise<IApiResult<IUserSession>> {
    return this.httpClient(`Points/GetUserPoints/${userId}`, {method: "GET"});
  }

  exchangePoints(payload: IExchangePointsPayload): Promise<IApiResult<IUserSession>> {
    return this.httpClient(`Points/ConvertPointsToWalletCredit/convert`, {method: "POST", body: payload});
  }
  getUserPointsReport(payload: { userId:number,pageNumber:number,count:number }): Promise<IApiResult<IUserSession>> {
    return this.httpClient(`Points/PointsReport`, {method: "GET", params: payload});
  }
  getUserPointsSummary(userId: number): Promise<IApiResult<IUserSession>> {
    return this.httpClient(`Points/MyPointsSummary`, {method: "GET", params: {
      userId
      }});
  }
  logout(): Promise<IApiResult<IUserSession>> {
    return this.httpClient(`Users/Logout/logout`, {method: "POST"});
  }

  sendGoogleToken(token: string): Promise<IApiResult<IUserSession>> {
    return this.httpClient(`Users/GoogleLogin/google`, {
      method: "POST", body: {
        credential:token
      }
    });
  }

  getUserById(userId: number): Promise<IApiResult<IUserSession>> {
    return this.httpClient(`Users/Get/${userId}`, {method: "GET"});
  }

  trackVisit(payload: object, userAgent: string): Promise<IApiResult<IUserSession>> {
    return this.httpClient(`VisitTracking/TrackVisit/track`, {
      method: "POST", body: payload, headers: {
        "User-Agent": userAgent
      }
    });
  }

  getSimilarUsers(userId: number): Promise<AxiosResponse<IApiResult<IGlobalGridResult<IUser[]>>>> {
    return this.httpClient('Plans/GetSimilarUsers', {
      method: "GET",
      params: {
        userId
      },
    })
  }

}

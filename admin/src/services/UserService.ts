import type {AxiosInstance, AxiosResponse} from 'axios'
import type {IApiResult} from '@/models/IApiResult'
import type {IGlobalGridResult} from '@/models/IGlobalGridResult'
import {GenderTypes, persianGenderTypes} from "@/models/Enums/GenderTypes";

export interface ILoginPayload {
  username: string,
  password: string
}

export interface IUserCreatePayload {
  userFullName: string,
  nationalCode: string,
  phoneNumber: string,
  typeId: number,
  password: string
}


export interface IGetUserFilters {
  pageNumber: string | number
  count: string | number
  search: string | null
  roleName:string
}

export interface ILoginResponse {
  token: string,
  user: IUser
}

export interface IUpdateUserRolePayload {
  userId: number
  role: string
}

export interface IRole {
  name: string,
  description: string,
}

export interface IUser {
  email: string
  emailConfirmed: boolean
  fullName: string
  gender: keyof persianGenderTypes
  id: number
  introduceCode: null | string
  isActive: boolean
  phoneNumber: null | string
  roles: IRole[]
  shabaNo: number
  userName: string
  walletCredit: number


}


export interface IUpdateUser {
  fullName: string,
  age: null,
  gender: GenderTypes,
  isActive: boolean,
  birthdate: Date,
  c: number,
  hight: number,
  wight: number,
  jobId: number,
  maritalStatus: string,
  instagramId: string
}

export interface IAccessToken {
  access_token: string
  expires_in: number
  refresh_token: null
  token_type: string
}

export interface IAuthResponse {
  accessToken: IAccessToken
  user: IUser
}

export default class UserService {
  Media
  DeleteSliders

  constructor(private axiosInstance: AxiosInstance) {
  }

  login(payload: ILoginPayload): Promise<AxiosResponse<IApiResult<IUser>>> {
    return this.axiosInstance.post('Users/Token/Token', payload)
  }

  getUsers(payload: IGetUserFilters): Promise<AxiosResponse<IApiResult<IGlobalGridResult<IUser[]>>>> {
    return this.axiosInstance.get('Users/Get', {
      params: payload,
    })
  }

  getUserById(id: number): Promise<AxiosResponse<IApiResult<IGlobalGridResult<IUser[]>>>> {
    return this.axiosInstance.get(`Users/Get/${id}`)
  }
  createANewUser(payload: IUserCreatePayload): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post('/User/CreateUserByAdmin', payload)
  }

  updateAUser(payload: IUserUpdateOrDeletePayload, userId: number): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.put(`Users/Update/${userId}`, payload)
  }

  getUserByToken(): Promise<AxiosResponse<IApiResult<IUser>>> {
    return this.axiosInstance.get('/Users/GetByToken')
  }

  deleteAUser(id: string | number): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.delete('/Users', {
      params: {
        id,
      },
    })
  }

  toggleIsActive(userId: string | number): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post('Users/ToggleIsActive', null,{
      params: {
        id: userId,
      },
    })
  }

  deleteSlides(pictureUrl: string): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post('/Media/DeleteSliders', null, {
      params: {
        pictureUrl
      },
    })
  }

  updateSlides(slide: object): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post('/Media/UpdateMedias', [slide])
  }

  updateBanner(banner: object): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post('/Banner/Update', banner)
  }

  createSlide(slide: object): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.post('/Media/CreateFile', slide)
  }


  addRoleToUser(payload: IUpdateUserRolePayload): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.patch('/Users/AddRoleToUser', payload)
  }

  revokeRoleFromUser(payload: IUpdateUserRolePayload): Promise<AxiosResponse<IApiResult>> {
    return this.axiosInstance.patch('/Users/RemoveRoleFromUser', payload)
  }
}

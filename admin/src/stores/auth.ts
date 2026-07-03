import {defineStore} from 'pinia'
import {isAxiosError} from 'axios'
import {useSpinner} from '@/composables/spinner'
import {useAlerts} from '@/composables/alert'
import router from '@/router'
import type {IRole} from '@/services/RoleService'
import type {IAccessToken, IUser} from '@/services/UserService'
import $api from '@/plugins/apiProvider'

// Interfaces
interface IAuthStoreState {
  isLoggedIn: boolean
  token: string | null
  user: IUser | null
  roles: IRole[] | []

}

export const useAuthStore = defineStore('_auth', {
  persist: {
    storage: localStorage,
    paths: ['isLoggedIn', 'roles', 'token', 'user'],
  },
  state: (): IAuthStoreState => ({
    isLoggedIn: false,
    roles: [],
    token: null,
    user: null,
  }),
  getters: {
    isLogged(): boolean {
      return this.isLoggedIn
    },
    getToken(): string | null {
      return this.token
    },
    getUser(): IUser | null {
      return this.user
    },
    getUserRoles(): IRole[] | [] {
      return this.roles
    },

  },
  actions: {
    logout(): void {
      this.isLoggedIn = false
      this.token = null
      this.user = null
      this.roles = []
      router.push('/login')
    },
    setUser(user: IUser): void {
      this.user = user
      this.roles = user.roles
    },
    setToken(token: IAccessToken) {
      this.token = token.access_token
      this.isLoggedIn = true

    },
    updateUser(userDetails: IUser): void {
      this.user = userDetails
    },
    async fetchUser(): Promise<void> {
      const spinner = useSpinner()
      const alert = useAlerts()
      try {
        spinner.showSpinner()

        const response = await $api?.users.getUserByToken()
        if (response?.data.isSuccess) {
          this.updateUser(response.data.data)
        } else {
          alert.error(
            response?.data.message || 'مشکلی پیش آمد، لطفا دوباره امتحان کنید',
          )
        }
      } catch (error) {
        if (isAxiosError(error)) {
          alert.error(
            error?.response?.data?.message
            || 'مشکلی پیش آمد، لطفا دوباره امتحان کنید',
          )
        } else {
          console.error(error)
        }
      } finally {
        spinner.hideSpinner()
      }
    },
  },
})

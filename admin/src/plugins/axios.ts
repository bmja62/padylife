// eslint-disable-next-line regex/invalid
import axios from 'axios'
import {useAuthStore} from '@/stores/auth'

const axiosIns = axios.create({
  // You can add your headers here
  // ================================

  baseURL: `${import.meta.env.VITE_BASE_API_URL}/api/v1/`,

  // timeout: 1000,
  // headers: {'X-Custom-Header': 'foobar'}
})

// ℹ️ Add request interceptor to send the authorization header on each subsequent request after login
axiosIns.interceptors.request.use(config => {
  const auth = useAuthStore()

  // Retrieve token from localStorage
  const token = auth.getToken

  // If token is found
  if (token) {
    // Get request headers and if headers is undefined assign blank object
    config.headers = config.headers || {}

    // Set authorization header
    config.headers.Authorization = token ? `Bearer ${token}` : ''
  }

  // Return modified config
  return config
})

// ℹ️ Add response interceptor to handle 401 response
axiosIns.interceptors.response.use(response => {
  return response
}, error => {
  const auth = useAuthStore()

  // Handle error
  if (error.response.status === 401) {
    // ℹ️ Logout user and redirect to login page
    auth.logout()

    // If 401 response returned from api
  } else {
    return Promise.reject(error)
  }
})

export default axiosIns

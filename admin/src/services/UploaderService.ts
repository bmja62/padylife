import type {AxiosInstance, AxiosResponse} from 'axios'


export default class UploaderService {
  constructor(private axiosInstance: AxiosInstance) {
  }

  upload(payload): Promise<AxiosResponse<any>> {
    return this.axiosInstance.post('/Uploaders/Upload', payload, {
      headers: {
        'Content-Type': 'multipart/form-data',
      }
    })
  }
  delete(guid): Promise<AxiosResponse<any>> {
    return this.axiosInstance.delete(`/Uploaders/Delete/${guid}`)
  }


}

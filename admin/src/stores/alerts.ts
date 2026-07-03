import { defineStore } from 'pinia'

export const useAlertStore = defineStore('alert', {
  state: () => {
    return {
      isRenderingSuccessAlert: false,
      isRenderingErrorAlert: false,
      alertMessage: null as null | string,
      timeoutId: null as null | ReturnType<typeof setTimeout>,
    }
  },

  actions: {
    renderSuccess(message: string | null, timeout: string | number = 3000) {
      this.closeAlert()
      this.isRenderingSuccessAlert = true
      this.alertMessage = message
      this.timeoutId = setTimeout(() => {
        this.closeAlert()
      }, +timeout)
    },
    renderError(message: string | null, timeout: string | number = 3000) {
      this.closeAlert()
      this.isRenderingErrorAlert = true
      this.alertMessage = message
      this.timeoutId = setTimeout(() => {
        this.closeAlert()
      }, +timeout)
    },
    closeAlert() {
      if (this.timeoutId)
        clearTimeout(this.timeoutId)
      this.isRenderingErrorAlert = false
      this.isRenderingSuccessAlert = false
      this.alertMessage = null
    },
  },
})

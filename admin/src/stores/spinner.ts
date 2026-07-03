import { defineStore } from 'pinia'

export const useSpinnerStore = defineStore('spinner', {
  state: () => {
    return {
      isRenderingSpinner: false,
    }
  },

  actions: {
    changeSpinnerState(state = false) {
      this.isRenderingSpinner = state
    },
  },
})

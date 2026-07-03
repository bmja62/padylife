import { defineStore } from "pinia";

export const useSpinnerStore = defineStore("spinner", {
  state: () => {
    return {
      isRenderingSpinner: false,
      uploadProgressPercent: null as number | null,
    };
  },

  getters: {
    getUploadProgressPercent(): number | null {
      return this.uploadProgressPercent;
    },
    isLoading(): boolean {
      return this.isRenderingSpinner;
    },
  },

  actions: {
    changeSpinnerState(state = false) {
      this.isRenderingSpinner = state;
    },
    setUploadProgressPercent(value: number | null = null) {
      this.uploadProgressPercent = value;
    },
  },
});

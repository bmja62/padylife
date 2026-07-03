import { defineStore } from "pinia";

export const useAuthorizeStore = defineStore("authorize", {
  state: () => {
    return {
      isRenderingSignInDialog: false,
      isRenderingSignUpDialog: false,
    };
  },

  getters: {
    getSignInDialogState(): boolean {
      return this.isRenderingSignInDialog;
    },
    getSignUpDialogState(): boolean {
      return this.isRenderingSignUpDialog;
    },
  },

  actions: {
    setSignInState(state: boolean = false) {
      this.isRenderingSignInDialog = state;
      if (state) this.isRenderingSignUpDialog = false;
    },
    setSignUpState(state: boolean = false) {
      this.isRenderingSignUpDialog = state;
      if (state) this.isRenderingSignInDialog = false;
    },
  },
});

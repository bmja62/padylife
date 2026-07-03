import {defineStore} from "pinia";

export const useAlertStore = defineStore("alert", {
  state: () => {
    return {
      isRenderingAlert: false,
      isError: true,
      isNotification: false,
      alertMessage: null as null | string,
      alertDescription: null as null | string,
      timeoutId: null as null | ReturnType<typeof setTimeout>,
      transitionBaseNumber: 100,
      transitionDuration: 1500,
      isClosingTransition: false,
    };
  },
  getters: {
    message(): string | null {
      return this.alertMessage;
    },
    description(): string | null {
      return this.alertDescription;
    },
    isRenderingAlertDialog(): boolean {
      return this.isRenderingAlert;
    },
    isErrorAlert(): boolean {
      return this.isError;
    },
    isNotificationAlert(): boolean {
      return this.isNotification;
    },
    isOutTransition(): boolean {
      return this.isClosingTransition;
    },
  },
  actions: {
    toggleTransition(): void {
      this.transitionBaseNumber = this.transitionBaseNumber === 100 ? 0 : 100;
    },
    renderSuccess(message: string | null, timeout: string | number = 3000) {
      this.isRenderingAlert = true;
      this.isClosingTransition = true;
      this.isNotification = false
      this.isError = false;
      this.alertMessage = message;
      nextTick(() => {
        this.toggleTransition();
      });
      this.timeoutId = setTimeout(() => {
        this.toggleTransition();
      }, +timeout);
    },
    renderNotification(subject: string | null, description: string | null, timeout: string | number = 3000) {
      this.isRenderingAlert = true;
      this.isClosingTransition = true;
      this.isError = false;
      this.alertMessage = subject;
      this.alertDescription = description;
      this.isNotification = true
      nextTick(() => {
        this.toggleTransition();
      });
      this.timeoutId = setTimeout(() => {
        this.toggleTransition();
      }, +timeout);
    },
    renderError(message: string | null, timeout: string | number = 3000) {
      this.isRenderingAlert = true;
      this.isClosingTransition = true;
      this.isNotification = false
      this.isError = true;
      this.alertMessage = message;
      nextTick(() => {
        this.toggleTransition();
      });
      this.timeoutId = setTimeout(() => {
        this.toggleTransition();
      }, +timeout);
    },
    closeAlert() {
      if (this.timeoutId) clearTimeout(this.timeoutId);
      this.isRenderingAlert = false;
      this.isClosingTransition = false;
      this.isError = true;
      this.alertMessage = null;
      this.alertDescription = null;
      this.isNotification = false
    },
  },
});

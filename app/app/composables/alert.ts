import {useAlertStore} from "@/stores/alerts";

export function useAlerts() {
  const alertStore = useAlertStore();
  function error(message: string | null, timeout?: string | number) {
    alertStore.renderError(message, timeout);
  }
  function success(message: string | null, timeout?: string | number) {
    alertStore.renderSuccess(message, timeout);
  }

  function notification(subject: string | null, description: string | null, timeout?: string | number) {
    alertStore.renderNotification(subject, description, timeout);

  }
  function close() {
    alertStore.closeAlert();
  }

  return {error, success, close, notification};
}

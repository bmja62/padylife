import { useSpinnerStore } from '@/stores/spinner'

export function useSpinner() {
  const spinnerStore = useSpinnerStore()
  function showSpinner() {
    spinnerStore.changeSpinnerState(true)
  }
  function hideSpinner() {
    spinnerStore.changeSpinnerState(false)
  }

  return { showSpinner, hideSpinner }
}

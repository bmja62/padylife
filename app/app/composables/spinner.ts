export function useSpinner() {
  const spinnerStore = useSpinnerStore();

  function renderSpinner() {
    spinnerStore.changeSpinnerState(true);
  }
  function hideSpinner() {
    spinnerStore.changeSpinnerState(false);
  }

  return {
    renderSpinner,
    hideSpinner,
  };
}

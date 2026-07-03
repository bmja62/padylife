export function useAuthorize() {
  const authorizeStore = useAuthorizeStore();

  function renderSignIn() {
    authorizeStore.setSignInState(true);
  }

  function renderSignUp() {
    authorizeStore.setSignUpState(true);
  }

  const isRenderingSignInDialog = computed<boolean>({
    get() {
      return authorizeStore.getSignInDialogState;
    },
    set(value: boolean) {
      authorizeStore.setSignInState(value);
    },
  });

  const isRenderingSignUpDialog = computed<boolean>({
    get() {
      return authorizeStore.getSignUpDialogState;
    },
    set(value: boolean) {
      authorizeStore.setSignUpState(value);
    },
  });

  return {
    renderSignIn,
    renderSignUp,
    isRenderingSignInDialog,
    isRenderingSignUpDialog,
  };
}

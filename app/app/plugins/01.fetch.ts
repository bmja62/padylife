export default defineNuxtPlugin(() => {
  enum ErrorCode {
    "ابتدا وارد حساب کاربری خود شوید" = 401,
    "دسترسی شما به این منابع امکان پذیر نیست" = 403,
    "خطا در برقراری ارتباط با سرور. با پشتیبان سایت تماس حاصل فرمایید" = 405,
    "صفحه مورد نظر یافت نشد" = 404,
    "صفحه مورد نظر از سامانه حذف شده است" = 410,
    "خطا در برقراری ارتباط با سرور. دوباره تلاش کنید" = 500,
  }


  const baseURL = useRuntimeConfig().public.apiAddress;
  const authStore = useAuthStore()
  const {token} = storeToRefs(authStore)
  const cookieToken = useCookie('token')

  const fetchInstance = $fetch.create({
    baseURL: `${baseURL}/api/v1`,
    onRequest({ options }) {
      if (cookieToken.value) {
        options.headers.set('Authorization', `${cookieToken.value}`)
      } else if (!cookieToken.value && token.value) {
        options.headers.set('Authorization', `${token.value}`)
      }

    },
    onResponseError({ response }) {
      if (response.status === 401) {
        authStore.logout()
      }
      if (response.status === 400) {
        throw createError({
          statusCode: response.status,
          message: response._data.message,
        });
      } else {
        throw createError({
          statusCode: response.status,
          message: ErrorCode[response.status],
        });
      }
    },
  });
  return {
    provide: {
      fetch: fetchInstance,
    },
  };
});

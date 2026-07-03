export default defineNuxtRouteMiddleware((to, from) => {
    if (import.meta.client) {
        const token = useCookie('_token')
        if (to.path === '/sign-in') {
            if (token.value) {
                if (useUtils().isSpecialist.value) {
                    return navigateTo('/dashboard/specialist')
                } else {
                    return navigateTo('/dashboard')
                }
            }
        }
    }
});


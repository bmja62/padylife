export default defineNuxtRouteMiddleware((to, from) => {
    if (import.meta.client) {
        const authStore = useAuthStore();
        const user = authStore.getUser;

        // // Handle role-based redirection
        // if (to.meta.roleCheck && user) {
        //   const role = user.roles[0]?.name;
        //   if (role === "User") {
        //     return navigateTo("/parents", { external: true });
        //   } else if (role === "Driver") {
        //     return navigateTo("/drivers/home", { external: true });
        //   }
        // }

        // Handle authenticated routes
        if (to?.meta?.auth) {
            if (authStore.isLoggedIn) {
                if (to?.meta?.access === undefined || to?.meta?.access?.includes('all')) {
                    navigateTo(to.path)
                } else {
                    if (!checkUserRole(user, to.meta.access)) {
                        return navigateTo(`/?redirectUrl=${to.path}`); // Optional: redirect if user role does not match
                    } else {
                        authStore.logout()
                        return navigateTo(`/?redirectUrl=${to.path}`)
                    }
                }
            } else {
                authStore.logout()
                return navigateTo(`/?redirectUrl=${to.path}`)
            }
        } else {
            navigateTo(to.path)
        }

        // if (to.meta.auth) {
        //     if (!user) {
        //         return navigateTo("/login");
        //     }
        //     if (!checkUserRole(user, to.meta.role)) {
        //         return navigateTo("/unauthorized"); // Optional: redirect if user role does not match
        //     }
        // }
    }
});

// Utility function to check user role
function checkUserRole(user, pageAccesses) {
    if (pageAccesses && pageAccesses.length > 0) {
        return pageAccesses.every((pageRole: string) => {
            return user.roles.some(
                (userRole: IRole) =>
                    userRole.name?.toLocaleLowerCase() === pageRole.toLocaleLowerCase()
            );
        });
    }
    return true;
}

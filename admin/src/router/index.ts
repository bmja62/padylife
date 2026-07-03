import { setupLayouts } from 'virtual:generated-layouts'
import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import routes from '~pages'
import type { IRole } from '@/services/RoleService'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    ...setupLayouts(routes),
  ],
})

function checkRoleAccess(pageAccesses: string[], roles: IRole[]) {
  if (pageAccesses && pageAccesses.length > 0) {
    return pageAccesses.every((pageRole: string) => {
      return roles.some(
        (userRole: IRole) =>
          userRole.name?.toLocaleLowerCase() === pageRole.toLocaleLowerCase(),
      )
    })
  }

  return true
}

router.beforeEach((to, _, next) => {
  const auth = useAuthStore()

  if (to.path != '/login') {
    if (to?.meta?.auth == false) {
      return next()
    }
    else if (to?.meta?.auth == undefined || to?.meta?.auth == true) {
      if (auth.isLogged) {
      // @ts-expect-error i dont know
        if (checkRoleAccess(to.meta.access, auth.getUserRoles))
          return next()

        return next('/login')
      }
      else {
        return next('/login')
      }
    }
    else {
      next()
    }
  }
  else {
    next()
  }

  // if (!auth.isLogged)
  //   return '/login'

  // explicitly return false to cancel the navigation
  // return false
})

// Docs: https://router.vuejs.org/guide/advanced/navigation-guards.html#global-before-guards

export default router

// router/index.ts
import { createRouter, createWebHistory } from 'vue-router'
import type { RouteRecordRaw } from 'vue-router'

const Home = () => import('../views/Home.vue')

const routes: RouteRecordRaw[] = [
    {
        path: '/',
        redirect: '/loginForm'
    },
    {
        path: '/home',
        name: 'Home',
        component: Home,
        meta: {
            title: 'Inicio - Banca Web',
            requiresAuth: false
        }
    },
    {
        path: '/dashboard',
        name: 'Dashboard',
        component: () => import('../components/Dashboard.vue'),
        meta: {
            title: 'Panel - Mi Banca',
            requiresAuth: true
        }
    },
    {
        path: '/cardsection',
        name: 'CreditCard',
        component: () => import('../components/CardSection.vue'),
        meta: {
            title: 'Panel - Mi Banca',
            requiresAuth: true
        }
    },
    {
        path: '/loginForm',
        name: 'loginform',
        component: () => import('../components/LoginPage.vue'),
        meta: {
            title: 'Inicio de sesión',
            requiresAuth: true
        }
    },
    {
        path: '/password-recovery',
        name: 'passwordrecovery',
        component: () => import('../components/PasswordRecovery.vue'),
        meta: {
            title: 'Recuperación de contraseña',
            requiresAuth: true
        }
    },
    {
        path: '/register',
        name: 'registeraccount',
        component: () => import('../components/RegisterAccount.vue'),
        meta: {
            title: 'Creación de cuenta',
            requiresAuth: true
        }
    }

]

const router = createRouter({
    history: createWebHistory(import.meta.env.BASE_URL),
    routes,
    scrollBehavior(to, from, savedPosition) {
        if (savedPosition) {
            return savedPosition
        } else {
            return { top: 0 }
        }
    }
})

// Guards de navegación para títulos y autenticación
router.beforeEach((to, from, next) => {
    if (to.meta?.title) {
        document.title = to.meta.title as string
    }

    next()
})

export default router
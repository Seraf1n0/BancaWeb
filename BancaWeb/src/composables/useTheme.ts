import { ref, watch, onMounted } from 'vue'


type Theme = 'dark' | 'light'

// Puede ser depende del navegador o preferencia de usuario
const theme = ref<Theme>('dark')

export function useTheme() {
    const toggleTheme = () => {
        theme.value = theme.value === 'dark' ? 'light' : 'dark'
        applyTheme()
        saveTheme()
    }

    const applyTheme = () => {
        const root = document.documentElement

        if (theme.value === 'dark') {
            // Variables para tema oscuro
            root.style.setProperty('--bg-primary', '#1a1a1a')
            root.style.setProperty('--bg-secondary', '#2d2d2d')
            root.style.setProperty('--bg-tertiary', '#404040')
            root.style.setProperty('--bg-card', '#1a1a1a')
            root.style.setProperty('--bg-modal', '#2d2d2d')

            root.style.setProperty('--text-primary', '#ffffff')
            root.style.setProperty('--text-secondary', '#b0b0b0')
            root.style.setProperty('--text-muted', '#808080')

            root.style.setProperty('--border-primary', '#404040')
            root.style.setProperty('--border-secondary', '#606060')
            root.style.setProperty('--border-focus', '#0066cc')

            root.style.setProperty('--accent-primary', '#0066cc')
            root.style.setProperty('--accent-hover', '#0052a3')
            root.style.setProperty('--success', '#22c55e')
            root.style.setProperty('--warning', '#f59e0b')
            root.style.setProperty('--error', '#ef4444')

            root.style.setProperty('--shadow-card', '0 10px 25px rgba(0, 0, 0, 0.2)')
            root.style.setProperty('--shadow-modal', '0 10px 25px rgba(0, 0, 0, 0.5)')

            root.setAttribute('data-theme', 'dark')
        } else {
            // Variables para tema claro
            root.style.setProperty('--bg-primary', '#ffffff')
            root.style.setProperty('--bg-secondary', '#f8fafc')
            root.style.setProperty('--bg-tertiary', '#e2e8f0')
            root.style.setProperty('--bg-card', '#ffffff')
            root.style.setProperty('--bg-modal', '#ffffff')

            root.style.setProperty('--text-primary', '#1f2937')
            root.style.setProperty('--text-secondary', '#4b5563')
            root.style.setProperty('--text-muted', '#6b7280')

            root.style.setProperty('--border-primary', '#e5e7eb')
            root.style.setProperty('--border-secondary', '#d1d5db')
            root.style.setProperty('--border-focus', '#0066cc')

            root.style.setProperty('--accent-primary', '#0066cc')
            root.style.setProperty('--accent-hover', '#0052a3')
            root.style.setProperty('--success', '#22c55e')
            root.style.setProperty('--warning', '#f59e0b')
            root.style.setProperty('--error', '#ef4444')

            root.style.setProperty('--shadow-card', '0 4px 12px rgba(0, 0, 0, 0.1)')
            root.style.setProperty('--shadow-modal', '0 10px 25px rgba(0, 0, 0, 0.15)')

            root.setAttribute('data-theme', 'light')
        }
    }

    const saveTheme = () => {
        localStorage.setItem('banca-web-theme', theme.value)
    }

    const loadTheme = () => {
        const savedTheme = localStorage.getItem('banca-web-theme') as Theme
        if (savedTheme && ['dark', 'light'].includes(savedTheme)) {
            theme.value = savedTheme
        }
        applyTheme()
    }

    const initTheme = () => {
        loadTheme()
    }

    return {
        theme: theme.value,
        toggleTheme,
        initTheme,
        applyTheme,
        isLight: theme.value === 'light',
        isDark: theme.value === 'dark'
    }
}
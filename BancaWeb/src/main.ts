import { createApp } from 'vue'
import App from './App.vue'
import router from './router/index'
import "@fortawesome/fontawesome-free/css/all.min.css"
import { useTheme } from './composables/useTheme'


const app = createApp(App)

app.use(router)
const { initTheme } = useTheme()
initTheme()

app.mount('#app')

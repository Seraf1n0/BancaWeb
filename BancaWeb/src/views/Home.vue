<template>
    <div class="home-container">
        <section class="welcome-section" role="banner" aria-labelledby="welcome-title">
            <div class="welcome-content">
                <h1 id="welcome-title" class="welcome-title">
                    Bienvenido a Banca Prometedores
                </h1>
                <p class="welcome-subtitle">
                    Gestiona tus finanzas de manera segura y eficiente
                </p>
            </div>
        </section>

        <!-- Dashboard principal -->
        <Dashboard />
    </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import Dashboard from '../components/Dashboard.vue'

onMounted(() => {
    console.log('Home page mounted')
})
</script>

<style scoped>
.home-container {
    min-height: 100vh;
    background-color: var(--bg-primary);
}

.welcome-section {
    background: linear-gradient(135deg, var(--bg-secondary) 0%, var(--bg-tertiary) 100%);
    padding: 2rem 1rem;
    border-bottom: 1px solid var(--border-primary);
    transition: all 0.3s ease;
}

.welcome-content {
    max-width: 1200px;
    margin: 0 auto;
    text-align: center;
}

.welcome-title {
    margin: 0 0 0.5rem 0;
    font-size: 1.75rem;
    font-weight: 700;
    color: var(--text-primary);
    letter-spacing: -0.025em;
}

.welcome-subtitle {
    margin: 0;
    font-size: 1rem;
    color: var(--text-secondary);
    font-weight: 400;
    transition: color 0.3s ease;
}

@media (min-width: 768px) {
    .welcome-section {
        padding: 3rem 2rem;
    }

    .welcome-title {
        font-size: 2.25rem;
    }

    .welcome-subtitle {
        font-size: 1.125rem;
    }
}

@media (min-width: 1024px) {
    .welcome-title {
        font-size: 2.5rem;
    }
}
</style>
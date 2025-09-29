<template>
  <button
    class="theme-toggle"
    @click="toggleTheme"
    :aria-label="isDark ? 'Cambiar a tema claro' : 'Cambiar a tema oscuro'"
    type="button"
  >
    <i :class="isDark ? 'fa-solid fa-sun' : 'fa-solid fa-moon'"></i>
    <span class="theme-text">{{ isDark ? 'Claro' : 'Oscuro' }}</span>
  </button>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useTheme } from '../composables/useTheme'

const isDark = ref(true)

const { toggleTheme: toggle } = useTheme()

const toggleTheme = () => {
  toggle()
  isDark.value = !isDark.value
}

onMounted(() => {
  // Se inicializa el estado en un local storage por defecto dark
  const currentTheme = localStorage.getItem('banca-web-theme') || 'dark'
  isDark.value = currentTheme === 'dark'
})
</script>

<style scoped>
.theme-toggle {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem 0.75rem;
  background: var(--bg-tertiary);
  border: 1px solid var(--border-primary);
  border-radius: 6px;
  color: var(--text-secondary);
  font-size: 0.875rem;
  cursor: pointer;
  transition: all 0.2s ease;
}

.theme-toggle:hover {
  background: var(--bg-secondary);
  color: var(--text-primary);
  border-color: var(--border-secondary);
}

.theme-toggle:focus {
  outline: 2px solid var(--border-focus);
  outline-offset: 2px;
}

.theme-text {
  font-weight: 500;
}

@media (max-width: 640px) {
  .theme-text {
    display: none;
  }

  .theme-toggle {
    padding: 0.5rem;
    border-radius: 50%;
    width: 40px;
    height: 40px;
    justify-content: center;
  }
}
</style>

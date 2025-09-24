<template>
  <div id="app">
    <!-- Aquí se renderizan todas las rutas -->
    <router-view />
  </div>
</template>

<script setup lang="ts">
// Puedes agregar lógica global aquí si es necesaria
// Por ejemplo: configuración de temas, estados globales, etc.
</script>

<style>
/* Estilos globales para toda la aplicación */
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

html,
body {
  height: 100%;
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;
}

#app {
  min-height: 100vh;
}

/* Estilos globales para accesibilidad */
:focus-visible {
  outline: 2px solid #0066cc;
  outline-offset: 2px;
}

/* Scroll suave */
html {
  scroll-behavior: smooth;
}
</style>
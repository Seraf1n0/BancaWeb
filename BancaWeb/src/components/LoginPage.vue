<script setup lang="ts">
import LoginForm from './LoginForm.vue'
import PasswordRecovery from './PasswordRecovery.vue'
</script>

<template>
  <div class="login-page-container">
    <h1>Inicia sesión en tu cuenta</h1>
    <LoginForm />
    <p>
      ¿Aún no tienes cuenta?
      <RouterLink to="/register">Registrate aquí</RouterLink>
    </p>
    <RouterLink to="/password-recovery">¿Olvidó su contraseña?</RouterLink>
  </div>
</template>

<style scoped>
.login-page-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: 100vh;
  padding: 20px;
  color: white;
  background: -webkit-linear-gradient(
    90deg,
    #0d0d0d,
    #0a2330,
    #4b495f
  ); /* Chrome 10-25, Safari 5.1-6 */
  background: linear-gradient(
    90deg,
    #0d0d0d,
    #0a2330,
    #4b495f
  ); /* W3C, IE 10+/ Edge, Firefox 16+, Chrome 26+, Opera 12+, Safari 7+ */
}
</style>

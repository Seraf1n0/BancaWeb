<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router'


const username = ref('');
const password = ref('');
const router = useRouter()

const submitData = () => {
  console.log('Datos listos para enviar:', {
    username: username.value,
    password: password.value
  });
  router.push('/home')};
</script>




<template>
    <form @submit.prevent="submitData" class="login-form">
        <div class="form-group">
            <label for="user">Nombre de usuario</label>
            <input id="user" type="text" v-model="username" required class="input-field" minlength="4" maxlength="20" 
              pattern="^[a-z0-9._-]{4,}$" >
        </div>
        <div class="form-group">
            <label for="password">Contraseña</label>
            <input id="password" type="password" v-model="password" required class="input-field" 
            minlength="8" pattern="^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d).{8,}$.">
        </div>
        <button type="submit" class="btn-login">Iniciar sesión</button>
    </form>
    

</template>

<style scoped>
.login-form {
  display: flex;
  flex-direction: column;
  gap: 15px;
width: 90%;
  max-width: 100%;
}

.form-group {
  display: flex;
  flex-direction: column;
  align-items: center;
}

.input-field{
    padding: 10px;
    width: 100%;
    max-width: 100%;
    border-radius: 5px;
    height: auto;
}
.btn-login{
  background-color: #4CAF50;
  color: white;
  padding: 10px;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  width: 100%;
  max-width: 100%;
}

.btn-login:hover {
  background-color: #45a049;
}
</style>
<script setup>
import { ref, computed } from 'vue';
import { useRouter } from 'vue-router'
import CustomAlert from './CustomAlert.vue'

const username = ref('');
const password = ref('');
const router = useRouter()


const customAlert = ref({
  show: false,
  type: 'success',
  message: '',
  autoClose: false,
  duration: 4000
});

const showAlert = (type, message, autoClose = true, duration = 4000) => {
  customAlert.value = { show: true, type, message, autoClose, duration };
}

const rigthPassword = computed(() => {
  const regex = /^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).+$/;
  return password.value.length >= 8 && regex.test(password.value);
});


const submitData = () => {
  if (username.value && password.value) {
    showAlert("loading", "🕐 Verificando credenciales...", false);
    setTimeout(() => {
      showAlert("success", "✅ Inicio de sesión exitoso");
      setTimeout(() => {
        router.push('/home');
      }, 2000);
    }, 3000);
  } else {
    showAlert("error", "❌ Por favor, corrija las credenciales ingresadas.");
  }
};

</script>




<template>
    <form @submit.prevent="submitData" class="login-form">
        <div class="form-group">
            <label for="user">Nombre de usuario</label>
            <input id="user" type="text" v-model="username" required class="input-field" minlength="4" maxlength="20" 
              pattern="^[a-z0-9._-]{4,20}$" >
        </div>
        <div class="form-group">
            <label for="password">Contraseña</label>
            <input id="password" type="password" v-model="password" required class="input-field" 
            minlength="8">
        </div>
        <CustomAlert
          :show="customAlert.show"
          :type="customAlert.type"
          :message="customAlert.message"
          :autoClose="customAlert.autoClose"
          :duration="customAlert.duration"
          @close="customAlert.show = false"
        />
        <div v-if="password && !rigthPassword" class="error-message">
          La contraseña debe tener al menos 8 caracteres, una mayúscula, una minúscula y un número.
        </div>
        <button type="submit" class="btn-login" :disabled="!rigthPassword">Iniciar sesión</button>
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

@media (max-width: 479px) {
  .login-form {
    width: 80%;
  }
  .form-group{
    text-align: center;
  }


}


@media (min-width: 480px) and (max-width: 767px) {
  .login-form {
    width: 50%;
  }
}

@media (min-width: 768px) and (max-width: 1023px) {
  .login-form {
    width: 45%; 
  }
}



@media (min-width: 1024px) {
  .login-form {
    width: 35%; 
  }
}
</style>
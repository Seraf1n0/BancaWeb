<script setup>
import { ref } from 'vue'
import Swal from 'sweetalert2'
import { useRouter } from 'vue-router'

const username = ref('')
const verificationCode = ref('')
const showCodeInput = ref(false)
const showUserRecuperation = ref(true)
const newpassword = ref('')
const showNewPassword = ref(false)
const router = useRouter()

const submitData = () => {
  console.log('Datos listos para enviar:', {
    username: username.value,
  })
  showUserRecuperation.value = false
  showCodeInput.value = true
  showNewPassword.value = false
}

const submitCode = () => {
  console.log('Datos listos para enviar:', {
    verificationCode: verificationCode.value,
  })
  showUserRecuperation.value = false
  showCodeInput.value = false
  showNewPassword.value = true
}

const changePassword = () => {
  console.log('Datos listos para enviar:', {
    newpassword: newpassword.value,
  })
  Swal.fire({
    title: 'Contraseña restablecida',
    text: 'Tu contraseña fue actualizada correctamente',
    icon: 'success',
    confirmButtonText: 'Continuar',
  }).then(() => {
    router.push('/loginForm')
  })
}
</script>

<template>
  <div class="wrap">
    <form @submit.prevent="submitData" class="login-form" v-show="showUserRecuperation">
      <h2>Escriba su usuario</h2>
      <p>
        El usuario que indique a continuación tiene que ser con el cual fue creado la cuenta, de lo
        contrario no sera posible recuperar la contraseña
      </p>
      <input
        v-model="username"
        type="text"
        id="username"
        required
        class="input-username"
        placeholder="Username"
      />
      <button type="submit" class="btn-submit" aria-label="Recuperar contraseña">
        Recuperar contraseña
      </button>
    </form>

    <div class="code-input" v-show="showCodeInput">
      <form @submit.prevent="submitCode">
        <p>Ingresa el código que recibiste:</p>
        <input
          type="text"
          placeholder="Código de verificación"
          class="input-verificationCode"
          v-model="verificationCode"
          required
        />
        <button class="btn-validate">Validar código</button>
      </form>
    </div>

    <div class="new-password" v-show="showNewPassword">
      <form @submit.prevent="changePassword">
        <h2>Nueva contraseña</h2>
        <p>
          Recuerde que su nueva contraseña debe tener minimo 8 caracteres, al menos 1 mayúscula, 1
          minúscula y 1 digito
        </p>
        <input
          v-model="newpassword"
          type="password"
          id="newpassword"
          class="input-password"
          placeholder="Nueva contraseña"
          required
          minlength="8"
          pattern="^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).+$"
        />
        <button class="btn-password">Restablecer contraseña</button>
      </form>
    </div>
  </div>
</template>

<style scoped>
.login-form {
  display: flex;
  flex-direction: column;
  gap: 15px;
  width: 90%;
  max-width: 100%;
}

.wrap {
  display: flex;
  flex-direction: column;
  align-items: center;
  background: -webkit-linear-gradient(90deg, #0d0d0d, #0a2330, #4b495f);
  background: linear-gradient(90deg, #0d0d0d, #0a2330, #4b495f);
  min-height: 100vh;
}

.wrap h2 {
  color: white;
  margin: 0 auto;
}

.wrap p {
  color: white;
  margin-bottom: 10px;
  letter-spacing: 2px;
  width: 50%;
  max-width: 100%;
  font-size: small;
  text-align: center;
  margin: 0 auto;
}

.input-username {
  border: 1.5px solid #000;
  border-radius: 0.5rem;
  box-shadow: 2.5px 3px 0 #000;
  outline: none;
  transition: ease 0.25s;
  margin-bottom: 10px;
  padding: 0.5rem;
  width: 80%;
  font-size: 1rem;
  margin: 0 auto;
}

.btn-submit {
  background-color: #4caf50;
  color: white;
  padding: 10px;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  width: 90%;
  max-width: 100%;
  margin: 0 auto;
  display: block;
}

.code-input p {
  margin-bottom: 20px;
  letter-spacing: 2px;
  width: 70%;
  margin: 0 auto;
  display: block;
  font-size: large;
}

.btn-validate {
  background-color: #4caf50;
  color: white;
  padding: 10px;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  width: 90%;
  max-width: 100%;
  margin: 0 auto;
  display: block;
}

.btn-validate:hover {
  background-color: #45a049;
}

.input-verificationCode {
  border: 1.5px solid #000;
  border-radius: 0.5rem;
  box-shadow: 2.5px 3px 0 #000;
  outline: none;
  transition: ease 0.25s;
  padding: 0.5rem;
  width: 80%;
  font-size: 1rem;
  margin: 0 auto;
  display: block;
  margin-bottom: 10px;
}

.new-password h2 {
  text-align: center;
}

.new-password p {
  color: white;
  margin-bottom: 10px;
  letter-spacing: 2px;
  width: 50%;
  max-width: 100%;
  font-size: small;
  text-align: center;
  margin: 0 auto;
  margin-bottom: 10px;
}

.input-password {
  border: 1.5px solid #000;
  border-radius: 0.5rem;
  box-shadow: 2.5px 3px 0 #000;
  outline: none;
  transition: ease 0.25s;
  padding: 0.5rem;
  width: 80%;
  font-size: 1rem;
  margin: 0 auto;
  display: block;
  margin-bottom: 20px;
}

.btn-password {
  background-color: #4caf50;
  color: white;
  padding: 10px;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  width: 90%;
  max-width: 100%;
  margin: 0 auto;
  display: block;
}

.btn-password:hover {
  background-color: #45a049;
}
</style>

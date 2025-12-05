<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'

const username = ref('')
const password = ref('')
const router = useRouter()

const submitData = async () => {
  console.log('Contraseña enviada:', password.value)
  console.log('Nombre de usuario enviado:', username.value)
  const result = await login(username.value, password.value)
  console.log('Respuesta del backend:', result)
  if (result.success) {
    router.push('/home')
  } else {
    alert(result.message || 'Credenciales incorrectas')
  }
}

async function login(username: string, password: string) {
  const response = await fetch('http://86.48.22.73/api/v1/auth/login', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    credentials: 'include',
    body: JSON.stringify({ p_userName: username, p_password: password}),
  })
  const data = await response.json()
  return data
}

</script>

<template>
  <div class="login-container">
    <img class="logo-icon" src="@/imagenes/Prometedores.png" alt="logo" />
    
    <form @submit.prevent="submitData" class="login-form">
      <div class="form-group">
        <label for="user">Nombre de usuario</label>

        <div class="input-wrapper">
          <span class="icon">
            <svg width="20" height="20" viewBox="0 0 24 24" fill="none">
              <path d="M12 12c2.76 0 5-2.24 5-5s-2.24-5-5-5-5 
              2.24-5 5 2.24 5 5 5zm0 2c-3.31 0-10 1.66-10 
              5v3h20v-3c0-3.34-6.69-5-10-5z" fill="#666"/>
            </svg>
          </span>

          <input
            id="user"
            type="text"
            v-model="username"
            required
            minlength="4"
            maxlength="20"
            pattern="^[a-zA-Z0-9._\-]{4,}$"
            placeholder="Tu usuario"
          />
        </div>
      </div>

      <div class="form-group">
        <label for="password">Contraseña</label>

        <div class="input-wrapper">
          <span class="icon">
            <svg width="20" height="20" viewBox="0 0 24 24" fill="none">
              <path d="M17 8h-1V6c0-2.76-2.24-5-5-5S6 
              3.24 6 6v2H5c-1.1 0-2 .9-2 2v10c0 
              1.1.9 2 2 2h12c1.1 0 2-.9 
              2-2V10c0-1.1-.9-2-2-2zm-5 
              9c-1.1 0-2-.9-2-2s.9-2 
              2-2 2 .9 2 2-.9 2-2 
              2zm3-9H9V6c0-1.65 1.35-3 
              3-3s3 1.35 3 3v2z" fill="#666"/>
            </svg>
          </span>

          <input
            id="password"
            type="password"
            v-model="password"
            required
            minlength="8"
            pattern=".{6,}"
            placeholder="Tu contraseña"
          />
        </div>
      </div>

      <button type="submit" class="btn-login">Iniciar sesión</button>
    </form>
  </div>
</template>

<style scoped>
.login-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: 100vh;
  padding: 20px;
}

.logo-icon {
  width: 60%;
  max-width: 250px;
  height: auto;
  object-fit: contain;
  display: block;
  margin: 0 auto 30px auto;
  filter: drop-shadow(0px 4px 8px rgba(0,0,0,0.35));
}

.login-form {
  background: white;
  padding: 8%;
  border-radius: 18px;
  box-shadow: 0 4px 18px rgba(0, 0, 0, 0.15);
  width: 90%;
  max-width: 100%;
  display: flex;
  flex-direction: column;
  gap: 22px;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.form-group label {
  font-weight: 500;
  color: #333;
  font-size: 15px;
}

.input-wrapper {
  display: flex;
  align-items: center;
  background: #f1f1f1;
  border-radius: 10px;
  padding: 12px 14px;
  gap: 12px;
  border: 1px solid transparent;
  transition: all 0.25s;
  width: 100%;
}

.input-wrapper:focus-within {
  border-color: #4CAF50;
  background: #fff;
}

.icon {
  min-width: 22px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.input-wrapper input {
  flex: 1;
  border: none;
  outline: none;
  background: transparent;
  font-size: 16px;
  padding: 6px 0;
}

.btn-login {
  background-color: #4CAF50;
  color: white;
  padding: 14px;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  width: 100%;
  font-size: 17px;
  font-weight: 600;
  transition: 0.25s;
}

.btn-login:hover {
  background-color: #45a049;
}

@media (max-width: 479px) {
  .login-form {
    width: 85%;
    padding: 30px 20px;
  }
  
  .logo-icon {
    width: 65%;
    max-width: 100%;
    margin-bottom: 25px;
  }
  
  .form-group {
    text-align: center;
  }
}

@media (min-width: 480px) and (max-width: 767px) {
  .login-form {
    width: 70%;
    padding: 35px 28px;
  }
  
  .logo-icon {
    width: 55%;
    max-width: 100%;
  }
}

@media (min-width: 768px) and (max-width: 1023px) {
  .login-form {
    width: 55%;
    padding: 35px 30px;
  }
  
  .logo-icon {
    width: 45%;
    max-width: 100%;
  }
}

@media (min-width: 1024px) {
  .login-form {
    width: 55%;
    padding: 40px 35px;
  }
  
  .logo-icon {
    width: 25%;
    height: auto;
    max-width: 100%;
  }
}
</style>
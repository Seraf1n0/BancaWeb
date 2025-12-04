<script setup>
import { ref, computed } from 'vue';
import Swal from 'sweetalert2'
import { useRouter } from 'vue-router'
import CustomAlert from './CustomAlert.vue'



const username = ref('');
const verificationCode =  ref('')
const showCodeInput = ref(false)
const showUserRecuperation = ref(true)
const newpassword = ref('')
const showNewPassword = ref(false)
const router = useRouter()
const userId = ref('');



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

const isValidUsername = computed(() => {
  return /^[a-z0-9._-]{4,20}$/.test(username.value);
});

async function passwordRecovery(username) {
  const response = await fetch(`http://localhost:5015/api/v1/users/uuid?username=${username}`, {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
    },
    
  });
  const data = await response.json()
  return data
}


const submitData = async () => {
  console.log('Datos listos para enviar:', { username: username.value });
  showAlert("loading", "🕐 Enviando el usuario...");
  if (!isValidUsername.value) {
    setTimeout(() => {
      showAlert("error", "❌ El usuario debe tener entre 4 y 20 caracteres, y solo puede contener letras minúsculas, números, puntos, guiones y guiones bajos");
    }, 2000);
    return;
  }

  try {
    const response = await fetch(`http://localhost:5015/api/v1/users/uuid?username=${username.value}`);
    if (!response.ok) {
      showAlert("error", "❌ Usuario no encontrado");
      return;
    }
    const data = await response.json();
    if (!data.userId) {
      showAlert("error", "❌ Usuario no encontrado");
      return;
    }
    userId.value = data.userId;

    // Enviar POST para generar OTP
    const otpBody = {
      user_id: userId.value,
      proposito: "RecuperarContra",
      codigo_hash: "1256", // Puedes cambiar esto si quieres un código dinámico
      expiresInt: 10000
    };
    const otpResponse = await fetch('http://localhost:5015/api/v1/auth/forgot-password', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(otpBody)
    });
    const otpText = await otpResponse.text();
    console.log("Respuesta OTP:", otpText);
    if (!otpResponse.ok) {
      showAlert("error", "❌ Error al generar código de recuperación");
      return;
    }
    setTimeout(() => {
      showAlert("success", "✅ Código enviado al correo asociado");
    }, 2000);
    showUserRecuperation.value = false;
    showCodeInput.value = true;
    showNewPassword.value = false;
  } catch (error) {
     showAlert("error", "❌ Error de conexión: " + (error?.message || error));
      console.error("Error detalle:", error);
  }
};




const submitCode = async () => {
  console.log('Datos listos para enviar:', {
    verificationCode: verificationCode.value,
    userId: userId.value
  });
  
  if (verificationCode.value.length <= 1) {
    showAlert("error", "❌ Código inválido, intente nuevamente");
    return;
  }

  showAlert("loading", "🕐 Verificando código...", false);

  try {
    const otpBody = {
      user_id: userId.value,
      proposito: "RecuperarContra",
      codigo_hash: verificationCode.value
    };

    const response = await fetch('http://localhost:5015/api/v1/auth/verify-otp', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(otpBody)
    });

    if (!response.ok) {
      setTimeout(() => {
        showAlert("error", "❌ Código inválido o expirado");
      }, 1500);
      return;
    }

    setTimeout(() => {
      showAlert("success", "✅ Código verificado correctamente");
    }, 1500);

    showUserRecuperation.value = false;
    showCodeInput.value = false;
    showNewPassword.value = true;

  } catch (error) {
    showAlert("error", "❌ Error de conexión: " + (error?.message || error));
    console.error("Error detalle:", error);
  }
};

const rigthPassword = computed(() => {
  const regex = /^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).+$/;
  return newpassword.value.length >= 8 && regex.test(newpassword.value);
});

const changePassword = async () => {
  console.log('Nueva contraseña lista para enviar');
  
  if (!rigthPassword.value) {
    showAlert("error", "❌ La contraseña debe tener mínimo 8 caracteres, al menos 1 mayúscula, 1 minúscula y 1 dígito");
    return;
  }

  showAlert("loading", "🕐 Restableciendo contraseña...", false);

  try {
    const resetBody = {
      user_id: userId.value,
      proposito: "RecuperarContra",
      codigo_hash: verificationCode.value,
      nueva_contrasena: newpassword.value
    };

    const response = await fetch('http://localhost:5015/api/v1/auth/reset-password', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(resetBody)
    });

    if (!response.ok) {
      setTimeout(() => {
        showAlert("error", "❌ No se pudo restablecer la contraseña");
      }, 1500);
      return;
    }

    setTimeout(() => {
      showAlert("success", "✅ Contraseña restablecida correctamente");
    }, 1500);

    
    setTimeout(() => {
      router.push('/loginForm');
    }, 3000);

  } catch (error) {
    showAlert("error", "❌ Error de conexión: " + (error?.message || error));
    console.error("Error detalle:", error);
  }
};
</script>

  


<template>
    <div class="wrap"> 

        <CustomAlert
          :show="customAlert.show"
          :type="customAlert.type"
          :message="customAlert.message"
          :autoClose="customAlert.autoClose"
          :duration="customAlert.duration"
          @close="customAlert.show = false"
        />
        <form @submit.prevent="submitData" class="login-form" v-show="showUserRecuperation">
                <h2>Escriba su usuario</h2>
                <p>El usuario que indique a continuación tiene que ser con el cual 
                    fue creado la cuenta, de lo contrario no sera posible recuperar la contraseña
                </p>
                <input v-model="username" type="text" minlength="4" maxlength="20" id="username" required class="input-username" placeholder="Username">
                <button type="submit" class="btn-submit" aria-label="Recuperar contraseña">Recuperar contraseña</button>
        </form>

        <div class="code-input" v-show="showCodeInput">
            <form @submit.prevent="submitCode">
              <p>Ingresa el código que recibiste:</p>
              <input type="text" placeholder="Código de verificación" class="input-verificationCode" v-model="verificationCode" required />
              <button class="btn-validate">Validar código</button>
            </form>
        </div>

        <div class="new-password" v-show="showNewPassword">
          <form @submit.prevent="changePassword">
            <h2>Nueva contraseña</h2>
            <p>Recuerde que su nueva contraseña debe tener minimo 8 caracteres, al menos 1 mayúscula, 1 minúscula y 1 digito </p>
            <input v-model="newpassword" type="password" id="newpassword" class="input-password" placeholder="Nueva contraseña"  
            required minlength="8" />
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
  justify-content: center;
  background: -webkit-linear-gradient(90deg, #0d0d0d,#0a2330,#4b495f);
  background: linear-gradient(90deg, #0d0d0d,#0a2330,#4b495f);
  min-height: 100vh;
}

.wrap h2{
  color: white;
  margin: 0 auto;
}

.wrap p{
    color: white;
    margin-bottom: 10px;
    letter-spacing: 2px;
    width: 50%;
    max-width: 100%;
    font-size: small;
    text-align: center;
    margin: 0 auto;
}

.input-username{
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

.btn-submit{
  background-color: #4CAF50;
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


.code-input p{
  margin-bottom: 20px;
  letter-spacing: 2px;
  width: 70%;
  margin: 0 auto;
  display: block;
  font-size:large;
}

.btn-validate{
  background-color: #4CAF50;
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

.input-verificationCode{
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


.new-password h2{

  text-align: center;
}

.new-password p{
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

.input-password{
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

.btn-password{
  background-color: #4CAF50;
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


@media (max-width: 480px){
  .login-form p{
    max-width: 100%;
    font-size: 0.7rem;
  }
  .input-username{
    width: 80%;
  }

  .btn-submit{
    width: 80%;
  }

  .code-input {
    text-align: center;
  }
  .input-verificationCode{
    width: 80%;
    font-size: 0.8rem;

  }

  .btn-validate{
    width: 80%;
    font-size: 0.8rem;
  }

  .new-password h2{
      font-size: 1.3rem;
  } 

  .new-password p{
      font-size: 0.8rem;
  }

  .input-password{
    width: 80%;
    font-size: 0.8rem;
  }

  .btn-password{
    width: 80%;
    font-size: 0.8rem;
  }

}


@media (min-width: 480px) and (max-width: 767px){
  .login-form p{
    max-width: 100%;
    font-size: 0.9rem;
  }
  .input-username{
    width: 50%;
  }

  .btn-submit{
    width: 50%;
  }

  .input-verificationCode{
    width: 60%;
    font-size: 0.9rem;

  }

  .btn-validate{
    width: 60%;
    font-size: 0.9rem;  
  }

  .new-password h2{
      font-size: 1.4rem;
  } 

  .new-password p{
      font-size: 0.9rem;
  }

  .input-password{
    width: 45%;
    font-size: 0.9rem;
  }

  .btn-password{
    width: 45%;
    font-size: 0.9rem;
  }


}

@media (min-width: 768px) and (max-width: 1023px){
  .login-form p{
    max-width: 100%;
    font-size: 1.1rem;
  }
  .input-username{
    width: 35%;
  }

  .btn-submit{
    width: 35%;
  }

  .input-verificationCode{
    width: 69%;
    font-size: 1.0rem;

  }

  .btn-validate{
    width: 69%;
    font-size: 1.0rem;
  }

  .new-password h2{
      font-size: 1.6rem;
  } 

  .new-password p{
      font-size: 1rem;
  }

  .input-password{
    width: 35%;
    font-size: 1rem;
  }

  .btn-password{
    width: 35%;
    font-size: 1rem;
  }

}

@media (min-width: 1024px){
  .login-form p{
    max-width: 100%;
    font-size: 1.2rem;
  }
  .input-username{
    width: 25%;
  }

  .btn-submit{
    width: 25%;
  }

  .input-verificationCode{
    width: 75%;
    font-size: 1.1rem;

  }

  .btn-validate{
    width: 75%;
    font-size: 1.1rem;
  }

  .new-password h2{
      font-size: 1.8rem;
  } 

  .new-password p{
      font-size: 1.1rem;
  }

  .input-password{
    width: 40%;
    font-size: 1.1rem;
  }

  .btn-password{
    width: 40%;
    font-size: 1.1rem;
  }

}


</style>
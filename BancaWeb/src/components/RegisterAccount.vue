<script setup>
import { ref, computed } from 'vue';
import { useRouter } from 'vue-router'
import Swal from 'sweetalert2'
import CardModal from './CardModal.vue'
const showTermsModal = ref(false)
const pdfUrl = '/TCBancoPrometedores.pdf' 

const checkboxValue = ref('')
const username = ref('');
const firstPassword = ref('');
const identificationNumber = ref('');
const firstName = ref('')
const lastName1 = ref('')
const lastName2 = ref('')
const dateBirth = ref('')
const email = ref('')
const phoneNumber = ref('')
const router = useRouter()
const secondPassword = ref('')
const checkboxTerms= ref(false)

const submitData = () => {
  if (isValidId.value) {
    console.log('Datos listos para enviar:', {
      identificationNumber: identificationNumber.value,
      username: username.value,
      firstName: firstName.value,
      lastName1: lastName1.value,
      lastName2: lastName2.value,
      dateBirth: dateBirth.value,
      email: email.value,
      phoneNumber: phoneNumber.value,
      password: firstPassword.value,
    });
    Swal.fire({
        title: 'Registro exitoso',
        text: 'Su cuenta ha sido creada exitosamente',
        icon: 'success',
        confirmButtonText: 'Continuar'
    }).then(() =>{
        router.push('/loginForm')
    })
  } else {
    Swal.fire({
        title: 'Error en el formulario',
        text: 'Por favor, corrija los errores antes de enviar el formulario.',
        icon: 'error',
        confirmButtonText: 'Aceptar'
    });
  }
};




const isAdult = computed(() => {
  if (!dateBirth.value) return true;
  const birth = new Date(dateBirth.value);
  const today = new Date();
  let age = today.getFullYear() - birth.getFullYear();
  const m = today.getMonth() - birth.getMonth();
  if (m < 0 || (m === 0 && today.getDate() < birth.getDate())) {
    age--;
  }
  return age >= 18;
});

const inputPattern = computed(() => {
  if (checkboxValue.value === 'option1') {
    return /^[1-7]\d{8}$/; // Nacional
  }
  if (checkboxValue.value === 'option2') {
    return /^[1-9]\d{10,11}$/; // DIMEX
  }
  if (checkboxValue.value === 'option3') {
    return /^[A-Z]\d{6,12}$/; // Pasaporte
  }
  return null;
});


const isValidId = computed(() => {
  if (!checkboxValue.value || !identificationNumber.value) return false;
  return inputPattern.value?.test(identificationNumber.value) || false;
});


const inputClass = computed(() => {
  if (!identificationNumber.value) return 'input-id';
  return isValidId.value ? 'input-id valid' : 'input-id invalid';
});

const idMinLength = computed(() => {
  if (checkboxValue.value === 'option1') return 9; // Nacional
  if (checkboxValue.value === 'option2') return 11; // DIMEX
  if (checkboxValue.value === 'option3') return 6; // Pasaporte 
  return 1;
});
const idMaxLength = computed(() => {
  if (checkboxValue.value === 'option1') return 9; // Nacional
  if (checkboxValue.value === 'option2') return 12; // DIMEX
  if (checkboxValue.value === 'option3') return 12; // Pasaporte 
  return 20;
});

const handleKeypress = (event) => {
  const char = event.key;
  
  if (event.ctrlKey || event.metaKey || 
      ['Backspace', 'Delete'].includes(char)) {
    return;
  }
  
  let allowedPattern;
  
  if (checkboxValue.value === 'option1') {
    
    if (identificationNumber.value.length === 0) {
      allowedPattern = /^[1-7]$/;
    } else {
      allowedPattern = /^[0-9]$/; 
    }
  } else if (checkboxValue.value === 'option2') {
    // DIMEX
    if (identificationNumber.value.length === 0) {
      allowedPattern = /^[1-9]$/; 
    } else {
      allowedPattern = /^[0-9]$/; 
    }
  } else if (checkboxValue.value === 'option3') {
    // Pasaporte
    allowedPattern = /^[A-Z0-9]$/;
  }
  
  if (allowedPattern && !allowedPattern.test(char)) {
    event.preventDefault(); 
  }
};


const fullName = computed(() => {
  return `${firstName.value} ${lastName1.value} ${lastName2.value}`.trim();
});

const isValidEmail = computed(() => {
    return /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/.test(email.value);
});

const onlyNumbers = (event) => {
  const char = event.key;
  if (char === 'Backspace') {
    return;
  }
  if (!/^[0-9]$/.test(char)) {
    event.preventDefault();
  }
}


const FormComplete = computed(() => {
  return isValidId.value && 
         username.value.trim() !== '' &&
         dateBirth.value !== '' &&
         email.value.trim() !== '' &&
         phoneNumber.value.trim() !== '' &&
         secondPassword.value.trim() !== '' &&
         checkboxTerms.value;
});

const samePassword = computed(() => {
  return firstPassword.value === secondPassword.value && firstPassword.value.trim() !== '';
});

const isValidUsername = computed(() => {
  return /^[a-z0-9._-]{4,20}$/.test(username.value);
});





</script>

<template>
    <form @submit.prevent="submitData">
        <div class="wrap">
      <div class="radio-group">
        <span><input type="radio" id="option1" class="input-checkbox" value="option1" v-model="checkboxValue"><label for="option1">Nacional</label></span>
        <span><input type="radio" id="option2" class="input-checkbox" value="option2" v-model="checkboxValue"><label for="option2">DIMEX</label></span>
        <span><input type="radio" id="option3" class="input-checkbox" value="option3" v-model="checkboxValue"><label for="option3">Pasaporte</label></span>
      </div>
            
        <input type="text" v-model="identificationNumber" :class="inputClass" required placeholder="Ingrese su id"
            :disabled="!checkboxValue" @keydown="handleKeypress"
            :minlength="idMinLength" :maxlength="idMaxLength"
        />
            
            <div v-if="identificationNumber && !isValidId" class="error-message">
                <span v-if="checkboxValue === 'option1'">Cédula nacional debe tener 9 dígitos y comenzar con 1-7</span>
                <span v-if="checkboxValue === 'option2'">DIMEX debe tener 11-12 dígitos y comenzar con 1-9</span>
                <span v-if="checkboxValue === 'option3'">Pasaporte debe comenzar con letra mayúscula seguida de 6-12 dígitos</span>
            </div>
            
            <div v-if="isValidId" class="success-message">
                ✓ Número de identificación válido
            </div>



            <input id="user" type="text" v-model="username" required class="input-field" minlength="4" maxlength="20"
            pattern="^[a-z0-9._-]{4,20}$" placeholder="Usuario">

            <div v-if="username && !isValidUsername" class="error-message">
                Usuario debe tener 4-20 caracteres y solo contener minúsculas, números y los símbolos ._-
            </div>

            <div v-if="username && isValidUsername" class="success-message">
                El usuario es válido
            </div>

            <input type="text" id="firstName" v-model="firstName" placeholder="Nombre">
            <input type="text" id="lastName1" v-model="lastName1" placeholder="Primer apellido">
            <input type="text" id="lastName2" v-model="lastName2" placeholder="Segundo apellido">

            <input type="date" id="datebirth" v-model="dateBirth" placeholder="Fecha de nacimiento" required >
            <div v-if="dateBirth && !isAdult" class="error-message">
            Debes tener al menos 18 años para registrarte.
            </div>
            <div v-if="dateBirth && isAdult" class="success-message">
            Eres mayor de edad.
            </div>

            <input type="text" id="email" v-model="email" placeholder="Correo electronico" required pattern="^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$">
            <div v-if="email && !isValidEmail" class="error-message">
                Correo electrónico no válido.
            </div>
            <div v-if="email && isValidEmail" class="success-message">
                Correo electrónico válido
            </div>

            <input type="text" id="phone" v-model="phoneNumber" placeholder="Numero de telefono" minlength="8" maxlength="8" pattern="^[0-9]{8}$" @keydown="onlyNumbers">

            <input id="password1" type="password" v-model="firstPassword" required class="input-field" 
            minlength="8" pattern="^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$" placeholder="Escriba la contraseña">            
            <input id="password2" type="password" v-model="secondPassword" required class="input-field" 
            minlength="8" pattern="^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$" placeholder="Escriba otra vez su contraseña">

            <div v-if="secondPassword && !samePassword" class="error-message">
            Las contraseñas no coinciden.
            </div>
            <div v-if="secondPassword && samePassword" class="success-message">
            Las contraseñas coinciden.
            </div>

            <button type="button" @click="showTermsModal = true" class="btn-terms">Ver términos y condiciones</button>
            <CardModal v-if="showTermsModal" @close="showTermsModal = false">
              <template #default>
                <iframe :src="pdfUrl" width="100%" height="650px" style="border:none;overflow-x:hidden;" scrolling="no"></iframe>
              </template>
            </CardModal>
            <div class="terms-row">
                <label for="terms" class="op1">He leído y acepto los términos y condiciones</label>
                <input type="checkbox" id="terms" class="input-checkbox" v-model="checkboxTerms" required>
            </div>


            
            <button type="submit" :disabled="!FormComplete" class="submit-btn">
                Enviar
            </button>
        </div>
    </form>
</template>

<style scoped>
.wrap {
    background: -webkit-linear-gradient(90deg, #0d0d0d,#0a2330,#4b495f);
    background: linear-gradient(90deg, #0d0d0d,#0a2330,#4b495f);
    min-height: 100vh; 
    padding: 2rem;
    display: flex;
    flex-direction: column;
    gap: 1rem;
}



.wrap label {
  color: white;
  margin-right: 0;
}

.input-id {
    padding: 0.5rem;
    border: 2px solid #ccc;
    border-radius: 4px;
    font-size: 1rem;
}

.input-id:disabled {
    background-color: #f5f5f5;
    cursor: not-allowed;
}

.input-id.valid {
    border-color: #28a745;
    background-color: #f8fff8;
}

.input-id.invalid {
    border-color: #dc3545;
    background-color: #fff8f8;
}

.error-message {
    color: #dc3545;
    font-size: 0.875rem;
    margin-top: 0.25rem;
}

.success-message {
    color: #28a745;
    font-size: 0.875rem;
    margin-top: 0.25rem;
}

.submit-btn {
    padding: 0.75rem 1.5rem;
    background-color: #007bff;
    color: white;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    font-size: 1rem;
    margin-top: 1rem;
}

.submit-btn:disabled {
    background-color: #6c757d;
    cursor: not-allowed;
}

.submit-btn:hover:not(:disabled) {
    background-color: #0056b3;
}


.wrap input {
    padding: 0.5rem;
    border: 2px solid #ccc;
    border-radius: 4px;
    font-size: 1rem;
    width: 100%;
    box-sizing: border-box;
}

.wrap input:focus {
    outline: none;
    border-color: #007bff;
}

.radio-group {
  display: flex;
  flex-direction: row;
  gap: 1.5rem;
  align-items: center;
}
.radio-group span {
  display: flex;
  align-items: center;
  gap: 0.1rem;
}

.op1 {
  display: inline-block;
  vertical-align: middle;
  margin-left: 0;
}
.terms-row {
  display: flex;
  align-items: center;
  gap: 0;
}

.input-checkbox {
  transform: scale(1.2);
  cursor: pointer;
}

.terms-row .input-checkbox {
  width: auto;
  margin-left: 0.4rem;
  margin-top: 5px;
}

.btn-terms{
    background-color: #4CAF50;
    color: white;
    padding: 10px;
    border: none;
    border-radius: 6px;
    cursor: pointer;
    width: 100%;
    max-width: 100%;
}

.btn-terms:hover {
    background-color: #45a049;
}

.submit-btn{
  background-color: #4CAF50;
  color: white;
  padding: 10px;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  width: 100%;
  max-width: 100%;
}

.submit-btn:hover {
  background-color: #45a049;
}

</style>
<script setup lang="ts">
import { ref } from "vue"
import Card from "./CreditCard.vue"
import Modal from "./CardModal.vue"

const cards = [
  { type: "Gold", number: "1111222233334444", valid: "12/26", owner: "Paulo Gonzales Maradona", pin: "1234", cvv: "999", currency: "USD", limit: 5000, balance: 1500  },
  { type: "Platinum", number: "5555666677778888", valid: "06/29", owner: "Benito Montes Lackwood", pin: "4321", cvv: "888", currency: "CRC", limit: 10000, balance: 2000  },
  { type: "Black", number: "9999000011112222", valid: "01/30", owner: "Neymar Santos Junior", pin: "0000", cvv: "777", currency: "USD", limit: 20000, balance: 7500  }
]

const movements = [
  { id: "mov001", card_id: "1111222233334444", date: "2025-09-25T12:00:00Z", type: "COMPRA", description: "Pago servicios", currency: "USD", amount: 200.50 },
  { id: "mov002", card_id: "1111222233334444", date: "2025-09-24T08:30:00Z", type: "PAGO", description: "Depósito nómina", currency: "USD", amount: 1500.00 },
  { id: "mov003", card_id: "5555666677778888", date: "2025-09-20T10:15:00Z", type: "COMPRA", description: "Supermercado", currency: "CRC", amount: 35000.75 }
]

// Estados del carrusel
const index = ref(0)

// Estados del modal de detalles
const isModalOpen = ref(false)
const selectedCard = ref(null)
const cardMovements = ref([])

// Estados del modal de PIN
const isPinModalOpen = ref(false)
const pinModalStep = ref(1) // 1: Verificación OTP, 2: Mostrar PIN
const otpCode = ref('')
const isLoadingOtp = ref(false)
const isLoadingPin = ref(false)
const otpError = ref('')
const pinVisible = ref(false)
const pinTimer = ref(null)
const remainingTime = ref(0)

// Funciones del carrusel
function nextCard() {
  if(index.value < cards.length - 1){
    index.value++
  }
}

function prevCard() {
  if(index.value > 0){
    index.value--
  }
}

// Funciones del modal de detalles
function openModal(card){
  selectedCard.value = {
    ...card,
    numberMasked: card.number.slice(0,4) + " **** **** " + card.number.slice(-4)
  }
  cardMovements.value = movements.filter(m => m.card_id === card.number)
  isModalOpen.value = true
}

function closeModal(){
  selectedCard.value = null
  cardMovements.value = []
  isModalOpen.value = false
}

// Funciones del modal de PIN
function openPinModal() {
  isPinModalOpen.value = true
  pinModalStep.value = 1
  resetPinModal()
}

function closePinModal() {
  isPinModalOpen.value = false
  resetPinModal()
}

function resetPinModal() {
  otpCode.value = ''
  otpError.value = ''
  pinVisible.value = false
  isLoadingOtp.value = false
  isLoadingPin.value = false
  pinModalStep.value = 1
  clearPinTimer()
}

function validateOtp() {
  if (!otpCode.value || otpCode.value.length !== 6) {
    otpError.value = 'Ingresa un código de 6 dígitos'
    return
  }

  isLoadingOtp.value = true
  otpError.value = ''

  // mocking para validación OTP
  setTimeout(() => {
    // codigos de ejemplo válidos
    const validCodes = ['123456', '000000', '111111']

    if (validCodes.includes(otpCode.value)) {
      // con esto vamos al siguiente paso
      pinModalStep.value = 2
      generatePin()
    } else {
      // OTP inválido
      otpError.value = 'El código no es válido o ha expirado.'
    }

    isLoadingOtp.value = false
  }, 2000)
}

function generatePin() {
  isLoadingPin.value = true

  // tiempo de espera para simular que se genera el pin
  setTimeout(() => {
    isLoadingPin.value = false
    pinVisible.value = true
    startPinTimer()
  }, 1500)
}

function startPinTimer() {
  remainingTime.value = 12

  pinTimer.value = setInterval(() => {
    remainingTime.value--

    if (remainingTime.value <= 0) {
      hidePinAndClose()
    }
  }, 1000)
}

function clearPinTimer() {
  if (pinTimer.value) {
    clearInterval(pinTimer.value)
    pinTimer.value = null
  }
  remainingTime.value = 0
}

function hidePinAndClose() {
  clearPinTimer()
  pinVisible.value = false

  // se cierra modal cuando se termina el tiempo
  setTimeout(() => {
    closePinModal()
  }, 500)
}

function copyPin() {
  if (selectedCard.value?.pin && pinVisible.value) {
    navigator.clipboard.writeText(selectedCard.value.pin)
      .then(() => {
        // esto debe de manejarse mejor en el caso funcional real
        console.log('PIN copiado al portapapeles')
      })
      .catch(err => {
        console.error('Error al copiar PIN:', err)
      })
  }
}

function resendOtp() {
  otpCode.value = ''
  otpError.value = ''

  // pronto aqui se envia a API para reenviar OTP
  console.log('Código reenviado')
}
</script>

<template>
  <div class="carousel-container">
    <div class="carousel-track" :style="{ transform: `translateX(${-index * 320}px)` }">
      <Card class="card" v-for="(card, i) in cards" :key="i" :type="card.type" :number="card.number" :valid="card.valid"
        :owner="card.owner" @click="openModal(card)" />
    </div>
    <button @click="prevCard" class="carousel-arrow left-arrow" :disabled="index === 0" aria-label="Tarjeta anterior">
      <i class="fa-solid fa-arrow-left"></i>
    </button>
    <button @click="nextCard" class="carousel-arrow right-arrow" :disabled="index === cards.length - 1"
      aria-label="Proxima tarjeta">
      <i class="fa-solid fa-arrow-right"></i>
    </button>

    <!-- Modal de detalles de tarjeta -->
    <Modal v-if="isModalOpen" @close="closeModal">
      <div class="card-detail">
        <h2>Información de la Tarjeta</h2>
        <p><b>Tipo:</b> {{ selectedCard.type }}</p>
        <p><b>Número:</b> {{ selectedCard.numberMasked }}</p>
        <p><b>Exp:</b> {{ selectedCard.valid }}</p>
        <p><b>CVV:</b> {{ selectedCard.cvv }}</p>
        <p><b>Titular:</b> {{ selectedCard.owner }}</p>
        <p><b>Moneda:</b> {{ selectedCard.currency }}</p>
        <p><b>Límite:</b> ${{ selectedCard.limit.toLocaleString() }}</p>
        <p><b>Saldo Usado:</b> ${{ selectedCard.balance.toLocaleString() }}</p>
        <p><b>Disponible:</b> ${{ (selectedCard.limit - selectedCard.balance).toLocaleString() }}</p>

        <!-- Botón para consultar PIN -->
        <div class="pin-section">
          <button class="btn-pin" @click="openPinModal" type="button">
            <i class="fa-solid fa-key"></i>
            Consultar PIN
          </button>
        </div>

        <h3>Últimos Movimientos</h3>
        <div v-if="cardMovements.length === 0" class="no-movements">
          <p>No hay movimientos registrados</p>
        </div>
        <ul v-else class="movements">
          <li v-for="m in cardMovements" :key="m.id"
            :class="{ 'movement-buy': m.type === 'COMPRA', 'movement-pay': m.type === 'PAGO' }">
            <p><b>{{ m.type }}</b> - {{ m.description }}</p>
            <small>{{ new Date(m.date).toLocaleDateString('es-CR') }}</small>
            <p><b>Monto:</b> <span class="amount-value">{{ m.amount }} {{ m.currency }}</span></p>
          </li>
        </ul>
      </div>
    </Modal>

    <!-- Modal de consulta de PIN -->
    <Modal v-if="isPinModalOpen" @close="closePinModal">
      <div class="pin-modal">
        <!-- primer flujo: Verificación OTP -->
        <div v-if="pinModalStep === 1" class="pin-step">
          <div class="pin-header">
            <i class="fa-solid fa-shield-halved pin-icon"></i>
            <h2>Verificación de Identidad</h2>
            <p>Por seguridad, ingresa el código de verificación enviado a tu correo registrado.</p>
          </div>

          <div class="otp-form">
            <label for="otp-input" class="otp-label">Código de verificación</label>
            <input id="otp-input" v-model="otpCode" type="text" maxlength="6" placeholder="000000" class="otp-input"
              :class="{ 'error': otpError }" :disabled="isLoadingOtp" @input="otpError = ''" />

            <div v-if="otpError" class="error-message" role="alert">
              <i class="fa-solid fa-triangle-exclamation"></i>
              {{ otpError }}
            </div>

            <div class="otp-actions">
              <button class="btn-secondary" @click="resendOtp" :disabled="isLoadingOtp" type="button">
                Reenviar código
              </button>

              <button class="btn-primary" @click="validateOtp" :disabled="isLoadingOtp || otpCode.length !== 6"
                type="button">
                <span v-if="isLoadingOtp" class="loading-spinner"></span>
                {{ isLoadingOtp ? 'Validando...' : 'Continuar' }}
              </button>
            </div>
          </div>
        </div>

        <!-- Segundo flujo: Mostrar PIN -->
        <div v-else-if="pinModalStep === 2" class="pin-step">
          <div class="pin-header">
            <i class="fa-solid fa-key pin-icon success"></i>
            <h2>PIN de tu Tarjeta</h2>
            <p>Información confidencial de tu tarjeta {{ selectedCard?.type }}</p>
          </div>

          <!-- estado de carga -->
          <div v-if="isLoadingPin" class="pin-loading">
            <div class="loading-spinner large"></div>
            <p>Generando PIN seguro...</p>
          </div>

          <!-- pin visible temporalmente -->
          <div v-else-if="pinVisible" class="pin-display">
            <div class="pin-card">
              <div class="pin-info">
                <p><strong>Tarjeta:</strong> {{ selectedCard?.type }} **** {{ selectedCard?.number.slice(-4) }}</p>
                <p><strong>CVV:</strong> <span class="pin-value">{{ selectedCard?.cvv }}</span></p>
                <p><strong>PIN:</strong> <span class="pin-value highlight">{{ selectedCard?.pin }}</span></p>
              </div>

              <div class="pin-actions">
                <button class="btn-copy" @click="copyPin" type="button">
                  <i class="fa-solid fa-copy"></i>
                  Copiar PIN
                </button>
              </div>
            </div>

            <div class="pin-timer">
              <div class="timer-bar">
                <div class="timer-progress" :style="{ width: `${(remainingTime / 12) * 100}%` }"></div>
              </div>
              <p class="timer-text">
                <i class="fa-solid fa-clock"></i>
                PIN visible por {{ remainingTime }} segundos
              </p>
            </div>
          </div>

          <!-- PIN oculto -->
          <div v-else class="pin-hidden">
            <i class="fa-solid fa-eye-slash"></i>
            <p>PIN oculto por seguridad</p>
          </div>
        </div>
      </div>
    </Modal>
  </div>
</template>

<style scoped>
/* Estilos existentes del carrusel... */
.carousel-container {
  width: 20rem;
  margin: 0 auto;
  overflow: hidden;
  position: relative;
}

.carousel-arrow {
  position: absolute;
  top: 60%;
  transform: translateY(-50%);
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.95);
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.15);
  transition: transform 0.12s ease, box-shadow 0.12s ease;
  color: #333;
  cursor: pointer;
}

.carousel-arrow:hover {
  transform: translateY(-50%) scale(1.06);
  box-shadow: 0 10px 28px rgba(0, 0, 0, 0.18);
}

.left-arrow {
  left: 1%;
  width: 25px;
  height: 25px;
}

.right-arrow {
  right: 1%;
  width: 25px;
  height: 25px;
}

.carousel-track {
  display: flex;
  transition: transform 0.3s ease;
}

.card {
  width: 100%;
  height: 13rem;
  border-radius: 0.6rem;
  padding: 1.20rem;
  color: white;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.2);
  flex-shrink: 0;
  transition: transform 0.2s ease;
  cursor: pointer;
}

.card:active {
  transform: scale(1.05);
}

/* Estilos del modal de detalles */
.card-detail {
  background: white;
  border-radius: 12px;
  padding: 24px;
  max-width: 100%;
  margin: 0 auto;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.15);
}

.card-detail h2 {
  color: #1f2937;
  font-size: 24px;
  font-weight: 700;
  margin-bottom: 20px;
  text-align: center;
  border-bottom: 2px solid #e5e7eb;
  padding-bottom: 12px;
}

.card-detail p {
  margin: 12px 0;
  font-size: 16px;
  color: #374151;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 8px 12px;
  background: #f9fafb;
  border-radius: 6px;
  border-left: 4px solid #3b82f6;
}

.card-detail p b {
  color: #1f2937;
  font-weight: 600;
  min-width: 80px;
}

/* Sección del PIN */
.pin-section {
  margin: 24px 0;
  text-align: center;
  padding: 16px;
  background: linear-gradient(135deg, #f3f4f6 0%, #e5e7eb 100%);
  border-radius: 8px;
  border: 1px solid #d1d5db;
}

.btn-pin {
  background: linear-gradient(135deg, #3b82f6 0%, #1d4ed8 100%);
  color: white;
  border: none;
  padding: 12px 24px;
  border-radius: 8px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 8px;
  transition: all 0.2s ease;
  margin: 0 auto;
}

.btn-pin:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(59, 130, 246, 0.4);
}

.btn-pin:active {
  transform: translateY(0);
}

.card-detail h3 {
  color: #1f2937;
  font-size: 20px;
  font-weight: 600;
  margin: 24px 0 16px 0;
  text-align: center;
  position: relative;
}

.card-detail h3::after {
  content: '';
  position: absolute;
  bottom: -4px;
  left: 50%;
  transform: translateX(-50%);
  width: 60px;
  height: 3px;
  background: linear-gradient(90deg, #3b82f6, #1d4ed8);
  border-radius: 2px;
}

.no-movements {
  text-align: center;
  padding: 24px;
  color: #6b7280;
  background: #f9fafb;
  border-radius: 8px;
  border: 1px dashed #d1d5db;
}

.movements {
  list-style: none;
  padding: 0;
  margin: 0;
  max-height: 250px;
  overflow-y: auto;
}

.movements li {
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  padding: 16px;
  margin-bottom: 12px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  transition: transform 0.2s ease, box-shadow 0.2s ease;
}

.movements li:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

.movements li p:first-child {
  margin: 0 0 8px 0;
  font-size: 16px;
  color: #1f2937;
  background: none;
  border: none;
  padding: 0;
  display: block;
}

.movements li p:first-child b {
  color: #059669;
  background: #d1fae5;
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 14px;
  margin-right: 8px;
}

.movements li small {
  color: #6b7280;
  font-size: 12px;
  font-style: italic;
  display: block;
  margin-bottom: 8px;
}

.movement-buy p:first-child b {
  color: #dc2626 !important;
  background: #fef2f2 !important;
}

.movement-buy .amount-value::before {
  content: '-';
  color: #dc2626;
}

.movement-pay .amount-value::before {
  content: '+';
  color: #16a34a;
}

/* Estilos del modal de PIN */
.pin-modal {
  background: white;
  border-radius: 16px;
  overflow: hidden;
  max-width: 400px;
  width: 100%;
  margin: 0 auto;
}

.pin-step {
  padding: 24px;
}

.pin-header {
  text-align: center;
  margin-bottom: 24px;
}

.pin-icon {
  font-size: 48px;
  color: #3b82f6;
  margin-bottom: 16px;
  display: block;
}

.pin-icon.success {
  color: #10b981;
}

.pin-header h2 {
  color: #1f2937;
  font-size: 24px;
  font-weight: 700;
  margin: 0 0 8px 0;
}

.pin-header p {
  color: #6b7280;
  font-size: 14px;
  margin: 0;
  line-height: 1.5;
}

/* Formulario OTP */
.otp-form {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.otp-label {
  color: #374151;
  font-size: 14px;
  font-weight: 500;
  margin-bottom: 4px;
}

.otp-input {
  padding: 12px 16px;
  border: 2px solid #d1d5db;
  border-radius: 8px;
  font-size: 18px;
  text-align: center;
  letter-spacing: 2px;
  font-weight: 600;
  color: #1f2937;
  transition: border-color 0.2s ease;
}

.otp-input:focus {
  outline: none;
  border-color: #3b82f6;
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

.otp-input.error {
  border-color: #ef4444;
  box-shadow: 0 0 0 3px rgba(239, 68, 68, 0.1);
}

.otp-input:disabled {
  background-color: #f3f4f6;
  color: #9ca3af;
  cursor: not-allowed;
}

.error-message {
  color: #ef4444;
  font-size: 14px;
  display: flex;
  align-items: center;
  gap: 6px;
  background: #fef2f2;
  padding: 8px 12px;
  border-radius: 6px;
  border: 1px solid #fecaca;
}

.otp-actions {
  display: flex;
  gap: 12px;
  flex-direction: column;
}

.btn-primary,
.btn-secondary {
  padding: 12px 16px;
  border-radius: 8px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s ease;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  border: none;
}

.btn-primary {
  background: linear-gradient(135deg, #3b82f6 0%, #1d4ed8 100%);
  color: white;
}

.btn-primary:hover:not(:disabled) {
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(59, 130, 246, 0.4);
}

.btn-primary:disabled {
  background: #d1d5db;
  color: #9ca3af;
  cursor: not-allowed;
  transform: none;
  box-shadow: none;
}

.btn-secondary {
  background: transparent;
  color: #6b7280;
  border: 2px solid #e5e7eb;
}

.btn-secondary:hover:not(:disabled) {
  border-color: #d1d5db;
  color: #374151;
}

/* PIN Display */
.pin-loading {
  text-align: center;
  padding: 32px;
}

.pin-loading p {
  color: #6b7280;
  margin-top: 16px;
  font-size: 14px;
}

.pin-display {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.pin-card {
  background: linear-gradient(135deg, #f8fafc 0%, #e2e8f0 100%);
  border: 2px solid #cbd5e1;
  border-radius: 12px;
  padding: 20px;
}

.pin-info {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.pin-info p {
  margin: 0;
  display: flex;
  justify-content: space-between;
  align-items: center;
  color: #374151;
  font-size: 14px;
  background: none;
  padding: 8px 0;
  border-bottom: 1px solid #e5e7eb;
}

.pin-info p:last-child {
  border-bottom: none;
}

.pin-value {
  font-family: 'Courier New', monospace;
  font-weight: 700;
  font-size: 16px;
  color: #1f2937;
  background: white;
  padding: 4px 8px;
  border-radius: 4px;
  border: 1px solid #d1d5db;
}

.pin-value.highlight {
  color: #059669;
  background: #ecfdf5;
  border-color: #a7f3d0;
  font-size: 18px;
  padding: 6px 12px;
}

.pin-actions {
  margin-top: 16px;
  text-align: center;
}

.btn-copy {
  background: #10b981;
  color: white;
  border: none;
  padding: 10px 20px;
  border-radius: 6px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  display: inline-flex;
  align-items: center;
  gap: 6px;
  transition: background-color 0.2s ease;
}

.btn-copy:hover {
  background: #059669;
}

.pin-timer {
  text-align: center;
}

.timer-bar {
  width: 100%;
  height: 4px;
  background: #e5e7eb;
  border-radius: 2px;
  overflow: hidden;
  margin-bottom: 8px;
}

.timer-progress {
  height: 100%;
  background: linear-gradient(90deg, #ef4444 0%, #f59e0b 50%, #10b981 100%);
  transition: width 1s linear;
  border-radius: 2px;
}

.timer-text {
  color: #6b7280;
  font-size: 12px;
  margin: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 4px;
}

.pin-hidden {
  text-align: center;
  padding: 32px;
  color: #6b7280;
}

.pin-hidden i {
  font-size: 48px;
  margin-bottom: 16px;
  display: block;
}

/* Loading spinner */
.loading-spinner {
  width: 16px;
  height: 16px;
  border: 2px solid transparent;
  border-top: 2px solid currentColor;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

.loading-spinner.large {
  width: 32px;
  height: 32px;
  border-width: 3px;
}

@keyframes spin {
  0% {
    transform: rotate(0deg);
  }

  100% {
    transform: rotate(360deg);
  }
}

/* Responsive para móviles */
@media (max-width: 640px) {
  .pin-modal {
    margin: 0;
    border-radius: 16px 16px 0 0;
  }

  .otp-actions {
    flex-direction: column;
  }

  .pin-info p {
    flex-direction: column;
    align-items: flex-start;
    gap: 4px;
  }

  .pin-value {
    align-self: flex-end;
  }
}

@media (min-width: 641px) {
  .otp-actions {
    flex-direction: row;
  }

  .btn-secondary {
    order: 1;
  }

  .btn-primary {
    order: 2;
  }
}
</style>

<template>
  <div class="carousel-container">
    <div class="carousel-track" :style="{ transform: `translateX(${-index * 320}px)` }">
      <Card
        class="card"
        v-for="(card, i) in cards"
        :key="i"
        :type="card.type"
        :number="card.number"
        :valid="card.valid"
        :owner="card.owner"
        @click="openModal(card)"
      />
    </div>
    <button
      @click="prevCard"
      class="carousel-arrow left-arrow"
      :disabled="index === 0"
      aria-label="Tarjeta anterior"
    >
      <i class="fa-solid fa-arrow-left"></i>
    </button>
    <button
      @click="nextCard"
      class="carousel-arrow right-arrow"
      :disabled="index === cards.length - 1"
      aria-label="Proxima tarjeta"
    >
      <i class="fa-solid fa-arrow-right"></i>
    </button>

    <!-- Modal de detalles de tarjeta -->
    <Modal v-if="isModalOpen" @close="closeModal">
      <div class="modal-title" style="font-size: 1.2rem; font-weight: bold; margin-bottom: 1rem">
        Detalles de {{ selectedCard?.type }}
      </div>

      <!-- Información de la tarjeta -->
      <div class="card-info-section" role="region" aria-labelledby="card-info-title">
        <h3 id="card-info-title" class="section-title">Información de la tarjeta</h3>
        <div class="card-details-grid">
          <div class="detail-item">
            <span class="detail-label">Tipo:</span>
            <span class="detail-value">{{ selectedCard?.type }}</span>
          </div>
          <div class="detail-item">
            <span class="detail-label">Número:</span>
            <span class="detail-value">{{ selectedCard?.numberMasked }}</span>
          </div>
          <div class="detail-item">
            <span class="detail-label">Vencimiento:</span>
            <span class="detail-value">{{ selectedCard?.valid }}</span>
          </div>
          <div class="detail-item">
            <span class="detail-label">CVV:</span>
            <span class="detail-value">{{ selectedCard?.cvv }}</span>
          </div>
          <div class="detail-item">
            <span class="detail-label">Titular:</span>
            <span class="detail-value">{{ selectedCard?.owner }}</span>
          </div>
          <div class="detail-item">
            <span class="detail-label">Moneda:</span>
            <span class="detail-value">{{ selectedCard?.currency }}</span>
          </div>
          <div class="detail-item highlight">
            <span class="detail-label">Límite disponible:</span>
            <span class="detail-value balance">
              {{ currencySymbol
              }}{{ formatAmount((selectedCard?.limit ?? 0) - (selectedCard?.balance ?? 0)) }}
            </span>
          </div>
          <div class="detail-item">
            <span class="detail-label">Saldo utilizado:</span>
            <span class="detail-value">
              {{ currencySymbol }}{{ formatAmount(selectedCard?.balance || 0) }}
            </span>
          </div>
        </div>

        <!-- Botón PIN -->
        <div class="pin-section">
          <button class="btn-pin" @click="openPinModal" type="button">
            <i class="fa-solid fa-key"></i>
            Consultar PIN
          </button>
        </div>
      </div>

      <!-- Filtros para movimientos -->
      <div class="filters-section" role="region" aria-labelledby="filters-title">
        <h3 id="filters-title" class="section-title">Filtrar movimientos</h3>
        <div class="filters-grid">
          <div class="filter-group">
            <label for="type-filter" class="filter-label">Tipo de movimiento:</label>
            <select
              id="type-filter"
              v-model="movementFilters.tipo"
              class="filter-select"
              @change="applyMovementFilters"
            >
              <option value="">Todos</option>
              <option value="COMPRA">Compra</option>
              <option value="PAGO">Pago</option>
            </select>
          </div>
          <div class="filter-group">
            <label for="search-filter" class="filter-label">Buscar por descripción:</label>
            <input
              id="search-filter"
              type="text"
              v-model="movementFilters.descripcion"
              class="filter-input"
              placeholder="Buscar..."
              @input="applyMovementFilters"
            />
          </div>
        </div>
      </div>

      <!-- Lista de movimientos -->
      <div class="movements-section" role="region" aria-labelledby="movements-title">
        <h3 id="movements-title" class="section-title">
          Últimos movimientos
          <span class="movements-count">({{ filteredMovements.length }})</span>
        </h3>

        <!-- Estados: Loading, Error, Vacío -->
        <div v-if="isLoadingMovements" class="state-container" role="status" aria-live="polite">
          <div class="loading-spinner" aria-hidden="true"></div>
          <p class="state-message">Cargando movimientos...</p>
        </div>

        <div v-else-if="hasMovementsError" class="state-container error" role="alert">
          <p class="state-message">Error al cargar los movimientos</p>
          <button class="retry-btn" @click="loadCardMovements" type="button">
            Intentar de nuevo
          </button>
        </div>

        <div v-else-if="filteredMovements.length === 0" class="state-container empty">
          <p class="state-message">
            {{ emptyMovementsMessage }}
          </p>
          <button
            v-if="hasActiveMovementFilters"
            class="clear-filters-btn"
            @click="clearMovementFilters"
            type="button"
          >
            Limpiar filtros
          </button>
        </div>

        <!-- Lista de movimientos -->
        <div v-else class="movements-list" role="list" aria-label="Lista de movimientos">
          <div
            v-for="movement in paginatedMovements"
            :key="movement.id"
            class="movement-item"
            role="listitem"
          >
            <div class="movement-header">
              <div class="movement-type" :class="movement.type.toLowerCase()">
                <span class="type-indicator" :aria-label="`Movimiento de tipo ${movement.type}`">
                  {{ movement.type === 'COMPRA' ? '-' : '+' }}
                </span>
                <span class="type-text">{{ movement.type }}</span>
              </div>
              <div class="movement-date">
                {{ formatDate(movement.date) }}
              </div>
            </div>

            <div class="movement-content">
              <p class="movement-description">{{ movement.description }}</p>
              <div class="movement-amounts">
                <span class="movement-amount" :class="movement.type.toLowerCase()">
                  {{ movement.type === 'COMPRA' ? '-' : '+' }}{{ currencySymbol
                  }}{{ formatAmount(movement.amount) }}
                </span>
              </div>
            </div>
          </div>
        </div>

        <!-- Paginación -->
        <div v-if="filteredMovements.length > itemsPerPage" class="pagination">
          <button
            class="pagination-btn"
            :disabled="currentMovementPage === 1"
            @click="currentMovementPage--"
            type="button"
          >
            Anterior
          </button>
          <span class="pagination-info"
            >Página {{ currentMovementPage }} de {{ totalMovementPages }}</span
          >
          <button
            class="pagination-btn"
            :disabled="currentMovementPage === totalMovementPages"
            @click="currentMovementPage++"
            type="button"
          >
            Siguiente
          </button>
        </div>
      </div>
    </Modal>
    <Modal v-if="isPinModalOpen" @close="closePinModal">
      <div class="modal-title" style="font-size: 1.2rem; font-weight: bold; margin-bottom: 1rem">
        Consulta de PIN
      </div>

      <!-- Contenido del PIN modal igual que antes -->
      <div class="pin-modal-content">
        <!-- primer flujo: Verificación OTP -->
        <div v-if="pinModalStep === 1" class="pin-step">
          <div class="pin-header">
            <i class="fa-solid fa-shield-halved pin-icon"></i>
            <h4>Verificación de Identidad</h4>
            <p>Por seguridad, ingresa el código de verificación enviado a tu correo registrado.</p>
          </div>

          <div class="otp-form">
            <label for="otp-input" class="otp-label">Código de verificación</label>
            <input
              id="otp-input"
              v-model="otpCode"
              type="text"
              maxlength="6"
              placeholder="000000"
              class="otp-input"
              :class="{ error: otpError }"
              :disabled="isLoadingOtp"
              @input="otpError = ''"
            />

            <div v-if="otpError" class="error-message" role="alert">
              <i class="fa-solid fa-triangle-exclamation"></i>
              {{ otpError }}
            </div>

            <div class="otp-actions">
              <button
                class="btn-secondary"
                @click="resendOtp"
                :disabled="isLoadingOtp"
                type="button"
              >
                Reenviar código
              </button>

              <button
                class="btn-primary"
                @click="validateOtp"
                :disabled="isLoadingOtp || otpCode.length !== 6"
                type="button"
              >
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
            <h4>PIN de tu Tarjeta</h4>
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
                <div class="pin-info-item">
                  <span class="pin-label">Tarjeta:</span>
                  <span class="pin-value-text"
                    >{{ selectedCard?.type }} **** {{ selectedCard?.number.slice(-4) }}</span
                  >
                </div>
                <div class="pin-info-item">
                  <span class="pin-label">CVV:</span>
                  <span class="pin-value">{{ selectedCard?.cvv }}</span>
                </div>
                <div class="pin-info-item highlight">
                  <span class="pin-label">PIN:</span>
                  <span class="pin-value pin-highlight">{{ selectedCard?.pin }}</span>
                </div>
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
                <div
                  class="timer-progress"
                  :style="{ width: `${(remainingTime / 12) * 100}%` }"
                ></div>
              </div>
              <p class="timer-text">
                <i class="fa-solid fa-clock"></i>
                PIN visible por {{ remainingTime }} segundos
              </p>
            </div>
          </div>

          <!-- PIN oculto en caso de que no esté visible -->
          <div v-else class="pin-hidden">
            <i class="fa-solid fa-eye-slash"></i>
            <p>PIN oculto por seguridad</p>
          </div>
        </div>
      </div>
    </Modal>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import Card from './CreditCard.vue'
import Modal from './CardModal.vue'

const cards = [
  {
    type: 'Gold',
    number: '1111222233334444',
    valid: '12/26',
    owner: 'Paulo Gonzales Maradona',
    pin: '1234',
    cvv: '999',
    currency: 'USD',
    limit: 5000,
    balance: 1500,
  },
  {
    type: 'Platinum',
    number: '5555666677778888',
    valid: '06/29',
    owner: 'Benito Montes Lackwood',
    pin: '4321',
    cvv: '888',
    currency: 'CRC',
    limit: 10000,
    balance: 2000,
  },
  {
    type: 'Black',
    number: '9999000011112222',
    valid: '01/30',
    owner: 'Neymar Santos Junior',
    pin: '0000',
    cvv: '777',
    currency: 'USD',
    limit: 20000,
    balance: 7500,
  },
]

const movements = [
  {
    id: 'mov001',
    card_id: '1111222233334444',
    date: '2025-09-25T12:00:00Z',
    type: 'COMPRA',
    description: 'Pago servicios',
    currency: 'USD',
    amount: 200.5,
  },
  {
    id: 'mov002',
    card_id: '1111222233334444',
    date: '2025-09-24T08:30:00Z',
    type: 'PAGO',
    description: 'Depósito nómina',
    currency: 'USD',
    amount: 1500.0,
  },
  {
    id: 'mov003',
    card_id: '5555666677778888',
    date: '2025-09-20T10:15:00Z',
    type: 'COMPRA',
    description: 'Supermercado',
    currency: 'CRC',
    amount: 35000.75,
  },
]

// Estados del carrusel
const index = ref(0)

// Estados del modal de detalles
const isModalOpen = ref(false)
type Card = {
  type: string
  number: string
  valid: string
  owner: string
  pin: string
  cvv: string
  currency: string
  limit: number
  balance: number
  numberMasked?: string
}

const selectedCard = ref<Card | null>(null)
type Movement = {
  id: string
  card_id: string
  date: string
  type: string
  description: string
  currency: string
  amount: number
}

const cardMovements = ref<Movement[]>([])

// Estados del modal de PIN
const isPinModalOpen = ref(false)
const pinModalStep = ref(1)
const otpCode = ref('')
const isLoadingOtp = ref(false)
const isLoadingPin = ref(false)
const otpError = ref('')
const pinVisible = ref(false)
const pinTimer = ref<number | null>(null)
const remainingTime = ref(0)

// Estados para movimientos
const isLoadingMovements = ref(false)
const hasMovementsError = ref(false)
const currentMovementPage = ref(1)
const itemsPerPage = 10

// Filtros de movimientos
const movementFilters = ref({
  tipo: '',
  descripcion: '',
})

// Computed properties
const emptyMovementsMessage = computed(() => {
  if (cardMovements.value.length === 0) {
    return 'No hay movimientos registrados'
  } else if (filteredMovements.value.length === 0) {
    return 'No se encontraron movimientos con los filtros aplicados'
  }
  return ''
})

const currencySymbol = computed(() => {
  return selectedCard.value?.currency === 'USD' ? '$' : '₡'
})

const filteredMovements = computed(() => {
  let filtered = cardMovements.value

  if (movementFilters.value.tipo) {
    filtered = filtered.filter((m) => m.type === movementFilters.value.tipo)
  }

  if (movementFilters.value.descripcion) {
    const searchTerm = movementFilters.value.descripcion.toLowerCase()
    filtered = filtered.filter((m) => m.description.toLowerCase().includes(searchTerm))
  }

  return filtered
})

const totalMovementPages = computed(() => {
  return Math.ceil(filteredMovements.value.length / itemsPerPage)
})

const paginatedMovements = computed(() => {
  const start = (currentMovementPage.value - 1) * itemsPerPage
  const end = start + itemsPerPage
  return filteredMovements.value.slice(start, end)
})

const hasActiveMovementFilters = computed(() => {
  return movementFilters.value.tipo !== '' || movementFilters.value.descripcion !== ''
})

// Funciones del carrusel
function nextCard() {
  if (index.value < cards.length - 1) {
    index.value++
  }
}

function prevCard() {
  if (index.value > 0) {
    index.value--
  }
}

// Funciones del modal de detalles
function openModal(card: Card) {
  selectedCard.value = {
    ...card,
    numberMasked: card.number.slice(0, 4) + ' **** **** ' + card.number.slice(-4),
  }
  loadCardMovements()
  isModalOpen.value = true
}

function closeModal() {
  selectedCard.value = null
  cardMovements.value = []
  movementFilters.value = { tipo: '', descripcion: '' }
  currentMovementPage.value = 1
  isModalOpen.value = false
}

// Nuevos methods para movimientos
const formatAmount = (amount: number) => {
  return new Intl.NumberFormat('es-CR', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  }).format(amount)
}

const formatDate = (dateString: string) => {
  const date = new Date(dateString)
  return date.toLocaleDateString('es-CR', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  })
}

const loadCardMovements = async () => {
  if (!selectedCard.value) return

  isLoadingMovements.value = true
  hasMovementsError.value = false

  try {
    await new Promise((resolve) => setTimeout(resolve, 1000))
    cardMovements.value = movements.filter((m) => m.card_id === selectedCard.value!.number)
  } catch (error) {
    hasMovementsError.value = true
    console.error('Error loading movements:', error)
  } finally {
    isLoadingMovements.value = false
  }
}

const applyMovementFilters = () => {
  currentMovementPage.value = 1
}

const clearMovementFilters = () => {
  movementFilters.value = { tipo: '', descripcion: '' }
  currentMovementPage.value = 1
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
    navigator.clipboard
      .writeText(selectedCard.value.pin)
      .then(() => {
        // esto debe de manejarse mejor en el caso funcional real
        console.log('PIN copiado al portapapeles')
      })
      .catch((err) => {
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

<style scoped>
/* carousel */
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
  background: var(--bg-primary);
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.15);
  transition:
    transform 0.12s ease,
    box-shadow 0.12s ease;
  color: var(--text-primary);
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
  padding: 1.2rem;
  color: white;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  box-shadow: var(--shadow-card);
  flex-shrink: 0;
  transition: transform 0.2s ease;
  cursor: pointer;
}

.card:active {
  transform: scale(1.05);
}

/* estilos parecidos al modal de detalles */
.card-info-section,
.filters-section,
.movements-section {
  padding: 1.5rem;
  border-bottom: 1px solid var(--border-primary);
}

.movements-section {
  border-bottom: none;
}

.section-title {
  margin: 0 0 1rem 0;
  font-size: 1rem;
  font-weight: 600;
  color: var(--text-primary);
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.movements-count {
  font-size: 0.875rem;
  color: var(--text-muted);
  font-weight: 400;
}

/* Información de tarjeta */
.card-details-grid {
  display: grid;
  gap: 1rem;
  grid-template-columns: 1fr;
  margin-bottom: 1.5rem;
}

.detail-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.75rem;
  background-color: var(--bg-primary);
  border-radius: 6px;
  border: 1px solid var(--border-primary);
}

.detail-item.highlight {
  border-color: var(--border-focus);
  background-color: var(--bg-primary);
}

.detail-label {
  font-size: 0.875rem;
  color: var(--text-muted);
}

.detail-value {
  font-size: 0.875rem;
  color: var(--text-primary);
  font-weight: 500;
}

.detail-value.balance {
  font-size: 1.125rem;
  font-weight: 700;
  color: var(--text-success);
}

/* Sección del PIN */
.pin-section {
  text-align: center;
  padding: 1rem;
  background-color: var(--bg-primary);
  border-radius: 8px;
  border: 1px solid var(--border-primary);
}

.btn-pin {
  background: linear-gradient(135deg, #003d82 0%, #0052a3 100%);
  color: white;
  border: none;
  padding: 12px 24px;
  border-radius: 6px;
  font-size: 0.875rem;
  font-weight: 600;
  cursor: pointer;
  display: inline-flex;
  align-items: center;
  gap: 8px;
  transition: all 0.2s ease;
}

.btn-pin:hover {
  background: linear-gradient(135deg, #0052a3 0%, #003d82 100%);
}

.btn-pin:focus {
  outline: 2px solid var(--border-focus);
  outline-offset: 2px;
}

/* Filtros */
.filters-grid {
  display: grid;
  gap: 1rem;
  grid-template-columns: 1fr;
}

.filter-group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.filter-label {
  font-size: 0.875rem;
  color: var(--text-muted);
  font-weight: 500;
}

.filter-select,
.filter-input {
  padding: 0.75rem;
  background-color: var(--bg-secondary);
  border: 1px solid var(--border-primary);
  border-radius: 6px;
  color: var(--text-primary);
  font-size: 0.875rem;
}

.filter-select:focus,
.filter-input:focus {
  outline: 2px solid var(--border-focus);
  outline-offset: 2px;
  border-color: var(--border-focus);
}

.filter-input::placeholder {
  color: var(--text-muted);
}

/* Estados */
.state-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 3rem 2rem;
  text-align: center;
  background-color: var(--bg-primary);
  border-radius: 8px;
  border: 1px solid #404040;
}

.state-container.error {
  border-color: var(--error);
  background-color: rgba(239, 68, 68, 0.1);
}

.state-container.empty {
  border: 1px dashed var(--border-primary);
}

.loading-spinner {
  width: 32px;
  height: 32px;
  border: 3px solid var(--border-primary);
  border-top: 3px solid var(--accent-primary);
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin-bottom: 1rem;
}

@keyframes spin {
  0% {
    transform: rotate(0deg);
  }

  100% {
    transform: rotate(360deg);
  }
}

.state-message {
  margin: 0;
  color: var(--text-muted);
  font-size: 1rem;
}

.retry-btn,
.clear-filters-btn {
  margin-top: 1rem;
  padding: 0.75rem 1.5rem;
  background-color: var(--accent-primary);
  color: var(--text-primary);
  border: none;
  border-radius: 6px;
  font-size: 0.875rem;
  cursor: pointer;
  transition: background-color 0.2s ease;
}

.retry-btn:hover,
.clear-filters-btn:hover {
  background-color: var(--accent-hover);
}

/* Lista de movimientos */
.movements-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.movement-item {
  background-color: var(--bg-primary);
  border: 1px solid var(--border-primary);
  border-radius: 8px;
  padding: 1rem;
  transition: all 0.2s ease;
}

.movement-item:hover {
  border-color: var(--border-focus);
  background-color: var(--bg-hover);
}

.movement-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.75rem;
}

.movement-type {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.type-indicator {
  width: 24px;
  height: 24px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: bold;
  font-size: 0.875rem;
}

.movement-type.compra .type-indicator {
  background-color: rgba(239, 68, 68, 0.2);
  color: var(--error);
}

.movement-type.pago .type-indicator {
  background-color: rgba(34, 197, 94, 0.2);
  color: var(--success);
}

.type-text {
  font-size: 0.75rem;
  color: var(--text-muted);
  text-transform: capitalize;
}

.movement-date {
  font-size: 0.75rem;
  color: var(--text-muted);
}

.movement-content {
  display: flex;
  justify-content: space-between;
  align-items: flex-end;
  gap: 1rem;
}

.movement-description {
  margin: 0;
  font-size: 0.875rem;
  color: var(--text-primary);
  flex: 1;
}

.movement-amounts {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  gap: 0.25rem;
}

.movement-amount {
  font-size: 1rem;
  font-weight: 600;
}

.movement-amount.compra {
  color: var(--error);
}

.movement-amount.pago {
  color: var(--success);
}

/* Paginación */
.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 1rem;
  padding: 1.5rem;
  border-top: 1px solid var(--border-primary);
  margin-top: 1rem;
}

.pagination-btn {
  padding: 0.5rem 1rem;
  background-color: var(--bg-secondary);
  color: var(--text-primary);
  border: none;
  border-radius: 6px;
  font-size: 0.875rem;
  cursor: pointer;
  transition: background-color 0.2s ease;
}

.pagination-btn:hover:not(:disabled) {
  background-color: var(--bg-tertiary);
}

.pagination-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.pagination-info {
  font-size: 0.875rem;
  color: var(--text-muted);
}

/* PIN Modal Content */
.pin-modal-content {
  padding: 1.5rem;
}

.pin-step {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.pin-header {
  text-align: center;
}

.pin-icon {
  font-size: 2rem;
  color: var(--);
  margin-bottom: 1rem;
  display: block;
}

.pin-icon.success {
  color: #22c55e;
}

.pin-header h4 {
  margin: 0 0 0.5rem 0;
  font-size: 1.125rem;
  font-weight: 600;
  color: #ffffff;
}

.pin-header p {
  margin: 0;
  font-size: 0.875rem;
  color: #b0b0b0;
  line-height: 1.5;
}

/* Formulario OTP */
.otp-form {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.otp-label {
  color: #b0b0b0;
  font-size: 0.875rem;
  font-weight: 500;
}

.otp-input {
  padding: 0.75rem;
  border: 1px solid #404040;
  border-radius: 6px;
  font-size: 1.125rem;
  text-align: center;
  letter-spacing: 2px;
  font-weight: 600;
  color: #ffffff;
  background-color: #1a1a1a;
  transition: border-color 0.2s ease;
}

.otp-input:focus {
  outline: 2px solid #0066cc;
  outline-offset: 2px;
  border-color: #0066cc;
}

.otp-input.error {
  border-color: #ef4444;
}

.otp-input:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.error-message {
  color: #ef4444;
  font-size: 0.875rem;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  background-color: rgba(239, 68, 68, 0.1);
  padding: 0.75rem;
  border-radius: 6px;
  border: 1px solid rgba(239, 68, 68, 0.2);
}

.otp-actions {
  display: flex;
  gap: 0.75rem;
  flex-direction: column;
}

.btn-primary,
.btn-secondary {
  padding: 0.75rem 1rem;
  border-radius: 6px;
  font-size: 0.875rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s ease;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  border: none;
}

.btn-primary {
  background-color: #0066cc;
  color: white;
}

.btn-primary:hover:not(:disabled) {
  background-color: #0052a3;
}

.btn-primary:disabled {
  background-color: #404040;
  color: #808080;
  cursor: not-allowed;
}

.btn-secondary {
  background: transparent;
  color: #b0b0b0;
  border: 1px solid #404040;
}

.btn-secondary:hover:not(:disabled) {
  border-color: #606060;
  color: #ffffff;
}

/* PIN Display */
.pin-loading {
  text-align: center;
  padding: 2rem;
}

.pin-loading p {
  color: #b0b0b0;
  margin-top: 1rem;
  font-size: 0.875rem;
}

.pin-display {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.pin-card {
  background-color: #1a1a1a;
  border: 1px solid #404040;
  border-radius: 8px;
  padding: 1.5rem;
}

.pin-info {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.pin-info-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.75rem 0;
  border-bottom: 1px solid #404040;
}

.pin-info-item:last-child {
  border-bottom: none;
}

.pin-info-item.highlight {
  background-color: rgba(0, 102, 204, 0.1);
  padding: 0.75rem;
  border-radius: 6px;
  border: 1px solid #0066cc;
  margin: 0.5rem 0;
}

.pin-label {
  font-size: 0.875rem;
  color: #b0b0b0;
}

.pin-value-text {
  font-size: 0.875rem;
  color: #ffffff;
}

.pin-value {
  font-family: 'Courier New', monospace;
  font-weight: 700;
  font-size: 1rem;
  color: #ffffff;
  background-color: #2d2d2d;
  padding: 0.25rem 0.5rem;
  border-radius: 4px;
  border: 1px solid #404040;
}

.pin-highlight {
  color: #22c55e !important;
  background-color: rgba(34, 197, 94, 0.1) !important;
  border-color: #22c55e !important;
  font-size: 1.125rem !important;
  padding: 0.5rem 0.75rem !important;
}

.pin-actions {
  text-align: center;
}

.btn-copy {
  background-color: #22c55e;
  color: white;
  border: none;
  padding: 0.75rem 1.5rem;
  border-radius: 6px;
  font-size: 0.875rem;
  font-weight: 600;
  cursor: pointer;
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  transition: background-color 0.2s ease;
}

.btn-copy:hover {
  background-color: #16a34a;
}

.pin-timer {
  text-align: center;
}

.timer-bar {
  width: 100%;
  height: 4px;
  background: #404040;
  border-radius: 2px;
  overflow: hidden;
  margin-bottom: 0.5rem;
}

.timer-progress {
  height: 100%;
  background: linear-gradient(90deg, #ef4444 0%, #f59e0b 50%, #22c55e 100%);
  transition: width 1s linear;
  border-radius: 2px;
}

.timer-text {
  color: #b0b0b0;
  font-size: 0.75rem;
  margin: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.25rem;
}

.pin-hidden {
  text-align: center;
  padding: 2rem;
  color: #b0b0b0;
}

.pin-hidden i {
  font-size: 2rem;
  margin-bottom: 1rem;
  display: block;
}

.loading-spinner.large {
  width: 2rem;
  height: 2rem;
  border-width: 3px;
}

/* Responsive */
@media (min-width: 768px) {
  .card-details-grid {
    grid-template-columns: repeat(2, 1fr);
  }

  .filters-grid {
    grid-template-columns: 1fr 2fr;
  }

  .otp-actions {
    flex-direction: row;
  }
}

@media (max-width: 640px) {
  .movement-content {
    flex-direction: column;
    align-items: flex-start;
    gap: 0.75rem;
  }

  .movement-amounts {
    align-items: flex-start;
    width: 100%;
  }

  .pin-info-item {
    flex-direction: column;
    align-items: flex-start;
    gap: 0.5rem;
  }

  .pin-value {
    align-self: flex-end;
  }
}
</style>

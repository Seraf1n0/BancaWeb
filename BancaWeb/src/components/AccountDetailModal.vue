<template>
  <div v-if="show" class="modal-backdrop" @click.self="closeModal" role="dialog" aria-modal="true"
    aria-labelledby="account-detail-title">
    <div class="modal" @click.stop>
      <!-- Header del modal -->
      <div class="modal-header">
        <h2 id="account-detail-title" class="modal-title">Detalles de {{ account?.alias }}</h2>
        <button class="close-btn" @click="closeModal" aria-label="Cerrar detalles de cuenta" type="button">
          ×
        </button>
      </div>

      <!-- Información de la cuenta -->
      <div class="account-info-section" role="region" aria-labelledby="account-info-title">
        <h3 id="account-info-title" class="section-title">Información de la cuenta</h3>
        <div class="account-details-grid">
          <div class="detail-item">
            <span class="detail-label">Número de cuenta:</span>
            <span class="detail-value" :aria-label="`Número ${account?.account_id}`">
              {{ account?.account_id }}
            </span>
          </div>
          <div class="detail-item">
            <span class="detail-label">Tipo:</span>
            <span class="detail-value">{{ account?.tipo }}</span>
          </div>
          <div class="detail-item">
            <span class="detail-label">Moneda:</span>
            <span class="detail-value">{{ account?.moneda }}</span>
          </div>
          <div class="detail-item highlight">
            <span class="detail-label">Saldo actual:</span>
            <span class="detail-value balance" :class="balanceClass">
              {{ currencySymbol }}{{ formatAmount(account?.saldo || 0) }}
            </span>
          </div>
        </div>
      </div>

      <!-- Filtros para movimientos -->
      <div class="filters-section" role="region" aria-labelledby="filters-title">
        <h3 id="filters-title" class="section-title">Filtrar movimientos</h3>
        <div class="filters-grid">
          <div class="filter-group">
            <label for="type-filter" class="filter-label">Tipo de movimiento:</label>
            <select id="type-filter" v-model="filters.tipo" class="filter-select" @change="applyFilters">
              <option value="">Todos</option>
              <option value="CREDITO">Crédito</option>
              <option value="DEBITO">Débito</option>
            </select>
          </div>
          <div class="filter-group">
            <label for="search-filter" class="filter-label">Buscar por descripción:</label>
            <input id="search-filter" type="text" v-model="filters.descripcion" class="filter-input"
              placeholder="Buscar..." @input="applyFilters" />
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
        <div v-if="isLoading" class="state-container" role="status" aria-live="polite">
          <div class="loading-spinner" aria-hidden="true"></div>
          <p class="state-message">Cargando movimientos...</p>
        </div>

        <div v-else-if="hasError" class="state-container error" role="alert">
          <p class="state-message">Error al cargar los movimientos</p>
          <button class="retry-btn" @click="loadMovements" type="button">Intentar de nuevo</button>
        </div>

        <div v-else-if="filteredMovements.length === 0" class="state-container empty">
          <p class="state-message">
            {{ emptyMovementsMessage }}
          </p>
          <button v-if="hasActiveFilters" class="clear-filters-btn" @click="clearFilters" type="button">
            Limpiar filtros
          </button>
        </div>

        <!-- Lista de movimientos -->
        <div v-else class="movements-list" role="list" aria-label="Lista de movimientos">
          <div v-for="movement in paginatedMovements" :key="movement.id" class="movement-item" role="listitem">
            <div class="movement-header">
              <div class="movement-type" :class="movement.tipo.toLowerCase()">
                <span class="type-indicator" :aria-label="`Movimiento de tipo ${movement.tipo}`">
                  {{ movement.tipo === 'CREDITO' ? '+' : '-' }}
                </span>
                <span class="type-text">{{ movement.tipo }}</span>
              </div>
              <div class="movement-date">
                {{ formatDate(movement.fecha) }}
              </div>
            </div>

            <div class="movement-content">
              <p class="movement-description">{{ movement.descripcion }}</p>
              <div class="movement-amounts">
                <span class="movement-amount" :class="movement.tipo.toLowerCase()">
                  {{ movement.tipo === 'CREDITO' ? '+' : '-' }}{{ currencySymbol
                  }}{{ formatAmount(movement.monto) }}
                </span>
                <span class="movement-balance">
                  Saldo: {{ currencySymbol }}{{ formatAmount(movement.saldo) }}
                </span>
              </div>
            </div>
          </div>
        </div>

        <!-- Paginación simple -->
        <div v-if="filteredMovements.length > itemsPerPage" class="pagination">
          <button class="pagination-btn" :disabled="currentPage === 1" @click="currentPage--" type="button">
            Anterior
          </button>
          <span class="pagination-info"> Página {{ currentPage }} de {{ totalPages }} </span>
          <button class="pagination-btn" :disabled="currentPage === totalPages" @click="currentPage++" type="button">
            Siguiente
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import type { Account, Movement, MovementFilters } from '../types'

// Props
const props = defineProps<{
  show: boolean
  account: Account | null
}>()

// Emits
const emit = defineEmits<{
  close: []
}>()



// Estados
const movements = ref<Movement[]>([])
const isLoading = ref(false)
const hasError = ref(false)
const currentPage = ref(1)
const itemsPerPage = 10

// Filtros
const filters = ref<MovementFilters>({
  tipo: '',
  descripcion: '',
})

// Computed properties
const emptyMovementsMessage = computed(() => {
  if (movements.value.length === 0) {
    return 'No hay movimientos registrados'
  }
  if (filteredMovements.value.length === 0) {
    return 'No se encontraron movimientos con los filtros aplicados'
  }
  return ''
})

const currencySymbol = computed(() => {
  return props.account?.moneda === 'USD' ? '$' : '₡'
})

const balanceClass = computed(() => {
  const saldo = props.account?.saldo || 0
  if (saldo < 0) return 'balance-negative'
  if (saldo < 10000) return 'balance-low'
  return 'balance-positive'
})

const filteredMovements = computed(() => {
  let filtered = movements.value

  if (filters.value.tipo) {
    filtered = filtered.filter((m) => m.tipo === filters.value.tipo)
  }

  if (filters.value.descripcion) {
    const searchTerm = filters.value.descripcion.toLowerCase()
    filtered = filtered.filter((m) => m.descripcion.toLowerCase().includes(searchTerm))
  }

  return filtered
})

const totalPages = computed(() => {
  return Math.ceil(filteredMovements.value.length / itemsPerPage)
})

const paginatedMovements = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage
  const end = start + itemsPerPage
  return filteredMovements.value.slice(start, end)
})

const hasActiveFilters = computed(() => {
  return filters.value.tipo !== '' || filters.value.descripcion !== ''
})

// Methods
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

const loadMovements = async () => {
  if (!props.account) return

  isLoading.value = true
  hasError.value = false

  try {
    // Simular carga de datos
    await new Promise((resolve) => setTimeout(resolve, 1500))

    // Datos simulados de movimientos
    movements.value = generateMockMovements(props.account.account_id)
  } catch (error) {
    hasError.value = true
    console.error('Error loading movements:', error)
  } finally {
    isLoading.value = false
  }
}

const generateMockMovements = (accountId: string): Movement[] => {
  const mockMovements: Movement[] = []
  const descriptions = [
    'Pago de servicios públicos',
    'Depósito de nómina',
    'Transferencia recibida',
    'Compra en supermercado',
    'Retiro en cajero automático',
    'Pago de tarjeta de crédito',
    'Transferencia enviada',
    'Depósito en efectivo',
  ]

  let currentBalance = props.account?.saldo || 0

  for (let i = 0; i < 25; i++) {
    const isCredit = Math.random() > 0.6
    const amount = Math.floor(Math.random() * 50000) + 1000
    const finalAmount = isCredit ? amount : -amount

    currentBalance -= finalAmount // Calculamos hacia atrás

    mockMovements.unshift({
      id: `TXN-${Date.now()}-${i}`,
      account_id: accountId,
      fecha: new Date(Date.now() - i * 24 * 60 * 60 * 1000).toISOString(),
      tipo: isCredit ? 'CREDITO' : 'DEBITO',
      descripcion: descriptions[Math.floor(Math.random() * descriptions.length)],
      moneda: props.account?.moneda || 'CRC',
      monto: amount,
      saldo: currentBalance + finalAmount,
    })
  }

  return mockMovements
}

const applyFilters = () => {
  currentPage.value = 1
}

const clearFilters = () => {
  filters.value = {
    tipo: '',
    descripcion: '',
  }
  currentPage.value = 1
}

const closeModal = () => {
  emit('close')
}

// Watchers
watch(
  () => props.show,
  (newShow) => {
    if (newShow && props.account) {
      loadMovements()
      currentPage.value = 1
      clearFilters()
    }
  },
)

watch(
  () => filters.value,
  () => {
    applyFilters()
  },
  { deep: true },
)
</script>

<style scoped>
.modal-backdrop {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.7);
  display: flex;
  justify-content: center;
  align-items: flex-start;
  z-index: 2000;
  padding: 2rem 1rem;
  overflow-y: auto;
}

.modal {
  background: #2d2d2d;
  border: 1px solid #404040;
  border-radius: 12px;
  width: 100%;
  max-width: 900px;
  color: #ffffff;
  margin: auto 0;
  min-height: fit-content;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1.5rem;
  border-bottom: 1px solid #404040;
}

.modal-title {
  margin: 0;
  font-size: 1.25rem;
  font-weight: 600;
  color: #ffffff;
}

.close-btn {
  background: none;
  border: none;
  font-size: 1.5rem;
  color: #b0b0b0;
  cursor: pointer;
  padding: 0.25rem 0.5rem;
  border-radius: 4px;
  transition: all 0.2s ease;
}

.close-btn:hover {
  background-color: #404040;
  color: #ffffff;
}

/* Secciones */
.account-info-section,
.filters-section,
.movements-section {
  padding: 1.5rem;
  border-bottom: 1px solid #404040;
}

.movements-section {
  padding: 1.5rem;
  border-bottom: none;
}

.section-title {
  margin: 0 0 1rem 0;
  font-size: 1rem;
  font-weight: 600;
  color: #ffffff;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.movements-count {
  font-size: 0.875rem;
  color: #b0b0b0;
  font-weight: 400;
}

/* Información de cuenta */
.account-details-grid {
  display: grid;
  gap: 1rem;
  grid-template-columns: 1fr;
}

.detail-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.75rem;
  background-color: #1a1a1a;
  border-radius: 6px;
  border: 1px solid #404040;
}

.detail-item.highlight {
  border-color: #0066cc;
  background-color: rgba(0, 102, 204, 0.1);
}

.detail-label {
  font-size: 0.875rem;
  color: #b0b0b0;
}

.detail-value {
  font-size: 0.875rem;
  color: #ffffff;
  font-weight: 500;
}

.detail-value.balance {
  font-size: 1.125rem;
  font-weight: 700;
}

.balance-positive {
  color: #22c55e;
}

.balance-low {
  color: #f59e0b;
}

.balance-negative {
  color: #ef4444;
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
  color: #b0b0b0;
  font-weight: 500;
}

.filter-select,
.filter-input {
  padding: 0.75rem;
  background-color: #1a1a1a;
  border: 1px solid #404040;
  border-radius: 6px;
  color: #ffffff;
  font-size: 0.875rem;
}

.filter-select:focus,
.filter-input:focus {
  outline: 2px solid #0066cc;
  outline-offset: 2px;
  border-color: #0066cc;
}

.filter-input::placeholder {
  color: #808080;
}

/* Estados */
.state-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 3rem 2rem;
  text-align: center;
  background-color: #1a1a1a;
  border-radius: 8px;
  border: 1px solid #404040;
}

.state-container.error {
  border-color: #ef4444;
  background-color: rgba(239, 68, 68, 0.1);
}

.state-container.empty {
  border: 1px dashed #404040;
}

.loading-spinner {
  width: 32px;
  height: 32px;
  border: 3px solid #404040;
  border-top: 3px solid #0066cc;
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
  color: #b0b0b0;
  font-size: 1rem;
}

.retry-btn,
.clear-filters-btn {
  margin-top: 1rem;
  padding: 0.75rem 1.5rem;
  background-color: #0066cc;
  color: #ffffff;
  border: none;
  border-radius: 6px;
  font-size: 0.875rem;
  cursor: pointer;
  transition: background-color 0.2s ease;
}

.retry-btn:hover,
.clear-filters-btn:hover {
  background-color: #0052a3;
}

/* Lista de movimientos */
.movements-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  max-height: none;
}

.movement-item {
  background-color: #1a1a1a;
  border: 1px solid #404040;
  border-radius: 8px;
  padding: 1rem;
  transition: all 0.2s ease;
}

.movement-item:hover {
  border-color: #606060;
  background-color: #252525;
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

.movement-type.credito .type-indicator {
  background-color: rgba(34, 197, 94, 0.2);
  color: #22c55e;
}

.movement-type.debito .type-indicator {
  background-color: rgba(239, 68, 68, 0.2);
  color: #ef4444;
}

.type-text {
  font-size: 0.75rem;
  color: #b0b0b0;
  text-transform: capitalize;
}

.movement-date {
  font-size: 0.75rem;
  color: #808080;
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
  color: #ffffff;
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

.movement-amount.credito {
  color: #22c55e;
}

.movement-amount.debito {
  color: #ef4444;
}

.movement-balance {
  font-size: 0.75rem;
  color: #b0b0b0;
}

/* Paginación */
.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 1rem;
  padding: 1.5rem;
  border-top: 1px solid #404040;
  margin-top: 1rem;
}

.pagination-btn {
  padding: 0.5rem 1rem;
  background-color: #404040;
  color: #ffffff;
  border: none;
  border-radius: 6px;
  font-size: 0.875rem;
  cursor: pointer;
  transition: background-color 0.2s ease;
}

.pagination-btn:hover:not(:disabled) {
  background-color: #606060;
}

.pagination-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.pagination-info {
  font-size: 0.875rem;
  color: #b0b0b0;
}

/* Responsive */
@media (min-width: 768px) {
  .account-details-grid {
    grid-template-columns: repeat(2, 1fr);
  }

  .filters-grid {
    grid-template-columns: 1fr 2fr;
  }

  .modal {
    max-width: 1000px;
  }
}

@media (max-width: 640px) {
  .modal-backdrop {
    padding: 1rem 0.5rem;
    align-items: flex-start;
  }

  .movement-content {
    flex-direction: column;
    align-items: flex-start;
    gap: 0.75rem;
  }

  .movement-amounts {
    align-items: flex-start;
    width: 100%;
  }
}
</style>

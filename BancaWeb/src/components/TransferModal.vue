<template>
  <div
    v-if="show"
    class="modal-overlay"
    @click="closeModal"
    role="dialog"
    aria-modal="true"
    aria-labelledby="transfer-modal-title"
  >
    <div class="modal-content" @click.stop>
      <!-- Header -->
      <header class="modal-header">
        <h3 id="transfer-modal-title" class="modal-title">Confirmar Transferencia</h3>
        <button class="modal-close" @click="closeModal" aria-label="Cerrar modal de confirmación">
          ×
        </button>
      </header>

      <div class="modal-body">
        <div class="transfer-summary" role="region" aria-labelledby="summary-title">
          <h4 id="summary-title" class="summary-title">Resumen de la transferencia</h4>

          <div class="summary-items">
            <div class="summary-item">
              <span class="summary-label">Tipo:</span>
              <span class="summary-value">{{ transferTypeLabel }}</span>
            </div>

            <div class="summary-item">
              <span class="summary-label">Cuenta origen:</span>
              <span class="summary-value">{{ originAccountLabel }}</span>
            </div>

            <div class="summary-item">
              <span class="summary-label">Cuenta destino:</span>
              <span class="summary-value">{{ destinationAccountLabel }}</span>
            </div>

            <div class="summary-item highlight">
              <span class="summary-label">Monto:</span>
              <span class="summary-value amount">
                {{ currencySymbol }}{{ formatAmount(transferData.monto) }}
              </span>
            </div>

            <div v-if="transferData.descripcion" class="summary-item">
              <span class="summary-label">Descripción:</span>
              <span class="summary-value">{{ transferData.descripcion }}</span>
            </div>

            <div class="summary-item">
              <span class="summary-label">Fecha y hora:</span>
              <span class="summary-value">{{ formattedDateTime }}</span>
            </div>
          </div>
        </div>

        <!-- Estado para cargando -->
        <div v-if="isProcessing" class="processing-state" role="status" aria-live="polite">
          <div class="loading-spinner" aria-hidden="true"></div>
          <p class="processing-text">Procesando transferencia...</p>
        </div>
      </div>

      <!-- Action footer -->
      <footer class="modal-footer">
        <button
          class="btn btn-secondary"
          @click="closeModal"
          :disabled="isProcessing"
          type="button"
        >
          Cancelar
        </button>
        <button
          class="btn btn-primary"
          @click="confirmTransfer"
          :disabled="isProcessing"
          type="button"
        >
          {{ isProcessing ? 'Procesando...' : 'Confirmar Transferencia' }}
        </button>
      </footer>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import type { Account } from '../types'

// Props
const props = defineProps<{
  show: boolean
  transferData: any
  accounts: Account[]
  isProcessing?: boolean
}>()

// Emits
const emit = defineEmits<{
  close: []
  confirm: [data: any]
}>()

// Computed properties
const transferTypeLabel = computed(() => {
  return props.transferData?.tipo === 'propias'
    ? 'Entre cuentas propias'
    : 'A terceros (mismo banco)'
})

const originAccountLabel = computed(() => {
  const account = props.accounts.find((a) => a.account_id === props.transferData?.origen)
  return account ? `${account.alias} - ****${account.account_id.slice(-4)}` : 'N/A'
})

const destinationAccountLabel = computed(() => {
  if (props.transferData?.tipo === 'propias') {
    const account = props.accounts.find((a) => a.account_id === props.transferData?.destino)
    return account ? `${account.alias} - ****${account.account_id.slice(-4)}` : 'N/A'
  } else {
    // Para terceros, mostrar el número de cuenta ingresado
    return props.transferData?.destino || 'N/A'
  }
})

const currencySymbol = computed(() => {
  return props.transferData?.moneda === 'USD' ? '$' : '₡'
})

const formattedDateTime = computed(() => {
  const now = new Date()
  const date = now.toLocaleDateString('es-CR', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
  })
  const time = now.toLocaleTimeString('es-CR', {
    hour: '2-digit',
    minute: '2-digit',
  })
  return `${date} ${time}`
})

// Methods
const formatAmount = (amount: number) => {
  return new Intl.NumberFormat('es-CR', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  }).format(amount)
}

const closeModal = () => {
  if (!props.isProcessing) {
    emit('close')
  }
}

const confirmTransfer = () => {
  emit('confirm', props.transferData)
}
</script>

<style scoped>
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 0, 0, 0.7);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 2000;
  padding: 1rem;
}

.modal-content {
  background-color: var(--bg-primary);
  border: 1px solid var(--border-primary);
  border-radius: 12px;
  width: 100%;
  max-width: 500px;
  max-height: 90vh;
  overflow-y: auto;
  display: flex;
  flex-direction: column;
  box-shadow: var(--shadow-modal);
}

/* Header */
.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1.5rem 1.5rem 0 1.5rem;
  border-bottom: 1px solid var(--border-secondary);
  margin-bottom: 1.5rem;
  padding-bottom: 1rem;
}

.modal-title {
  margin: 0;
  font-size: 1.25rem;
  font-weight: 600;
  color: var(--text-primary);
}

.modal-close {
  background: none;
  border: none;
  font-size: 1.5rem;
  color: var(--text-muted);
  cursor: pointer;
  padding: 0.25rem;
  border-radius: 4px;
  transition:
    color 0.2s ease,
    background-color 0.2s ease;
}

.modal-close:hover {
  color: var(--text-primary);
  background-color: var(--bg-hover);
}

.modal-close:focus {
  outline: 2px solid var(--accent-primary);
  outline-offset: 2px;
}

.modal-body {
  padding: 0 1.5rem;
  flex: 1;
}

.summary-title {
  margin: 0 0 1rem 0;
  font-size: 1rem;
  font-weight: 500;
  color: var(--text-primary);
}

.summary-items {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.summary-item {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  padding: 0.75rem;
  background-color: var(--bg-secondary);
  border-radius: 6px;
  border: 1px solid var(--border-secondary);
}

.summary-item.highlight {
  border-color: var(--accent-primary);
  background-color: rgba(0, 102, 204, 0.1);
}

.summary-label {
  font-size: 0.875rem;
  color: var(--text-muted);
  font-weight: 500;
}

.summary-value {
  font-size: 0.875rem;
  color: var(--text-primary);
  text-align: right;
  max-width: 60%;
  word-break: break-word;
}

.summary-value.amount {
  font-size: 1.125rem;
  font-weight: 600;
  color: var(--success);
}

/* Processing state */
.processing-state {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 1rem;
  padding: 1.5rem;
  margin-top: 1rem;
  background-color: var(--bg-secondary);
  border-radius: 8px;
  border: 1px solid var(--border-secondary);
}

.loading-spinner {
  width: 20px;
  height: 20px;
  border: 2px solid var(--border-secondary);
  border-top: 2px solid var(--accent-primary);
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  0% {
    transform: rotate(0deg);
  }

  100% {
    transform: rotate(360deg);
  }
}

.processing-text {
  margin: 0;
  color: var(--text-muted);
  font-size: 0.875rem;
}

.modal-footer {
  padding: 1.5rem;
  border-top: 1px solid var(--border-secondary);
  display: flex;
  gap: 1rem;
  margin-top: 1.5rem;
}

.btn {
  flex: 1;
  padding: 0.875rem 1.5rem;
  border: none;
  border-radius: 6px;
  font-size: 0.875rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s ease;
  text-align: center;
}

.btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.btn-primary {
  background-color: var(--accent-primary);
  color: var(--text-primary);
}

.btn-primary:hover:not(:disabled) {
  background-color: var(--accent-hover);
}

.btn-secondary {
  background-color: transparent;
  color: var(--text-muted);
  border: 1px solid var(--border-secondary);
}

.btn-secondary:hover:not(:disabled) {
  background-color: var(--border-secondary);
  color: var(--text-primary);
  border-color: var(--border-secondary);
}

.btn:focus {
  outline: 2px solid var(--accent-primary) s;
  outline-offset: 2px;
}

@media (min-width: 768px) {
  .modal-content {
    max-width: 600px;
  }

  .modal-header {
    padding: 2rem 2rem 1rem 2rem;
  }

  .modal-body {
    padding: 0 2rem;
  }

  .modal-footer {
    padding: 1.5rem 2rem 2rem 2rem;
  }

  .summary-item {
    padding: 1rem;
  }
}

@media (max-width: 640px) {
  .modal-footer {
    flex-direction: column;
  }

  .summary-item {
    flex-direction: column;
    gap: 0.5rem;
  }

  .summary-value {
    text-align: left;
    max-width: 100%;
  }
}
</style>

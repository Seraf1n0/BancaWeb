<template>
  <article
    class="account-card"
    role="article"
    :aria-labelledby="`card-title-${account.account_id}`"
    :aria-describedby="`card-balance-${account.account_id}`"
  >
    <!-- Header de la tarjeta -->
    <div class="card-header">
      <div class="account-info">
        <h4 :id="`card-title-${account.account_id}`" class="account-alias">
          {{ account.alias }}
        </h4>
        <p class="account-type">{{ account.tipo }} • {{ account.moneda }}</p>
      </div>
      <div class="account-status" :aria-label="`Cuenta ${statusText}`">
        <span class="status-indicator" :class="statusClass"></span>
        <span class="status-text">{{ statusText }}</span>
      </div>
    </div>

    <!-- Número de la cuenta (sin mostrarlo todo) -->
    <div class="account-number">
      <span class="number-label">Número de cuenta:</span>
      <span class="number-value" :aria-label="`Número de cuenta ${maskedAccountNumber}`">
        {{ maskedAccountNumber }}
      </span>
    </div>

    <div class="balance-section">
      <span class="balance-label">Saldo disponible</span>
      <div
        :id="`card-balance-${account.account_id}`"
        class="balance-amount"
        :class="balanceClass"
        :aria-label="`Saldo disponible ${formattedBalance}`"
      >
        {{ currencySymbol }}{{ formattedBalance }}
      </div>
    </div>

    <!-- Futuro: Poner aqui la fecha del ultimo movimiento de la cuenta -->
    <div class="card-details">
      <div class="detail-item">
        <span class="detail-label">Última actualización:</span>
        <span class="detail-value">{{ lastUpdated }}</span>
      </div>
    </div>

    <div class="card-actions">
      <button
        class="action-btn primary"
        type="button"
        @click="handleViewDetails"
        :aria-label="`Ver detalles de ${account.alias}`"
      >
        Ver detalles
      </button>
    </div>
  </article>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import type { Account } from '../types/index'

// Props
const props = defineProps<{
  account: Account
}>()

// Emits
const emit = defineEmits<{
  viewDetails: [accountId: string]
}>()

// Los calculos para algunas cosas con computed
const maskedAccountNumber = computed(() => {
  const fullNumber = props.account.account_id
  // esto para mostrar unicamente los 4 digitos ultimos y ptrfijo
  const lastFour = fullNumber.slice(-4)
  const prefix = fullNumber.split('-').slice(0, -1).join('-')
  return `${prefix}-****${lastFour}`
})

const formattedBalance = computed(() => {
  return new Intl.NumberFormat('es-CR', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  }).format(props.account.saldo)
})

const currencySymbol = computed(() => {
  return props.account.moneda === 'USD' ? '$' : '₡'
})

const balanceClass = computed(() => {
  if (props.account.saldo < 0) return 'balance-negative'
  if (props.account.saldo < 10000) return 'balance-low'
  return 'balance-positive'
})

const statusText = computed(() => {
  // Simulamos estado basado en el saldo
  if (props.account.saldo < 0) return 'Con sobregiro'
  if (props.account.saldo < 10000) return 'Saldo bajo'
  return 'Activa'
})

const statusClass = computed(() => {
  if (props.account.saldo < 0) return 'status-warning'
  if (props.account.saldo < 10000) return 'status-caution'
  return 'status-active'
})

const lastUpdated = computed(() => {
  // Simulamos una fecha de última actualización. luego será del ultimo movimiento
  const now = new Date()
  const today = now.toLocaleDateString('es-CR', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
  })
  const time = now.toLocaleTimeString('es-CR', {
    hour: '2-digit',
    minute: '2-digit',
  })
  return `${today} ${time}`
})

// metodo para ver detalles
const handleViewDetails = () => {
  emit('viewDetails', props.account.account_id)
}
</script>

<style scoped>
.account-card {
  background-color: var(--bg-secondary);
  border: 1px solid var(--border-primary);
  border-radius: 12px;
  padding: 1.5rem;
  transition: all 0.2s ease;
  position: relative;
  overflow: hidden;
}

.account-card:hover {
  border-color: var(--border-primary);
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.3);
}

.account-card:focus-within {
  border-color: var(--border-focus);
  outline: 2px solid var(--border-focus);
  outline-offset: 2px;
}

/* Header */
.card-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 1rem;
}

.account-info {
  flex: 1;
}

.account-alias {
  margin: 0 0 0.25rem 0;
  font-size: 1.125rem;
  font-weight: 600;
  color: var(--text-primary);
  line-height: 1.3;
}

.account-type {
  margin: 0;
  font-size: 0.875rem;
  color: var(--text-secondary);
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.account-status {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-left: 1rem;
}

.status-indicator {
  width: 8px;
  height: 8px;
  border-radius: 50%;
}

.status-indicator.status-active {
  background-color: var(--success);
}

.status-indicator.status-caution {
  background-color: var(--warning);
}

.status-indicator.status-warning {
  background-color: var(--danger);
}

.status-text {
  font-size: 0.75rem;
  color: var(--text-secondary);
  white-space: nowrap;
}

/* Número de cuenta */
.account-number {
  margin-bottom: 1.5rem;
  padding: 0.75rem;
  background-color: var(--bg-secondary);
  border-radius: 6px;
  border: 1px solid var(--border-primary);
}

.number-label {
  display: block;
  font-size: 0.75rem;
  color: var(--text-secondary);
  margin-bottom: 0.25rem;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.number-value {
  font-family: 'Courier New', monospace;
  font-size: 0.875rem;
  color: var(--text-primary);
  font-weight: 500;
}

/* Saldo */
.balance-section {
  margin-bottom: 1.5rem;
}

.balance-label {
  display: block;
  font-size: 0.875rem;
  color: var(--text-secondary);
  margin-bottom: 0.5rem;
}

.balance-amount {
  font-size: 1.75rem;
  font-weight: 700;
  line-height: 1;
  font-family: 'Franklin Gothic Medium', 'Arial Narrow', Arial, sans-serif;
}

.balance-positive {
  color: var(--success);
}

.balance-low {
  color: var(--warning);
}

.balance-negative {
  color: var(--danger);
}

/* Detalles adicionales */
.card-details {
  margin-bottom: 1.5rem;
  padding-top: 1rem;
  border-top: 1px solid var(--border-primary);
}

.detail-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.detail-label {
  font-size: 0.75rem;
  color: var(--text-secondary);
}

.detail-value {
  font-size: 0.75rem;
  color: var(--text-primary);
}

/* Acciones */
.card-actions {
  display: flex;
  gap: 0.75rem;
}

.action-btn {
  flex: 1;
  padding: 0.75rem 1rem;
  border: none;
  border-radius: 6px;
  font-size: 0.875rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s ease;
  text-align: center;
}

.action-btn.primary {
  background-color: var(--accent-primary);
  color: white;
}

.action-btn.primary:hover {
  background-color: var(--accent-hover);
}

.action-btn:focus {
  outline: 2px solid var(--accent-primary);
  outline-offset: 2px;
}

/* Responsive */
@media (max-width: 480px) {
  .account-card {
    padding: 1rem;
  }

  .card-header {
    flex-direction: column;
    gap: 0.75rem;
  }

  .account-status {
    margin-left: 0;
    align-self: flex-start;
  }

  .card-actions {
    flex-direction: column;
  }

  .balance-amount {
    font-size: 1.5rem;
    font-weight: 500;
  }
}

@media (min-width: 1024) {
  .balance-amount {
    font-size: 1.5rem;
    overflow: scroll;
  }
}
</style>

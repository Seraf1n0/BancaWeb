<template>
  <div class="accounts-section" role="region" aria-labelledby="accounts-main-title">
    <!-- header de seccion -->
    <div class="section-header">
      <h3 id="accounts-main-title" class="section-title">Mis Cuentas</h3>
      <p class="section-subtitle">Gestiona tus cuentas bancarias</p>
    </div>

    <!-- resumen de todo (en caso de haber muchas cuentas) -->
    <div
      v-if="showSummary"
      class="accounts-summary"
      role="complementary"
      aria-labelledby="summary-title"
    >
      <h4 id="summary-title" class="summary-title">Resumen</h4>
      <div class="summary-grid">
        <div class="summary-item">
          <span class="summary-label">Total de cuentas</span>
          <span class="summary-value">{{ totalAccounts }}</span>
        </div>
        <div class="summary-item">
          <span class="summary-label">Saldo total CRC</span>
          <span class="summary-value">₡{{ formatCurrency(totalBalanceCRC) }}</span>
        </div>
        <div class="summary-item">
          <span class="summary-label">Saldo total USD</span>
          <span class="summary-value">${{ formatCurrency(totalBalanceUSD) }}</span>
        </div>
      </div>
    </div>

    <!-- Contenido principal: van los children -->
    <div class="accounts-content" role="list" aria-labelledby="accounts-list-title">
      <h4 id="accounts-list-title" class="list-title">Cuentas:</h4>

      <!-- Slot para el contenido (AccountCards) -->
      <div class="accounts-grid">
        <slot>
          <!-- En caso de no haber cuentas entonces: -->
          <div class="empty-state">
            <p class="empty-message">No hay contenido para mostrar</p>
          </div>
        </slot>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// Props para resumen
const {
  showSummary = true,
  totalAccounts = 0,
  totalBalanceCRC = 0,
  totalBalanceUSD = 0,
} = defineProps<{
  showSummary?: boolean
  totalAccounts?: number
  totalBalanceCRC?: number
  totalBalanceUSD?: number
}>()

// funcion para formatear la moneda
const formatCurrency = (amount: number): string => {
  return new Intl.NumberFormat('es-CR', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  }).format(amount)
}
</script>

<style scoped>
.accounts-section {
  width: 100%;
}

/* Header de la sección */
.section-header {
  margin-bottom: 1.5rem;
  padding-bottom: 1rem;
  border-bottom: 1px solid var(--border-primary);
}

.section-title {
  margin: 0 0 0.5rem 0;
  font-size: 1.25rem;
  font-weight: 600;
  color: var(--text-primary);
}

.section-subtitle {
  margin: 0;
  font-size: 0.875rem;
  color: var(--text-muted);
}

/* Resumen */
.accounts-summary {
  background-color: var(--bg-secondary);
  border: 1px solid var(--border-primary);
  border-radius: 8px;
  padding: 1.5rem;
  margin-bottom: 2rem;
}

.summary-title {
  margin: 0 0 1rem 0;
  font-size: 1rem;
  font-weight: 500;
  color: var(--text-primary);
}

.summary-grid {
  display: grid;
  grid-template-columns: 1fr;
  gap: 1rem;
}

.summary-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.75rem;
  background-color: var(--bg-secondary);
  border-radius: 4px;
  border: 1px solid var(--border-secondary);
}

.summary-label {
  font-size: 0.875rem;
  color: var(--text-muted);
}

.summary-value {
  font-size: 1rem;
  font-weight: 600;
  color: var(--text-primary);
}

/* Contenido principal */
.accounts-content {
  margin-top: 2rem;
}

.list-title {
  margin: 0 0 1.5rem 0;
  font-size: 1.1rem;
  font-weight: 500;
  color: var(--text-primary);
}

.accounts-grid {
  display: grid;
  grid-template-columns: 1fr;
  gap: 1rem;
}

/* Estado vacío (fallback) */
.empty-state {
  text-align: center;
  padding: 3rem 1rem;
  background-color: var(--bg-secondary);
  border: 1px dashed var(--border-primary);
  border-radius: 8px;
}

.empty-message {
  margin: 0;
  font-size: 1rem;
  color: var(--text-muted);
}

/* Media queries responsive */
@media (min-width: 768px) {
  .summary-grid {
    grid-template-columns: repeat(2, 1fr);
  }

  .accounts-grid {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (min-width: 1024px) {
  .summary-grid {
    grid-template-columns: repeat(3, 1fr);
  }

  .accounts-grid {
    grid-template-columns: repeat(3, 1fr);
  }

  .section-header {
    display: flex;
    justify-content: space-between;
    align-items: flex-end;
  }
}
</style>

<template>
    <div class="transfer-form-container">
        <div class="form-header">
            <h4 class="form-title">{{ formTitle }}</h4>
            <p class="form-subtitle">{{ formSubtitle }}</p>
        </div>

        <form class="transfer-form" @submit.prevent="submitForm" role="form" aria-labelledby="form-title">
            <!-- Cuenta origen (comun entre tipos) -->
            <div class="form-group">
                <label for="origen" class="form-label">Cuenta origen *</label>
                <select id="origen" v-model="form.origen" class="form-select" required aria-describedby="origen-help">
                    <option disabled value="">Seleccione una cuenta</option>
                    <option v-for="acc in accounts" :key="acc.account_id" :value="acc.account_id">
                        {{ acc.alias }} - ****{{ acc.account_id.slice(-4) }} ({{ acc.moneda }})
                    </option>
                </select>
                <small id="origen-help" class="form-help">Selecciona la cuenta desde la que transferirás</small>
            </div>

            <!-- vista condicional para tipos -->
            <div v-if="transferType === 'propias'" class="form-group">
                <label for="destino-select" class="form-label">Cuenta destino *</label>
                <select id="destino-select" v-model="form.destino" class="form-select" required
                    aria-describedby="destino-help">
                    <option disabled value="">Seleccione cuenta destino</option>
                    <option v-for="acc in ownDestinyAccounts" :key="acc.account_id" :value="acc.account_id">
                        {{ acc.alias }} - ****{{ acc.account_id.slice(-4) }} ({{ acc.moneda }})
                    </option>
                </select>
                <small id="destino-help" class="form-help">Cuenta donde recibirás el dinero</small>
            </div>

            <!-- vista condicional para tipos -->
            <div v-else-if="transferType === 'terceros'" class="form-group">
                <label for="destino-input" class="form-label">Número de cuenta destino *</label>
                <input id="destino-input" type="text" v-model="form.destino" class="form-input"
                    placeholder="CR01-XXXX-XXXX-XXXXXXXXXXXX" required pattern="CR\d{2}-\d{4}-\d{4}-\d{12}"
                    aria-describedby="destino-tercero-help" />
                <small id="destino-tercero-help" class="form-help">
                    Formato: CR01-XXXX-XXXX-XXXXXXXXXXXX
                </small>
            </div>

            <div class="form-group">
                <label for="moneda" class="form-label">Moneda *</label>
                <select id="moneda" v-model="form.moneda" class="form-select" required aria-describedby="moneda-help">
                    <option :value="originCurrency">{{ currencyLabel(originCurrency) }}</option>
                    <option v-if="originCurrency !== 'USD'" value="USD">Dólares Americanos (USD)</option>
                    <option v-if="originCurrency !== 'CRC'" value="CRC">Colones Costarricenses (CRC)</option>
                </select>
                <small id="moneda-help" class="form-help">Moneda para la transferencia</small>
            </div>

            <div class="form-group">
                <label for="monto" class="form-label">Monto *</label>
                <div class="input-group">
                    <span class="input-prefix">{{ currencySymbol }}</span>
                    <input id="monto" type="number" step="0.01" min="1" :max="maxAmount" v-model.number="form.monto"
                        class="form-input" required aria-describedby="monto-help" placeholder="0.00" />
                </div>
                <small id="monto-help" class="form-help">
                    Monto mínimo: {{ currencySymbol }}1.00
                    <span v-if="maxAmount"> | Máximo disponible: {{ currencySymbol }}{{ formatAmount(maxAmount)
                        }}</span>
                </small>
            </div>

            <div class="form-group">
                <label for="descripcion" class="form-label">Descripción</label>
                <input id="descripcion" type="text" v-model="form.descripcion" class="form-input" maxlength="255"
                    placeholder="Concepto de la transferencia (opcional)" aria-describedby="descripcion-help" />
                <small id="descripcion-help" class="form-help">
                    {{ form.descripcion.length }}/255 caracteres
                </small>
            </div>

            <div class="form-actions">
                <button type="button" class="btn btn-secondary" @click="resetForm">
                    Limpiar
                </button>
                <button type="submit" class="btn btn-primary" :disabled="!isFormValid">
                    Continuar
                </button>
            </div>
        </form>
    </div>
</template>

<script setup lang="ts">
import { computed, reactive, watch } from 'vue'
import type { Account } from '../types'

// Props
const props = defineProps<{
    transferType: string
    accounts: Account[]
}>()

// Emits
const emit = defineEmits<{
    'submit-transfer': [data: any]
}>()

// estructura del formulario
const form = reactive({
    origen: '',
    destino: '',
    moneda: '',
    monto: null as number | null,
    descripcion: ''
})

// Computed properties
const formTitle = computed(() => {
    return props.transferType === 'propias'
        ? 'Transferencia entre cuentas propias'
        : 'Transferencia a terceros'
})

const formSubtitle = computed(() => {
    return props.transferType === 'propias'
        ? 'Transfiere dinero entre tus cuentas de forma instantánea'
        : 'Envía dinero a cuentas de otras personas en el mismo banco'
})

const ownDestinyAccounts = computed(() => {
    return props.accounts.filter(a => a.account_id !== form.origen)
})

const originAccount = computed(() => {
    return props.accounts.find(a => a.account_id === form.origen)
})

const originCurrency = computed(() => {
    return originAccount.value?.moneda || 'CRC'
})

const maxAmount = computed(() => {
    return originAccount.value?.saldo || 0
})

const currencySymbol = computed(() => {
    return form.moneda === 'USD' ? '$' : '₡'
})

const isFormValid = computed(() => {
    return form.origen && form.destino && form.moneda &&
        form.monto && form.monto > 0 && form.monto <= maxAmount.value
})

// Methods
const currencyLabel = (currency: string) => {
    return currency === 'USD' ? 'Dólares Americanos (USD)' : 'Colones Costarricenses (CRC)'
}

const formatAmount = (amount: number) => {
    return new Intl.NumberFormat('es-CR', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    }).format(amount)
}

const submitForm = () => {
    if (isFormValid.value) {
        emit('submit-transfer', {
            ...form,
            tipo: props.transferType,
            timestamp: new Date().toISOString()
        })
    }
}

const resetForm = () => {
    Object.assign(form, {
        origen: '',
        destino: '',
        moneda: '',
        monto: null,
        descripcion: ''
    })
}

// Watchers
watch(() => form.origen, (newOrigen) => {
    if (newOrigen && !form.moneda) {
        form.moneda = originCurrency.value
    }
    // se resetea el destino si cambia a propias
    if (props.transferType === 'propias') {
        form.destino = ''
    }
})

watch(() => props.transferType, () => {
    resetForm()
})
</script>

<style scoped>
.transfer-form-container {
    width: 100%;
}

.form-header {
    margin-bottom: 2rem;
    padding-bottom: 1rem;
    border-bottom: 1px solid #404040;
}

.form-title {
    margin: 0 0 0.5rem 0;
    font-size: 1.125rem;
    font-weight: 600;
    color: #ffffff;
}

.form-subtitle {
    margin: 0;
    font-size: 0.875rem;
    color: #b0b0b0;
}

.transfer-form {
    display: flex;
    flex-direction: column;
    gap: 1.5rem;
}

.form-group {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
}

.form-label {
    font-size: 0.875rem;
    font-weight: 500;
    color: #e0e0e0;
}

.form-input,
.form-select {
    padding: 0.75rem;
    border: 1px solid #404040;
    border-radius: 6px;
    background-color: #1a1a1a;
    color: #e0e0e0;
    font-size: 0.9rem;
    transition: border-color 0.2s ease;
}

.form-input:focus,
.form-select:focus {
    outline: none;
    border-color: #0066cc;
    box-shadow: 0 0 0 2px rgba(0, 102, 204, 0.2);
}

.form-input::placeholder {
    color: #808080;
}

.input-group {
    position: relative;
}

.input-prefix {
    position: absolute;
    left: 0.75rem;
    top: 50%;
    transform: translateY(-50%);
    color: #b0b0b0;
    font-size: 0.9rem;
    pointer-events: none;
}

.input-group .form-input {
    padding-left: 2.5rem;
}

.form-help {
    font-size: 0.75rem;
    color: #808080;
    margin: 0;
}

.form-actions {
    display: flex;
    gap: 1rem;
    margin-top: 1rem;
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
    background-color: #0066cc;
    color: #ffffff;
}

.btn-primary:hover:not(:disabled) {
    background-color: #0052a3;
}

.btn-secondary {
    background-color: transparent;
    color: #b0b0b0;
    border: 1px solid #404040;
}

.btn-secondary:hover {
    background-color: #404040;
    color: #ffffff;
    border-color: #606060;
}

.btn:focus {
    outline: 2px solid #0066cc;
    outline-offset: 2px;
}

/* Media queries */
@media (max-width: 640px) {
    .form-actions {
        flex-direction: column;
    }
}
</style>
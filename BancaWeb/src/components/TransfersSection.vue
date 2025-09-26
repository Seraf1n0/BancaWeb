<template>
    <div class="transfers-section" role="region" aria-labelledby="transfers-main-title">
        <!-- Header -->
        <div class="section-header">
            <h3 id="transfers-main-title" class="section-title">Transferencias</h3>
            <p class="section-subtitle">Realiza transferencias entre tus cuentas o a terceros</p>
        </div>

        <!-- Select custom para el tipo de transferencia -->
        <TransferTypeSelector v-model="transferType" />

        <!-- slots para el cntenido dinamico (forms) -->
        <div class="transfers-content" v-if="transferType">
            <slot :transfer-type="transferType" :accounts="accounts">
                <div class="empty-state">
                    <p class="empty-message">No hay formulario disponible para este tipo de transferencia</p>
                </div>
            </slot>
        </div>

        <div v-else class="selection-prompt">
            <p class="prompt-message">Selecciona un tipo de transferencia para continuar</p>
        </div>
    </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import TransferTypeSelector from './TransfersTypeSelector.vue'
import type { Account } from '../types'

// Props
const props = withDefaults(defineProps<{
    accounts?: Account[]
}>(), {
    accounts: () => []
})

// Estado interno
const transferType = ref<string>('')

defineExpose({
    transferType
})
</script>

<style scoped>
.transfers-section {
    width: 100%;
}

/* Header */
.section-header {
    margin-bottom: 1.5rem;
    padding-bottom: 1rem;
    border-bottom: 1px solid #404040;
}

.section-title {
    margin: 0 0 0.5rem 0;
    font-size: 1.25rem;
    font-weight: 600;
    color: #ffffff;
}

.section-subtitle {
    margin: 0;
    font-size: 0.875rem;
    color: #b0b0b0;
}

/* Contenido */
.transfers-content {
    margin-top: 2rem;
}

.selection-prompt {
    text-align: center;
    padding: 3rem 1rem;
    background-color: #2d2d2d;
    border: 1px solid #404040;
    border-radius: 8px;
    margin-top: 2rem;
}

.prompt-message {
    margin: 0;
    color: #b0b0b0;
    font-size: 1rem;
}

.empty-state {
    text-align: center;
    padding: 2rem;
    background-color: #2d2d2d;
    border: 1px dashed #404040;
    border-radius: 8px;
}

.empty-message {
    margin: 0;
    color: #b0b0b0;
}
</style>
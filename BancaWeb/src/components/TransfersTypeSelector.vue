<template>
  <div class="transfer-type-selector" role="radiogroup" aria-labelledby="type-selector-title">
    <h4 id="type-selector-title" class="selector-title">Tipo de transferencia</h4>
    <div class="options">
      <label class="option" :class="{ active: modelValue === 'propias' }">
        <input
          type="radio"
          value="propias"
          :checked="modelValue === 'propias'"
          @change="handleChange"
          aria-describedby="propias-desc"
        />
        <span class="option-text">Entre Cuentas Propias</span>
        <span id="propias-desc" class="option-desc">Transfiere entre tus cuentas</span>
      </label>

      <label class="option" :class="{ active: modelValue === 'terceros' }">
        <input
          type="radio"
          value="terceros"
          :checked="modelValue === 'terceros'"
          @change="handleChange"
          aria-describedby="terceros-desc"
        />
        <span class="option-text">A Terceros</span>
        <span id="terceros-desc" class="option-desc">Transfiere a otras personas</span>
      </label>
    </div>
  </div>
</template>

<script setup lang="ts">
// Props
const props = withDefaults(
  defineProps<{
    modelValue?: string
  }>(),
  {
    modelValue: 'propias',
  },
)

// Emits
const emit = defineEmits<{
  'update:modelValue': [value: string]
}>()

// metodo para el cambio de valor
const handleChange = (event: Event) => {
  const target = event.target as HTMLInputElement
  emit('update:modelValue', target.value)
}
</script>

<style scoped>
.transfer-type-selector {
  margin-bottom: 2rem;
}

.selector-title {
  margin: 0 0 1rem 0;
  font-size: 1rem;
  font-weight: 500;
  color: var(--text-primary);
}

.options {
  display: grid;
  grid-template-columns: 1fr;
  gap: 1rem;
}

.option {
  display: flex;
  flex-direction: column;
  padding: 1rem;
  background-color: var(--bg-secondary);
  border: 1px solid var(--border-primary);
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.2s ease;
  position: relative;
}

.option:hover {
  border-color: var(--border-primary);
  background-color: var(--bg-hover);
}

.option.active {
  border-color: var(--accent-primary);
  background-color: rgba(0, 102, 204, 0.1);
}

.option input {
  position: absolute;
  top: 1rem;
  right: 1rem;
  cursor: pointer;
  accent-color: var(--accent-primary);
}

.option-text {
  font-size: 0.95rem;
  font-weight: 500;
  color: var(--text-primary);
  margin-bottom: 0.25rem;
}

.option-desc {
  font-size: 0.8rem;
  color: var(--text-muted);
}

.option input:focus {
  outline: 2px solid var(--accent-primary);
  outline-offset: 2px;
}

/* Media queries */
@media (min-width: 640px) {
  .options {
    grid-template-columns: repeat(2, 1fr);
  }
}
</style>

<script setup lang="ts">
const emit = defineEmits(['close'])

const closeModal = () => {
  emit('close')
}
</script>

<template>
  <div
    class="modal-backdrop"
    @click.self="closeModal"
    role="dialog"
    aria-modal="true"
    aria-labelledby="card-modal-title"
  >
    <div class="modal" @click.stop>
      <div class="modal-header">
        <h2 id="card-modal-title" class="modal-title">
          <slot name="title"></slot>
        </h2>
        <button class="close-btn" @click="closeModal" aria-label="Cerrar modal" type="button">
          ×
        </button>
      </div>

      <div class="modal-body">
        <slot></slot>
      </div>
    </div>
  </div>
</template>

<style scoped>
.modal-backdrop {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.7);
  display: flex;
  justify-content: center;
  align-items: flex-start;
  z-index: 2500;
  padding: 2rem 1rem;
  overflow-y: auto;
}

.modal {
  background: var(--bg-primary);
  border: 1px solid var(--border-primary);
  border-radius: 12px;
  width: 100%;
  max-width: 900px;
  color: var(--text-primary);
  margin: auto 0;
  min-height: fit-content;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1.5rem;
  border-bottom: 1px solid var(--border-primary);
}

.modal-title {
  margin: 0;
  font-size: 1.25rem;
  font-weight: 600;
  color: var(--text-primary);
}

.close-btn {
  background: none;
  border: none;
  font-size: 1.5rem;
  color: var(--text-muted);
  cursor: pointer;
  padding: 0.25rem 0.5rem;
  border-radius: 4px;
  transition: all 0.2s ease;
}

.close-btn:hover {
  background-color: var(--bg-hover);
  color: var(--text-on-hover);
}

.close-btn:focus {
  outline: 2px solid var(--border-focus);
  outline-offset: 2px;
}

.modal-body {
  padding: 0;
}

@media (max-width: 640px) {
  .modal-backdrop {
    padding: 1rem 0.5rem;
    align-items: flex-start;
  }
}
</style>

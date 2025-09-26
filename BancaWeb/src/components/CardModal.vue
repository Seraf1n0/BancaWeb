<script setup>
    const emit = defineEmits(["close"])
</script>

<template>
    <div class="modal-backdrop" @click.self="emit('close')">
        <div class="modal">
            <button class="close-btn" @click="emit('close')">X</button>
            <slot></slot>
        </div>
    </div>

</template>


<style scoped>
.modal-backdrop {
  position: fixed;
  inset: 0;
  background: rgba(0,0,0,0.6);
  display: flex;
  justify-content: center;
  align-items: center;
}
.modal {
  background: white;
  padding: 1rem;
  border-radius: 0.6rem;
  width: 90%;
  max-height: 90vh;
  overflow-y: auto;
}
.close-btn {
  float: right;
}
</style>
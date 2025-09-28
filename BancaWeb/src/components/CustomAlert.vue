<script setup>
import { watch, onMounted } from 'vue';
const props = defineProps({
  show: Boolean,
  type: {
    type: String,
    default: 'success' // success | error | warning | loading
  },
  message: {
    type: String,
    default: ''
  },
  autoClose: {
    type: Boolean,
    default: false
  },
  duration: {
    type: Number,
    default: 4000
  }
});
const emit = defineEmits(['close']);

watch(
  () => props.show,
  (val) => {
    if (val && props.autoClose) {
      setTimeout(() => {
        emit('close');
      }, props.duration);
    }
  }
);
</script>




<template>
  <div v-if="show" class="custom-modal-overlay">
    <div :class="['custom-modal', type]">
      <span class="close-btn" @click="$emit('close')">&times;</span>
      <div class="modal-message">{{ message }}</div>
    </div>
  </div>
</template>

<style scoped>


.custom-modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100vw;
  height: 100vh;
  background: rgba(0,0,0,0.4);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 9999;
  animation: fadeIn 0.2s;
}
.custom-modal {
  background: #fff;
  border-radius: 10px;
  padding: 2rem 2.5rem;
  min-width: 300px;
  max-width: 90vw;
  box-shadow: 0 8px 32px rgba(0,0,0,0.25);
  text-align: center;
  position: relative;
  font-size: 1.3rem;
  animation: popIn 0.2s;
}
.custom-modal.success {
  border: 2px solid #28a745;
}
.custom-modal.error {
  border: 2px solid #dc3545;
}
.custom-modal.warning {
  border: 2px solid #ffc107;
}
.modal-message {
  margin-bottom: 1rem;
  color: #222;
  font-weight: 500;
  word-break: break-word;
}
.close-btn {
  position: absolute;
  top: 10px;
  right: 18px;
  font-size: 2rem;
  color: #888;
  cursor: pointer;
  transition: color 0.2s;
}
.close-btn:hover {
  color: #333;
}
@keyframes popIn {
  0% { transform: scale(0.8); opacity: 0; }
  100% { transform: scale(1); opacity: 1; }
}


</style>
<script setup>
import { ref } from "vue"
import Card from "./CreditCard.vue"

const cards = [
  { type: "Gold", number: "1111222233334444", valid: "12/26", owner: "Paulo Gonzales Maradona" },
  { type: "Platinum", number: "5555666677778888", valid: "06/29", owner: "Benito Montes Lackwood" },
  { type: "Black", number: "9999000011112222", valid: "01/30", owner: "Neymar Santos Junior" }
]

const index = ref(0)
const dragStart = ref(0)
const dragOffset = ref(0)
const isDragging = ref(false)

function startDrag(event) {
  isDragging.value = true
  dragStart.value = event.type.includes("touch")
    ? event.touches[0].clientX
    : event.clientX
}

function onDrag(event) {
  if (!isDragging.value) return
  const currentX = event.type.includes("touch")
    ? event.touches[0].clientX
    : event.clientX
  dragOffset.value = currentX - dragStart.value
}

function endDrag() {
  if (!isDragging.value) return
  isDragging.value = false

  if (dragOffset.value < -50 && index.value < cards.length - 1) {
    index.value++
  } else if (dragOffset.value > 50 && index.value > 0) {
    index.value--
  }
  dragOffset.value = 0
}
</script>

<template>
  <div
    class="carousel-container"
    @mousedown="startDrag"
    @mousemove="onDrag"
    @mouseup="endDrag"
    @mouseleave="endDrag"
    @touchstart="startDrag"
    @touchmove="onDrag"
    @touchend="endDrag"
  >
    <div
      class="carousel-track"
      :style="{ transform: `translateX(${-index * 320 + dragOffset}px)` }"
    >
      <Card
        class="card"
        v-for="(card, i) in cards"
        :key="i"
        :type="card.type"
        :number="card.number"
        :valid="card.valid"
        :owner="card.owner"
      />
    </div>
  </div>
</template>

<style scoped>
/* Contenedor del carrusel */
.carousel-container {
  width: 20rem;        
  margin: 0 auto;      
  overflow: hidden;    
  position: relative;  
}

/* Pista con todas las cards */
.carousel-track {
  display: flex;
  transition: transform 0.3s ease;
}

/* Estilos de cada card */
.card {
  width: 100%;
  height: 13rem;
  border-radius: 0.6rem;
  padding: 1.20rem;
  color: white;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.2);
  flex-shrink: 0; 
}
</style>

<template>
  <div class="card-container" :class="bgClass">
    <span class="card-type">{{ type }} Card</span>
    <div class="card-number">{{ number.slice(0,4) }} **** **** {{ number.slice(-4) }}</div>
    <div class="card-info">
      <span class="card-valid">VT:{{ valid }}</span>
      <span class="card-owner">{{ owner }}</span>
    </div>
  </div>
</template>

<script setup>
import { computed } from "vue"

const props = defineProps({
  type: String,
  number: String,
  valid: String,
  owner: String
})

const bgClass = computed(() => {
  switch (props.type.toLowerCase()) {
    case "gold": return "gradient-gold shine-effect chip-pattern"
    case "platinum": return "gradient-platinum shine-effect chip-pattern"
    case "black": return "gradient-black shine-effect chip-pattern"
    default: return "gradient-blue shine-effect chip-pattern"
  }
})
</script>



<style scoped>

.card-container {
  width: 320px;
  height: 192px;
  border-radius: 16px;
  padding: 20px;
  color: white;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  position: relative;
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.2);
}

.card-type {
  font-weight: bold;
  text-transform: uppercase;
  font-size: 0.9rem;
}

.card-number {
  font-size: 1.2rem;
  letter-spacing: 0.1rem;
  font-family: monospace;
}

.card-info {
  display: flex;
  flex-direction: column;
  font-size: 0.85rem;
}

.card-valid {
  opacity: 0.7;
  margin-bottom: 4px;
  font-family: monospace;
}

.card-owner {
  font-weight: 600;
}

.gradient-gold {
    background: linear-gradient(135deg, #fbbf24 0%, #f59e0b 25%, #d97706 50%, #92400e 100%);
}

.gradient-platinum {
    background: linear-gradient(135deg, #e5e7eb 0%, #9ca3af 25%, #6b7280 50%, #374151 100%);
}

.gradient-black {
    background: linear-gradient(135deg, #1f2937 0%, #111827 25%, #000000 50%, #000000 100%);
}

.gradient-blue {
    background: linear-gradient(135deg, #3b82f6 0%, #2563eb 25%, #1d4ed8 50%, #1e40af 100%);
}

/* Efecto de brillo */
.shine-effect {
    position: relative;
    overflow: hidden;
}

.shine-effect::before {
    content: '';
    position: absolute;
    top: 0;
    left: -100%;
    width: 100%;
    height: 100%;
    background: linear-gradient(90deg, transparent, rgba(255,255,255,0.2), transparent);
    transition: left 0.5s;
}

.shine-effect:hover::before {
    left: 100%;
}

.chip-pattern {
  position: relative;
}

.chip-pattern::after {
  content: '';
  position: absolute;
  top: 1rem;
  right: 1rem;
  width: 2rem;
  height: 1.5rem;
  background-color: #c0c0c0;
  border-radius: 0.25rem;
  border: 1px solid #999;
  box-shadow: inset 0 1px 2px rgba(0,0,0,0.3);
  outline: 1px solid rgba(255,255,255,0.2);

  
  background-image:
    /* Estas son las lineas */
    repeating-linear-gradient(
      to bottom,
      rgba(0.3, 0.3, 0.3, 0.2) 0px,
      rgba(0, 0, 0, 0.2) 1px,
      transparent 2px,
      transparent 4px
    ),

    /* puntos simulando lineas */
    radial-gradient(circle at 1% 30%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 2% 30%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 3% 30%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 4% 30%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 5% 30%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 6% 30%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 7% 30%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 8% 30%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 9% 30%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 10% 30%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 11% 30%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 12% 30%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 13% 30%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 14% 30%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 15% 30%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 16% 30%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 17% 30%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 18% 30%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 19% 30%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 20% 30%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 21% 30%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 22% 30%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 23% 30%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 24% 30%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 25% 30%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 26% 30%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 27% 30%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 28% 30%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 29% 30%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 30% 30%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 31% 30%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 32% 30%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 33% 30%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 34% 30%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 35% 30%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 30%, rgba(0,0,0,0.3) 1px, transparent 1px),



    radial-gradient(circle at 36% 1%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 2%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 3%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 4%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 5%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 6%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 7%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 8%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 9%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 10%, rgba(0,0,0,0.3) 1px, transparent 1px), 
    radial-gradient(circle at 36% 11%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 12%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 13%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 14%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 15%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 16%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 17%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 18%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 19%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 20%, rgba(0,0,0,0.3) 1px, transparent 1px), 
    radial-gradient(circle at 36% 21%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 22%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 23%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 24%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 25%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 26%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 27%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 28%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 29%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 30%, rgba(0,0,0,0.3) 1px, transparent 1px),    





    radial-gradient(circle at 37% 17%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 38% 17%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 39% 17%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 40% 17%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 41% 17%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 42% 17%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 43% 17%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 44% 17%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 45% 17%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 46% 17%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 47% 17%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 48% 17%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 49% 17%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 50% 17%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 51% 17%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 52% 17%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 53% 17%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 54% 17%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 54% 17%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 55% 17%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 56% 17%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 57% 17%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 58% 17%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 59% 17%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 60% 17%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 65% 17%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 66% 17%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 67% 17%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 68% 17%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 69% 17%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 70% 17%, rgba(0,0,0,0.3) 1px, transparent 1px),


    radial-gradient(circle at 70% 16%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 70% 15%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 70% 14%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 70% 13%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 70% 12%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 70% 11%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 70% 10%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 70% 9%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 70% 8%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 70% 7%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 70% 6%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 70% 5%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 70% 4%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 70% 3%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 70% 4%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 70% 3%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 70% 2%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 70% 1%, rgba(0,0,0,0.3) 1px, transparent 1px),






    radial-gradient(circle at 0% 60%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 1% 60%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 2% 60%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 3% 60%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 4% 60%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 5% 60%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 6% 60%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 7% 60%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 8% 60%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 9% 60%, rgba(0,0,0,0.3) 1px, transparent 1px), 
    radial-gradient(circle at 10% 60%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 11% 60%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 12% 60%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 13% 60%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 14% 60%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 15% 60%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 16% 60%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 17% 60%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 18% 60%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 19% 60%, rgba(0,0,0,0.3) 1px, transparent 1px), 
    radial-gradient(circle at 21% 60%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 22% 60%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 23% 60%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 24% 60%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 25% 60%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 26% 60%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 27% 60%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 28% 60%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 29% 60%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 30% 60%, rgba(0,0,0,0.3) 1px, transparent 1px), 
    radial-gradient(circle at 31% 60%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 32% 60%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 33% 60%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 34% 60%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 35% 60%, rgba(0,0,0,0.3) 1px, transparent 1px), 
    radial-gradient(circle at 36% 60%, rgba(0,0,0,0.3) 1px, transparent 1px), 


    radial-gradient(circle at 36% 60%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 61%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 62%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 63%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 64%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 65%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 66%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 67%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 68%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 69%, rgba(0,0,0,0.3) 1px, transparent 1px), 
    radial-gradient(circle at 36% 70%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 71%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 72%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 73%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 74%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 75%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 76%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 77%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 78%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 79%, rgba(0,0,0,0.3) 1px, transparent 1px), 
    radial-gradient(circle at 36% 80%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 81%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 82%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 83%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 84%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 85%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 86%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 87%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 88%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 89%, rgba(0,0,0,0.3) 1px, transparent 1px), 
    radial-gradient(circle at 36% 90%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 91%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 92%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 93%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 94%, rgba(0,0,0,0.3) 1px, transparent 1px), 
    radial-gradient(circle at 36% 95%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 96%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 97%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 98%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 99%, rgba(0,0,0,0.3) 1px, transparent 1px),
    radial-gradient(circle at 36% 100%, rgba(0,0,0,0.3) 1px, transparent 1px);



  
  background-repeat: no-repeat;
  background-size: 100% 100%, 100% 100%, 100% 100%;
}



     

/* Efecto holográfico */
.holographic {
    background: linear-gradient(45deg, #ff0080, #ff8c00, #40e0d0, #ff0080);
    background-size: 400% 400%;
    animation: hologram 3s ease-in-out infinite;
}

@keyframes hologram {
    0%, 100% { background-position: 0% 50%; }
    50% { background-position: 100% 50%; }
}

/* Para que parezca de metal */
.metal-texture {
    background: 
        radial-gradient(circle at 25% 25%, rgba(255,255,255,0.2) 0%, transparent 50%),
        radial-gradient(circle at 75% 75%, rgba(255,255,255,0.1) 0%, transparent 50%);
}
</style>
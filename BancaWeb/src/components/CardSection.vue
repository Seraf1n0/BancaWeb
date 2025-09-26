<script setup>
import { ref } from "vue"
import Card from "./CreditCard.vue"
import Modal from "./CardModal.vue"


const cards = [
  { type: "Gold", number: "1111222233334444", valid: "12/26", owner: "Paulo Gonzales Maradona", pin: "1234", cvv: "999", currency: "USD", limit: 5000, balance: 1500  },
  { type: "Platinum", number: "5555666677778888", valid: "06/29", owner: "Benito Montes Lackwood", pin: "4321", cvv: "888", currency: "CRC", limit: 10000, balance: 2000  },
  { type: "Black", number: "9999000011112222", valid: "01/30", owner: "Neymar Santos Junior", pin: "0000", cvv: "777", currency: "USD", limit: 20000, balance: 7500  }
]

const movements = [
  { id: "mov001", card_id: "1111222233334444", date: "2025-09-25T12:00:00Z", type: "COMPRA", description: "Pago servicios", currency: "USD", amount: 200.50 },
  { id: "mov002", card_id: "1111222233334444", date: "2025-09-24T08:30:00Z", type: "PAGO", description: "Depósito nómina", currency: "USD", amount: 1500.00 },
  { id: "mov003", card_id: "5555666677778888", date: "2025-09-20T10:15:00Z", type: "COMPRA", description: "Supermercado", currency: "CRC", amount: 35000.75 }
]

const index = ref(0)
const isModalOpen = ref(false);
const selectedCard= ref(null)
const cardMovements = ref([])

function nextCard() {
  if(index.value < cards.length - 1){
    index.value++
  }
}

function prevCard() {
  if(index.value > 0){
    index.value--
  }
}

function openModal(card){
  selectedCard.value = {
    ...card,
    numberMasked: card.number.slice(0,4) + " **** **** " + card.number.slice(-4)
  }
  cardMovements.value = movements.filter(m => m.card_id === card.number)
  isModalOpen.value = true
}

function closeModal(){
  selectedCard.value = null
  cardMovements.value = []
  isModalOpen.value = false
}
</script>

<template>
  <div class="carousel-container">

    <div
      class="carousel-track"
      :style="{ transform: `translateX(${-index * 320}px)` }"
    >
      <Card
        class="card"
        v-for="(card, i) in cards"
        :key="i"
        :type="card.type"
        :number="card.number"
        :valid="card.valid"
        :owner="card.owner"
        @click="openModal(card)"
      />
    </div>
  <button @click="prevCard" class="carousel-arrow left-arrow" :disabled="index === 0" aria-label="Tarjeta anterior">
    <i class="fa-solid fa-arrow-left"></i>
  </button>
  <button @click="nextCard" class="carousel-arrow right-arrow" :disabled="index === cards.length - 1" aria-label="Proxima tarjeta">
    <i class="fa-solid fa-arrow-right"></i>
  </button>

  <Modal v-if="isModalOpen" @close="closeModal">
    <div class="card-detail">
      <h2>Información de la cuenta</h2>
      <p><b>Tipo cuenta:</b> {{ selectedCard.type }}</p>
      <p><b>Número:</b> {{ selectedCard.numberMasked }}</p>
      <p><b>Exp:</b> {{ selectedCard.valid }}</p>
      <p><b>PIN:</b> {{ selectedCard.pin }}</p>
      <p><b>CVV:</b> {{ selectedCard.cvv }}</p>
      <p><b>Titular:</b> {{ selectedCard.owner }}</p>
      <p><b>Moneda:</b> {{ selectedCard.currency }}</p>
      <p><b>Límite:</b> {{ selectedCard.limit }}</p>
      <p><b>Saldo:</b> {{ selectedCard.balance }}</p>

      <h3>Movimientos</h3>
      <ul class="movements">
        <li v-for="m in cardMovements" :key="m.id" :class="{'movement-buy': m.type === 'COMPRA','movement-pay': m.type === 'PAGO' }">
          <p><b>{{ m.type }}</b> - {{ m.description }}</p>
          <small>{{ m.date }}</small>
          <p><b>Monto:</b> <span class="amount-value">{{ m.amount }} {{ m.currency }}</span></p>
        </li>
      </ul>
    </div>
  </Modal>
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

/*Estilos de flecha */
.carousel-arrow{
  position: absolute;
  top: 60%;
  transform: translateY(-50%);
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 50%;

  background: rgba(255,255,255,0.95);
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.15);  
  transition: transform 0.12s ease, box-shadow 0.12s ease,;

  color: #333;
  cursor: pointer;
}

.carousel-arrow:hover {
  transform: translateY(-50%) scale(1.06);
  box-shadow: 0 10px 28px rgba(0,0,0,0.18);
}

.left-arrow {
  left: 1%;
  width: 25px;
  height: 25px;
}

.right-arrow{
  right: 1%;
  width: 25px;
  height: 25px;
}
/*Hasta aqui estilos flecha */

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
  transition: transform 0.2s ease;
  cursor: pointer;
}

.card:active{
  transform: scale(1.05);
}


.card-detail {
  background: white;
  border-radius: 12px;
  padding: 24px;
  max-width: 100%;
  margin: 0 auto;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.15);
}

.card-detail h2 {
  color: #000000; /* Aqui después lo hare por color de tarjeta*/
  font-size: 24px;
  font-weight: 700;
  margin-bottom: 20px;
  text-align: center;
  border-bottom: 2px solid #e5e7eb;
  padding-bottom: 12px;
}

.card-detail p {
  margin: 12px 0;
  font-size: 16px;
  color: #374151;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 8px 12px;
  background: #f9fafb;
  border-radius: 6px;
  border-left: 4px solid #3b82f6;
}

.card-detail p b {
  color: #1f2937;
  font-weight: 600;
  min-width: 80px;
}

.card-detail h3 {
  color: #1f2937;
  font-size: 20px;
  font-weight: 600;
  margin: 24px 0 16px 0;
  text-align: center;
  position: relative;
}

.card-detail h3::after {
  content: '';
  position: absolute;
  bottom: -4px;
  left: 50%;
  transform: translateX(-50%);
  width: 60px;
  height: 3px;
  background: linear-gradient(90deg, #3b82f6, #1d4ed8);
  border-radius: 2px;
}

.movements {
  list-style: none;
  padding: 0;
  margin: 0;
  max-height: 300px;
  overflow-y: auto;
}

.movements li {
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  padding: 16px;
  margin-bottom: 12px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  transition: transform 0.2s ease, box-shadow 0.2s ease;
}

.movements li:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

.movements li p:first-child {
  margin: 0 0 8px 0;
  font-size: 16px;
  color: #1f2937;
  background: none;
  border: none;
  padding: 0;
  display: block;
}

.movements li p:first-child b {
  color: #059669; 
  background: #d1fae5;
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 14px;
  margin-right: 8px;
}

.movements li small {
  color: #6b7280;
  font-size: 12px;
  font-style: italic;
  display: block;
  margin-bottom: 8px;
}

.movement-buy p:last-child {
  content: '- ';
  color: #dc2626;
}

.movement-buy p:last-child b::after {
  content: '-';
}

.movement-pay p:last-child {
  color: #16a34a;
}

.movement-buy .amount-value::before {
  content: '-';
  color: #dc2626;
}

.movement-pay .amount-value::before {
  content: '+';
  color: #16a34a;
}

.movement-buy p:first-child b {
  color: #dc2626 !important;
  background: #fef2f2 !important;
}
</style>

<template>
  <div class="dashboard-container" role="main" aria-label="Panel principal del banco">
    <!-- Header con iconos y otras cosas -->
    <header class="dashboard-header" role="banner">
      <h1 class="dashboard-title" id="main-title">Mi Banca</h1>
      <div class="header-controls">
        <ThemeToggle />
        <button
          class="menu-toggle"
          @click="toggleMenu"
          :aria-expanded="isMenuOpen"
          aria-controls="dashboard-nav"
          aria-label="Alternar menú de navegación"
          type="button"
        >
          <span class="menu-icon"></span>
          <span class="menu-icon"></span>
          <span class="menu-icon"></span>
        </button>
      </div>
    </header>

    <!-- Navegación lateral para movil -->
    <nav
      class="dashboard-nav"
      :class="{ 'nav-open': isMenuOpen }"
      role="navigation"
      aria-labelledby="nav-title"
      id="dashboard-nav"
    >
      <h2 class="nav-title" id="nav-title">Navegación</h2>
      <ul class="nav-list" role="list">
        <li class="nav-item" role="listitem">
          <button
            class="nav-button"
            type="button"
            :class="{ active: activeSection === 'accounts' }"
            @click="setActiveSection('accounts')"
            aria-describedby="accounts-desc"
          >
            Cuentas
          </button>
          <span id="accounts-desc" class="sr-only">Ver estado de tus cuentas bancarias</span>
        </li>
        <li class="nav-item" role="listitem">
          <button
            class="nav-button"
            type="button"
            :class="{ active: activeSection === 'cards' }"
            @click="setActiveSection('cards')"
            aria-describedby="cards-desc"
          >
            Tarjetas
          </button>
          <span id="cards-desc" class="sr-only">Gestionar tus tarjetas de crédito y débito</span>
        </li>
        <li class="nav-item" role="listitem">
          <button
            class="nav-button"
            type="button"
            :class="{ active: activeSection === 'transfers' }"
            @click="setActiveSection('transfers')"
            aria-describedby="transfers-desc"
          >
            Transferencias
          </button>
          <span id="transfers-desc" class="sr-only">Realizar transferencias de dinero</span>
        </li>
      </ul>
    </nav>

    <!-- Contenido principal dl dashboar -->
    <main class="dashboard-main" role="main" aria-labelledby="content-title">
      <div class="content-header">
        <h2 class="content-title" id="content-title">
          {{ getSectionTitle(activeSection) }}
        </h2>
      </div>

      <div
        class="content-grid"
        role="region"
        :aria-label="`Contenido de ${getSectionTitle(activeSection)}`"
      >
        <!-- Secciones con el contenido dinamico -->

        <!-- Componente AccountSection-->
        <section
          v-if="activeSection === 'accounts'"
          class="content-section"
          aria-labelledby="accounts-section-title"
        >
          <h3 id="accounts-section-title" class="section-title">Estado de Cuentas</h3>
          <AccountSection
            :showSummary="true"
            :totalAccounts="accounts.length"
            :totalBalanceCRC="totalBalanceCRC"
            :totalBalanceUSD="totalBalanceUSD"
          >
            <AccountCard
              v-for="acc in accounts"
              :key="acc.account_id"
              :account="acc"
              @viewDetails="handleViewDetails"
            />
          </AccountSection>
        </section>

        <!-- Componente CardsSection-->
        <section
          v-else-if="activeSection === 'cards'"
          class="content-section"
          aria-labelledby="cards-section-title"
        >
          <h3 id="cards-section-title" class="section-title">Mis Tarjetas</h3>
          <CardSection> </CardSection>
        </section>

        <!-- Componente Transferencias -->
        <section
          v-else-if="activeSection === 'transfers'"
          class="content-section"
          aria-labelledby="transfers-section-title"
        >
          <h3 id="transfers-section-title" class="section-title">Formulario de Transferencias</h3>
          <TransfersSection :accounts="accounts">
            <template v-slot="{ transferType, accounts }">
              <TransferForm
                v-if="transferType"
                :transfer-type="transferType"
                :accounts="accounts"
                @submit-transfer="handleSubmitTransfer"
              />
            </template>
          </TransfersSection>
        </section>
      </div>
    </main>

    <!-- Transfer Modal -->
    <TransferModal
      :show="showTransferModal"
      :transfer-data="pendingTransfer"
      :accounts="accounts"
      :is-processing="isProcessingTransfer"
      @close="handleCloseTransferModal"
      @confirm="handleConfirmTransfer"
    />

    <!-- Account Detail Modal -->
    <AccountDetailModal
      :show="showAccountDetailModal"
      :account="selectedAccount"
      @close="handleCloseAccountDetail"
    />
  </div>
</template>

<script lang="ts">
import AccountSection from '../components/AccountsSection.vue'
import AccountCard from '../components/AccountCard.vue'
import CardSection from '../components/CardSection.vue'
import TransfersSection from './TransfersSection.vue'
import TransferForm from './TransferForm.vue'
import TransferModal from './TransferModal.vue'
import AccountDetailModal from './AccountDetailModal.vue'
import ThemeToggle from './ThemeToggle.vue'

interface Account {
  account_id: string
  alias: string
  tipo: 'Ahorro' | 'Corriente'
  moneda: 'CRC' | 'USD'
  saldo: number
  propietario: string
}

interface TransferData {
  fromAccount: string
  toAccount: string
  amount: number
  currency?: string
  description?: string
  type?: string
}

export default {
  name: 'Dashboard',
  components: {
    AccountSection,
    AccountCard,
    CardSection,
    TransfersSection,
    TransferForm,
    TransferModal,
    AccountDetailModal,
    ThemeToggle,
  },
  data() {
    return {
      isMenuOpen: false as boolean,
      activeSection: 'accounts' as 'accounts' | 'cards' | 'transfers',
      accounts: [
        {
          account_id: 'CR-001-12345',
          alias: 'Cuenta Nómina',
          tipo: 'Ahorro',
          moneda: 'CRC',
          saldo: 250000,
          propietario: 'USER-001',
        },
        {
          account_id: 'US-009-54322',
          alias: 'Cuenta Dólares',
          tipo: 'Corriente',
          moneda: 'USD',
          saldo: 400,
          propietario: 'USER-001',
        },
        {
          account_id: 'US-009-54323',
          alias: 'Cuenta Dólares',
          tipo: 'Corriente',
          moneda: 'USD',
          saldo: 400,
          propietario: 'USER-001',
        },
        {
          account_id: 'US-009-54324',
          alias: 'Cuenta Dólares',
          tipo: 'Corriente',
          moneda: 'USD',
          saldo: 400,
          propietario: 'USER-001',
        },
        {
          account_id: 'US-009-54321',
          alias: 'Cuenta Dólares',
          tipo: 'Corriente',
          moneda: 'USD',
          saldo: 400,
          propietario: 'USER-001',
        },
        {
          account_id: 'US-009-54322',
          alias: 'Cuenta Dólares',
          tipo: 'Corriente',
          moneda: 'USD',
          saldo: 400,
          propietario: 'USER-001',
        },
        {
          account_id: 'US-009-54325',
          alias: 'Cuenta Dólares',
          tipo: 'Corriente',
          moneda: 'USD',
          saldo: 400,
          propietario: 'USER-001',
        },
        {
          account_id: 'US-009-54326',
          alias: 'Cuenta Dólares',
          tipo: 'Corriente',
          moneda: 'USD',
          saldo: 400,
          propietario: 'USER-001',
        },
        {
          account_id: 'US-009-543217',
          alias: 'Cuenta Dólares',
          tipo: 'Corriente',
          moneda: 'USD',
          saldo: 400,
          propietario: 'USER-001',
        },
      ] as Account[],
      transferType: 'propias' as string,
      showTransferModal: false as boolean,
      pendingTransfer: null as TransferData | null,
      isProcessingTransfer: false as boolean,
      showAccountDetailModal: false as boolean,
      selectedAccount: null as Account | null,
    }
  },
  computed: {
    totalBalanceCRC(): number {
      return this.accounts
        .filter((a: Account) => a.moneda === 'CRC')
        .reduce((acc: number, a: Account) => acc + a.saldo, 0)
    },
    totalBalanceUSD(): number {
      return this.accounts
        .filter((a: Account) => a.moneda === 'USD')
        .reduce((acc: number, a: Account) => acc + a.saldo, 0)
    },
  },
  methods: {
    toggleMenu(): void {
      this.isMenuOpen = !this.isMenuOpen
      const menuButton = document.querySelector('.menu-toggle') as HTMLElement | null
      if (menuButton) {
        menuButton.setAttribute('aria-expanded', this.isMenuOpen.toString())
      }
    },
    setActiveSection(section: 'accounts' | 'cards' | 'transfers'): void {
      this.activeSection = section
      this.isMenuOpen = false
      const menuButton = document.querySelector('.menu-toggle') as HTMLElement | null
      if (menuButton) {
        menuButton.setAttribute('aria-expanded', 'false')
      }
    },
    getSectionTitle(section: 'accounts' | 'cards' | 'transfers'): string {
      const titles: Record<string, string> = {
        accounts: 'Cuentas',
        cards: 'Tarjetas',
        transfers: 'Transferencias',
      }
      return titles[section] || 'Dashboard'
    },
    handleViewDetails(accountId: string): void {
      console.log('Detalles de cuenta:', accountId)
      this.selectedAccount = this.accounts.find((acc) => acc.account_id === accountId) || null
      this.showAccountDetailModal = !!this.selectedAccount
    },
    handleSubmitTransfer(transferData: TransferData): void {
      this.pendingTransfer = transferData
      this.showTransferModal = true
    },
    handleConfirmTransfer(transferData: TransferData): void {
      this.isProcessingTransfer = true
      setTimeout(() => {
        console.log('Transferencia procesada:', transferData)
        this.isProcessingTransfer = false
        this.showTransferModal = false
        this.pendingTransfer = null
        alert('¡Transferencia realizada con éxito!')
      }, 2000)
    },
    handleCloseTransferModal(): void {
      if (!this.isProcessingTransfer) {
        this.showTransferModal = false
        this.pendingTransfer = null
      }
    },
    handleCloseAccountDetail(): void {
      this.showAccountDetailModal = false
      this.selectedAccount = null
    },
  },
}
</script>

<style scoped>
/* mobile first, estilos base: */
.dashboard-container {
  min-height: 100vh;
  background-color: var(--bg-primary);
  color: var(--text-primary);
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
}

/* Header */
.dashboard-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem;
  background-color: var(--bg-primary);
  border-bottom: 1px solid var(--border-primary);
  position: sticky;
  top: 0;
  z-index: 1000;
}

.dashboard-title {
  margin: 0;
  font-size: 1.25rem;
  font-weight: 600;
  color: var(--text-primary);
}

.header-controls {
  display: flex;
  align-items: center;
  gap: 1rem;
}

/* Botón hamburguesa */
.menu-toggle {
  background: none;
  border: none;
  cursor: pointer;
  padding: 0.5rem;
  border-radius: 4px;
  transition: background-color 0.2s ease;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  width: 25px;
  height: 25px;
  position: relative;
}

.menu-toggle:hover {
  background-color: var(--bg-tertiary);
}

.menu-toggle:focus {
  outline: 2px solid var(--accent-primary);
  outline-offset: 2px;
}

/* Las 3 rayitas del hamburger */
.menu-icon {
  display: block;
  width: 100%;
  height: 2px;
  background-color: var(--text-secondary);
  transition: all 0.3s ease;
  border-radius: 1px;
}

/* Nav menu */
.dashboard-nav {
  position: fixed;
  top: 0;
  left: -100%;
  width: 280px;
  height: 100vh;
  background-color: var(--bg-secondary);
  padding: 1rem;
  transition: left 0.3s ease;
  z-index: 1500;
  border-right: 1px solid var(--border-primary);
  overflow-y: auto;
}

.dashboard-nav.nav-open {
  left: 0;
}

.nav-title {
  margin: 0 0 1.5rem 0;
  font-size: 1.1rem;
  font-weight: 500;
  color: var(--text-primary);
  border-bottom: 1px solid var(--border-primary);
  padding-bottom: 0.5rem;
}

.nav-list {
  list-style: none;
  padding: 0;
  margin: 0;
}

.nav-item {
  margin-bottom: 0.5rem;
}

.nav-button {
  width: 100%;
  padding: 0.75rem 1rem;
  background: none;
  border: none;
  color: var(--text-secondary);
  text-align: left;
  cursor: pointer;
  border-radius: 4px;
  font-size: 0.95rem;
  transition:
    background-color 0.2s ease,
    color 0.2s ease;
}

.nav-button:hover {
  background-color: var(--bg-tertiary);
  color: var(--text-primary);
}

.nav-button.active {
  background-color: var(--accent-primary);
  color: white;
}

.nav-button:focus {
  outline: 2px solid var(--accent-primary);
  outline-offset: 2px;
}

/* main container */
.dashboard-main {
  padding: 1rem;
}

.content-header {
  margin-bottom: 1.5rem;
}

.content-title {
  margin: 0;
  font-size: 1.5rem;
  font-weight: 600;
  color: var(--text-primary);
}

.content-grid {
  display: grid;
  gap: 1rem;
  grid-template-columns: 1fr;
}

.content-section {
  background-color: var(--bg-secondary);
  border: 1px solid var(--border-primary);
  border-radius: 8px;
  padding: 1.5rem;
}

.section-title {
  margin: 0 0 1rem 0;
  font-size: 1.1rem;
  font-weight: 500;
  color: var(--text-primary);
}

.placeholder-content {
  padding: 2rem;
  text-align: center;
  color: var(--text-secondary);
  background-color: var(--bg-tertiary);
  border-radius: 4px;
  border: 1px dashed var(--border-primary);
}

/* Para accesibilidad */
.sr-only {
  position: absolute;
  width: 1px;
  height: 1px;
  padding: 0;
  margin: -1px;
  overflow: hidden;
  white-space: nowrap;
  border: 0;
}

.dashboard-nav.nav-open::after {
  content: '';
  position: fixed;
  top: 0;
  left: 280px;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  z-index: -1;
}

/* Media query para tablets y desktop */
@media (min-width: 1024px) {
  .dashboard-container {
    display: grid;
    grid-template-columns: 280px 1fr;
    grid-template-rows: auto 1fr;
    grid-template-areas:
      'nav header'
      'nav main';
  }

  .dashboard-header {
    grid-area: header;
  }

  .menu-toggle {
    display: none;
  }

  .dashboard-nav {
    position: static;
    grid-area: nav;
    left: 0;
    width: auto;
    height: auto;
    z-index: auto;
  }

  .dashboard-nav::after {
    display: none;
  }

  .dashboard-main {
    grid-area: main;
    padding: 2rem;
  }

  .content-grid {
    grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  }
}
</style>

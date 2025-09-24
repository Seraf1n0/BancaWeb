<!-- Componente para card de una cuenta -->
<template>
    <div class="dashboard-container" role="main" aria-label="Panel principal del banco">
        <!-- Header con navegación -->
        <header class="dashboard-header" role="banner">
            <h1 class="dashboard-title" id="main-title">Mi Banca</h1>
            <button class="menu-toggle" type="button" aria-expanded="false" aria-controls="dashboard-nav"
                aria-label="Abrir menú de navegación" @click="toggleMenu">
                <span class="menu-icon"></span>
                <span class="menu-icon"></span>
                <span class="menu-icon"></span>
            </button>
        </header>

        <!-- Navegación lateral/móvil -->
        <nav class="dashboard-nav" :class="{ 'nav-open': isMenuOpen }" role="navigation" aria-labelledby="nav-title"
            id="dashboard-nav">
            <h2 class="nav-title" id="nav-title">Navegación</h2>
            <ul class="nav-list" role="list">
                <li class="nav-item" role="listitem">
                    <button class="nav-button" type="button" :class="{ active: activeSection === 'accounts' }"
                        @click="setActiveSection('accounts')" aria-describedby="accounts-desc">
                        Cuentas
                    </button>
                    <span id="accounts-desc" class="sr-only">Ver estado de tus cuentas bancarias</span>
                </li>
                <li class="nav-item" role="listitem">
                    <button class="nav-button" type="button" :class="{ active: activeSection === 'cards' }"
                        @click="setActiveSection('cards')" aria-describedby="cards-desc">
                        Tarjetas
                    </button>
                    <span id="cards-desc" class="sr-only">Gestionar tus tarjetas de crédito y débito</span>
                </li>
                <li class="nav-item" role="listitem">
                    <button class="nav-button" type="button" :class="{ active: activeSection === 'transfers' }"
                        @click="setActiveSection('transfers')" aria-describedby="transfers-desc">
                        Transferencias
                    </button>
                    <span id="transfers-desc" class="sr-only">Realizar transferencias de dinero</span>
                </li>
                <li class="nav-item" role="listitem">
                    <button class="nav-button" type="button" :class="{ active: activeSection === 'reports' }"
                        @click="setActiveSection('reports')" aria-describedby="reports-desc">
                        Reportes
                    </button>
                    <span id="reports-desc" class="sr-only">Ver reportes y análisis financiero</span>
                </li>
            </ul>
        </nav>

        <!-- Contenido principal -->
        <main class="dashboard-main" role="main" aria-labelledby="content-title">
            <div class="content-header">
                <h2 class="content-title" id="content-title">
                    {{ getSectionTitle(activeSection) }}
                </h2>
            </div>

            <div class="content-grid" role="region" :aria-label="`Contenido de ${getSectionTitle(activeSection)}`">
                <!-- Sección de contenido dinámico -->
                <section v-if="activeSection === 'accounts'" class="content-section"
                    aria-labelledby="accounts-section-title">
                    <h3 id="accounts-section-title" class="section-title">Estado de Cuentas</h3>
                    <div class="placeholder-content">
                        <p>Contenido de cuentas será agregado aquí</p>
                    </div>
                </section>

                <section v-else-if="activeSection === 'cards'" class="content-section"
                    aria-labelledby="cards-section-title">
                    <h3 id="cards-section-title" class="section-title">Mis Tarjetas</h3>
                    <div class="placeholder-content">
                        <p>Contenido de tarjetas será agregado aquí</p>
                    </div>
                </section>

                <section v-else-if="activeSection === 'transfers'" class="content-section"
                    aria-labelledby="transfers-section-title">
                    <h3 id="transfers-section-title" class="section-title">Transferencias</h3>
                    <div class="placeholder-content">
                        <p>Contenido de transferencias será agregado aquí</p>
                    </div>
                </section>

                <section v-else-if="activeSection === 'reports'" class="content-section"
                    aria-labelledby="reports-section-title">
                    <h3 id="reports-section-title" class="section-title">Reportes</h3>
                    <div class="placeholder-content">
                        <p>Contenido de reportes será agregado aquí</p>
                    </div>
                </section>
            </div>
        </main>
    </div>
</template>

<script>
export default {
    name: 'Dashboard',
    data() {
        return {
            isMenuOpen: false,
            activeSection: 'accounts'
        }
    },
    methods: {
        toggleMenu() {
            this.isMenuOpen = !this.isMenuOpen
            // Actualizar aria-expanded para accesibilidad
            const menuButton = document.querySelector('.menu-toggle')
            menuButton.setAttribute('aria-expanded', this.isMenuOpen.toString())
        },
        setActiveSection(section) {
            this.activeSection = section
            // Cerrar menú en móvil después de seleccionar
            this.isMenuOpen = false
            const menuButton = document.querySelector('.menu-toggle')
            menuButton.setAttribute('aria-expanded', 'false')
        },
        getSectionTitle(section) {
            const titles = {
                accounts: 'Cuentas',
                cards: 'Tarjetas',
                transfers: 'Transferencias',
                reports: 'Reportes'
            }
            return titles[section] || 'Dashboard'
        }
    }
}
</script>

<style scoped>
/* Mobile First - Estilos base para móvil */
.dashboard-container {
    min-height: 100vh;
    background-color: #1a1a1a;
    color: #e0e0e0;
    font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
}

/* Header */
.dashboard-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 1rem;
    background-color: #2d2d2d;
    border-bottom: 1px solid #404040;
}

.dashboard-title {
    margin: 0;
    font-size: 1.25rem;
    font-weight: 600;
    color: #ffffff;
}

.menu-toggle {
    display: flex;
    flex-direction: column;
    justify-content: center;
    width: 2rem;
    height: 2rem;
    background: none;
    border: none;
    cursor: pointer;
    padding: 0.25rem;
}

.menu-icon {
    width: 100%;
    height: 2px;
    background-color: #e0e0e0;
    margin: 2px 0;
}

/* Navegación */
.dashboard-nav {
    position: fixed;
    top: 0;
    left: -100%;
    width: 280px;
    height: 100vh;
    background-color: #2d2d2d;
    padding: 1rem;
    transition: left 0.3s ease;
    z-index: 1000;
    border-right: 1px solid #404040;
}

.dashboard-nav.nav-open {
    left: 0;
}

.nav-title {
    margin: 0 0 1.5rem 0;
    font-size: 1.1rem;
    font-weight: 500;
    color: #ffffff;
    border-bottom: 1px solid #404040;
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
    color: #b0b0b0;
    text-align: left;
    cursor: pointer;
    border-radius: 4px;
    font-size: 0.95rem;
    transition: background-color 0.2s ease, color 0.2s ease;
}

.nav-button:hover {
    background-color: #404040;
    color: #ffffff;
}

.nav-button.active {
    background-color: #0066cc;
    color: #ffffff;
}

.nav-button:focus {
    outline: 2px solid #0066cc;
    outline-offset: 2px;
}

/* Contenido principal */
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
    color: #ffffff;
}

.content-grid {
    display: grid;
    gap: 1rem;
    grid-template-columns: 1fr;
}

.content-section {
    background-color: #2d2d2d;
    border: 1px solid #404040;
    border-radius: 8px;
    padding: 1.5rem;
}

.section-title {
    margin: 0 0 1rem 0;
    font-size: 1.1rem;
    font-weight: 500;
    color: #ffffff;
}

.placeholder-content {
    padding: 2rem;
    text-align: center;
    color: #808080;
    background-color: #1a1a1a;
    border-radius: 4px;
    border: 1px dashed #404040;
}

/* Clases de accesibilidad */
.sr-only {
    position: absolute;
    width: 1px;
    height: 1px;
    padding: 0;
    margin: -1px;
    overflow: hidden;
    clip: rect(0, 0, 0, 0);
    white-space: nowrap;
    border: 0;
}

/* Media query para tablets y desktop */
@media (min-width: 768px) {
    .dashboard-container {
        display: grid;
        grid-template-columns: 280px 1fr;
        grid-template-rows: auto 1fr;
        grid-template-areas:
            "nav header"
            "nav main";
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
<template>
<!-- Rentabilidad -->
<div class="neumorphism-card w-[756px] h-[470px]">

    <!-- Space container -->
    <div class="space-y-[26px] w-full h-full">

        <!-- Titulo card -->
        <div>
            <p class="font-bold text-[20px] m-0">Rentabilidad</p>
            <p class="text-gray-700 text-[16px] m-0">Observa las ganancias de tus proyectos.</p>
        </div>

        <!-- Rentabilidad Total -->
        <div class="flex items-center justify-between">
            <div class="flex flex-col">
                <p class="font-bold text-[20px] m-0 whitespace-nowrap">Rentabilidad Total</p>
                <p class="text-gray-700 text-[16px] m-0">{{ companies.length }} empresas</p>
            </div>

            <div class="flex flex-col items-end gap-2">
                <span class="bg-green-500 text-white rounded px-3 py-1 font-bold text-base block w-fit neumorphism-on-small-item">
                    {{ companies.length > 0 ? (companies.reduce((sum, c) => sum + c.CurrentProfitability, 0) / companies.length).toFixed(1) : 0 }}%
                </span>
                <span class="text-green-600 text-xs block">
                    {{ companies.length > 0 ? getProfitabilityChange(
                    companies.reduce((sum, c) => sum + c.CurrentProfitability, 0) / companies.length,
                    companies.reduce((sum, c) => sum + c.LastMonthProfitability, 0) / companies.length
                    ) : '+0% vs mes ant.' }}
                </span>
            </div>
        </div>

        <div class="h-60">
            <div class="scrollable-card">
                <div
                    v-for="(company, index) in companies"
                    :key="company.Id"
                    class="flex items-center justify-between w-full pl-5 pt-1"
                >
                    <!-- Izquierda: número + info -->
                    <div class="flex items-center gap-3">
                            <div class="w-[52px] h-[52px] flex items-center justify-center neumorphism-on-small-item font-bold text-lg">
                                {{ index + 1 }}
                            </div>
                            <div>
                                <span class="font-bold">{{ company.name }}</span>
                                <div class="text-gray-600 text-sm">{{ company.ActiveEmployees }} empleados</div>
                            </div>
                    </div>
     
                    <!-- Derecha: rentabilidad -->
                    <div class="flex flex-col items-end">
                        <span :class="`${getProfitabilityColor(company.CurrentProfitability)} text-white rounded px-3 py-1 font-bold text-base neumorphism-on-small-item`">
                            {{ company.CurrentProfitability }}%
                        </span>
                        <span :class="`${getProfitabilityTextColor(company.CurrentProfitability)} text-xs mt-1`">
                            {{ getProfitabilityChange(company.CurrentProfitability, company.LastMonthProfitability) }}
                        </span>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

</template>

<script>
  export default {
    name: 'TarjetaEstadisticas',

    props: {
        userId: {
        type: [String, Number],
        required: true
        }
    },

    data() {
        return {
            companies: [],
            loading: true,
            error: null
        }
    },

    watch: {
        userId: {
            immediate: true,
            handler(newId) {
                if (newId) this.fetchCompaniesByUser(newId);
            }
        }
    },

    methods: {
        getProfitabilityColor(profitability) {
            if (profitability >= 20) return 'bg-green-500'
            if (profitability >= 15) return 'bg-gray-400'
            return 'bg-red-500'
        },

        getProfitabilityTextColor(profitability) {
            if (profitability >= 20) return 'text-green-600'
            if (profitability >= 15) return 'text-gray-600'
            return 'text-red-600'
        },

        getProfitabilityChange(current, last) {
            const change = current - last
            if (change > 0) return `+${change}% vs mes ant.`
            if (change < 0) return `${change}% vs mes ant.`
            return '+0% vs mes ant.'
        },

        async fetchCompaniesByUser(userId) {
            try {
                this.loading = true;
                this.error = null;
                const response = await fetch(`http://localhost:5011/api/Project/dashboard/${userId}`);
                if (!response.ok) throw new Error('No se pudieron cargar las empresas');
                const data = await response.json();
                this.companies = data;
            } catch (err) {
                this.error = err.message || 'Error al cargar las empresas';
            } finally {
                this.loading = false;
            }
        },
    }
}
</script>
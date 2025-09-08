<script>
export default {
  name: 'CountriesList',
  data() {
    return {
      countries: [
        { name: "Costa Rica", continent: "América", language: "Español"},
        { name: "Japón", continent: "Asia", language: "Japonés" },
        { name: "Corea del Sur", continent: "Asia", language: "Coreano"},
        { name: "Italia", continent: "Europa", language: "Italiano" },
        { name: "Alemania", continent: "Europa", language: "Alemán" },
      ],
    };
  },
  methods: {
    eliminar(index) {
      this.countries.splice(index, 1);
    },
  },
}
</script>

<template>
  <div class="container mt-5">
  <h1 class="display-4 text-center">Lista de países</h1>
    <table
    class="table is-bordered is-striped is-narrow is-hoverable
    is-fullwidth"
    >
      <thead>
        <tr>
          <th>Nombre</th>
          <th>Continente</th>
          <th>Idioma</th>
          <th>Acciones</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(country, index) in countries" :key="index">
          <td>{{country.name}}</td>
          <td>{{country.continent}}</td>
          <td>{{country.language}}</td>
          <td>
            <button class="btn btn-secondary btn-sm">Editar</button>
            <button class="btn btn-danger btn-sm" v-on:click=eliminar(index)>Eliminar</button>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<style lang="scss" scoped>

</style>
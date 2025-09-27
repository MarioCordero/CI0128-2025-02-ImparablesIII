<template>
  <div id="app">
    <NavigationBar />
    <router-view />
  </div>
</template>

<script>
import NavigationBar from './components/Navigation.vue'

export default {
  name: 'App',
  components: {
    NavigationBar
  }
};
</script>

<style>
#app {
  min-height: 100vh;
  background-color: #f9fafb;
}
</style>
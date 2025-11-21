<template> 
  <div class="neumorphism-card p-[25px]! w-full!">
    <EmployeesList :project-id="projectId"/>
  </div>
</template>

<script>
import EmployeesList from "./EmployeesList.vue";

export default {
  name: 'EmployeesSection',
  components: {
    EmployeesList
  },
  props: {
    projectId: {
      type: [String, Number],
      required: true
    }
  }
}
</script>
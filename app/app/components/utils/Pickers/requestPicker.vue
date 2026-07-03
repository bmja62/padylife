<script setup lang="ts">
const props = defineProps({
  required: {
    type: Boolean as PropType<boolean>,
    default: false
  }
})
const selectedOption = defineModel()
const specialistCategories = ref([])
const specialistCategoriesFilters = ref({
  pageNumber: 1,
  count: 10,
  searchCommand: ''
})

function searchCategories(event) {
  specialistCategoriesFilters.value.searchCommand = event
  getSpecialistCategories()
}

async function getSpecialistCategories() {
  try {
    const {data} = await useApi.getSpecialistCategories.setParams(specialistCategoriesFilters.value, false)
    if (data.value) {
      specialistCategories.value = []
      specialistCategories.value = data.value.data.data.map((item) => {
        return {
          label: item.name,
          value: item.id
        }
      })
    }
  } catch (e) {
    console.log(e)
  } finally {

  }
}

getSpecialistCategories()
</script>

<template>
  <LazyUtilsPickersBasePicker v-if="specialistCategories.length" @search="searchCategories" :required="props.required"
                              title="انتخاب دسته بندی تخصص"
                              v-model="selectedOption"
                              :options="specialistCategories"></LazyUtilsPickersBasePicker>
</template>

<style scoped>

</style>
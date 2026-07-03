<script setup lang="ts">
import type {IApiProvider} from "~/models/IApiProvider";
import type {IPlanCategory} from "~/services/PlanService";

const props = defineProps({
  required: {
    type: Boolean as PropType<boolean>,
    default: false
  }
})
const selectedOption = defineModel()
const {$api} = useNuxtApp<IApiProvider>()
const options = ref<IPlanCategory[]>([])
const optionsFilters = ref<IGlobalGridRequest>({
  pageNumber: 1,
  count: 10,
  search: ''
})

function searchCategories(event) {
  optionsFilters.value.search = event
  getOptions()
}

async function getOptions() {
  try {
    const response = await $api.plan.getAllPlanCategories(optionsFilters.value)
    if (response.data) {
      options.value = []
      options.value = response.data.data.map((item: IPlanCategory) => {
        return {
          label: item.name,
          value: item.id
        }
      })
    }
  } catch (e) {
    console.log(e)
  }
}

getOptions()
</script>

<template>
  <LazyUtilsPickersBasePicker
v-if="options.length" clearable v-model="selectedOption"
                              :required="props.required"
                              @search="searchCategories"
                              placeholder="انتخاب دسته بندی"
                              :options="options"></LazyUtilsPickersBasePicker>
</template>

<style scoped>

</style>
<script setup lang="ts">
import type {IApiProvider} from "~/models/IApiProvider";
import type {ICountry} from "~/services/SettingService";

const props = defineProps({
  required: {
    type: Boolean as PropType<boolean>,
    default: false
  }
})
const selectedOption = defineModel()
const {$api} = useNuxtApp<IApiProvider>()
const options = ref<ICountry[]>([])
const optionsFilters = ref({
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
    const response = await $api.setting.getCountries(optionsFilters.value)

    if (response.data) {
      options.value = []
      options.value = response.data.data.map((item: ICountry) => {
        return {
          label: item.countryName,
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
  <LazyUtilsPickersBasePicker v-if="options.length" @search="searchCategories" :required="props.required"
                              v-model="selectedOption"
                              placeholder="انتخاب کشور"
                              :options="options"></LazyUtilsPickersBasePicker>
</template>

<style scoped>

</style>
<script lang="ts" setup>
import {useSpinner} from "@/composables/spinner";
import {ICitiesFilters, ICitiesListItem, IStatesListItem} from "@/services/CoordinateService";
import {IApiProvider} from "@/models/IApiProvider";

const spinner = useSpinner()
const $api = inject<IApiProvider>('$api')
const isLoading = ref<boolean>(false)
const states = ref<IStatesListItem[]>([])
const cities = ref<ICitiesListItem[]>([])
const citiesFilters = ref<ICitiesFilters>({
  pageNumber: 1,
  count: 200,
  stateId: null
})

export interface ICityState {
  stateId: number | null,
  cityId: number | null

}

const cityStateModel = ref<ICityState>({
  stateId: null,
  cityId: null
})

interface IProps {
  dropdownColor?: string,
  required: boolean,
  defaultStateId?: number,
  defaultCityId?: number,

}

const props = withDefaults(defineProps<IProps>(), {
  dropdownColor: 'success',
  required: false
})

const emits = defineEmits<{
  (e: 'setCityStateId', cityStateModel: ICityState): void
}>()
onMounted(() => {
  getAllStates()
})
watch(() => props.defaultStateId, async (val) => {
  cityStateModel.value.stateId = val as number
  citiesFilters.value.stateId = val as number
  getCitiesByStateId()

}, {immediate: true})
watch(() => props.defaultCityId, async (val) => {
  cityStateModel.value.cityId = val as number

}, {immediate: true})
watch(() => cityStateModel.value.stateId, async (val) => {
  citiesFilters.value.stateId = val
  cityStateModel.value.cityId = null
  getCitiesByStateId()
})
watch(() => cityStateModel.value.cityId, async (val) => {
  emits('setCityStateId', cityStateModel.value)
})

async function getAllStates() {
  try {
    spinner.showSpinner()
    const response = await $api?.coordinates.getAllStates()
    states.value = response?.data.cities as Array<IStatesListItem>
  } catch (error) {
    console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}

async function getCitiesByStateId() {
  try {
    spinner.showSpinner()
    const response = await $api?.coordinates.getCitiesByStateId(citiesFilters.value.stateId)
    cities.value = response?.data.cities as Array<ICitiesListItem>

  } catch (error) {
    console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}
</script>

<template>
  <VRow>
    <VCol cols="6">
      <CustomPicker
        v-model="cityStateModel.stateId"
        :color="props.dropdownColor"
        :dropdownColor="props.dropdownColor"
        :is-loading="isLoading"
        :item-title="(item: IStatesListItem) => item.name"
        :item-value="(item:  IStatesListItem) => item.iranStateId"
        :items="states"
        :return-object="false"
        :rules="required?[(value) => !!value || 'انتخاب استان اجباری است']:''"
        :search-callback="() => {}"
        label="انتخاب استان"
      />
    </VCol>
    <VCol cols="6">
      <CustomPicker
        v-model="cityStateModel.cityId"
        :color="props.dropdownColor"
        :disabled="!cityStateModel.stateId"
        :dropdownColor="props.dropdownColor"
        :is-loading="isLoading"
        :item-title="(item: ICitiesListItem) => item.name"
        :item-value="(item:  ICitiesListItem) => item.iranCityId"
        :items="cities"
        :return-object="false"
        :rules="required?[(value) => !!value || 'انتخاب شهر اجباری است']:''"
        :search-callback="() => {}"
        label="انتخاب شهر"
      />
    </VCol>
  </VRow>
</template>

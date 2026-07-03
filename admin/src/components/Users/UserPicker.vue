<script setup lang="ts">
import {inject} from 'vue'
import {useSpinner} from '@/composables/spinner'
import type {IApiProvider} from '@/models/IApiProvider'
import {IGetUserFilters} from "@/services/UserService";

// Interfaces
interface IEmit {
  (e: 'getUserIds', value: IUser | undefined): void
}
interface IProps {
  defaultUserIds?: IUser
  defaultTagName?: string | null | undefined
  dropdownLabel?: string
  dropdownColor?: string
  multiple?: boolean
  returnObject?: boolean
  roleFilter?: roleNames | null
}

// Props
const props = withDefaults(defineProps<IProps>(), {
  dropdownLabel: 'انتخاب کاربر',
  dropdownColor: 'success',
  multiple: false,
  returnObject: true,
})

// Emits
const emit = defineEmits<IEmit>()

// LifeCycles
onMounted(() => {
  getTags()
})

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const usersList = ref<null | IUser[]>(null)
const totalCount = ref<null | string | number | undefined>(null)
const tempUsers = ref <IUser>()
const isLoading = ref<boolean>(false)

const usersGetFilters = ref<IGetUserFilters>({
  pageNumber: 1,
  count: 100,
  search: undefined,
  roleName: undefined,
})

// Watchers
watch(tempUsers, value => {
  emit('getUserIds', value)
})
watch(() => props.defaultUserIds, async value => {
  if (value)
    tempUsers.value = props.defaultUserIds
}, {
  immediate: true,
})
watch(() => usersGetFilters.value.search, async () => {
  isLoading.value = true
  await getTags()
  isLoading.value = false
})

// Functions
async function getTags() {
  try {
    // spinner.showSpinner()

    if (props.roleFilter)
      usersGetFilters.value.roleName = props.roleFilter
    else
      usersGetFilters.value.roleName = null

    const response = await $api?.users.getUsers(usersGetFilters.value)

    usersList.value = response?.data.data.data as Array<IUser>
    totalCount.value = response?.data.data.totalCo as number
  }
  catch (error) {
    console.error(error)
  }
  finally {
    // spinner.hideSpinner()
  }
}

function debouncedRequest(value: string | null) {
    usersGetFilters.value.name = value ? value.trim() : null
}

function handleSearch(value: string) {
  debouncedRequest(value)
}
</script>

<template>

  <CustomPicker
    v-model="tempUsers"
    :chips="false"
    :multiple="props.multiple"
    :color="props.dropdownColor"
    :label="props.dropdownLabel"
    :is-loading="isLoading"
    :items="usersList"
    :item-title="(item: IUserUpdateOrDeletePayload) => `${item.fullName? item.fullName : item.userName} ${item.phoneNumber ?`(${item.phoneNumber})` : ''}`"
    :item-value="(item: IUserUpdateOrDeletePayload) => item.id"
    :search-callback="handleSearch"
    :return-object="props.returnObject"
  />
</template>

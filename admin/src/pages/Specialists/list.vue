<script setup lang="ts">
import {inject, onMounted, ref} from 'vue'
import type {ITableHeaders} from '@/models/ITableHeader'
import type {IApiProvider} from '@/models/IApiProvider'
import type {IGetUserFilters, IUser} from '@/services/UserService'
import {useSpinner} from '@/composables/spinner'
import {useAlerts} from "@/composables/alert";

// LifeCycles
onMounted(() => {
  getUsers()
})

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const isRenderingCreateDialog = ref<boolean>(false)
const isRenderingUpdateDialog = ref<boolean>(false)
const isRenderingDeleteDialog = ref<boolean>(false)
const usersList = ref<null | IUser[]>(null)
const totalCount = ref<null | string | number | undefined>(null)
const tempUser = ref<IUser>()

const tableHeaders: ITableHeaders = [
  {title: 'شناسه', key: 'id'},
  {
    title: 'نام کاربری',
    key: 'userName',
  },
  {
    title: 'نام و نام خانوادگی',
    key: 'fullName',
  },
  {title: 'شماره همراه', key: 'phoneNumber'},
  {title: 'ایمیل', key: 'email'},
  {title: 'نقش کاربر', key: 'roles'},
  {title: 'وضعیت کاربر', key: 'isActive'},
  {title: 'عملیات', key: 'actions'},
]

const usersFilters = ref<IGetUserFilters>({
  pageNumber: 1,
  count: 10,
  roleName: 'Specialist',
  isActive: null,
  search:''
})

function renderCreateDialog() {
  isRenderingCreateDialog.value = true
}

function renderUpdateDialog(item: IUser) {
  tempUser.value = JSON.parse(JSON.stringify(item))
  isRenderingUpdateDialog.value = true
}

async function getUsers() {
  try {
    spinner.showSpinner()
    const response = await $api?.users.getUsers(usersFilters.value)
    usersList.value = response?.data.data.data as Array<IUser>
    totalCount.value = response?.data.data.totalCount as number
  } catch (error) {
    console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}

async function changePage(pageNumber: number | string) {
  usersFilters.value.pageNumber = +pageNumber
  await getUsers()
}

async function resetFiltersAndGetUsers() {
  usersFilters.value.pageNumber = 1
  await getUsers()
}

async function toggleUserIsActive(user: IUser) {
  try {
    spinner.showSpinner()
    const response = await $api?.users.toggleIsActive(user.id)
    if (response.data.isSuccess) {
      getUsers()
    } else {
      useAlerts().error(response.data.error)
    }
  } catch (error) {
    console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}
</script>

<template>
  <PageWrapper
      has-filters
    @submit-filters="resetFiltersAndGetUsers"
  >
    <template #title>
      لیست متخصصین
    </template>
    <template #filters>
      <VCol
        cols="12"
        md="3"
      >
        <VTextField
          v-model="usersFilters.search"
          label="نام کاربر"
          hide-details
          @keydown.enter="getUsers"
        />
      </VCol>
    
    </template>
    <CustomTable
      :items-list="usersList"
      :count="usersFilters.count"
      :page-number="usersFilters.pageNumber"
      :table-headers="tableHeaders"
      :total-count="totalCount"
      @change-page="changePage"
    >
      <template #actions="data">
        <VBtn
          color="transparent"
          elevation="0"
          icon="mdi-pencil"
          @click="renderUpdateDialog(data.item)"
        />
        <!-- <VBtn
          color="transparent"
          elevation="0"
          :to="`/Users/${data.item.id}`"
        >
          <VIcon icon="mdi-gamepad"></VIcon>
          <VTooltip
            activator="parent"
          >
            امتیازات کاربر
          </VTooltip>
        </VBtn> -->
      </template>
      <template #isActive="data">
        <CustomSwitch
          :label="`${data.item.isActive ? 'فعال' : 'غیرفعال'}`"
          id="customizer-menu-collapsed"
          :has-tooltip="false"
          v-model="data.item.isActive"
          class="ms-2"
          @click="toggleUserIsActive(data.item)"
        />
        <!--        <VChip color="success" v-if="data.item.isActive">فعال</VChip>-->
        <!--        <VChip color="error" v-if="!data.item.isActive">غیر فعال</VChip>-->
      </template>
      <template #roles="data">
        <VChip
          v-for="(role, index) in data.item.roles"
          :key="index"
          flat
          size="small"
          label
          color="primary"
          class="ma-1"
        >
          {{ role.description }}
        </VChip>
      </template>
    </CustomTable>

    <CreateUserDialog
      v-model:dialogState="isRenderingCreateDialog"
      @refetch="getUsers"
    />

    <UpdateUserDialog
      v-model:dialogState="isRenderingUpdateDialog"
      :default-user="tempUser"
      @refetch="getUsers"
    />
  </PageWrapper>
</template>

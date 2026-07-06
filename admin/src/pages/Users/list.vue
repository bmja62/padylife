<script setup lang="ts">
import {isAxiosError} from 'axios'
import {inject, onMounted, ref} from 'vue'
import type {ITableHeaders} from '@/models/ITableHeader'
import type {IApiProvider} from '@/models/IApiProvider'
import type {IGetUserFilters, IUpdateUserRolePayload, IUser} from '@/services/UserService'
import {useSpinner} from '@/composables/spinner'
import {useAlerts} from '@/composables/alert'
import {useAuthStore} from "@/stores/auth"

onMounted(() => {
  getUsers()
  getAllRoles()
})

const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const alert = useAlerts()
const isRenderingCreateDialog = ref<boolean>(false)
const isRenderingUpdateDialog = ref<boolean>(false)
const isRenderingDeleteDialog = ref<boolean>(false)
const isRenderingRoleDialog = ref<boolean>(false)
const usersList = ref<null | IUser[]>(null)
const totalCount = ref<null | string | number | undefined>(null)
const tempUser = ref<IUser>()
const selectedUserRole = ref<string | null>(null)
const authStore = useAuthStore()
const roleOptions = ref<Array<{ title: string, value: string }>>([
  {
    title: 'کاربر عادی',
    value: 'User',
  },
  {
    title: 'ادمین',
    value: 'Admin',
  },
  {
    title: 'متخصص',
    value: 'Specialist',
  },
])

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
  search: '',
  roleName: '',
  onlyExpertUser: authStore.getUser.roles.filter(e => e.name === 'Admin').length ? false : true,
})

function renderDeleteDialog(item: IUser) {
  tempUser.value = JSON.parse(JSON.stringify(item))
  isRenderingDeleteDialog.value = true
}

function renderCreateDialog() {
  isRenderingCreateDialog.value = true
}

function renderUpdateDialog(item: IUser) {
  tempUser.value = JSON.parse(JSON.stringify(item))
  isRenderingUpdateDialog.value = true
}

function extractCurrentManageableRole(user: IUser): string | null {
  const manageableRoles = roleOptions.value.map(item => item.value)
  const role = user.roles.find((item) => manageableRoles.includes(item.name))

  return role?.name || null
}

function renderRoleDialog(item: IUser) {
  tempUser.value = JSON.parse(JSON.stringify(item))
  selectedUserRole.value = extractCurrentManageableRole(item)
  isRenderingRoleDialog.value = true
}

function closeRoleDialog() {
  isRenderingRoleDialog.value = false
  selectedUserRole.value = null
}

async function updateUserRole() {
  if (!tempUser.value || !selectedUserRole.value) {
    alert.error('لطفا نقش کاربر را انتخاب کنید')
    return
  }

  const rolePayload: IUpdateUserRolePayload = {
    userId: tempUser.value.id,
    roles: [selectedUserRole.value],
  }

  const currentRoles = tempUser.value.roles.map((role) => role.name)
  const manageableRoles = roleOptions.value.map(item => item.value)
  const rolesToRemove = currentRoles.filter((role) => manageableRoles.includes(role) && role !== selectedUserRole.value)

  try {
    spinner.showSpinner()

    for (const roleName of rolesToRemove) {
      const revokeResponse = await $api?.users.revokeRoleFromUser({
        userId: tempUser.value.id,
        roles: [roleName],
      })

      if (!revokeResponse?.data.isSuccess) {
        alert.error(revokeResponse?.data.message || 'حذف نقش قبلی انجام نشد')
        return
      }
    }

    if (!currentRoles.includes(selectedUserRole.value)) {
      const addRoleResponse = await $api?.users.addRoleToUser(rolePayload)

      if (!addRoleResponse?.data.isSuccess) {
        alert.error(addRoleResponse?.data.message || 'ثبت نقش جدید انجام نشد')
        return
      }
    }

    alert.success('نقش کاربر با موفقیت تغییر کرد')
    closeRoleDialog()
    await getUsers()
  } catch (error: unknown) {
    if (isAxiosError(error))
      alert.error(error?.response?.data?.message || 'مشکلی پیش آمد، لطفا دوباره امتحان کنید')
    else
      console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}

async function getAllRoles() {
  try {
    const response = await $api?.users.getAllRoles({
      pageNumber: 1,
      count: 100,
    })

    const roles = response?.data?.data?.data || []
    if (roles.length) {
      roleOptions.value = roles.map(role => ({
        title: role.description || role.name,
        value: role.name,
      }))
    }
  } catch (error) {
    console.error(error)
  }
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
</script>

<template>
  <PageWrapper
    has-filters
    @submit-filters="resetFiltersAndGetUsers"
  >
    <template #title>
      لیست کاربر ها
    </template>
<!--    <template #append>-->
<!--      <VBtn-->
<!--        color="success"-->
<!--        @click="renderCreateDialog"-->
<!--      >-->
<!--        ایجاد کاربر جدید-->
<!--      </VBtn>-->
<!--    </template>-->
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
      <VCol
        cols="12"
        md="3"
      >
        <RolePicker :return-object="false" v-model="usersFilters.roleName"></RolePicker>
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
          v-if="$can('manage','all')"
          color="transparent"
          elevation="0"
          icon="mdi-pencil"
          @click="renderUpdateDialog(data.item)"
        />
        <VBtn
          v-if="$can('manage','all')"
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
        </VBtn>
        <VBtn
          color="transparent"
          elevation="0"
          :to="`/Users/${data.item.id}/plans`"
        >
          <VIcon icon="mdi-clipboard-text"></VIcon>
          <VTooltip
            activator="parent"
          >
            پلن های کاربر
          </VTooltip>
        </VBtn>
      </template>

      <template #isActive="data">
        <VChip color="success" v-if="data.item.isActive">فعال</VChip>
        <VChip color="error" v-if="!data.item.isActive">غیر فعال</VChip>
      </template>

      <template #roles="data">
        <VChip
          v-for="(role, index) in data.item.roles"
          :key="index"
          flat
          size="small"
          label
          color="primary"
          class="ma-1 cursor-pointer"
          @click="renderRoleDialog(data.item)"
        >
          {{ role.description }}
          <VTooltip activator="parent">
            تغییر نقش کاربر
          </VTooltip>
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

    <CustomUpdateDialog
      v-model:dialogState="isRenderingRoleDialog"
      :title="`تغییر نقش کاربر: ${tempUser?.fullName || ''}`"
      action-text="ثبت تغییر نقش"
      @update:dialog-state="closeRoleDialog"
      @update="updateUserRole"
    >
      <VCol cols="12">
        <VSelect
          v-model="selectedUserRole"
          :items="roleOptions"
          item-title="title"
          item-value="value"
          label="نقش کاربر"
          variant="outlined"
          density="compact"
          color="success"
          hide-details
        />
      </VCol>
    </CustomUpdateDialog>
  </PageWrapper>
</template>

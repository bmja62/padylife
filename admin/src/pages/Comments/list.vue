<script setup lang="ts">
import type {VDataTable} from 'vuetify/components'
import {inject} from 'vue'
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {IAttribute} from "@/services/ProductAttributes";
import {IEntityComment, IEntityCommentsListFilters} from "@/services/CommentService";
import ApproveCommentDialog from "@/components/Comment/ApproveCommentDialog.vue";
import DeleteCommentDialog from "@/components/Comment/DeleteCommentDialog.vue";
import EntityApprovedPicker from "@/components/Comment/EntityApprovedPicker.vue";

// LifeCycles
onMounted(() => {
  getEntityComments()
})

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const isRenderingApproveDialog = ref<boolean>(false)
const isRenderingDeleteDialog = ref<boolean>(false)
const entityList = ref<IAttribute[]>(null)
const totalCount = ref<null | string | number | undefined>(null)
const selectedEntity = ref<IAttribute>()

const entitiesFilters = ref<IEntityCommentsListFilters>({
  pageNumber: 1,
  count: 10,
  entityType: 'Excersie',
  search: '',
  isApproved: null,
})

const tableHeaders: VDataTable['headers'] = [
  {title: 'شناسه', key: 'id'},
  {title: 'متن تجربه', key: 'text'},
  {title: 'کاربر', key: 'userInfo', value: (item: IEntityComment) => item.userInfo.fullName},
  {title: 'تعداد لایک ها', key: 'likeCount'},
  {title: 'وضعیت', key: 'isApproved'},
  {title: 'عملیات', key: 'actions'},
]

function renderApproveDialog(item: IAttribute) {
  selectedEntity.value = JSON.parse(JSON.stringify(item))
  isRenderingApproveDialog.value = true
}

function renderDeleteDialog(item: IAttribute) {
  selectedEntity.value = JSON.parse(JSON.stringify(item))
  isRenderingDeleteDialog.value = true
}

async function changePage(pageNumber: number | string) {
  entitiesFilters.value.pageNumber = +pageNumber
  await getEntityComments()
}

async function getEntityComments() {
  try {
    spinner.showSpinner()
    const response = await $api?.comment.getEntityCommentsForAdmin(entitiesFilters.value)
    entityList.value = response?.data.data.data as Array<IAttribute>
    totalCount.value = response?.data.data.totalCount as number

  } catch (error) {
    console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}

watch(()=>entitiesFilters.value.isApproved,async(val)=>{
  entitiesFilters.value.pageNumber = 1
  getEntityComments()
})
watch(()=>entitiesFilters.value.entityType,async(val)=>{
  entitiesFilters.value.pageNumber = 1
  getEntityComments()
})
</script>

<template>
  <PageWrapper
    has-filters
    @submit-filters="getEntityComments"
  >
    <template #title>
      لیست تجربه های ثبت شده
    </template>
    <template #filters>
      <VCol
        cols="12"
        md="3"
      >
        <VTextField
          v-model="entitiesFilters.search"
          label="جستجو... "
          hide-details
          @keydown.enter="getEntityComments"
        />
      </VCol>
      <VCol
        cols="12"
        md="3"
      >
        <EntityTypePicker :required="false" v-model="entitiesFilters.entityType"></EntityTypePicker>
      </VCol>
      <VCol
        cols="12"
        md="3"
      >
        <EntityApprovedPicker :required="false" v-model="entitiesFilters.isApproved"></EntityApprovedPicker>
      </VCol>
    </template>
    <CustomTable
      :items-list="entityList"
      :count="entitiesFilters.count"
      :page-number="entitiesFilters.pageNumber"
      :table-headers="tableHeaders"
      :total-count="totalCount"
      @change-page="changePage"
    >
      <template #actions="data">
        <VBtn
          color="transparent"
          elevation="0"
          icon="mdi-check"
          @click="renderApproveDialog(data.item as IAttribute)"
        />
               <VBtn
                 color="transparent"
                 elevation="0"
                 icon="mdi-delete"
                 @click="renderDeleteDialog(data.item as IAttribute)"
        /> 
      </template>
      <template #isApproved="data">
        <VChip color="success" v-if="data.item.isApproved">تایید شده</VChip>
        <VChip color="error" v-else>تایید نشده</VChip>
      </template>
    </CustomTable>

    <ApproveCommentDialog
      v-model:dialogState="isRenderingApproveDialog"
      :selected-item="selectedEntity"
      @refetch="getEntityComments"
    />

    <DeleteCommentDialog
      v-model:dialogState="isRenderingDeleteDialog"
      :selected-item="selectedEntity"
      @refetch="getEntityComments"
    />
  </PageWrapper>
</template>

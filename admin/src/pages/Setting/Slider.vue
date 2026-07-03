<template>
  <PageWrapper>
    <template #title>
      اسلایدر صفحه اصلی
    </template>
    <template #append>
      <VBtn
        color="success"
        @click="renderCreateDialog"
      >
        ایجاد اسلاید جدید
      </VBtn>
    </template>

    <VRow>
      <VCol cols="12" class="px-0">
        <v-card flat>
          <v-tabs
            v-model="tab"
          >
            <v-tab :value="3">دسکتاپ</v-tab>
            <v-tab :value="1">موبایل</v-tab>
            <v-tab :value="2"> تبلت</v-tab>
          </v-tabs>

          <v-card-text>
            <v-tabs-window v-model="tab">
              <v-tabs-window-item v-if="tab===3">
                <VCol v-for="banner in filteredSliders(3)" md="12" cols="12">
                  <VBtn
                    class="mb-2"
                    color="error"
                    @click="renderDeleteDialog(banner)"
                  >
                    حذف اسلاید
                  </VBtn>
                  <AdvertiseBanner :height="`height:20rem`"
                                   :image="baseUrl + '/media/Gallery/slider/'+ banner.pictureUrl"
                                   @click="renderUpdateModal(banner)"></AdvertiseBanner>
                </VCol>
              </v-tabs-window-item>

              <v-tabs-window-item v-if="tab===1">
                <VCol v-for="banner in filteredSliders(1)" md="12" cols="12">
                  <VBtn
                    class="mb-2"
                    color="error"
                    @click="renderDeleteDialog(banner)"
                  >
                    حذف اسلاید
                  </VBtn>
                  <AdvertiseBanner :height="`height:20rem`"
                                   :image="baseUrl + '/media/Gallery/slider/'+ banner.pictureUrl"
                                   @click="renderUpdateModal(banner)"></AdvertiseBanner>
                </VCol>

              </v-tabs-window-item>
              <v-tabs-window-item v-if="tab===2">
                <VCol v-for="banner in filteredSliders(2)" md="12" cols="12">
                  <VBtn
                    class="mb-2"
                    color="error"
                    @click="renderDeleteDialog(banner)"
                  >
                    حذف اسلاید
                  </VBtn>
                  <AdvertiseBanner :height="`height:20rem`"
                                   :image="baseUrl + '/media/Gallery/slider/'+ banner.pictureUrl"
                                   @click="renderUpdateModal(banner)"></AdvertiseBanner>
                </VCol>

              </v-tabs-window-item>
            </v-tabs-window>
          </v-card-text>
        </v-card>
      </VCol>

    </VRow>
    <UpdateSliderDialog @refetch="getAllSliders" v-model:dialog-state="isRenderingUpdateModal"
                        :default-slide="tempSlide"></UpdateSliderDialog>
    <DeleteSlideDialog @refetch="getAllSliders" v-model:dialog-state="isRenderingDeleteDialog"
                       :default-slide="tempSlide"></DeleteSlideDialog>
    <CreateSliderDialog :screen="tab" @refetch="getAllSliders"
                        v-model:dialog-state="isRenderingCreateDialog"></CreateSliderDialog>
  </PageWrapper>
</template>

<script lang="ts" setup>
import {useSpinner} from "@/composables/spinner";
import {IApiProvider} from "@/models/IApiProvider";
import AdvertiseBanner from "@/components/Banners/AdvertiseBanner.vue";
import DeleteSlideDialog from "@/components/Banners/DeleteSlideDialog.vue";

const sliders = ref([])
const spinner = useSpinner()
const $api = inject<IApiProvider>('$api')
const isRenderingUpdateModal = ref(false)
const isRenderingCreateDialog = ref(false)
const isRenderingDeleteDialog = ref(false)
const tempSlide = ref(null)
const tab = ref(3)
const baseUrl = computed(() => {
  return import.meta.env.VITE_BASE_API_URL
})

function renderUpdateModal(item) {
  tempSlide.value = JSON.parse(JSON.stringify(item))
  isRenderingUpdateModal.value = true
}

function renderCreateDialog() {
  isRenderingCreateDialog.value = true
}

function renderDeleteDialog(item) {
  isRenderingDeleteDialog.value = true
  tempSlide.value = JSON.parse(JSON.stringify(item))

}

async function getAllSliders() {
  try {
    spinner.showSpinner()

    const response = await $api?.settings.getAllSliders()

    sliders.value = response.data.data
  } catch (error) {
    console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}

function filteredSliders(displayType: number) {
  return sliders.value.filter(e => e.screen === displayType)
}

onMounted(() => {
  getAllSliders()
})
</script>

<style scoped>

</style>

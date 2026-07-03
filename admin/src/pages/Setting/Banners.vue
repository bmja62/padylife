<template>
  <PageWrapper>
    <template #title>
      بنر های سایت
    </template>
    <VRow>
      <VCol cols="12" class="px-0">
        <v-card flat>
          <v-tabs
            v-model="tab"
          >
            <v-tab value="one">دسکتاپ</v-tab>
            <v-tab value="two">موبایل</v-tab>
          </v-tabs>

          <v-card-text>
            <v-tabs-window>
              <v-tabs-window-item v-if="tab==='one'" value="one">
                <VRow>
                  <VCol v-for="banner in filteredBanners(1,'desktop')" md="4">
                    <AdvertiseBanner :height="`height:10rem`" :image="banner.fileData"
                                     @click="renderUpdateModal(banner)"></AdvertiseBanner>
                  </VCol>
                  <VCol v-for="banner in filteredBanners(2,'desktop')" md="6" class="py-0">
                    <AdvertiseBanner :height="`height:12rem`" :image="banner.fileData"
                                     @click="renderUpdateModal(banner)"></AdvertiseBanner>
                  </VCol>
                  <VRow>
                    <VCol v-for="banner in filteredBanners(3,'desktop')" md="6">
                      <AdvertiseBanner :height="`height:5rem`" :image="banner.fileData"
                                       @click="renderUpdateModal(banner)"></AdvertiseBanner>
                    </VCol>
                    <VCol v-for="banner in filteredBanners(4,'desktop')" md="12">
                      <AdvertiseBanner :height="`height:5rem`" :image="banner.fileData"
                                       @click="renderUpdateModal(banner)"></AdvertiseBanner>
                    </VCol>

                  </VRow>
                  <VCol cols="12">
                    <strong>بنر صفحه فروشگاه</strong>
                  </VCol>
                  <VCol md="4" v-for="banner in filteredBanners(5,'desktop')">
                    <AdvertiseBanner :height="`height:10rem`" :image="banner.fileData"
                                     @click="renderUpdateModal(banner)"></AdvertiseBanner>
                  </VCol>
                </VRow>
              </v-tabs-window-item>

              <v-tabs-window-item v-if="tab==='two'" value="two">
                <VRow>

                  <VCol v-for="banner in filteredBanners(1,'mobile')" md="12">
                    <AdvertiseBanner :height="`height:10rem`" :image="banner.fileData"
                                     @click="renderUpdateModal(banner)"></AdvertiseBanner>
                  </VCol>
                  <VCol v-for="banner in filteredBanners(2,'mobile')" md="12" class="">
                    <AdvertiseBanner :height="`height:10rem`" :image="banner.fileData"
                                     @click="renderUpdateModal(banner)"></AdvertiseBanner>
                  </VCol>
                  <VRow>
                    <VCol v-for="banner in filteredBanners(3,'mobile')" md="12">
                      <AdvertiseBanner :height="`height:10rem`" :image="banner.fileData"
                                       @click="renderUpdateModal(banner)"></AdvertiseBanner>
                    </VCol>
                    <VCol v-for="banner in filteredBanners(4,'mobile')" md="12">
                      <AdvertiseBanner :height="`height:10rem`" :image="banner.fileData"
                                       @click="renderUpdateModal(banner)"></AdvertiseBanner>
                    </VCol>
                  </VRow>
                  <VCol cols="12">
                    <strong>بنر صفحه فروشگاه</strong>
                  </VCol>
                  <VCol cols="12" v-for="banner in filteredBanners(5,'mobile')">
                    <AdvertiseBanner :height="`height:10rem`" :image="banner.fileData"
                                     @click="renderUpdateModal(banner)"></AdvertiseBanner>
                  </VCol>
                </VRow>
              </v-tabs-window-item>
            </v-tabs-window>
          </v-card-text>
        </v-card>
      </VCol>

    </VRow>
    <UpdateBannerDialog @refetch="getAllBanners" v-model:dialog-state="isRenderingUpdateModal"
                        :default-banner="tempBanner"></UpdateBannerDialog>
  </PageWrapper>
</template>

<script lang="ts" setup>
import {useSpinner} from "@/composables/spinner";
import {IApiProvider} from "@/models/IApiProvider";
import AdvertiseBanner from "@/components/Banners/AdvertiseBanner.vue";
import UpdateBannerDialog from "@/components/Banners/UpdateBannerDialog.vue";

const banners = ref([])
const spinner = useSpinner()
const $api = inject<IApiProvider>('$api')
const isRenderingUpdateModal = ref(false)
const tempBanner = ref(null)
const tab = ref('one')

function renderUpdateModal(item) {
  tempBanner.value = JSON.parse(JSON.stringify(item))
  isRenderingUpdateModal.value = true
}

async function getAllBanners() {
  try {
    spinner.showSpinner()

    const response = await $api?.settings.getAllBanners()
    banners.value = response?.data
    banners.value.forEach((item) => {
      item.fileData = import.meta.env.VITE_BASE_API_URL + '/' + item.fileData
    })
  } catch (error) {
    console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}

function filteredBanners(type: number, displayType: string) {
  return banners.value.filter(e => e.type === type && e.description.includes(displayType))
}

onMounted(() => {
  getAllBanners()
})
</script>

<style scoped>

</style>

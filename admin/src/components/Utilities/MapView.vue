<script lang="ts" setup>
const props = defineProps({
  nodeId: {
    type: Number,
    required: true,
  },
  defaultViewGeoLoc: {
    type: Array,
    required: false,
  },
  defaultMarkerGeoLoc: {
    type: Array,
    required: false,
  },
})

const map = ref(null)
const marker = ref(null)

watch(() => props.defaultMarkerGeoLoc, async val => {
  if (val && val.length > 0) {
    if (val[0] && val[1])
      addMarker(val[0], val[1])
  }
})
watch(() => props.defaultViewGeoLoc, async val => {
  if (map.value && val && val.length > 0) {
    if (val[0] && val[1])
      map.value.setView(val, 14)
  }
})
onMounted(() => {
  if (!map.value) {
    setTimeout(() => {
      map.value = L.map(`map${props.nodeId}`, { zoomControl: false }).setView([35.751128, 51.418679], 14)
      L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: 19,
        attribution: '© OpenStreetMap',
      }).addTo(map.value)

      // Leaflet has a known issue with wrong marker address
      // This section is their own provided solution for fixing this issue
      // Please don't touch it or marker won't be shown on the map
      delete L.Icon.Default.prototype._getIconUrl
      L.Icon.Default.mergeOptions({
        iconRetinaUrl: '/marker-icon-2x.png',
        iconUrl: '/marker-icon.png',
        shadowUrl: '/marker-shadow.png',
      })

      // Solution ends here
      if (map.value && props.defaultMarkerGeoLoc) {
        if (props.defaultMarkerGeoLoc[0] && props.defaultMarkerGeoLoc[1]) {
          map.value.setView(props.defaultMarkerGeoLoc, 14)
          marker.value = L.marker(props.defaultMarkerGeoLoc)
          marker.value.addTo(map.value)
        }
      }
    }, 1000)
  }
})

onBeforeUnmount(() => {
  destroyMap()
})

function destroyMap() {
  map.value.off()
  map.value.remove()
  map.value = null
}

async function addMarker(lat, long) {
  if (map.value) {
    if (!marker.value) {
      // Check if a marker already exists
      // If not, create a new one
      marker.value = L.marker([lat, long])
      marker.value.addTo(map.value)
    }
    else {
      if (map.value) {
      // If a marker already exists, remove it first,
      // Then add new marker
        await marker.value.removeFrom(map.value)
        marker.value = L.marker([lat, long])
        marker.value.addTo(map.value)
      }
    }
    map.value.setView([lat, long], 14)
  }
}
</script>

<template>
  <div
    :id="`map${nodeId}`"
    class="w-full map relative rounded-lg"
  />
</template>

<style>
.map {
  height: 300px !important;
}
</style>

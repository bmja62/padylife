<script setup lang="ts">

import {orderStatusesPicker} from "@/services/OrderService";
const selectedStatus = defineModel()
</script>

<template>

  <CustomPicker
    v-model="selectedStatus"
    :item-title="(item) => item.name"
    :item-value="(item) => item.value"
    :items="orderStatusesPicker"
    :return-object="false"
    clearable
    color="success"
    item-color="white"
    label="وضعیت سفارش"
    variant="outlined"
  />
</template>

<style scoped lang="scss">

</style>

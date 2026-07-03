import { computed, ref } from 'vue'

export function usePriceFormatter() {
  const convertNumbers2English = function (str: string | undefined | null): string {
    if (str) {
      return str.replace(/[\u0660-\u0669\u06F0-\u06F9]/g, c => {
        return c.charCodeAt(0) & 0xF
      })
    }
    else {
      return ''
    }
  }

  const pureValue = ref<string>('')

  const formattedPrice = computed({
    get() {
      return pureValue.value
    },
    set(newValue) {
      // This setter is getting number, replace all commas with empty str
      // Then start to separate numbers with ',' from beginning to prevent
      // from data corruption
      if (newValue) {

        formattedPrice.value = convertNumbers2English(newValue)
          .toString()
          .replaceAll(',', '')
          .replace(/\B(?=(\d{3})+(?!\d))/g, ',')

        // Remove all characters that are NOT number
        formattedPrice.value = formattedPrice.value.replace(
          /[a-zA-Z+*!@#$%^&*()_;:'"|<>/?{}\u0600-\u06EC/\-/\.]/g,
          '',
        )
      }
      else if (!newValue || newValue === '') {
        formattedPrice.value = ''
      }
    },
  },

  )

  return { formattedPrice }
}

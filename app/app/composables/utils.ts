import {UserRoles} from "~/models/Enums/UserRoles";

export function useUtils() {
  // Convert Numbers to english
  function convertNumbers2English(input: string) {
    if (input) {
      // @ts-expect-error string/number evaluation error
      return input.replace(/[\u0660-\u0669\u06f0-\u06f9]/g, (c: string) => {
        return c.charCodeAt(0) & 0xf;
      });
    }
    return input;
  }
  function logFormData(myFormData: FormData): void {
    for (let pair of myFormData.entries()) {
      console.log(pair[0] + ', ' + pair[1]);
    }
  }

  function handlePWAInstallDialog() {
    const authStore = useAuthStore()
    const isFirstVisitCookie = useCookie<boolean>("_firstVisit");
    if (isFirstVisitCookie.value == false) {
      return;
    } else {
      if (
          !navigator.standalone &&
          !window.matchMedia("(display-mode: standalone)").matches
      ) {
        if (navigator.userAgent.match(/iPhone|iPad|iPod/)) {
          authStore.setIphone();
          authStore.updatePwaModalState(true);
        } else if (/android/i.test(navigator.userAgent)) {
          authStore.setAndroid();
          authStore.updatePwaModalState(true);
        }
      }
    }
  }
  function time_ago(time: string | number) {
    switch (typeof time) {
      case 'number':
        break;
      case 'string':
        time = +new Date(time);
        break;
      case 'object':
        if (time.constructor === Date) time = time.getTime();
        break;
      default:
        time = +new Date();
    }
    const time_formats = [
      [60, 'ثانیه ', 1], // 60
      [120, '1 دقیقه پیش', 'یک دقیقه پیش'], // 60*2
      [3600, 'دقیقه ', 60], // 60*60, 60
      [7200, '1 ساعت پیش', '1 ساعت پیش'], // 60*60*2
      [86400, 'ساعت ', 3600], // 60*60*24, 60*60
      [172800, 'دیروز', 'فردا'], // 60*60*24*2
      [604800, 'روز ', 86400], // 60*60*24*7, 60*60*24
      [1209600, 'هفته پیش', 'هفته بعد'], // 60*60*24*7*4*2
      [2419200, 'هفته', 604800], // 60*60*24*7*4, 60*60*24*7
      [4838400, 'ماه پیش', 'ماه بعد'], // 60*60*24*7*4*2
      [29030400, 'ماه', 2419200], // 60*60*24*7*4*12, 60*60*24*7*4
      [58060800, 'سال پیش', 'سال بعد'], // 60*60*24*7*4*12*2
      [2903040000, 'سال', 29030400], // 60*60*24*7*4*12*100, 60*60*24*7*4*12

    ];
    let seconds = (+new Date() - time) / 1000,
        token = 'پیش',
        list_choice = 1;

    if (seconds == 0) {
      return 'پیش'
    }
    if (seconds < 0) {
      seconds = Math.abs(seconds);
      token = 'پیش';
      list_choice = 2;
    }
    let i = 0,
        format;
    // eslint-disable-next-line no-cond-assign
    while (format = time_formats[i++])
      if (seconds < format[0]) {
        if (typeof format[2] == 'string')
          return format[list_choice];
        else
          return Math.floor(seconds / format[2]) + ' ' + format[1] + ' ' + token;
      }
    return time;
  }
  
  function generateAnyFormData(data) {
    const formData = new FormData()
    Object.keys(data).forEach(key => {
      if (data && data[key]) {
        if (!Array.isArray(data[key])) {
          if (data[key] instanceof File) {
            formData.append(key, data[key])
          } else if (typeof data[key] === 'object') {
            Object.keys(data[key]).forEach(objKey => {
              formData.append(`${key}.${objKey}`, data[key][objKey])
            })
          } else {
            formData.append(key, data[key])
          }
        } else if (Array.isArray(data[key])) {
          data[key].forEach(arrayItem => {
            if (typeof arrayItem == 'object' && !(arrayItem instanceof File)) {
              Object.keys(arrayItem).forEach(innerKey => {
                formData.append(`${key}.${innerKey}`, arrayItem[innerKey])
              })
            } else {
              formData.append(`${key}`, arrayItem)
            }
          })
        }
      }
    })

    return formData
  }

  // Easing animation for toasts
  function easeOutElasticAnimation(n: number): number {
    return n === 0
      ? 0
      : n === 1
      ? 1
      : 2 ** (-10 * n) * Math.sin((n * 10 - 0.75) * ((2 * Math.PI) / 3)) + 1;
  }

  const isSpecialist = computed(() => {
    const authStore = useAuthStore()
    const idx = authStore?.getUser?.roles?.findIndex((e) => e.name.toLowerCase() === UserRoles.Specialist.toLowerCase())
    if (idx > -1) {
      return true
    }
    return false
  })
  return {
    convertNumbers2English,
    easeOutElasticAnimation,
    handlePWAInstallDialog,
    logFormData,
    isSpecialist,
    time_ago,
    generateAnyFormData
  };
}

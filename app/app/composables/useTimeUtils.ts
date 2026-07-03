export function useTimeUtils() {
    function convertNumbers2English(number: string) {
        if (number) {
            return number.replace(/[\u0660-\u0669\u06f0-\u06f9]/g, function (c) {
                return c.charCodeAt(0) & 0xf;
            });
        }
    }

    function getDayOfMonth(date: Date) {
        return +convertNumbers2English(new Date(date).toLocaleDateString('fa-IR', {day: 'numeric'}))
    }

    function getCurrentYear() {
        return +convertNumbers2English(new Date(Date.now()).toLocaleDateString('fa-IR', {year: 'numeric'}))
    }

    function getCurrentMonth() {
        return +convertNumbers2English(new Date(Date.now()).toLocaleDateString('fa-IR', {month: 'numeric'}))
    }

    function getMonthName(monthNumber: number) {
        return new Date(new Date(Date.now()).setMonth(monthNumber+2)).toLocaleDateString('fa-IR', {month: 'long'})
    }

    function addDayToDate(date: Date, dayCount: number | 1) {
        return new Date(new Date(date).getTime() + (dayCount * 86000000))
    }

    function isToday(date: Date) {
        return new Date(date).setHours(0, 0, 0, 0) === new Date(Date.now()).setHours(0, 0, 0, 0)
    }

    return {
        getDayOfMonth,
        getCurrentYear,
        isToday,
        getCurrentMonth,
        addDayToDate,
        getMonthName
    }
}


export default defineNuxtRouteMiddleware(async (to, from) => {
    if (import.meta.client) {
        const {$api} = useNuxtApp();
        const authStore = useAuthStore()
        if (authStore.isLogged) {
        try {
            const requestBody = {
                pageUrl: getSanitizedPath(to),
                pageTitle: to.name,
                referrer: window.location.href
            }
            const response = await $api.users.trackVisit(requestBody, navigator.userAgent)
        } catch (error) {
            console.error(error.message);
        }
        }
    }
});

function getSanitizedPath(to) {
    const path = to.path || "";
    const encodedPath = path.split('/').map(segment =>
        segment === "" ? "" : isEncoded(segment) ? segment : encodeURIComponent(segment)
    ).join('/');
    return unescape(encodeURIComponent(encodedPath));
}

function isEncoded(uri) {
    uri = uri || ''; // Handle empty or null input
    try {
        return uri !== decodeURIComponent(uri);
    } catch (e) {
        // Handle cases where decodeURIComponent might throw an error
        // for malformed URI sequences, indicating it's likely encoded
        return true;
    }
}

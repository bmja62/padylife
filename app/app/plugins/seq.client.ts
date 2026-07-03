import seq from "seq-logging/browser";
import type {SeqLoggerConfig} from "seq-logging";

export default defineNuxtPlugin((nuxtApp) => {
    const config = useRuntimeConfig();
    const auth = useAuthStore();
    nuxtApp.hook("vue:error", (error, instance, info) => {
        console.error(error);
        let loggerConfig: SeqLoggerConfig = {
            onError(e: Error): void {
                logger.close();
                console.log("seq-connection-error", e);
            },
            serverUrl: config.public.seqAddress,
            apiKey: config.public.seqApiKey,
        };
        let logger = new seq.Logger(loggerConfig);
        logger.emit({
            timestamp: new Date(),
            level: "Error",
            messageTemplate: "Error Happened In {Initiator} From {Host}",
            properties: {
                Initiator: info,
                Host: window.location.href,
                User: auth.getUser ? auth.getUser : "",
                Navigator:
                    navigator && navigator.userAgent ? navigator.userAgent : "",
                // @ts-ignore-next-line
                Route: instance?.$parent
                    ? instance?.$parent?.$route?.fullPath
                    : window && window.location && window.location.href
                        ? window.location.href
                        : "",
                ParentElementClass: instance?.$el
                    ? instance?.$el?.parentElement?.className
                    : "",
            },
            // @ts-ignore-next-line
            exception: error.stack,
        });
    });
});
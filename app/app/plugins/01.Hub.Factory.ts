import {HttpTransportType, type HubConnection, HubConnectionBuilder, LogLevel} from "@microsoft/signalr";

export default defineNuxtPlugin(() => {
    class HubFactory {
        private readonly logLevel: LogLevel
        private readonly hubName: string

        constructor(logLevel: LogLevel, hubName: string) {
            this.hubName = hubName
            this.logLevel = logLevel || LogLevel.Error

        }

        public createHubConnection(): HubConnection {
            try {
                const config = useRuntimeConfig()
                const authStore = useAuthStore()
                const token = JSON.parse(JSON.stringify(authStore.getToken))
                return new HubConnectionBuilder()
                    .configureLogging(this.logLevel)
                    .withUrl(`${config.public.apiAddress}/hub/${this.hubName}`, {
                        accessTokenFactory: function () {
                            return token.replace('Bearer ', '')
                        },
                        skipNegotiation: true,
                        transport: HttpTransportType.WebSockets,
                        headers: {
                            'Access-Control-Allow-Origin': '*',
                        },
                    })
                    .withAutomaticReconnect([0, 0, 5000, 10000])
                    .build()
            } catch (e) {
                console.log(e)
            }
        }


    }

    return {
        provide: {
            hubFactory: HubFactory
        }
    }
})

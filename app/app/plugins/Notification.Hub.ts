import {HubFactory} from "~/plugins/01.Hub.Factory";
import {HubConnectionState, LogLevel} from "@microsoft/signalr";

export default defineNuxtPlugin((nuxtApp) => {
    class NotificationHub {
        private static instance: NotificationHub;
        private hubFactory: HubFactory;

        private constructor() {
            this.hubFactory = new nuxtApp.$hubFactory(LogLevel.Error, 'NotifyHub');
        }

        public getInstance(): NotificationHub {
            if (!NotificationHub.instance) {
                NotificationHub.instance = this.hubFactory.createHubConnection();
                this.connect()
            }
            return NotificationHub.instance;
        }

        private async connect() {
            await NotificationHub.instance.start()
                .then(() => {
                    console.log('Hub connection started');
                })
                .catch(err => console.error('Error while starting connection: ', err));
        }

        public async closeInstance() {
            if (NotificationHub.instance?.state === HubConnectionState.Connected) {
                await NotificationHub.instance.stop()
                    .then(() => {
                        console.log('Hub connection stopped');
                        NotificationHub.instance = null
                    })
                    .catch(err => console.error('Error while stopping connection: ', err));
            }
        }
    }

    return {
        provide: {
            notifyHub: new NotificationHub()
        }
    }

})



import {HubFactory} from "~/plugins/01.Hub.Factory";
import {HubConnectionState, LogLevel} from "@microsoft/signalr";

export default defineNuxtPlugin((nuxtApp) => {
    class ChatHub {
        private static instance: ChatHub;
        private hubFactory: HubFactory;

        private constructor() {
            this.hubFactory = new nuxtApp.$hubFactory(LogLevel.Error, 'ChatHub');
        }

        public getInstance(): ChatHub {
            if (!ChatHub.instance) {
                ChatHub.instance = this.hubFactory.createHubConnection();
                this.connect()
            }
            return ChatHub.instance;
        }

        private async connect() {
            await ChatHub.instance.start()
                .then(() => {
                    console.log('Hub connection started');
                })
                .catch(err => console.error('Error while starting connection: ', err));
        }

        public async closeInstance() {
            if (ChatHub.instance?.state === HubConnectionState.Connected) {
                await ChatHub.instance.stop()
                    .then(() => {
                        console.log('Hub connection stopped');
                        ChatHub.instance = null
                    })
                    .catch(err => console.error('Error while stopping connection: ', err));
            }
        }
    }

    return {
        provide: {
            chatHub: new ChatHub()
        }
    }

})



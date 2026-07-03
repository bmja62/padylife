import type {IUserSession} from "@/services/UserService";
import type {IUserPoint} from "~/services/UsersService";

// Interfaces
interface IAuthUser {
    isLoggedIn: boolean;
    token: string | null;
    user: IUserSession | null;
    unreadNotificationCount: null | number
    userPoints: IUserPoint | null
}

export const useAuthStore = defineStore("_auth", {
    state: (): IAuthUser => ({
        isLoggedIn: false,
        token: null,
        user: null,
        unreadNotificationCount: null,
        firstVisit: false,
        userPoints: null,
        isRenderingPwaDialog: false,
        isAndroid: false,
        isIphone: false,
    }),
    getters: {
        isFirstVisit(): boolean {
            return this.firstVisit;
        },
        isLogged(): boolean {
            return this.isLoggedIn;
        },
        getToken(): string | null {
            const tokenCookie = useCookie("_token");
            return this.token
                ? this.token
                : tokenCookie.value
                    ? tokenCookie.value
                    : null;
        },
        isRenderingPwaModal(): boolean {
            return this.isRenderingPwaDialog;
        },
        isIphoneDevice(): boolean {
            return this.isIphone;
        },
        isAndroidDevice(): boolean {
            return this.isAndroid;
        },
        getUser(): IUser | null {
            return this.user;
        },
        getUserPoints(): IUserPoint | null {
            return this.userPoints;
        },
        getNotificationCount(): number | null {
            return this.unreadNotificationCount;
        }
    },
    actions: {
        setIphone(): void {
            this.isIphone = true;
            this.isAndroid = false;
        },
        setAndroid(): void {
            this.isAndroid = true;
            this.isIphone = false;
        },
        updatePwaModalState(state: boolean): void {
            const isFirstVisitCookie = useCookie<boolean>("_firstVisit");
            this.isRenderingPwaDialog = state;
            if (!state) {
                isFirstVisitCookie.value = false;
            }
        },
        setFirstVisit() {
            this.firstVisit = false
        },
        setNewNotificationCount(notificationCount: number): void {
            this.unreadNotificationCount = notificationCount
        },
        logout(): void {
            this.isLoggedIn = false;
            this.token = null;
            this.user = null;
            this.notificationCount = null;
            this.userPoints = null
            const tokenCookie = useCookie("_token");
            tokenCookie.value = null;
            useRouter().push("/");
        },
        async setUser(loginResponse): void {
            const cookieMaxAge = loginResponse.accessToken.expires_in; // One month in seconds
            const tokenCookie = useCookie("_token", {
                maxAge: cookieMaxAge,
            });
            this.token = loginResponse.accessToken.token_type === 'Bearer' ? `Bearer ${loginResponse.accessToken.access_token}` : loginResponse.accessToken.access_token;
            tokenCookie.value = this.token;
            this.isLoggedIn = true;
            setTimeout(async () => {
                await this.updateUser(loginResponse.user)
            }, 500)
        },
        updateUser(userDetails: IUser): void {
            this.user = userDetails;
        },
        async fetchUser(): Promise<void> {
            const {$api} = useNuxtApp();
            const spinner = useSpinner();
            const alert = useAlerts();
            try {
                spinner.renderSpinner();
                const response = await $api.users.getUserByToken();
                if (response.isSuccess) {
                    this.updateUser(response.data);
                } else {
                    alert.error(
                        response?.message || "مشکلی پیش آمد، لطفا دوباره امتحان کنید"
                    );
                }
            } catch (error) {
                console.error(error);
            } finally {
                spinner.hideSpinner();
            }
        },
        async fetchUserPoints(): void {

            const {$api} = useNuxtApp();
            const spinner = useSpinner();
            const alert = useAlerts();
            try {
                spinner.renderSpinner();
                const response = await $api.users.getUserPoints(this.user.id);
                if (response.isSuccess) {
                    this.userPoints = response.data
                } else {
                    alert.error(
                        response?.message || "مشکلی پیش آمد، لطفا دوباره امتحان کنید"
                    );
                }
            } catch (error) {
                console.error(error);
            } finally {
                spinner.hideSpinner();
            }
        },
    },
    persist: {
        storage: piniaPluginPersistedstate.localStorage(),
        pick: ["isLoggedIn", "firstVisit", "token", "user", 'userPoints',
            'isRenderingPwaDialog',
    'isAndroid',
    'isIphone',
        ],
    },
});

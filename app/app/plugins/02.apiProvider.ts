import type {IApiProvider} from "@/models/IApiProvider";
import UserService from "@/services/UsersService";
import PlanService from "~/services/PlanService";
import ExerciseService from "~/services/ExerciseService";
import DailyFeelings from "~/services/DailyFeelings";
import StepService from "~/services/StepService";
import CommentService from "~/services/CommentService";
import UploaderService from "~/services/UploaderService";
import RateService from "~/services/RateService";
import ChallengeService from "~/services/ChallengeService";
import BlogCategoryService from "~/services/BlogCategoryService";
import BlogService from "~/services/BlogService";
import NotificationService from "~/services/NotificationService";
import AddressService from "~/services/AddressService";
import SettingService from "~/services/SettingService";
import BasketService from "~/services/BasketService";
import LeaderBoardService from "~/services/LeaderBoardService";
import ReportService from "~/services/ReportService";
import WalletService from "~/services/WalletService";
import ProductService from "~/services/ProductService";
import ProductCategoryService from "~/services/ProductCategoryService";
import OrderService from "~/services/OrderService";
import SiteDynamicSetting from "~/services/SiteDynamicSetting";
import TicketService from "~/services/TicketService";
import CalendarService from "~/services/CalendarService";

export default defineNuxtPlugin((nuxtApp) => {
  const httpClient = nuxtApp.$fetch! as LocalFetch;
  const api: IApiProvider = {
    users: new UserService(httpClient),
    plan: new PlanService(httpClient),
    exercises:new ExerciseService(httpClient),
    dailyFeelings:new DailyFeelings(httpClient),
    step:new StepService(httpClient),
    comment: new CommentService(httpClient),
    uploader: new UploaderService(httpClient),
    rate: new RateService(httpClient),
    challenge: new ChallengeService(httpClient),
    blogCategory: new BlogCategoryService(httpClient),
    blog: new BlogService(httpClient),
    notification: new NotificationService(httpClient),
    address: new AddressService(httpClient),
    setting: new SettingService(httpClient),
    basket: new BasketService(httpClient),
    leaderboard: new LeaderBoardService(httpClient),
    report: new ReportService(httpClient),
    wallet: new WalletService(httpClient),
    product: new ProductService(httpClient),
    productCategory: new ProductCategoryService(httpClient),
    order: new OrderService(httpClient),
    dynamicSetting: new SiteDynamicSetting(httpClient),
    ticket: new TicketService(httpClient),
    calendar: new CalendarService(httpClient),
  };
  return {
    provide: { api: api },
  };
});

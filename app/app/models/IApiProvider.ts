import type UserService from "@/services/UsersService";
import type PlanService from "~/services/PlanService";
import type ExerciseService from "~/services/ExerciseService";
import type DailyFeelings from "~/services/DailyFeelings";
import type StepService from "~/services/StepService";
import CommentService from "~/services/CommentService";
import type RateService from "~/services/RateService";
import type ChallengeService from "~/services/ChallengeService";
import type BlogCategoryService from "~/services/BlogCategoryService";
import type NotificationService from "~/services/NotificationService";
import type AddressService from "~/services/AddressService";
import type SettingService from "~/services/SettingService";
import LeaderBoardService from "~/services/LeaderBoardService";
import type ReportService from "~/services/ReportService";
import type WalletService from "~/services/WalletService";
import type ProductService from "~/services/ProductService";
import type SiteDynamicSetting from "~/services/SiteDynamicSetting";
import TicketService from "~/services/TicketService";
import CalendarService from "~/services/CalendarService";

export interface IApiProvider {
  users: UserService;
  plan: PlanService;
  exercises:ExerciseService;
  dailyFeelings:DailyFeelings;
  step:StepService;
  comment:CommentService;
  rate:RateService;
  challenge:ChallengeService;
  blogCategory:BlogCategoryService;
  notification: NotificationService;
  address: AddressService;
  setting:SettingService;
  leaderboard:LeaderBoardService;
  report: ReportService;
  wallet: WalletService;
  product: ProductService;
  dynamicSetting: SiteDynamicSetting;
  ticket: TicketService;
  calendar: CalendarService;

}

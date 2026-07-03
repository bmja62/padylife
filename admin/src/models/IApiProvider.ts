import type DynamicPageService from '@/services/DynamicPageService'
import type TicketService from '@/services/TicketService'
import type UserService from '@/services/UserService'
import SettingsService from "@/services/SettingsService";
import BlogService from "@/services/BlogService";
import UploaderService from "@/services/UploaderService";
import ExerciseService from "@/services/ExerciseService";
import StepsService from "@/services/StepsService";
import QuestionService from "@/services/QuestionService";
import PlanService from "@/services/PlanService";
import StepOptionService from "@/services/StepOptionService";
import ProductAttributes from "@/services/ProductAttributes";
import ProductCategories from "@/services/ProductCategories";
import CommentService from "@/services/CommentService";
import ChallengeService from "@/services/ChallengeService";
import PointsService from "@/services/PointsService";
import NotificationsService from "@/services/NotificationsService";
import CountriesService from "@/services/CountriesService";
import ProvinceService from "@/services/ProvinceService";
import CityService from "@/services/CityService";
import WalletService from "@/services/WalletService";
import ProductService from "@/services/ProductService";
import WarehouseService from "@/services/WarehouseService";
import InventoryService from "@/services/InventoryService";
import ChatService from "@/services/ChatService";
import OrderService from "@/services/OrderService";
import RelatedPlansService from "@/services/RelatedPlans";
import DiscountService from "@/services/DiscountService";
import VisitService from "@/services/VisitService";
import PaymentService from "@/services/PaymentService";

export interface IApiProvider {
  dynamicPages: DynamicPageService
  tickets: TicketService
  users: UserService
  settings: SettingsService
  blog: BlogService,
  uploader: UploaderService,
  exercise: ExerciseService,
  steps: StepsService,
  question: QuestionService,
  plan: PlanService,
  stepOption: StepOptionService,
  productAttributes: ProductAttributes,
  productCategories: ProductCategories,
  comment: CommentService,
  challenge: ChallengeService,
  points: PointsService,
  notification: NotificationsService,
  countries: CountriesService,
  provinces: ProvinceService,
  cities: CityService,
  wallet: WalletService,
  product: ProductService,
  warehouse: WarehouseService,
  inventory: InventoryService,
  chat: ChatService,
  order: OrderService,
  relatedPlans: RelatedPlansService,
  discounts: DiscountService,
  visit: VisitService,
  payment: PaymentService,


}

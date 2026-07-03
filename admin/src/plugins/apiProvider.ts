import axiosIns from './axios'
import type {IApiProvider} from '@/models/IApiProvider'
import DynamicPageService from '@/services/DynamicPageService'
import TicketService from '@/services/TicketService'
import UserService from '@/services/UserService'
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

const api: IApiProvider = {
  dynamicPages: new DynamicPageService(axiosIns),
  tickets: new TicketService(axiosIns),
  users: new UserService(axiosIns),
  settings: new SettingsService(axiosIns),
  blog: new BlogService(axiosIns),
  uploader: new UploaderService(axiosIns),
  exercise: new ExerciseService(axiosIns),
  steps: new StepsService(axiosIns),
  question: new QuestionService(axiosIns),
  plan: new PlanService(axiosIns),
  stepOption: new StepOptionService(axiosIns),
  productAttributes: new ProductAttributes(axiosIns),
  productCategories: new ProductCategories(axiosIns),
  comment: new CommentService(axiosIns),
  challenge: new ChallengeService(axiosIns),
  points: new PointsService(axiosIns),
  notification: new NotificationsService(axiosIns),
  countries: new CountriesService(axiosIns),
  provinces: new ProvinceService(axiosIns),
  cities: new CityService(axiosIns),
  wallet: new WalletService(axiosIns),
  product: new ProductService(axiosIns),
  warehouse: new WarehouseService(axiosIns),
  inventory: new InventoryService(axiosIns),
  chat: new ChatService(axiosIns),
  order: new OrderService(axiosIns),
  relatedPlans: new RelatedPlansService(axiosIns),
  discounts: new DiscountService(axiosIns),
  visit: new VisitService(axiosIns),
  payment: new PaymentService(axiosIns),
}

export default api

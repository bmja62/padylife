import type {VerticalNavItems} from '@/@layouts/types'

export default [
  {
    title: 'Menu:Dashboard',
    to: {name: 'dashboard-detail'},
    icon: {icon: 'tabler-dashboard'},
    action: 'manage',
    subject: 'all',
  },
  {
    title: 'Menu:Tickets',
    to: {name: 'Tickets-list'},
    icon: {icon: 'tabler-message'},
    action: 'manage',
    subject: 'all',
  },
  {
    title: 'Menu:Users',
    to: {name: 'Users-list'},
    icon: {icon: 'tabler:users'},
    action: 'manage',
    subject: 'specialist',
  },
  {
    title: 'Menu:UserChats',
    to: {name: 'Users-chats-list'},
    icon: {icon: 'tabler:message'},
    action: 'manage',
    subject: 'all',
  },
  {
    title: 'Menu:Wallets',
    to: {name: 'Wallets-list'},
    icon: {icon: 'tabler:wallet'},
    action: 'manage',
    subject: 'all',
  },
  {
    title: 'Menu:Payments',
    to: {name: 'payments-list'},
    icon: {icon: 'tabler:coin'},
    action: 'manage',
    subject: 'all',
  },
  {
    title: 'Menu:Specialists',
    to: {name: 'Specialists-list'},
    icon: {icon: 'tabler:category'},
    action: 'manage',
    subject: 'all',
  },
  {
    title: 'Menu:Coupon',
    to: {name: 'coupon-list'},
    icon: {icon: 'tabler:ticket'},
    action: 'manage',
    subject: 'all',
  },
  {
    title: 'Menu:Orders',
    icon: {icon: 'tabler-align-box-left-stretch'},
    action: 'manage',
    subject: 'all',
    children: [
      {
        title: 'Menu:OrdersList',
        to: {name: 'orders-list'},
      },
    ],
  },
  {
    title: 'Menu:Questions',
    icon: {icon: 'mdi-comment-question-outline'},
    action: 'manage',
    subject: 'specialist',
    children: [
      {
        title: 'Menu:QuestionsList',
        to: {name: 'Questions-list'},
        action: 'manage',
        subject: 'specialist',
      },
      {
        title: 'Menu:QuestionsCreate',
        to: {name: 'Questions-create'},
        action: 'manage',
        subject: 'specialist',
      },
      {
        title: 'Menu:QuestionCategories',
        to: {name: 'Questions-categories'},
      },
    ],
  },
  {
    title: 'Menu:Excersies',
    icon: {icon: 'mdi-lead-pencil'},
    action: 'manage',
    subject: 'specialist',
    children: [
      {
        title: 'Menu:ExcersiesList',
        to: {name: 'Exercises-list'},
        action: 'manage',
        subject: 'specialist',
      },
      {
        title: 'Menu:ExcersiesCreate',
        to: {name: 'Exercises-create'},
        action: 'manage',
        subject: 'specialist',
      },
      {
        title: 'Menu:ExcersiesCategories',
        to: {name: 'Exercises-categories'},
      },
      {
        title: 'Menu:ExerciseSteps',
        to: {name: 'Exercises-steps'},
        action: 'manage',
        subject: 'specialist',
      },
    ],
  },
  {
    title: 'Menu:Plan',
    icon: {icon: 'mdi-book-open-page-variant'},
    action: 'manage',
    subject: 'specialist',
    children: [
      {
        title: 'Menu:PlanList',
        to: {name: 'Plan-list'},
        action: 'read',
        subject: 'specialist',
      },
      {
        title: 'Menu:PlanCreate',
        to: {name: 'Plan-create'},
        action: 'manage',
        subject: 'specialist',
      },
      {
        title: 'Menu:PlanCategories',
        to: {name: 'Plan-categories'},
      },
      {
        title: 'Menu:PlanPriceList',
        to: {name: 'Plan-plan-prices'},
      },
    ],
  },
  {
    title: 'Menu:Notifications',
    icon: {icon: 'tabler:bell-ringing'},
    action: 'manage',
    subject: 'all',
    children: [
      {
        title: 'Menu:NotificationsList',
        to: {name: 'notifications-list'},
      },
      {
        title: 'Menu:NotificationsSend',
        to: {name: 'notifications-send'},
      },
    ],

  },
  {
    title: 'Menu:Products',
    icon: {icon: 'mdi-database'},
    action: 'manage',
    subject: 'all',
    children: [
      {
        title: 'Menu:ProductList',
        to: {name: 'products-list'},
      },
      {
        title: 'Menu:CreateProduct',
        to: {name: 'products-create'},
      },
      {
        title: 'Menu:ProductCategories',
        to: {name: 'products-categories'},
      },
      {
        title: 'Menu:CategoryProperties',
        to: {name: 'products-attributes'},
      },
    ],
  },
  {
    title: 'Menu:Warehouses',
    icon: {icon: 'mdi-folder-multiple'},
    action: 'manage',
    subject: 'all',
    children: [
      {
        title: 'Menu:WarehousesManage',
        to: {name: 'Warehouses-list'},
      },
      // {
      //   title: 'Menu:WarehousesCreate',
      //   to: {name: 'Warehouses-create'},
      // },
    ],
  },

  {
    title: 'Menu:Blog',

    icon: {icon: 'mdi-newspaper-variant'},
    action: 'manage',
    subject: 'all',
    children: [
      {
        title: 'Menu:BlogList',
        to: {name: 'Blog-List'},
      },
      {
        title: 'Menu:BlogCreate',
        to: {name: 'Blog-Create'},
      },
      {
        title: 'Menu:BlogCategory',
        to: {name: 'Blog-Categories'},
      },
    ],
  },
  {
    title: 'Menu:Challenges',

    icon: {icon: 'mdi-orbit'},
    action: 'manage',
    subject: 'specialist',
    children: [
      {
        title: 'Menu:ChallengesList',
        to: {name: 'Challenges-list'},
            action: 'manage',
        subject: 'specialist',
      },
      {
        title: 'Menu:ChallengesCreate',
        to: {name: 'Challenges-create'},
            action: 'manage',
        subject: 'specialist',
      }
    ],
  },

  {
    title: 'Menu:Comments',
    icon: {icon: 'mdi-comment'},
    action: 'manage',
    subject: 'all',
    children: [
      {
        title: 'Menu:CommentsList',
        to: {name: 'Comments-list'},
      },
    ],
  },
  {
    title: 'Menu:BasicSetting',
    icon: {icon: 'mdi-cog'},
    action: 'manage',
    subject: 'all',
    children: [
      {
        title: 'Menu:Country',
        to: {name: 'Setting-country'},
      },
      {
        title: 'Menu:Province',
        to: {name: 'Setting-province'},
      },
      {
        title: 'Menu:City',
        to: {name: 'Setting-city'},
      },
      {
        title: 'Menu:SettingOther',
        to: {name: 'Setting-Other'},
      },
    ],
  },


  // {
  //   title: 'Menu:Setting',
  //
  //   icon: {icon: 'mdi-cog-play'},
  //   action: 'manage',
  //   subject: 'all',
  //   children: [
  //     {
  //       title: 'Menu:SettingBanners',
  //       to: {name: 'Setting-Banners'},
  //     },
  //     {
  //       title: 'Menu:SettingSlider',
  //       to: {name: 'Setting-Slider'},
  //     },
  //     {
  //       title: 'Menu:SettingCategoryBanners',
  //       to: {name: 'Setting-CategoryBanners'},
  //     },
  //     // {
  //     //   title: 'Menu:Gallery',
  //     //   to: {name: 'Setting-Gallery'},
  //     // },
  //     {
  //       title: 'Menu:SettingSpecialBanner',
  //       to: {name: 'Setting-SpecialBanner'},
  //     },
  //     {
  //       title: 'Menu:SettingDefaultAddress',
  //       to: {name: 'Setting-DefaultAddress'},
  //     },
  //     {
  //       title: 'Menu:SettingOther',
  //       to: {name: 'Setting-Other'},
  //     },
  //   ],
  // },
  // {
  //   title: 'Menu:DynamicPages',
  //   icon: {icon: 'mdi-folder-arrow-left-right-outline'},
  //   action: 'manage',
  //   subject: 'all',
  //   children: [
  //     {
  //       title: 'Menu:CreateDynamicPage',
  //       to: {name: 'Dynamic-Pages-create'},
  //     },
  //     {
  //       title: 'Menu:DynamicPageList',
  //       to: {name: 'Dynamic-Pages-list'},
  //     },
  //   ],
  // },
  // {
  //   title: 'Menu:SEO',
  //   icon: {icon: 'mdi-google'},
  //   action: 'manage',
  //   subject: 'all',
  //   children: [
  //     {
  //       title: 'Menu:SEOProduct',
  //       to: {name: 'SEO-Product'},
  //     },
  //     {
  //       title: 'Menu:SEOProductCategory',
  //       to: {name: 'SEO-ProductCategory'},
  //     },
  //     {
  //       title: 'Menu:SEOBlog',
  //       to: {name: 'SEO-Blog'},
  //     },
  //     {
  //       title: 'Menu:SEOBlogCategory',
  //       to: {name: 'SEO-BlogCategory'},
  //     },
  //   ],
  // },
] as VerticalNavItems

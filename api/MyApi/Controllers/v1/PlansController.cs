using Application.Cqrs.Commands;
using Application.Cqrs.Queris;
using Application.PlanRelations.Commands;
using Application.PlanRelations.DTOs;
using Application.PlanRelations.Queries;
using Application.Plans.Commands;
using Application.Plans.DTOs;
using Application.Plans.Queries;
using Application.Questions.DTOs;
using Asp.Versioning;
using Common.GridResults;
using Common.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFramework.Api;
using static Entities.Plans.Plan;

namespace PadyLife.Api.Controllers.v1
{
    /// <summary>
    /// کنترلر پلن
    /// </summary>
    [ApiVersion("1")]
    public class PlansController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher) : BaseController
    {

        /// <summary>
        /// ساخت پلن
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult> CreateOrUpdate([FromBody] CreateOrUpdatePlanCommandDTO input) =>
           await (await commandDispatcher
            .SendAsync(new CreateOrUpdatePlanCommand(input)))
            .ToApiResult()
             .ExecuteOrReturn((r) => Get(r.Id));

        /// <summary>
        /// افزودن سوالات به پلن
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult> AddPlanQuestionToPlan([FromBody] AddPlanQuestionToPlanCommandDTO input) =>
           (await commandDispatcher
            .SendAsync(new AddPlanQuestionToPlanCommand(input)))
            .ToApiResult();

        /// <summary>
        /// تنظیمات لینک بندی سوالات
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult> CreateOrUpdateLinkedQuestion([FromBody] CreateOrUpdateLinkedQuestionCommandDTO input) =>
           (await commandDispatcher
            .SendAsync(new CreateOrUpdateLinkedQuestionCommand(input)))
            .ToApiResult();


        /// <summary>
        /// دریافت پلن
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ApiResult<GetPlanDTO>> Get(long id) =>
           (await queryDispatcher
            .SendAsync(new GetPlanQuery(id)))
            .ToApiResult();

        /// <summary>
        /// دریافت پاسخ ها
        /// </summary>
        /// <param name="Dto"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = $"{UserRoleNames.Admin},{UserRoleNames.Specialist},{UserRoleNames.User}")]
      
        public async Task<ApiResult<GlobalGridResult<PlanAnswersItem>>> GetPlanAnswersRequest([FromQuery] GetPlanAnswersRequest Dto) =>
           (await queryDispatcher
            .SendAsync(new GetPlanAnswersQuery(Dto)))
            .ToApiResult();


        /// <summary>
        /// دریافت پاسخ های مراحل
        /// </summary>
        /// <param name="Dto"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = $"{UserRoleNames.Admin},{UserRoleNames.Specialist},{UserRoleNames.User}")]
        public async Task<ApiResult<GlobalGridResult<PlanExerciseAnswersItem>>> GetPlanExerciseAnswers([FromQuery] GetPlanExerciseAnswersRequest Dto) =>
           (await queryDispatcher
            .SendAsync(new GetPlanExerciseAnswersQuery(Dto)))
            .ToApiResult();

        /// <summary>
        /// پلن های دارای پاسخ
        /// </summary>
        /// <param name="Dto"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = $"{UserRoleNames.Admin},{UserRoleNames.Specialist}")]
        public async Task<ApiResult<GlobalGridResult<PlanWithAnswersListItem>>> GetPlansHaveAnswer([FromQuery] GetPlansHaveAnswerDto Dto) =>
           (await queryDispatcher
            .SendAsync(new GetPlansWithAnswersQuery(Dto)))
            .ToApiResult();


        /// <summary>
        /// دریافت سوالات پلن
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ApiResult<GlobalGridResult<PlanQuestionDTO>>> GetPlanQuestionsByPlanId(long planId, [FromQuery] GlobalGrid globalGrid) =>
           (await queryDispatcher
            .SendAsync(new GetPlanQuestionsQuery(planId, globalGrid)))
            .ToApiResult();


        /// <summary>
        /// دریافت پلن
        /// </summary>
        /// <param name="globalGrid"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult> GetAll([FromQuery] GetAllPlanFilter input) =>
           (await queryDispatcher
            .SendAsync(new GetAllPlanQuery(input)))
            .ToApiResult();

        /// <summary>
        /// دریافت پلن کلاینت
        /// </summary>
        /// <param name="globalGrid"></param>
        /// <param name="containUserPlans"></param>
        /// <param name="containSginUpPlans"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult> GetAllForUI([FromQuery] GlobalGrid globalGrid, bool? containUserPlans, bool? containSginUpPlans, long? categoryId) =>
           (await queryDispatcher
            .SendAsync(new GetAllForUIQuery(globalGrid, containUserPlans, containSginUpPlans, categoryId)))
            .ToApiResult();

        /// <summary>
        /// افزودن سوال به پلن
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult> AddPlanQuestion(AddPlanQuestionDTO input) =>
           (await commandDispatcher
            .SendAsync(new AddPlanQuestionCommand(input)))
            .ToApiResult();

        /// <summary>
        /// حذف سوال از پلن
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult> RemovePlanQuestion(long id) =>
           (await commandDispatcher
            .SendAsync(new RemovePlanQuestionCommand(id)))
            .ToApiResult();


        /// <summary>
        /// اتصال شخص به پلن
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult> AssginPlanToUser(long planId, long userId) =>
           (await commandDispatcher
            .SendAsync(new AssginPlanToUserCommand(planId, userId)))
            .ToApiResult();


        /// <summary>
        /// ثبت پاسخ کاربر
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult<GetPlanFlowDTO>> CreateUserPlanAnswer(CreateUserPlanAnswerDTO input) =>
      await (await commandDispatcher
            .SendAsync(new CreateUserPlanAnswerCommand(input)))
            .ToApiResult()
            .ExecuteOrReturn((r) => GetPlanFlow(r.PlanFlow.PlanId, r.PlanFlow.CurrentPlanQuestionId, r.PlanFlow.SelectedQuestionOptionId));

        /// <summary>
        /// اتمام تمرین پلن فرد
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult> CompletedUserExercise(CompletedUserExerciseCommand input) =>
            (await commandDispatcher.SendAsync(input)).ToApiResult();


        /// <summary>
        /// درصد انتخاب گزینه‌های هر سوال
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<ApiResult<List<AnswerDistributionDTO>>> GetAnswerDistribution([FromQuery] GetAnswerDistributionQuery input) =>
       (await queryDispatcher
            .SendAsync(input))
            .ToApiResult();
        /// <summary>
        /// دریافت لیست همراهان یک پلن
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("list")]
        [Authorize]
        public async Task<ApiResult<GlobalGridResult<UserPlanCompanionDto>>> GetUserPlanCompanions(
            [FromQuery] GetUserPlanCompanionsRequestDto input) =>
            (await queryDispatcher.SendAsync(new GetUserPlanCompanionsQuery(input))).ToApiResult();

        /// <summary>
        /// حذف یک همراه از پلن
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize]
        public async Task<ApiResult> DeleteUserPlanCompanion([FromQuery] DeleteUserPlanCompanionCommand input) =>
            (await commandDispatcher.SendAsync(input)).ToApiResult();

        /// <summary>
        /// دریافت کاربران مشابه با من بر اساس پاسخ‌ها
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<ApiResult<List<SimilarUserDTO>>> GetSimilarUsers([FromQuery] GetSimilarUsersQuery input) =>
            (await queryDispatcher.SendAsync(input)).ToApiResult();


        /// <summary>
        /// دریافت فلو پلن
        /// </summary>
        /// <param name="planId"></param>
        /// <param name="currentPlanQuestionId"></param>
        /// <param name="selectedQuestionOptionId"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<ApiResult<GetPlanFlowDTO>> GetPlanFlow([FromQuery] long planId, long currentPlanQuestionId, long selectedQuestionOptionId) =>
           (await queryDispatcher
            .SendAsync(new GetPlanFlowQuery(PlanFlowDTO.Create(planId, currentPlanQuestionId, selectedQuestionOptionId))))
            .ToApiResult();

        /// <summary>
        /// دریافت اخرین سوال پاسخ نداده شده پلن فرد
        /// </summary>
        /// <param name="planId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<ApiResult<GetByIdQuestionDTO>> GetNextUnansweredQuestion([FromQuery] long planId, long userId) =>
           (await queryDispatcher
            .SendAsync(new GetNextUnansweredQuestionQuery(GetNextUnansweredQuestionDTO.Create(planId, userId))))
            .ToApiResult();


        /// <summary>
        /// حذف پلن
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult> Delete(long id) =>
           (await commandDispatcher
            .SendAsync(new DeletePlanCommand(id)))
            .ToApiResult();

        /// <summary>
        /// تغییر وضعیت
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult> ChangePlanStatus(long id, PlanStatus status) =>
           (await commandDispatcher
            .SendAsync(new ChangePlanStatusCommand(id, status)))
            .ToApiResult();


        /// <summary>
        /// دریافت پلن اکتور بعد از ثبت نام
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ApiResult<GetSignUpPlanDTO>> GetSignUpPlan() =>
           (await queryDispatcher
            .SendAsync(new GetSignUpPlanQuery()))
            .ToApiResult();

        /// <summary>
        /// تغییر پلن به پلن بعد از ثبت نام
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult> ToogleIsSginUpPlan(long planId) =>
           (await commandDispatcher
            .SendAsync(new ToogleIsSginUpPlanCommand(planId)))
            .ToApiResult();



        /// <summary>
        /// دریافت پلن های کاربر
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<ApiResult> GetUserPlansStatus([FromQuery] long userId, [FromQuery] GlobalGrid globalGrid, [FromQuery] long? planId) =>
           (await queryDispatcher
            .SendAsync(new GetUserPlansStatusQuery(userId, globalGrid, planId)))
            .ToApiResult();


        /// <summary>
        /// دریافت پلن های کاربر
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<ApiResult> GetUserPlanExercises([FromQuery] GetUserPlanExercisesInput input) =>
           (await queryDispatcher
            .SendAsync(new GetUserPlanExercisesQuery(input.UserPlanId)))
            .ToApiResult();

        /// <summary>
        /// افزودن متخصص به پلن کاربر و ایجاد چت
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult> AddExpertToUserPlan([FromBody] CreateUserPlanExpertCommandDTO input) =>
            (await commandDispatcher.SendAsync(new CreateUserPlanExpertCommand(input)))
            .ToApiResult();

        /// <summary>
        /// افزودن همراه به پلن کاربر و ایجاد چت
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult> AddCompanion([FromBody] AddUserPlanCompanionCommandDTO input)
    => (await commandDispatcher.SendAsync(new AddUserPlanCompanionCommand(input)))
        .ToApiResult();

        /// <summary>
        /// قیمت گذاری متخصص برای هر پلن
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = $"{UserRoleNames.Admin},{UserRoleNames.Specialist}")]
        public async Task<ApiResult> SetPlanPrice([FromBody] SetExpertPlanPriceCommand command) =>
        (await commandDispatcher.SendAsync(command)).ToApiResult();

        /// <summary>
        /// غیرفعال سازی متخصص برای هر پلن
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = $"{UserRoleNames.Admin},{UserRoleNames.Specialist}")]
        public async Task<ApiResult> DiactiveExpertPlanByExpert([FromBody] DiactiveExpertPlanByExpertCommand command) =>
        (await commandDispatcher.SendAsync(command)).ToApiResult();

        /// <summary>
        /// متخصصین یک پلن خاص که همراه کاربران هستند
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost]
        //[Authorize(Roles = $"{UserRoleNames.Admin},{UserRoleNames.Specialist}")]
        [AllowAnonymous]
        public async Task<ApiResult<GlobalGridResult<GetExpertsThatPeresentPlanIdDTO>>> GetExpertsThatPeresentPlanId([FromBody] GetExpertsThatPeresentPlanIdQuery query) =>
        (await queryDispatcher.SendAsync(query)).ToApiResult();

        /// <summary>
        /// کاربرانی که در پلن شرکت کرده اند
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost]
        //[Authorize(Roles = $"{UserRoleNames.Admin},{UserRoleNames.Specialist}")]
        [AllowAnonymous]
        public async Task<ApiResult<GlobalGridResult<GetUsersThatAlreadyInPlanIdDTO>>> GetUsersThatAlreadyInPlanId([FromBody] GetUsersThatAlreadyInPlanIdQuery query) =>
        (await queryDispatcher.SendAsync(query)).ToApiResult();

        /// <summary>
        /// لیست قیمت متخصصین برای پلن ها
        /// </summary>
        /// <param name="globalGrid"></param>
        /// <param name="planId"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<ApiResult<GlobalGridResult<ExpertPlanPriceDTO>>> GetAllPlanPrices([FromQuery] GlobalGrid globalGrid, long? planId) =>
            (await queryDispatcher.SendAsync(new GetAllExpertPlanPricesQuery(globalGrid, planId))).ToApiResult();


        /// <summary>
        /// در سمت کلاینت لیست قیمت متخصصین برای پلن ها
        /// </summary>
        /// <param name="globalGrid"></param>
        /// <param name="planId"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<ApiResult<GlobalGridResult<ExpertPlanPriceForUIDTO>>> GetAllPlanPricesForUI([FromQuery] GlobalGrid globalGrid, bool? ExpertCompanions, long? planId, bool? RateFilter) =>
            (await queryDispatcher.SendAsync(new GetAllExpertPlanPricesForUIQuery(globalGrid, ExpertCompanions, planId, RateFilter))).ToApiResult();

        /// <summary>
        /// دریافت قیمت متخصص با شناسه
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{expertId}")]
        [Authorize(Roles = $"{UserRoleNames.Admin},{UserRoleNames.Specialist}")]
        public async Task<ApiResult<ExpertPlanPriceDTO>> GetPlanPrice(long expertId, bool? isActive) =>
            (await queryDispatcher.SendAsync(new GetExpertPlanPriceQuery(expertId, isActive))).ToApiResult();

        /// <summary>
        /// دریافت قیمت متخصص با شناسه
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<ApiResult<ExpertPlanPriceDTO>> GetPlanPriceForUI(long expertId, long planId, bool? isActive) =>
            (await queryDispatcher.SendAsync(new GetExpertPlanPriceForUIQuery(expertId, planId, isActive))).ToApiResult();

        /// <summary>
        /// تعداد افرادی که همزمان با من وارد این پلن شدن
        /// </summary>
        /// <param name="UserPlanId"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<ApiResult<GetPlanStartStatsDto>> GetPlanStartStats(long UserPlanId) =>
            (await queryDispatcher.SendAsync(new GetPlanStartStatsQuery(UserPlanId))).ToApiResult();


        /// <summary>
        /// دریافت تعداد همراهی های متخصص
        /// </summary>
        /// <param name="expertId"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<ApiResult<GetExpertCompanionsCountDTO>> GetExpertCompanionsCount(long expertId, bool? isCompleted) =>
            (await queryDispatcher.SendAsync(new GetExpertCompanionsCountQuery(expertId, isCompleted))).ToApiResult();


        /// <summary>
        /// دریافت شرکت کنندگان این پلن
        /// </summary>
        /// <param name="planId"></param>
        /// <param name="globalGrid"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<ApiResult<GlobalGridResult<GetPlanSubscribersDTO>>> GetPlanSubscribers([FromQuery] long planId, [FromQuery] GlobalGrid globalGrid, bool? isCompleted) =>
            (await queryDispatcher.SendAsync(new GetPlanSubscribersQuery(planId, globalGrid, isCompleted))).ToApiResult();

        /// <summary>
        /// اتمام پلن
        /// </summary>
        /// <param name="planId"></param>
        /// <param name="globalGrid"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult> CompleteUserPlan(long userPlanId) =>
            (await commandDispatcher.SendAsync(new CompleteUserPlanCommand(userPlanId))).ToApiResult();


        /// <summary>
        /// ایجاد رابطه بین دو پلن (با اوردر)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = $"{UserRoleNames.Admin},{UserRoleNames.Specialist}")]
        public async Task<ApiResult> CreatePlanRelation([FromBody] CreatePlanRelationDto dto) =>
            (await commandDispatcher.SendAsync(new CreatePlanRelationCommand(dto)))
            .ToApiResult();

        /// <summary>
        /// بروزرسانی اوردر رابطه بین دو پلن
        /// </summary>
        [HttpPut]
        [Authorize(Roles = $"{UserRoleNames.Admin},{UserRoleNames.Specialist}")]
        public async Task<ApiResult> UpdatePlanRelation([FromBody] UpdatePlanRelationDto dto) =>
            (await commandDispatcher.SendAsync(new UpdatePlanRelationCommand(dto)))
            .ToApiResult();

        /// <summary>
        /// حذف رابطه بین دو پلن
        /// </summary>
        [HttpDelete]
        [Authorize(Roles = $"{UserRoleNames.Admin},{UserRoleNames.Specialist}")]
        public async Task<ApiResult> DeletePlanRelation([FromBody] DeletePlanRelationDto dto) =>
            (await commandDispatcher.SendAsync(new DeletePlanRelationCommand(dto)))
            .ToApiResult();

        /// <summary>
        /// دریافت پلن به همراه پلن‌های قبلی و بعدی آن
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<ApiResult<PlanRelationsResultDto>> GetPlanRelations(long planId) =>
            (await queryDispatcher.SendAsync(new GetPlanRelationsQuery(planId)))
            .ToApiResult();



        /// <summary>
        /// ثبت تخفیف رو پلن
        /// </summary>
        [HttpPost]
        [Authorize(Roles = $"{UserRoleNames.Admin},{UserRoleNames.Specialist}")]
        public async Task<ApiResult> SetPlanDiscount([FromBody] SetPlanDiscountDTO input) =>
            (await commandDispatcher.SendAsync(new SetPlanDiscountDTOCommand(input)))
            .ToApiResult();

        /// <summary>
        /// دریافت تخفیف پلن
        /// </summary>
        [HttpGet]
        [Authorize(Roles = $"{UserRoleNames.Admin},{UserRoleNames.Specialist}")]
        public async Task<ApiResult> GetPlanDiscount([FromQuery] long id) =>
            (await queryDispatcher.SendAsync(new GetPlanDiscountQuery(id)))
            .ToApiResult();


        /// <summary>
        /// دریافت پلن های کاربر
        /// </summary>
        [HttpGet]
        [Authorize(Roles = $"{UserRoleNames.Admin},{UserRoleNames.Specialist}")]
        public async Task<ApiResult> GetUserPlans([FromQuery] long userId , [FromQuery] int? pageNumber , int? count,bool? allExperts = false) =>
            (await queryDispatcher.SendAsync(new GetUserPlansPlansQuery(userId, pageNumber,count,allExperts)))
            .ToApiResult();
    }
}

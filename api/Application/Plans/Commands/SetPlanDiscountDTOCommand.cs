using Application.Cqrs.Commands;
using Application.Plans.DTOs;
using Data.Contracts;
using Entities.Plans;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Plans.Commands
{
    public record SetPlanDiscountDTOCommand : ICommand<ServiceResult>
    {
        public SetPlanDiscountDTOCommand(SetPlanDiscountDTO input)
        {
            Input = input;
        }

        public SetPlanDiscountDTO Input { get; }
    }

    public class SetPlanDiscountDTOCommandHandler : ICommandHandler<SetPlanDiscountDTOCommand, ServiceResult>
    {
        private readonly IRepository<Plan> _repository;

        public SetPlanDiscountDTOCommandHandler(IRepository<Plan> repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResult> Handle(SetPlanDiscountDTOCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var input = request.Input;
                // پیدا کردن پلن
                var plan = await _repository.GetByIdAsync(cancellationToken, input.PlanId);
                if (plan == null)
                    return ServiceResult.BadRequest("پلن مورد نظر یافت نشد");

                // اگر قیمت اصلی وجود ندارد، نمی‌توان تخفیف تنظیم کرد
                if (plan.Price == null || plan.Price == 0)
                    return ServiceResult.BadRequest("برای تنظیم تخفیف، پلن باید قیمت اصلی داشته باشد");

                // اعتبارسنجی قیمت تخفیف
                if (input.DiscountPrice.HasValue)
                {
                    if (input.DiscountPrice.Value <= 0)
                        return ServiceResult.BadRequest("قیمت تخفیف باید بزرگتر از صفر باشد");

                    if (input.DiscountPrice.Value >= plan.Price.Value)
                        return ServiceResult.BadRequest("قیمت تخفیف باید کمتر از قیمت اصلی باشد");

                    // اعتبارسنجی تاریخ‌ها
                    if (input.DiscountPriceStartDate.HasValue && input.DiscountPriceEndDate.HasValue)
                    {
                        if (input.DiscountPriceStartDate.Value >= input.DiscountPriceEndDate.Value)
                            return ServiceResult.BadRequest("تاریخ شروع باید قبل از تاریخ پایان باشد");

                        if (input.DiscountPriceEndDate.Value <= DateTime.Now)
                            return ServiceResult.BadRequest("تاریخ پایان باید در آینده باشد");
                    }
                }

                // تنظیم تخفیف
                plan.SetDiscount(input.DiscountPrice, input.DiscountPriceStartDate, input.DiscountPriceEndDate);

                // ذخیره تغییرات
                await _repository.UpdateAsync(plan, cancellationToken);


                return ServiceResult.Ok("تخفیف با موفقیت ثبت شد");
            }
            catch (Exception ex)
            {
                // Log exception
                return ServiceResult.BadRequest("خطا در ثبت تخفیف: " + ex.Message);
            }
        }
    }
}

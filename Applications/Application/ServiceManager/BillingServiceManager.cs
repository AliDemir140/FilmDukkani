using Application.Interfaces;
using Application.Repositories;
using Domain.Entities;
using Domain.Enums;

namespace Application.ServiceManager
{
    public class BillingServiceManager
    {
        private readonly IMemberRepository _memberRepository;
        private readonly IMembershipPlanRepository _membershipPlanRepository;
        private readonly IBillingAttemptRepository _billingAttemptRepository;
        private readonly IBillingGateway _billingGateway;
        private readonly IEmailService _emailService;

        public BillingServiceManager(
            IMemberRepository memberRepository,
            IMembershipPlanRepository membershipPlanRepository,
            IBillingAttemptRepository billingAttemptRepository,
            IBillingGateway billingGateway,
            IEmailService emailService)
        {
            _memberRepository = memberRepository;
            _membershipPlanRepository = membershipPlanRepository;
            _billingAttemptRepository = billingAttemptRepository;
            _billingGateway = billingGateway;
            _emailService = emailService;
        }

        public async Task<(bool Success, string Message)> ChargeNowAsync(int memberId, string period, string adminEmail)
        {
            if (memberId <= 0)
                return (false, "memberId zorunludur.");

            if (string.IsNullOrWhiteSpace(period))
                return (false, "period zorunludur.");

            var member = await _memberRepository.GetByIdAsync(memberId);
            if (member == null)
                return (false, "Üye bulunamadı.");

            var plan = await _membershipPlanRepository.GetByIdAsync(member.MembershipPlanId);
            if (plan == null)
                return (false, "Üyelik planı bulunamadı.");

            var already = await _billingAttemptRepository.ExistsForPeriodAsync(memberId, period);
            if (already)
                return (true, "Bu dönem için zaten tahsilat denemesi var.");

            var amount = plan.Price;

            var gw = await _billingGateway.ChargeAsync(memberId, amount, period);

            var attempt = new BillingAttempt
            {
                MemberId = memberId,
                Period = period,
                Amount = amount,
                Success = gw.Success,
                Error = gw.Success ? null : (gw.Error ?? "Charge failed"),
                AttemptedAt = DateTime.UtcNow
            };

            await _billingAttemptRepository.AddAsync(attempt);

            if (gw.Success)
            {
                if (member.Status == MemberStatus.PaymentDue)
                {
                    member.Status = MemberStatus.Active;
                    await _memberRepository.UpdateAsync(member);
                }

                return (true, "Tahsilat başarılı.");
            }

            member.Status = MemberStatus.PaymentDue;
            await _memberRepository.UpdateAsync(member);

            var subject = "FilmDukkani - Ödeme Alınamadı";
            var body =
                $"Merhaba {member.FirstName} {member.LastName},\n\n" +
                $"Üyelik ücretiniz tahsil edilemedi.\n" +
                $"Dönem: {period}\n" +
                $"Tutar: {amount}₺\n" +
                $"Hata: {attempt.Error}\n\n" +
                $"Üyeliğiniz ödeme bekliyor durumuna alındı. Lütfen ödeme yapın.\n";

            await _emailService.SendAsync(member.Email, subject, body);

            if (!string.IsNullOrWhiteSpace(adminEmail))
            {
                var adminBody =
                    $"MemberId: {memberId}\n" +
                    $"Email: {member.Email}\n" +
                    $"Period: {period}\n" +
                    $"Amount: {amount}\n" +
                    $"Error: {attempt.Error}\n";

                await _emailService.SendAsync(adminEmail, "FilmDukkani - Tahsilat Başarısız", adminBody);
            }

            return (false, "Tahsilat başarısız. Üye ödeme bekliyor durumuna alındı.");
        }

        public async Task RunForMemberAsync(int memberId, string period, string adminEmail)
        {
            await ChargeNowAsync(memberId, period, adminEmail);
        }
    }
}

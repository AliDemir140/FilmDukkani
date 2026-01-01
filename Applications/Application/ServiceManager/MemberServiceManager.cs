using Application.DTOs.MemberDTOs;
using Application.Repositories;
using Domain.Entities;

namespace Application.ServiceManager
{
    public class MemberServiceManager
    {
        private readonly IMemberRepository _memberRepository;
        private readonly IMembershipPlanRepository _membershipPlanRepository;

        public MemberServiceManager(
            IMemberRepository memberRepository,
            IMembershipPlanRepository membershipPlanRepository)
        {
            _memberRepository = memberRepository;
            _membershipPlanRepository = membershipPlanRepository;
        }

        public async Task<List<MemberDto>> GetMembersAsync()
        {
            var members = await _memberRepository.GetAllAsync();
            var plans = await _membershipPlanRepository.GetAllAsync();

            return members
                .Select(m =>
                {
                    var planName = plans.FirstOrDefault(p => p.ID == m.MembershipPlanId)?.PlanName ?? string.Empty;

                    return new MemberDto
                    {
                        Id = m.ID,
                        FirstName = m.FirstName,
                        LastName = m.LastName,
                        Email = m.Email,
                        Phone = m.Phone,
                        MembershipPlanId = m.MembershipPlanId,
                        MembershipPlanName = planName,
                        MembershipStartDate = m.MembershipStartDate,
                        IdentityUserId = m.IdentityUserId,
                        Role = string.Empty
                    };
                })
                .ToList();
        }

        public async Task<UpdateMemberDto?> GetMember(int id)
        {
            // BaseRepository GetByIdAsync exception atıyorsa bunu null'a çeviriyoruz
            Member? member;
            try
            {
                member = await _memberRepository.GetByIdAsync(id);
            }
            catch
            {
                return null;
            }

            if (member == null)
                return null;

            return new UpdateMemberDto
            {
                Id = member.ID,
                FirstName = member.FirstName,
                LastName = member.LastName,
                Email = member.Email,
                Password = member.Password,
                Phone = member.Phone,
                MembershipPlanId = member.MembershipPlanId,
                MembershipStartDate = member.MembershipStartDate,
                IdentityUserId = member.IdentityUserId,
                Role = string.Empty
            };
        }

        public async Task<bool> AddMember(CreateMemberDto dto)
        {
            var plan = await _membershipPlanRepository.GetByIdAsync(dto.MembershipPlanId);
            if (plan == null)
                return false;

            var member = new Member
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Password = dto.Password,
                Phone = dto.Phone,
                MembershipPlanId = dto.MembershipPlanId,
                MembershipStartDate = DateTime.Now,

                // EN KRİTİK FIX:
                IdentityUserId = dto.IdentityUserId ?? string.Empty
            };

            await _memberRepository.AddAsync(member);
            return true;
        }

        public async Task<bool> UpdateMember(UpdateMemberDto dto)
        {
            Member? member;
            try
            {
                member = await _memberRepository.GetByIdAsync(dto.Id);
            }
            catch
            {
                return false;
            }

            if (member == null)
                return false;

            var plan = await _membershipPlanRepository.GetByIdAsync(dto.MembershipPlanId);
            if (plan == null)
                return false;

            member.FirstName = dto.FirstName;
            member.LastName = dto.LastName;
            member.Email = dto.Email;
            member.Phone = dto.Phone;
            member.MembershipPlanId = dto.MembershipPlanId;

            if (!string.IsNullOrWhiteSpace(dto.Password))
                member.Password = dto.Password;

            if (dto.MembershipStartDate.HasValue)
                member.MembershipStartDate = dto.MembershipStartDate.Value;

            if (!string.IsNullOrWhiteSpace(dto.IdentityUserId))
                member.IdentityUserId = dto.IdentityUserId;

            await _memberRepository.UpdateAsync(member);
            return true;
        }

        public async Task<bool> DeleteMember(int id)
        {
            Member? member;
            try
            {
                member = await _memberRepository.GetByIdAsync(id);
            }
            catch
            {
                return false;
            }

            if (member == null)
                return false;

            member.IsDeleted = true;
            member.DeletedAt = DateTime.UtcNow;

            await _memberRepository.UpdateAsync(member);
            return true;
        }

    }
}

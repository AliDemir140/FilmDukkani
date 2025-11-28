using Application.DTOs.MemberDTOs;
using Application.Repositories;
using Domain.Entities;

namespace Application.ServiceManager
{
    public class MemberServiceManager
    {
        private readonly IMemberRepository _memberRepository;

        public MemberServiceManager(IMemberRepository memberRepository)
        {
            _memberRepository = memberRepository;
        }

        // LISTELEME
        public async Task<List<MemberDto>> GetMembersAsync()
        {
            var members = await _memberRepository.GetAllAsync();

            return members
                .Select(m => new MemberDto
                {
                    Id = m.ID,
                    FirstName = m.FirstName,
                    LastName = m.LastName,
                    Email = m.Email,
                    Phone = m.Phone,
                    MembershipPlanId = m.MembershipPlanId,
                    MembershipStartDate = m.MembershipStartDate
                })
                .ToList();
        }
        
        // TEK GETIRME
        public async Task<UpdateMemberDto?> GetMember(int id)
        {
            var member = await _memberRepository.GetByIdAsync(id);
            if (member == null) return null;

            return new UpdateMemberDto
            {
                Id = member.ID,
                FirstName = member.FirstName,
                LastName = member.LastName,
                Email = member.Email,
                Password = member.Password,
                Phone = member.Phone,
                MembershipPlanId = member.MembershipPlanId,
                MembershipStartDate = member.MembershipStartDate
            };
        }

        // EKLEME
        public async Task AddMember(CreateMemberDto dto)
        {
            var member = new Member
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Password = dto.Password,
                Phone = dto.Phone,
                MembershipPlanId = dto.MembershipPlanId,
                MembershipStartDate = DateTime.Now
            };

            await _memberRepository.AddAsync(member);
        }

        // GUNCELLEME
        public async Task<bool> UpdateMember(UpdateMemberDto dto)
        {
            var member = await _memberRepository.GetByIdAsync(dto.Id);
            if (member == null)
                return false;

            member.FirstName = dto.FirstName;
            member.LastName = dto.LastName;
            member.Email = dto.Email;
            member.Password = dto.Password;
            member.Phone = dto.Phone;

            if (member.MembershipPlanId != dto.MembershipPlanId)
            {
                member.MembershipPlanId = dto.MembershipPlanId;
                member.MembershipStartDate = DateTime.Now; // sadece plan değişince güncelle
            }

            await _memberRepository.UpdateAsync(member);
            return true;
        }


        // SILME
        public async Task<bool> DeleteMember(int id)
        {
            var member = await _memberRepository.GetByIdAsync(id);
            if (member == null)
                return false;

            await _memberRepository.DeleteAsync(member);
            return true;
        }
    }
}

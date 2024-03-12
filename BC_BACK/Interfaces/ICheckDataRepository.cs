using BC_BACK.Dto;

namespace BC_BACK.Interfaces
{
    public interface ICheckDataRepository
    {
        bool CheckStringLengs(string word, int length);
        string CheckUser(UserDto user);
        bool IsSlovakWord(string input);
    }
}
